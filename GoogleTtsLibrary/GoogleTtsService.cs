using Google.Cloud.TextToSpeech.V1;
using NAudio.Lame;
using NAudio.Wave; // MP3の結合に使用


namespace GoogleTtsLibrary
{
    public class GoogleTtsService
    {
        private async Task<TextToSpeechClient> GetTextToSpeechClient()
        {
            string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "client_secret.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
            TextToSpeechClient client = await TextToSpeechClient.CreateAsync();
            return client;
        }
        public async Task ExportTextToMp3(string TextFilePath, string ExportPath)
        {
            var client = await GetTextToSpeechClient();
            var audioConfig = new AudioConfig { AudioEncoding = AudioEncoding.Linear16 };
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "ja-JP", // 言語コードを指定
                Name = "ja-JP-Standard-D", // 話す声を変更（例: 男性のWavenet）
                SsmlGender = SsmlVoiceGender.Male // 声の性別を指定
            };
            var lines = File.ReadAllLines(TextFilePath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var tempFolder = Directory.CreateDirectory(Path.Combine(ExportPath.Replace("mp3", "temp"), timestamp)).FullName;
            for (var i=0;i<lines.Length;i++)
            {
                var text = lines[i];
                text = $"<speak>{text}</speak>";
                text = text
                    .Replace("。", "。<break time='0.7s'/>")
                    .Replace("！", "！<break time='0.3s'/>")
                    .Replace("？", "？<break time='0.3s'/>")
                    .Replace("---", "<break time='10s'/>")
                    .Replace("--", "<break time='5s'/>");
                var input = new SynthesisInput { Ssml = text };
                var response = await client.SynthesizeSpeechAsync(input, voiceSelection, audioConfig);
                
                using (var output = File.Create(Path.Combine(tempFolder, $"{i.ToString("D6")}.wav")))
                {
                    response.AudioContent.WriteTo(output);
                }
            }
            await ConcatAllWav(tempFolder,ExportPath.Replace("mp3", "wav"));
            await ConvertWavToMp3(ExportPath.Replace("mp3", "wav"), ExportPath);


         }
        public async Task ConcatAllWav(string tempFolder,string ExportPath)
        {
            var WavFiles = Directory.GetFiles(tempFolder, "*.wav").Order().ToList();
            using (var waveFileWriter = new WaveFileWriter(ExportPath, new WaveFileReader(WavFiles[0]).WaveFormat))
            {
                foreach (var wavFile in WavFiles)
                {
                    using (var reader = new WaveFileReader(wavFile))
                    {
                        reader.CopyTo(waveFileWriter);
                    }
                }
            }
        }

        public async Task ConvertWavToMp3(string wavPath, string mp3Path)
        {
            using (var reader = new WaveFileReader(wavPath))
            using (var writer = new LameMP3FileWriter(mp3Path, reader.WaveFormat, LAMEPreset.VBR_90))
            {
                reader.CopyTo(writer);
            }
        }
    }
}

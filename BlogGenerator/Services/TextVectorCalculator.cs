using MeCab;
using System.Text.Json;
namespace BlogGenerator.Application.Services
{
    public class TextVectorCalculator
    {
        // 複数のテキストに対してコーパス全体からIDFを計算し、個々のテキストのTF-IDFを計算する
        public string CalcTextVector(string text, List<string[]> corpus)
        {
            // 1. 現在のテキストを形態素解析して単語リストを取得
            var documentWords = DivideWords(text).ToArray();

            // 2. コーパス全体（複数の文書）のIDFを計算
            var idf = CalculateIdf(corpus);

            // 3. 現在のテキストに対するTF-IDFベクトルを計算
            var tfidfVector = CalculateTfIdf(documentWords, idf);

            // 4. ベクトルをJSON形式にシリアライズして返す
            return JsonSerializer.Serialize(tfidfVector);
        }
        public Dictionary<string,Double> CalcTextVector2(string text, List<string[]> corpus)
        {
            // 1. 現在のテキストを形態素解析して単語リストを取得
            var documentWords = DivideWords(text).ToArray();

            // 2. コーパス全体（複数の文書）のIDFを計算
            var idf = CalculateIdf(corpus);

            // 3. 現在のテキストに対するTF-IDFベクトルを計算
            return CalculateTfIdf(documentWords, idf);

        }
        // テキストを形態素解析して単語リストを返す
        public string[] DivideWords(string text)
        {
            var mecab = MeCabTagger.Create();
            List<string> words = new List<string>();
            var nodes = mecab.ParseToNodes(text);
            foreach (var node in nodes)
            {
                if (!string.IsNullOrEmpty(node.Surface))
                {
                    words.Add(node.Surface);
                }
            }
            return words.ToArray();
        }

        // 各単語のTF（Term Frequency）を計算
        public Dictionary<string, double> CalculateTf(string[] document)
        {
            var tf = new Dictionary<string, double>();
            int totalTerms = document.Length;

            foreach (var term in document)
            {
                if (tf.ContainsKey(term))
                    tf[term]++;
                else
                    tf[term] = 1;
            }

            // 出現頻度を文書の単語数で割って正規化
            foreach (var key in tf.Keys.ToList())
            {
                tf[key] /= totalTerms;
            }

            return tf;
        }

        // コーパス全体に対して各単語のIDFを計算
        public Dictionary<string, double> CalculateIdf(List<string[]> corpus)
        {
            var idf = new Dictionary<string, double>();
            int totalDocuments = corpus.Count;

            foreach (var document in corpus)
            {
                foreach (var term in document.Distinct()) // 重複しない単語で計算
                {
                    if (idf.ContainsKey(term))
                        idf[term]++;
                    else
                        idf[term] = 1;
                }
            }

            // IDFスコアを計算（log(総文書数 / その単語を含む文書数)）
            foreach (var key in idf.Keys.ToList())
            {
                idf[key] = Math.Log((double)totalDocuments / idf[key]);
            }

            return idf;
        }

        // 各文書のTF-IDFスコアを計算
        public Dictionary<string, double> CalculateTfIdf(string[] document, Dictionary<string, double> idf)
        {
            var tf = CalculateTf(document);
            var tfIdf = new Dictionary<string, double>();

            foreach (var term in tf.Keys)
            {
                if (idf.ContainsKey(term))
                    tfIdf[term] = tf[term] * idf[term];
                else
                    tfIdf[term] = 0;
            }

            return tfIdf;
        }

        public Dictionary<string, double> ConvertJsonToDictionary(string jsonTextVector)
        {
            // System.Text.Jsonを使用してデシリアライズ
            return JsonSerializer.Deserialize<Dictionary<string, double>>(jsonTextVector);

            // Newtonsoft.Jsonを使用する場合:
            // return JsonConvert.DeserializeObject<Dictionary<string, double>>(jsonTextVector);
        }
        public double CalculateCosineSimilarity(Dictionary<string, double> vectorA, Dictionary<string, double> vectorB)
        {
            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            // ベクトルの内積を計算
            foreach (var term in vectorA.Keys)
            {
                if (vectorB.ContainsKey(term))
                    dotProduct += vectorA[term] * vectorB[term];

                magnitudeA += Math.Pow(vectorA[term], 2);
            }

            // ベクトルの大きさを計算
            foreach (var value in vectorB.Values)
            {
                magnitudeB += Math.Pow(value, 2);
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            // コサイン類似度の計算
            if (magnitudeA == 0 || magnitudeB == 0)
                return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}

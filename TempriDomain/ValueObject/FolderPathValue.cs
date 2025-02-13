

using TempriDomain.Config;

namespace TempriDomain.ValueObject
{
    public class FolderPathValue
    {
        public string PdfqDir { get; private set; }
        public string Pdf4Dir { get; private set; }
        public string PdfaDir { get; private set; }
        public string PdfDir { get; private set; }
        public string CoverDir { get; private set; }
        public string GoodsDir { get; private set; }
        public string SvgDir { get; private set; }

        /// <summary>
        /// このクラスの各プロパティを参照し、BaseDir,PrintId,プロパティ名(Dirは削除)を組み合わせたパスを、各プロパティにセットします。
        /// フォルダがなかった場合は作成します。
        /// </summary>
        /// <param name="printId"></param>
        public FolderPathValue(string printId)
        {
            string basePath = Path.Combine(TempriConstants.BaseDir, printId);

            foreach (var prop in GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                string dir = Path.Combine(basePath, prop.Name.ToLower().Replace("dir", ""));
                prop.SetValue(this, dir);
                Directory.CreateDirectory(dir);
            }
        }
    }

}

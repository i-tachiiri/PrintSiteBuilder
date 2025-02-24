

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
    }

}

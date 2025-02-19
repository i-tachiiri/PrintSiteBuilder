

using PrintGenerater.Factories;

namespace PrintGenerater.Services
{
    public class PrintController
    {
        private readonly SvgDownloader svgDownloader;
        private readonly PdfGenerator pdfGenerator;
        private readonly PrintFactory printFactory;
        public PrintController(SvgDownloader svgDownloader, PdfGenerator pdfGenerator, PrintFactory printFactory)
        {
            this.svgDownloader = svgDownloader;
            this.pdfGenerator = pdfGenerator;   
            this.printFactory = printFactory;
        }
        public async Task GeneratePrint(int PrintId)
        {
            var PrintClass = printFactory.CreateInstance(PrintId);
        }
    }
}



using PrintGenerater.Factories;

namespace PrintGenerater.Services
{
    public class PrintController
    {
        private readonly SvgDownloader svgDownloader;
        private readonly PdfGenerator pdfGenerator;
        private readonly PrintFactory printFactory;
        private readonly HtmlGenerator htmlGenerator;
        private readonly TemplateDuplicator templateDuplicator;

        public PrintController(SvgDownloader svgDownloader, PdfGenerator pdfGenerator, PrintFactory printFactory,HtmlGenerator htmlGenerator, TemplateDuplicator templateDuplicator)
        {
            this.svgDownloader = svgDownloader;
            this.pdfGenerator = pdfGenerator;   
            this.printFactory = printFactory;
            this.htmlGenerator = htmlGenerator;
            this.templateDuplicator = templateDuplicator;
        }
        public async Task GeneratePrint(int PrintId)
        {
            var PrintClass = printFactory.CreateInstance(PrintId);
            templateDuplicator.SetPrintDirectory();
            await svgDownloader.ExportSvgs(PrintClass);
            pdfGenerator.CreatePdf();
            htmlGenerator.GenerateHtml();
        }
    }
}

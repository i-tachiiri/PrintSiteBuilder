

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
        private readonly QrGenerator qrGenerator;
        private readonly QrAttacher qrAttacher;
        public PrintController(SvgDownloader svgDownloader, PdfGenerator pdfGenerator, PrintFactory printFactory,
            HtmlGenerator htmlGenerator, TemplateDuplicator templateDuplicator, QrGenerator qrGenerator,QrAttacher qrAttacher)
        {
            this.svgDownloader = svgDownloader;
            this.pdfGenerator = pdfGenerator;   
            this.printFactory = printFactory;
            this.htmlGenerator = htmlGenerator;
            this.templateDuplicator = templateDuplicator;
            this.qrGenerator = qrGenerator;
            this.qrAttacher = qrAttacher;
        }
        public async Task GeneratePrint(int PrintId)
        {
            //Old Print classes are at:C:\drive\work\solution\PrintSiteBuilder
            //Create Svg and movin classes to this projects

            var PrintClass = printFactory.CreateInstance(PrintId);
            templateDuplicator.SetPrintDirectory(PrintClass);
            await svgDownloader.ExportSvgs(PrintClass);
            qrGenerator.ExportQrCodes(PrintClass);
            qrAttacher.InsertQrToPrint(PrintClass);
            //Barcode...
            //cover generator(now on this part)
            pdfGenerator.CreatePdf(PrintClass);
            htmlGenerator.GenerateHtml(PrintClass);
        }
    }
}

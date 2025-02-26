using System.Xml.Linq;
using TempriDomain.Config;
using TempriDomain.Interfaces;

namespace PrintGenerater.Services
{
    public class QrAttacher
    {
        private const int Dpi = 300;             
        private readonly double PrintWidth = 21.0 * Dpi / 2.54;  
        private readonly double PrintHeight = 29.7 * Dpi / 2.54; 
        private readonly double QrWidth = 1.5 * Dpi / 2.54;
        private readonly double QrHeight = 1.5 * Dpi / 2.54;
        private readonly double LogoWidth = 1.5 * Dpi / 2.54;
        private readonly double LogoHeight = 6 * Dpi / 2.54;
        private readonly double MarginWidth = 1 * Dpi / 2.54;
        private readonly double MarginHeight = 1 * Dpi / 2.54;

        /// <summary>
        /// insert answer qr, question qr and logo to print
        /// </summary>
        /// <param name="print"></param>
        public void InsertQrToPrint(IPrintEntity print)
        {
            foreach (var page in print.Pages)
            {
                if (!SvgFileExists(page)) continue;
                var svgDoc = LoadSvg(page);

                // Insert both QR codes
                InsertAnswerQr(page,svgDoc);
                InsertQuestionQr(page,svgDoc);
                InsertLogo(page,svgDoc);

                // Save the modified SVG
                string outputSvgPath = page.GetFilePathWithExtension("svg-qr", "svg");
                svgDoc.Save(outputSvgPath);
            }
        }
        /// <summary>
        /// check qr, print, logo exist in this page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private bool SvgFileExists(IPageEntity page)
        {
            string answerQrPath = page.GetFilePathWithExtension("a", "svg");
            string questionQrPath = page.GetFilePathWithExtension("q", "svg");
            string svgPath = page.GetFilePathWithExtension("svg", "svg");
            string logoPath = TempriConstants.LogoPath;

            if (!File.Exists(svgPath) || !File.Exists(questionQrPath) || !File.Exists(answerQrPath) || !File.Exists(logoPath))
            {
                Console.WriteLine($"[WARNING] Missing file: {svgPath} or {questionQrPath} or {answerQrPath} or {logoPath}");
                return false;
            }
            return true;
        }
        /// <summary>
        /// load each print svg and return as XDocument
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private XDocument LoadSvg(IPageEntity page)
        {
            string svgPath = page.GetFilePathWithExtension("svg", "svg");
            return XDocument.Load(svgPath);
        }
        /// <summary>
        /// insert answer qr to print
        /// </summary>
        /// <param name="page"></param>
        /// <param name="printSvg"></param>
        private void InsertAnswerQr(IPageEntity page,XDocument printSvg)
        {
            string answerQrPath = page.GetFilePathWithExtension("a", "svg");
            double x = PrintWidth - (MarginWidth + QrWidth);
            double y = PrintHeight - (MarginHeight + QrHeight);
            InsertSvgToPrint(printSvg, answerQrPath, x,y,QrWidth,QrHeight);
        }
        /// <summary>
        /// insert question qr to print 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="printSvg"></param>
        private void InsertQuestionQr(IPageEntity page,XDocument printSvg)
        {
            string questionQrPath = page.GetFilePathWithExtension("q", "svg");
            double x = MarginWidth;
            double y = PrintHeight - (MarginHeight + QrHeight);
            InsertSvgToPrint(printSvg, questionQrPath, x, y, QrWidth, QrHeight);
        }
        /// <summary>
        /// insert logo to print
        /// </summary>
        /// <param name="page"></param>
        /// <param name="printSvg"></param>
        private void InsertLogo(IPageEntity page,XDocument printSvg)
        {
            //Get logo path
            double x = PrintWidth - (MarginWidth + LogoWidth);
            double y = PrintHeight - (MarginHeight + LogoHeight);
            InsertSvgToPrint(printSvg, TempriConstants.LogoPath, x, y, LogoWidth, LogoHeight);
        }
        /// <summary>
        /// add qr or logo svg to print svg. coordinates and width/height of qr or logo are needed.
        /// </summary>
        /// <param name="printSvg"></param>
        /// <param name="path"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="Exception"></exception>
        private void InsertSvgToPrint(XDocument printSvg, string path,double x,double y,double width,double height)
        {

            var svgNamespace = printSvg.Root?.Name.Namespace;
            if (svgNamespace == null) throw new Exception("SVG namespace is missing");
            var imageElement = new XElement(svgNamespace + "image",
                new XAttribute("x", x),
                new XAttribute("y", y),
                new XAttribute("width", width),
                new XAttribute("height", height),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"),
                new XAttribute("xlink:href", path)
            );
            printSvg.Root?.Add(imageElement);
        }
    }
}

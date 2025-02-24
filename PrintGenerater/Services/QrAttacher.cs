using System.Xml.Linq;
using TempriDomain.Config;
using TempriDomain.Interfaces;

namespace PrintGenerater.Services
{
    public class QrAttacher
    {
        public class SvgQrInserter
        {
            private const double SvgWidthCm = 21.0;  // Standard A4 width
            private const double SvgHeightCm = 29.7; // Standard A4 height
            private const int Dpi = 300; // DPI for pixel conversion (Google Slides SVGs)
            private const double CmToPx = Dpi / 2.54; // Conversion factor

            public void InsertQrToPrint(IPrintEntity print)
            {
                foreach(var page in print.Pages) 
                {
                    var SvgPath = page.GetFilePathWithExtension("svg", "svg");
                    var Xdoc = LoadSvg(SvgPath);
                    Xdoc = InsertAnswerQr(Xdoc);  //implementing
                    Xdoc = InsertQuestionQr();
                    Xdoc.Save(page.GetFilePathWithExtension("svg-qr","svg"));
                }
            }

            public XDocument LoadSvg(string SvgPath)
            {
                return XDocument.Load(SvgPath);
            }
            public XDocument InsertAnswerQr(XDocument XDoc) 
            {
                double marginPx = 1.0 * CmToPx; // Convert 1cm to pixels
                var x = svgWidth - marginPx - 50;
                var y = svgHeight - marginPx;

                var svgNamespace = XDoc.Root?.Name.Namespace;
                var imageElement = new XElement(svgNamespace + "image",
                    new XAttribute("x", x),
                    new XAttribute("y", y),
                    new XAttribute("width", 50), // Set width explicitly
                    new XAttribute("height", 50), // Set height explicitly
                    new XAttribute("xlink:href", qrFilePath) // Reference to QR SVG
                );
                XDoc.Root?.Add(imageElement);
            }
            public XDocument InsertAnswerQr(XDocument XDoc)
                {
                    double marginPx = 1.0 * CmToPx; // Convert 1cm to pixels
                    var type = IsAnswerPage ? "a" : "q";

                    return type switch
                    {
                        "q" => (marginPx, svgHeight - marginPx), // Lower-left
                        "a" => (svgWidth - marginPx - 50, svgHeight - marginPx), // Lower-right


                        var svgNamespace = XDoc.Root?.Name.Namespace;
                var imageElement = new XElement(svgNamespace + "image",
                    new XAttribute("x", x),
                    new XAttribute("y", y),
                    new XAttribute("width", 50), // Set width explicitly
                    new XAttribute("height", 50), // Set height explicitly
                    new XAttribute("xlink:href", qrFilePath) // Reference to QR SVG
                );
                XDoc.Root?.Add(imageElement);
            }

            public (double x, double y) GetQrPosition(bool IsAnswerPage, double svgWidth, double svgHeight)
            {
                double marginPx = 1.0 * CmToPx; // Convert 1cm to pixels
                var type = IsAnswerPage ? "a" : "q";            

                return type switch
                {
                    "q" => (marginPx, svgHeight - marginPx), // Lower-left
                    "a" => (svgWidth - marginPx - 50, svgHeight - marginPx), // Lower-right
                    _ => throw new ArgumentException("Invalid QR type")
                };
            }
            public void InsertQrToSvg(XDocument svgDoc, string qrFilePath, double x, double y)
            {
                var svgNamespace = svgDoc.Root?.Name.Namespace;

                if (svgNamespace == null)
                    throw new Exception("SVG namespace is missing");

                var imageElement = new XElement(svgNamespace + "image",
                    new XAttribute("x", x),
                    new XAttribute("y", y),
                    new XAttribute("width", 50), // Set width explicitly
                    new XAttribute("height", 50), // Set height explicitly
                    new XAttribute("xlink:href", qrFilePath) // Reference to QR SVG
                );

                svgDoc.Root?.Add(imageElement);
            }
        }
    }
}

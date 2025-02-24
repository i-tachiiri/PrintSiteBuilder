using System;
using System.IO;
using System.Xml.Linq;
using TempriDomain.Interfaces;

namespace PrintGenerater.Services
{
    public class SvgQrInserter
    {
        private const double SvgWidthCm = 21.0;  // Standard A4 width
        private const double SvgHeightCm = 29.7; // Standard A4 height
        private const int Dpi = 300;             // DPI for pixel conversion (Google Slides SVGs)
        private const double CmToPx = Dpi / 2.54; // Conversion factor (1 cm = x pixels)

        public void InsertQrToPrint(IPrintEntity print)
        {
            foreach (var page in print.Pages)
            {
                string svgPath = page.GetFilePathWithExtension("svg", "svg");
                string questionQrPath = page.GetFilePathWithExtension("q", "svg");
                string answerQrPath = page.GetFilePathWithExtension("a", "svg");

                if (!File.Exists(svgPath) || !File.Exists(questionQrPath) || !File.Exists(answerQrPath))
                {
                    Console.WriteLine($"[WARNING] Missing file: {svgPath} or {questionQrPath} or {answerQrPath}");
                    continue;
                }

                var svgDoc = LoadSvg(svgPath);
                double svgWidth = SvgWidthCm * CmToPx;
                double svgHeight = SvgHeightCm * CmToPx;

                // Insert both QR codes
                InsertQrToSvg(svgDoc, questionQrPath, GetQrPosition(false, svgWidth, svgHeight));
                InsertQrToSvg(svgDoc, answerQrPath, GetQrPosition(true, svgWidth, svgHeight));

                // Save the modified SVG
                string outputSvgPath = page.GetFilePathWithExtension("svg-qr", "svg");
                svgDoc.Save(outputSvgPath);
            }
        }

        private XDocument LoadSvg(string svgPath)
        {
            return XDocument.Load(svgPath);
        }

        private (double x, double y) GetQrPosition(bool isAnswerPage, double svgWidth, double svgHeight)
        {
            double marginPx = 1.0 * CmToPx; // Convert 1cm to pixels
            return isAnswerPage
                ? (svgWidth - marginPx - 50, svgHeight - marginPx)  // Lower-right
                : (marginPx, svgHeight - marginPx);                 // Lower-left
        }

        private void InsertQrToSvg(XDocument svgDoc, string qrFilePath, (double x, double y) position)
        {
            var svgNamespace = svgDoc.Root?.Name.Namespace;

            if (svgNamespace == null)
            {
                throw new Exception("SVG namespace is missing");
            }

            var imageElement = new XElement(svgNamespace + "image",
                new XAttribute("x", position.x),
                new XAttribute("y", position.y),
                new XAttribute("width", 50),
                new XAttribute("height", 50),
                new XAttribute(XNamespace.Xmlns + "xlink", "http://www.w3.org/1999/xlink"), // Define xlink namespace
                new XAttribute("xlink:href", qrFilePath)
            );

            svgDoc.Root?.Add(imageElement);
        }
    }
}

using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using PdfSharpCore.Test.Helpers;
using PdfSharpCore.Test.IO;
using Xunit;
using Xunit.Abstractions;

namespace PdfSharpCore.Test
{
    public class LargePDFReadWrite : IoBaseTest
    {
        private readonly ITestOutputHelper output;

        public LargePDFReadWrite(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "Too slow for Unit test runner")]
        public void CanCreatePdfOver2gb()
        {
            const string outName = "CreateLargePdf.pdf";
            int pageCount = 70000; //2.1gb @ 369sec to create
            ValidateTargetAvailable(outName);

            var document = new PdfDocument();
            var watch = new System.Diagnostics.Stopwatch();
            var font = new XFont("Arial", 10);

            watch.Start();
            for (var i = 0; i < pageCount; i++)
            {
                AddAPage(document, font);
            }

            watch.Stop();

            SaveDocument(document, outName);
            output.WriteLine($"CreatePDF took {watch.Elapsed.TotalSeconds} sec");
            ValidateFileIsPdf(outName);
            CanReadPdf(outName);
        }

        private void AddAPage(PdfDocument document, XFont font)
        {
            const int x = 40;
            const int y = 50;
            var page = document.AddPage();
            var renderer = XGraphics.FromPdfPage(page);
            var tf = new XTextFormatter(renderer);
            var width = page.Width.Value - 50 - x;
            var height = page.Height.Value - 50 - y;
            var rect = new XRect(40, 50, width, height);
            renderer.DrawRectangle(XBrushes.SeaShell, rect);
            tf.DrawString(TestData.LoremIpsumText, font, XBrushes.Black, rect);
        }
    }
}
using System.IO;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Xunit;

namespace PdfSharpCore.Test
{
    public class PdfModify
    {
        [Fact]
        public void Should_beAbleToWriteTextToPdf()
        {
            var root = Path.GetDirectoryName(GetType().Assembly.Location);
            var existingPdfPath = Path.Combine(root, "Assets", "FamilyTree.pdf");

            var fs = File.OpenRead(existingPdfPath);
            var inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);

            var newDoc = new PdfDocument();
            foreach (var page in inputDocument.Pages)
            {
                var newPage = newDoc.AddPage(page);

                var gfx = XGraphics.FromPdfPage(newPage);

                // Create a font
                var font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

                // Draw the text
                gfx.DrawString("Hello, World!", font, XBrushes.Black,
                    new XRect(0, 0, newPage.Width, newPage.Height),
                    XStringFormats.TopCenter);
            }

            newDoc.Save(Path.Combine(root, "Assets", "FamilyTreeResult.pdf"));

            fs.Dispose();

            Assert.True(true);
        }
    }
}
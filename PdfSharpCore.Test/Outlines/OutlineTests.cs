using System.IO;
using FluentAssertions;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Xunit;

namespace PdfSharpCore.Test.Outlines
{
    public class OutlineTests
    {
        [Fact]
        public void CanCreateDocumentWithOutlines()
        {
            var document = new PdfDocument();
            var font = new XFont("Verdana", 16);
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            gfx.DrawString("Page 1", font, XBrushes.Black, 20, 50, XStringFormats.Default);

            // Create the root bookmark. You can set the style and the color.
            var outline = document.Outlines.Add("Root", page, true,
                PdfOutlineStyle.Bold, XColors.Red);

            // Create some more pages
            for (var idx = 2; idx <= 5; idx++)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);

                var text = $"Page {idx}";
                gfx.DrawString(text, font, XBrushes.Black, 20, 50, XStringFormats.Default);

                // Create a sub bookmark
                outline.Outlines.Add(text, page, true);
            }

            document.Outlines.Count.Should().Be(1);
            
            using var ms = new MemoryStream();
            document.Save(ms);
            ms.ToArray().Length.Should().BeGreaterThan(1);
        }
    }
}
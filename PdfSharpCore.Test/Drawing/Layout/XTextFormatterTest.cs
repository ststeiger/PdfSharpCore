using System.IO;
using FluentAssertions;
using ImageMagick;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Drawing.Layout.enums;
using PdfSharpCore.Pdf;
using PdfSharpCore.Test.Helpers;
using Xunit;

namespace PdfSharpCore.Test.Drawing.Layout
{
    public class XTextFormatterTest
    {
        private static readonly string _outDir = "TestResults/XTextFormatterTest";
        private static readonly string _expectedImagesPath = Path.Combine("Drawing", "Layout");

        private PdfDocument _document;
        private XGraphics _renderer;
        private XTextFormatter _textFormatter;
        
        // Run before each test
        public XTextFormatterTest()
        {
            _document = new PdfDocument();
            var page = _document.AddPage();
            page.Size = PageSize.A6; // 295 x 417 pts
            _renderer = XGraphics.FromPdfPage(page);
            _textFormatter = new XTextFormatter(_renderer);
        }
        
        [Fact]
        public void DrawSingleLineString()
        {
            var layout = new XRect(12, 12, 200, 50);
            _textFormatter.DrawString("This is a simple single line test", new XFont("Arial", 12), XBrushes.Black, layout);

            var diffResult = DiffPage(_document, "DrawSingleLineString", 1);
            
            diffResult.DiffValue.Should().Be(0);
        }
        
        [Fact]
        public void DrawMultilineStringWithTruncate()
        {
            var layout = new XRect(12, 12, 200, 40);
            _renderer.DrawRectangle(XBrushes.LightGray, layout);
            _textFormatter.DrawString("This is text\nspanning 3 lines\nbut only space for 2", new XFont("Arial", 12), XBrushes.Black, layout);

            var diffResult = DiffPage(_document, "DrawMultilineStringWithTruncate", 1);
            
            diffResult.DiffValue.Should().Be(0);
        }
        
        [Fact]
        public void DrawMultiLineStringWithOverflow()
        {
            var layout = new XRect(12, 12, 200, 40);
            _renderer.DrawRectangle(XBrushes.LightGray, layout);
            _textFormatter.AllowVerticalOverflow = true;
            _textFormatter.DrawString("This is text\nspanning 3 lines\nand overflow shows all three", new XFont("Arial", 12), XBrushes.Black, layout);

            var diffResult = DiffPage(_document, "DrawMultiLineStringWithOverflow", 1);
            
            diffResult.DiffValue.Should().Be(0);
        }
        
        [Fact]
        public void DrawMultiLineStringsWithAlignment()
        {
            var layout1 = new XRect(12, 12, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout1);
            _textFormatter.DrawString("This is text\naligned to the top-left", new XFont("Arial", 12), XBrushes.Black, layout1);

            var layout2 = new XRect(12, 100, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout2);
            _textFormatter.SetAlignment(new TextFormatAlignment { Horizontal = XParagraphAlignment.Center, Vertical = XVerticalAlignment.Middle});
            _textFormatter.DrawString("This is text\naligned to the middle-center", new XFont("Arial", 12), XBrushes.Black, layout2);

            var layout3 = new XRect(12, 200, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout3);
            _textFormatter.SetAlignment(new TextFormatAlignment { Horizontal = XParagraphAlignment.Right, Vertical = XVerticalAlignment.Bottom});
            _textFormatter.DrawString("This is text\naligned to the bottom-right", new XFont("Arial", 12), XBrushes.Black, layout3);

            var diffResult = DiffPage(_document, "DrawMultiLineStringsWithAlignment", 1);
            
            diffResult.DiffValue.Should().Be(0);
        }
        
        [Fact]
        public void DrawMultiLineStringsWithLineHeight()
        {
            var font = new XFont("Arial", 12);
            
            var layout1 = new XRect(10, 10, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout1);
            _textFormatter.DrawString("This is text\naligned to the top-left\nand a custom line height", font, XBrushes.Black, layout1, 16);

            var layout2 = new XRect(10, 110, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout2);
            _textFormatter.SetAlignment(new TextFormatAlignment { Horizontal = XParagraphAlignment.Center, Vertical = XVerticalAlignment.Middle});
            _textFormatter.DrawString("This is text\naligned to the middle-center\nand a custom line height", font, XBrushes.Black, layout2, 16);

            var layout3 = new XRect(10, 210, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout3);
            _textFormatter.SetAlignment(new TextFormatAlignment { Horizontal = XParagraphAlignment.Right, Vertical = XVerticalAlignment.Bottom});
            _textFormatter.DrawString("This is text\naligned to the bottom-right\nand a custom line height", font, XBrushes.Black, layout3, 16);

            var layout4 = new XRect(10, 310, 200, 80);
            _renderer.DrawRectangle(XBrushes.LightGray, layout4);
            _textFormatter.SetAlignment(new TextFormatAlignment { Horizontal = XParagraphAlignment.Center, Vertical = XVerticalAlignment.Middle});
            _textFormatter.DrawString("This is text\nwith a very small\nline height", font, XBrushes.Black, layout4, 6);

            var diffResult = DiffPage(_document, "DrawMultiLineStringsWithLineHeight", 1);
            
            diffResult.DiffValue.Should().Be(0);
        }

        private static DiffOutput DiffPage(PdfDocument document, string filePrefix, int pageNum)
        {
            var rasterized = PdfHelper.Rasterize(document);
            var rasterizedFiles = PdfHelper.WriteImageCollection(rasterized.ImageCollection, _outDir, filePrefix);
            var expectedImagePath = PathHelper.GetInstance().GetAssetPath(_expectedImagesPath, $"{filePrefix}_{pageNum}.png");
            return PdfHelper.Diff(rasterizedFiles[pageNum-1], expectedImagePath, _outDir, filePrefix);
        }
    }
}
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace PdfSharpCore.Test
{
    public class CreateSimplePDF
    {
        private static readonly string rootPath = Path.GetDirectoryName(typeof(CreateSimplePDF).GetTypeInfo().Assembly.Location);

        private const string outputDirName = "Out";

        private void SaveDocument(PdfDocument document, string name)
        {
            var outFilePAth = Path.Combine(rootPath, outputDirName, name);
            var dir = Path.GetDirectoryName(outFilePAth);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            document.Save(outFilePAth);
        }

        private void ValidateFileIsPDF(string v)
        {
            var path = Path.Combine(rootPath, outputDirName, v);
            Assert.True(File.Exists(path));
            var fi = new FileInfo(path);
            Assert.True(fi.Length > 1);
            using (var stream = File.OpenRead(path))
            {
                var readBuffer = new byte[5];
                // PDF must start with %PDF-
                var pdfsignature = new byte[5] { 0x25, 0x50, 0x44, 0x46, 0x2d };

                stream.Read(readBuffer, 0, readBuffer.Length);
                Assert.Equal(pdfsignature, readBuffer);
            }
        }

        private void ValidateTargetAvailable(string file)
        {
            var path = Path.Combine(rootPath, outputDirName, file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Assert.False(File.Exists(path));
        }

        [Fact]
        public void CreateTestPDF()
        {
            var outName = "test1.pdf";

            ValidateTargetAvailable(outName);

            var document = new PdfDocument();

            PdfPage pageNewRenderer = document.AddPage();

            var renderer = XGraphics.FromPdfPage(pageNewRenderer);

            renderer.DrawString("Testy Test Test", new XFont("Arial", 12), XBrushes.Black, new XPoint(12, 12));

            SaveDocument(document, outName);
            ValidateFileIsPDF(outName);
        }

        [Fact]
        public void CreateTestPDFWithImage()
        {
            var outName = "test_image.pdf";

            ValidateTargetAvailable(outName);

            var document = new PdfDocument();

            PdfPage pageNewRenderer = document.AddPage();

            var renderer = XGraphics.FromPdfPage(pageNewRenderer);

            renderer.DrawImage(XImage.FromFile(Path.Combine(rootPath, "Assets", "lenna.png")), new XPoint(0, 0));

            SaveDocument(document, outName);

            ValidateFileIsPDF(outName);
        }

    }
}

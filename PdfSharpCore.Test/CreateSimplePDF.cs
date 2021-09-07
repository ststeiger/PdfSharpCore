using System;
using System.IO;
using System.Text;
using FluentAssertions;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Test.Helpers;
using Xunit;

namespace PdfSharpCore.Test
{
    public class CreateSimplePdf
    {
        private readonly string _rootPath = PathHelper.GetInstance().RootDir;
        private const string OutputDirName = "Out";

        [Fact]
        public void CreateTestPdf()
        {
            const string outName = "test1.pdf";

            ValidateTargetAvailable(outName);

            var document = new PdfDocument();

            var pageNewRenderer = document.AddPage();

            var renderer = XGraphics.FromPdfPage(pageNewRenderer);

            renderer.DrawString("Testy Test Test", new XFont("Arial", 12), XBrushes.Black, new XPoint(12, 12));

            SaveDocument(document, outName);
            ValidateFileIsPdf(outName);
        }

        [Fact]
        public void CreateTestPdfWithImage()
        {
            using var stream = new MemoryStream();
            var document = new PdfDocument();

            var pageNewRenderer = document.AddPage();

            var renderer = XGraphics.FromPdfPage(pageNewRenderer);

            renderer.DrawImage(XImage.FromFile(PathHelper.GetInstance().GetAssetPath("lenna.png")), new XPoint(0, 0));

            document.Save(stream);
            stream.Position = 0;
            Assert.True(stream.Length > 1);
            ReadStreamAndVerifyPdfHeaderSignature(stream);
        }

        private void SaveDocument(PdfDocument document, string name)
        {
            var outFilePath = GetOutFilePath(name);
            var dir = Path.GetDirectoryName(outFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            document.Save(outFilePath);
        }

        private void ValidateFileIsPdf(string v)
        {
            var path = GetOutFilePath(v);
            Assert.True(File.Exists(path));
            var fi = new FileInfo(path);
            Assert.True(fi.Length > 1);

            using var stream = File.OpenRead(path);
            ReadStreamAndVerifyPdfHeaderSignature(stream);
        }

        private static void ReadStreamAndVerifyPdfHeaderSignature(Stream stream)
        {
            var readBuffer = new byte[5];
            var pdfSignature = Encoding.ASCII.GetBytes("%PDF-"); // PDF must start with %PDF-

            stream.Read(readBuffer, 0, readBuffer.Length);
            readBuffer.Should().Equal(pdfSignature);
        }

        private void ValidateTargetAvailable(string file)
        {
            var path = GetOutFilePath(file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assert.False(File.Exists(path));
        }

        private string GetOutFilePath(string name)
        {
            return Path.Combine(_rootPath, OutputDirName, name);
        }
    }
}
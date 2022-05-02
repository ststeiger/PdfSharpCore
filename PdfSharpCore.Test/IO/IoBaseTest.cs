using System.IO;
using System.Text;
using FluentAssertions;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Test.Helpers;
using Xunit;

namespace PdfSharpCore.Test.IO
{
    public abstract class IoBaseTest
    {
        private readonly string _rootPath = PathHelper.GetInstance().RootDir;
        private const string _outputDirName = "Out";

        public void CanReadPdf(string fileName)
        {
            var path = GetOutFilePath(fileName);
            using var fs = File.OpenRead(path);
            var inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
            var info = inputDocument.Info;
            info.Should().NotBeNullOrEmpty();
        }

        protected void SaveDocument(PdfDocument document, string name)
        {
            var outFilePath = GetOutFilePath(name);
            var dir = Path.GetDirectoryName(outFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            document.Save(outFilePath);
        }

        protected void ValidateFileIsPdf(string v)
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

        protected void ValidateTargetAvailable(string file)
        {
            var path = GetOutFilePath(file);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            Assert.False(File.Exists(path));
        }

        protected string GetOutFilePath(string name)
        {
            return Path.Combine(_rootPath, _outputDirName, name);
        }
    }
}
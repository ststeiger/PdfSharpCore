using System.IO;
using System.Reflection;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Xunit;

namespace PdfSharpCore.Test
{
    public class PdfReader
    {
        [Fact]
        public void Should_beAbleToReadExistingPdf_When_inputIsStream()
        {
            var root = Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location);
            var existingPdfPath = Path.Combine(root, "Assets", "FamilyTree.pdf");

            var fs = File.OpenRead(existingPdfPath);
            PdfDocument inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
            fs.Dispose();

            Assert.True(true);
        }

        [Fact]
        public void Should_beAbleToReadExistingPdf_When_inputIsStream_Lazy()
        {
            var root = Path.GetDirectoryName(GetType().GetTypeInfo().Assembly.Location);
            var existingPdfPath = Path.Combine(root, "Assets", "FamilyTreeLazy.pdf");

            var fs = File.OpenRead(existingPdfPath);
            PdfDocument inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import, Pdf.IO.enums.PdfReadAccuracy.Lazy);
            fs.Dispose();

            Assert.True(true);
        }
    }
}

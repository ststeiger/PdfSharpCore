using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Xunit;

namespace PdfSharpCore.Test
{
    public class Merge
    {
        [Fact]
        public void ShouldBePossible()
        {
            var root = Path.GetDirectoryName(GetType().Assembly.Location);

            var pdf1Path = Path.Combine(root, "Assets", "FamilyTree.pdf");
            var pdf2Path = Path.Combine(root, "Assets", "test.pdf");

            PdfDocument outputDocument = new PdfDocument();

            foreach (var pdfPath in new[] {pdf1Path, pdf2Path})
            {
                using (var fs = File.OpenRead(pdfPath))
                {
                    PdfDocument inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
                    int count = inputDocument.PageCount;
                    for (int idx = 0; idx < count; idx++)
                    {
                        PdfPage page = inputDocument.Pages[idx];
                        outputDocument.AddPage(page);
                    }
                }
            }

            var outFilePAth = Path.Combine(root, "Out", "merge.pdf");
            var dir = Path.GetDirectoryName(outFilePAth);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            outputDocument.Save(outFilePAth);
        }
    }
}

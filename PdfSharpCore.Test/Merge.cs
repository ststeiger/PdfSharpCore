using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Test.Helpers;
using Xunit;

namespace PdfSharpCore.Test
{
    public class Merge
    {
        [Fact]
        public void CanMerge2Documents()
        {
            var pdf1Path =  PathHelper.GetInstance().GetAssetPath("FamilyTree.pdf");
            var pdf2Path = PathHelper.GetInstance().GetAssetPath("test.pdf");

            var outputDocument = new PdfDocument();

            foreach (var pdfPath in new[] { pdf1Path, pdf2Path })
            {
                using var fs = File.OpenRead(pdfPath);
                var inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
                var count = inputDocument.PageCount;
                for (var idx = 0; idx < count; idx++)
                {
                    var page = inputDocument.Pages[idx];
                    outputDocument.AddPage(page);
                }
            }

            var outFilePath = Path.Combine(PathHelper.GetInstance().RootDir, "Out", "merge.pdf");
            var dir = Path.GetDirectoryName(outFilePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            outputDocument.Save(outFilePath);
        }
    }
}
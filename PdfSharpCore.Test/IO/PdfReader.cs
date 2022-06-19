using FluentAssertions;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Test.Helpers;
using System;
using System.IO;
using Xunit;

namespace PdfSharpCore.Test.IO
{
    public class PdfReader
    {
        [Fact]
        public void Should_beAbleToReadExistingPdf_When_inputIsStream()
        {
            using var fs = File.OpenRead(PathHelper.GetInstance().GetAssetPath("FamilyTree.pdf"));
            var inputDocument = Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.Import);
            AssertIsAValidPdfDocumentWithProperties(inputDocument, 38148);
        }

        [Fact]
        public void WillThrowExceptionWhenReadingInvalidPdf()
        {
            using var fs = File.OpenRead(PathHelper.GetInstance().GetAssetPath("NotAValid.pdf"));
            Action act = () => Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.ReadOnly);
            act.Should().Throw<InvalidOperationException>().WithMessage("The file is not a valid PDF document.");
        }

        internal static void AssertIsAValidPdfDocumentWithProperties(PdfDocument inputDocument, int expectedFileSize)
        {
            inputDocument.Should().NotBeNull();
            inputDocument.FileSize.Should().Be(expectedFileSize);
            inputDocument.Info.Should().NotBeNull();
            inputDocument.PageCount.Should().BeGreaterThan(0);
        }
    }
}
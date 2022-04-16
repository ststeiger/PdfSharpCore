using FluentAssertions;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.IO.enums;
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
        public void ShouldBeAbleToOpenAesEncryptedDocuments()
        {
            // this document has a V value of 4 (see PdfReference 1.7, Chapter 7.6.1, Table 20)
            // and an R value of 4 (see PdfReference 1.7, Chapter 7.6.3.2, Table 21)
            // see also: Adobe Supplement to the ISO 32000, BaseVersion: 1.7, ExtensionLevel: 3
            //           Chapter 3.5.2, Table 3.19
            // TODO: find documents with V value of 5 and R values of 5 and 6
            var file = PathHelper.GetInstance().GetAssetPath("AesEncrypted.pdf");
            var fi = new FileInfo(file);
            var document = Pdf.IO.PdfReader.Open(file, PdfDocumentOpenMode.Import);

            // verify document was actually AES-encrypted
            var cf = document.SecurityHandler.Elements.GetDictionary("/CF");
            var stdCf = cf.Elements.GetDictionary("/StdCF");
            stdCf.Elements.GetString("/CFM").Should().Be("/AESV2");

            AssertIsAValidPdfDocumentWithProperties(document, (int)fi.Length);
        }

        [Fact]
        public void CanReadDocumentThatWasSavedEncrypted()
        { 
            // document that is known to be AES-encrypted
            var file = PathHelper.GetInstance().GetAssetPath("AesEncrypted.pdf");
            var document = Pdf.IO.PdfReader.Open(file, PdfDocumentOpenMode.Import);
            
            // import pages into a new document
            var encryptedDoc = new PdfDocument();
            foreach (var page in document.Pages)
                encryptedDoc.AddPage(page);
            
            // save enrypted
            encryptedDoc.SecuritySettings.OwnerPassword = "supersecret!11";
            var saveFileName = PathHelper.GetInstance().GetAssetPath("SavedEncrypted.pdf");
            encryptedDoc.Save(saveFileName);

            // read back and check
            var fi = new FileInfo(saveFileName);
            var readBackDoc = Pdf.IO.PdfReader.Open(saveFileName, PdfDocumentOpenMode.Import);
            AssertIsAValidPdfDocumentWithProperties(readBackDoc, (int)fi.Length);
            readBackDoc.PageCount.Should().Be(document.PageCount);
        }

        [Fact]
        public void DocumentWithUserPasswordCannotBeOpenedWithoutPassword()
        {
            var file = PathHelper.GetInstance().GetAssetPath("AesEncrypted.pdf");
            var document = Pdf.IO.PdfReader.Open(file, PdfDocumentOpenMode.Import);
            
            // import pages into a new document
            var encryptedDoc = new PdfDocument();
            foreach (var page in document.Pages)
                encryptedDoc.AddPage(page);

            // save enrypted
            encryptedDoc.SecuritySettings.UserPassword = "supersecret!11";
            var saveFileName = PathHelper.GetInstance().GetAssetPath("SavedEncrypted.pdf");
            encryptedDoc.Save(saveFileName);

            // should throw because no password was provided
            var ex = Assert.Throws<PdfReaderException>(() =>
            {
                var readBackDoc = Pdf.IO.PdfReader.Open(saveFileName, PdfDocumentOpenMode.Import);
            });
            ex.Message.Should().Contain("A password is required to open the PDF document");

            // check with password
            // TODO: should be checked in a separate test, but i was lazy...
            var fi = new FileInfo(saveFileName);
            var readBackDoc = Pdf.IO.PdfReader.Open(saveFileName, "supersecret!11", PdfDocumentOpenMode.Import);
            AssertIsAValidPdfDocumentWithProperties(readBackDoc, (int)fi.Length);
            readBackDoc.PageCount.Should().Be(document.PageCount);
        }

        [Fact]
        public void WillThrowExceptionWhenReadingInvalidPdf()
        {
            using var fs = File.OpenRead(PathHelper.GetInstance().GetAssetPath("NotAValid.pdf"));
            Action act = () => Pdf.IO.PdfReader.Open(fs, PdfDocumentOpenMode.ReadOnly);
            act.Should().Throw<InvalidOperationException>().WithMessage("The file is not a valid PDF document.");
        }

        private void AssertIsAValidPdfDocumentWithProperties(PdfDocument inputDocument, int expectedFileSize)
        {
            inputDocument.Should().NotBeNull();
            inputDocument.FileSize.Should().Be(expectedFileSize);
            inputDocument.Info.Should().NotBeNull();
            inputDocument.PageCount.Should().BeGreaterThan(0);
        }
    }
}
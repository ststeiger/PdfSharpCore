using FluentAssertions;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Security;
using PdfSharpCore.Test.Helpers;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace PdfSharpCore.Test.Security
{
    public class PdfSecurity
    {
        private readonly ITestOutputHelper output;

        public PdfSecurity(ITestOutputHelper testOutputHelper)
        {
            output = testOutputHelper;
        }

        [Theory]
        [InlineData(PdfDocumentSecurityLevel.Encrypted40Bit, "hunter1")]
        [InlineData(PdfDocumentSecurityLevel.Encrypted128Bit, "hunter1")]
        public void CreateAndReadPasswordProtectedPdf(PdfDocumentSecurityLevel securityLevel, string password)
        {
            var document = new PdfDocument();
            var pageNewRenderer = document.AddPage();
            var renderer = XGraphics.FromPdfPage(pageNewRenderer);
            renderer.DrawString("Test Test Test", new XFont("Arial", 12), XBrushes.Black, new XPoint(12, 12));
            // validate correct handling of unicode strings (issue #264)
            document.Outlines.Add("The only page", pageNewRenderer);
            document.SecuritySettings.DocumentSecurityLevel = securityLevel;
            document.SecuritySettings.UserPassword = password;

            using var ms = new MemoryStream();
            document.Save(ms);
            ms.Position = 0;

            var loadDocument = Pdf.IO.PdfReader.Open(ms, PdfDocumentOpenMode.Modify,
                delegate(PdfPasswordProviderArgs args) { args.Password = password; });

            loadDocument.PageCount.Should().Be(1);
            loadDocument.Outlines[0].Title.Should().Be("The only page");
            loadDocument.Info.Producer.Should().Contain("PDFsharp");
        }

        [Fact]
        public void ShouldBeAbleToOpenAesEncryptedDocuments()
        {
            // this document has a V value of 4 (see PdfReference 1.7, Chapter 7.6.1, Table 20)
            // and an R value of 4 (see PdfReference 1.7, Chapter 7.6.3.2, Table 21)
            // see also: Adobe Supplement to the ISO 32000, BaseVersion: 1.7, ExtensionLevel: 3
            //           Chapter 3.5.2, Table 3.19
            var file = PathHelper.GetInstance().GetAssetPath("AesEncrypted.pdf");
            var fi = new FileInfo(file);
            var document = Pdf.IO.PdfReader.Open(file, PdfDocumentOpenMode.Import);

            // verify document was actually AES-encrypted
            var cf = document.SecurityHandler.Elements.GetDictionary("/CF");
            var stdCf = cf.Elements.GetDictionary("/StdCF");
            stdCf.Elements.GetString("/CFM").Should().Be("/AESV2");

            IO.PdfReader.AssertIsAValidPdfDocumentWithProperties(document, (int)fi.Length);
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
            IO.PdfReader.AssertIsAValidPdfDocumentWithProperties(readBackDoc, (int)fi.Length);
            readBackDoc.PageCount.Should().Be(document.PageCount);
        }

        // Same PDF protected by different tools or online-services
        [Theory]
        // https://www.ilovepdf.com/protect-pdf, 128 bit, /V 2 /R 3
        [InlineData(@"protected-ilovepdf.pdf", "test123")]
        
        // https://www.adobe.com/de/acrobat/online/password-protect-pdf.html, 128 bit, /V 4 /R 4
        [InlineData(@"protected-adobe.pdf", "test123")]

        // https://pdfencrypt.net, 256 bit, /V 5 /R 5
        [InlineData(@"protected-pdfencrypt.pdf", "test123")]

        // https://www.sodapdf.com/password-protect-pdf/
        // this is the only tool tested, that encrypts with the latest known algorithm (256 bit, /V 5 /R 6)
        // Note: SodaPdf also produced a pdf that would be considered "invalid" by PdfSharp, because of incorrect stream-lengths
        // (in the Stream-Dictionary, the length was reported as 32, but in fact the length was 16)
        // this needed to be handled as well
        [InlineData(@"protected-sodapdf.pdf", "test123")]
        public void CanReadPdfEncryptedWithSupportedAlgorithms(string fileName, string password)
        {
            var path = PathHelper.GetInstance().GetAssetPath(fileName);

            var doc = Pdf.IO.PdfReader.Open(path, password, PdfDocumentOpenMode.Import);
            doc.Should().NotBeNull();
            doc.PageCount.Should().BeGreaterThan(0);
            output.WriteLine("Creator : {0}", doc.Info.Creator);
            output.WriteLine("Producer: {0}", doc.Info.Producer);
        }
    }
}
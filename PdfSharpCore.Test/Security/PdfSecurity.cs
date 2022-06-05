using System.IO;
using FluentAssertions;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Security;
using Xunit;

namespace PdfSharpCore.Test.Security
{
    public class PdfSecurity
    {
        [Theory]
        [InlineData(PdfDocumentSecurityLevel.Encrypted40Bit, "hunter1")]
        [InlineData(PdfDocumentSecurityLevel.Encrypted128Bit, "hunter1")]
        public void CreateAndReadPasswordProtectedPdf(PdfDocumentSecurityLevel securityLevel, string password)
        {
            var document = new PdfDocument();
            var pageNewRenderer = document.AddPage();
            var renderer = XGraphics.FromPdfPage(pageNewRenderer);
            renderer.DrawString("Test Test Test", new XFont("Arial", 12), XBrushes.Black, new XPoint(12, 12));
            document.SecuritySettings.DocumentSecurityLevel = securityLevel;
            document.SecuritySettings.UserPassword = password;

            using var ms = new MemoryStream();
            document.Save(ms);
            ms.Position = 0;

            var loadDocument = Pdf.IO.PdfReader.Open(ms, PdfDocumentOpenMode.Modify,
                delegate(PdfPasswordProviderArgs args) { args.Password = password; });

            loadDocument.PageCount.Should().Be(1);
        }
    }
}
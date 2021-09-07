using System;
using Xunit;

namespace PdfSharpCore.Test
{
    public class PdfInteger
    {
        [Fact]
        public void Should_beAbleToConvertToInt32()
        {
            var pdfInt = new Pdf.PdfInteger(10);
            var convertedInt = Convert.ToInt32(pdfInt);
            Assert.Equal(10, convertedInt);
        }
    }
}

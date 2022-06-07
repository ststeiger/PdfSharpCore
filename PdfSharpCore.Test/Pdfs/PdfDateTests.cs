using System;
using FluentAssertions;
using PdfSharpCore.Pdf;
using Xunit;

namespace PdfSharpCore.Test.Pdfs
{
    public class PdfDateTests
    {
        // format for pdf date is generally D:YYYYMMDDHHmmSSOHH'mm'

        [Fact]
        public void ParseDateString_WithTimezoneOffset()
        {
            var pdfDate = new PdfDate("D:19981223195200-02'00'");
            var expectedDateWithOffset = new DateTimeOffset(new DateTime(1998, 12, 23, 19, 52, 0), new TimeSpan(-2, 0, 0));
            pdfDate.Value.ToUniversalTime().Should().Be(expectedDateWithOffset.UtcDateTime);
        }

        [Fact]
        public void ParseDateString_WithNoOffset()
        {
            var pdfDate = new PdfDate("D:19981223195200Z");
            var expectedDateWithOffset = new DateTimeOffset(new DateTime(1998, 12, 23, 19, 52, 0), new TimeSpan(0, 0, 0));
            pdfDate.Value.ToUniversalTime().Should().Be(expectedDateWithOffset.UtcDateTime);
        }
    }
}
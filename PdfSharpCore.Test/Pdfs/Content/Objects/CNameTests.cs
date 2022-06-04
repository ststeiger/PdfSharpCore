using System;
using FluentAssertions;
using PdfSharpCore.Pdf.Content.Objects;
using Xunit;

namespace PdfSharpCore.Test.Pdfs.Content.Objects
{
    public class CNameTests
    {
        [Theory]
        [InlineData("/Foo")]
        public void SetNameTests(string name)
        {
            var cName = new CName
            {
                Name = name
            };

            cName.Name.Should().Be(name);
        }

        [Fact]
        public void SetNameNullThrowsException()
        {
            Action act = () => new CName
            {
                Name = null
            };
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("")]
        public void SetNameWithoutPrefixThrowsException(string name)
        {
            Action act = () => new CName
            {
                Name = name
            };
            act.Should().Throw<ArgumentException>();
        }
    }
}
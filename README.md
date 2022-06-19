# PdfSharpCore

[![NuGet Version](https://img.shields.io/nuget/v/PdfSharpCore.svg)](https://www.nuget.org/packages/PdfSharpCore/)
[![CI](https://github.com/ststeiger/PdfSharpCore/actions/workflows/build.yml/badge.svg)](https://github.com/ststeiger/PdfSharpCore/actions/workflows/build.yml)
[![codecov.io](https://codecov.io/github/ststeiger/PdfSharpCore/coverage.svg?branch=master)](https://codecov.io/github/ststeiger/PdfSharpCore?branch=master)

**PdfSharpCore** is a partial port of [PdfSharp.Xamarin](https://github.com/roceh/PdfSharp.Xamarin/) for .NET Standard.
Additionally MigraDoc has been ported as well (from version 1.32).
Image support has been implemented with [SixLabors.ImageSharp](https://github.com/JimBobSquarePants/ImageSharp/) and Fonts support with [SixLabors.Fonts](https://github.com/SixLabors/Fonts).


## Table of Contents

- [Documentation](docs/index.md)
- [Example](#example)
- [Contributing](#contributing)
- [License](#license)


## Example

```cs
static void Main(string[] args)
{
    var document = new PdfDocument();
    var page = document.AddPage();
    var gfx = XGraphics.FromPdfPage(page);
    var font = new XFont("OpenSans", 20, XFontStyle.Bold);
            
    gfx.DrawString(
        "Hello World!", font, XBrushes.Black,
        new XRect(20, 20, page.Width, page.Height),
        XStringFormats.Center);

    document.Save("test.pdf");
}
```


## Contributing

We appreciate feedback and contribution to this repo!


## License

This software is released under the MIT License. See the [LICENSE](LICENCE.md) file for more info.

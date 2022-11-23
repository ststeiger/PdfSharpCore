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

The following code snippet creates a simple PDF-file with the text 'Hello World!'.
The code is written for a .NET 6 console app with top level statements.

```csharp
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

GlobalFontSettings.FontResolver = new FontResolver();

var document = new PdfDocument();
var page = document.AddPage();

var gfx = XGraphics.FromPdfPage(page);
var font = new XFont("Arial", 20, XFontStyle.Bold);

var textColor = XBrushes.Black;
var layout = new XRect(20, 20, page.Width, page.Height);
var format = XStringFormats.Center;

gfx.DrawString("Hello World!", font, textColor, layout, format);

document.Save("helloworld.pdf");
```

## Contributing

We appreciate feedback and contribution to this repo!


## License

This software is released under the MIT License. See the [LICENSE](LICENCE.md) file for more info.

PdfSharpCore relies on the following projects, that are not under the MIT license:

* *SixLabors.ImageSharp* and *SixLabors.Fonts*
  * SixLabors.ImageSharp and SixLabors.Fonts, libraries which PdfSharpCore relies upon, are licensed under Apache 2.0 when distributed as part of PdfSharpCore. The SixLabors.ImageSharp license covers all other usage, see https://github.com/SixLabors/ImageSharp/blob/master/LICENSE
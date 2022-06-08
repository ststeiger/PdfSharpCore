# PdfSharpCore

[![codecov.io](https://codecov.io/github/ststeiger/PdfSharpCore/coverage.svg?branch=master)](https://codecov.io/github/ststeiger/PdfSharpCore?branch=master) ![Nuget](https://img.shields.io/nuget/v/PdfSharpCore)

## About

**PdfSharpCore** is a partial port of [PdfSharp.Xamarin](https://github.com/roceh/PdfSharp.Xamarin/) for .NET Standard.
Additionally MigraDoc has been ported as well (from version 1.32).<br />

Support for images has been implemented with [ImageSharp](https://github.com/SixLabors/ImageSharp). 

## Example project 

An example project is not included in this repository, but exists in the repository linked below.

[Go to the example project](https://github.com/ststeiger/Stammbaum)

### Font resolving

There's a default font-resolver in [`FontResolver.cs`](https://github.com/ststeiger/PdfSharpCore/blob/master/PdfSharpCore/Utils/FontResolver.cs).<br />
It should work on Windows, Linux, OSX and Azure. <br />
Some limitations apply. <br />
See open issues.

## Usage

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

See the [example project](#example-project) for MigraDoc usage.

## License

Distributed under the MIT License. See [`LICENSE.md`](LICENSE.md) for more information.
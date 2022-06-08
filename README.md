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

```csharp
// See the "Example" Project for a MigraDoc example
static void Main(string[] args)
{
    GlobalFontSettings.FontResolver = new FontResolver();
    
    var document = new PdfDocument();
    var page = document.AddPage();
    var gfx = XGraphics.FromPdfPage(page);
    var font = new XFont("OpenSans", 20, XFontStyle.Bold);
            
    gfx.DrawString("Hello World!", font, XBrushes.Black, new XRect(20, 20, page.Width, page.Height), XStringFormats.Center);

    document.Save("test.pdf");
}

// This implementation is obviously not very good --> Though it should be enough for everyone to implement their own.
public class FontResolver : IFontResolver
{
    public byte[] GetFont(string faceName)
    {
        using(var ms = new MemoryStream())
        {
            using(var fs = File.Open(faceName, FileMode.Open))
            {
                fs.CopyTo(ms);
                ms.Position = 0;
                return ms.ToArray();
                }
            }
        }
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("OpenSans", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("OpenSans-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("OpenSans-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("OpenSans-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("OpenSans-Regular.ttf");
                }
            }
            return null;
        }
    }
}
```

## License

Distributed under the MIT License. See [`LICENSE.md`](LICENSE.md) for more information.
# PdfSharpCore

**PdfSharpCore** is a partial port of [PdfSharp.Xamarin](https://github.com/roceh/PdfSharp.Xamarin/) for .NET Standard
Additionally MigraDoc has been ported as well (from version 1.32).
Images have been implemented with [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp/), which is still in Alpha. They State on their readme that it is still in Alpha status and shouldn't be used in productive environments. Since I didn't find any good alternatives it's still used.

ImageSharp beeing Alpha isn't a big issure either since this code isn't by far done yet. So please chime in ;)

**PdfSharp.Xamarin** is a partial port of [PdfSharp](http://www.pdfsharp.net/) for iOS and Android using Xamarin, it allows for creation and modification of PDF files.

## Example

```cs
//See the "Example" Project for a MigraDoc example
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

//This implementation is obviously not very good --> Though it should be enough for everyone to implement their own.
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

Copyright (c) 2005-2007 empira Software GmbH, Cologne (Germany)  
Modified work Copyright (c) 2016 David Dunscombe

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

# PdfSharpCore

PdfSharpCore is a .NET library for processing PDF file.
You create PDF pages using drawing routines known from GDI+.
Almost anything that can be done with GDI+ will also work with PdfSharpCore.
Keep in mind it does no longer depend on GDI+, as it was ported to make use of [ImageSharp](https://github.com/SixLabors/ImageSharp).
Only basic text layout is supported by PdfSharpCore, and page breaks are not created automatically.
The same drawing routines can be used for screen, PDF, or meta files.

* [Features](#features)
* [First steps](#first-steps)
* [Samples](samples/index.md)
* [FAQ](faq.md)


## Features

* Creates PDF documents on the fly from any .NET language
* Easy to understand object model to compose documents
* One source code for drawing on a PDF page as well as in a window or on the printer
* Modify, merge, and split existing PDF files
* Images with transparency (color mask, monochrome mask, alpha mask)
* Newly designed from scratch and written entirely in C#
* The graphical classes go well with .NET


## First steps

Both PdfSharpCore and MigraDocCore provide a lot of `AddXxx` functions.
Typically these functions return the newly created objects. Once you’ve learned the basic principles it’s quite easy to work with.
Intellisense helps a lot then.

We’ll discuss a few lines of the [Hello World](samples/HelloWorld.md) sample here.

```cs
// You’ll first need a PDF document:
PdfDocument document = new PdfDocument();

// And you need a page:
PdfPage page = document.AddPage();

// Drawing is done with an XGraphics object:
XGraphics gfx = XGraphics.FromPdfPage(page);

// Then you'll create a font:
XFont font = new XFont("Verdana", 20, XFontStyle.Bold);

// And you'll create an alignment to your text
XStringFormat stringFormat = new XStringFormat
{
    Alignment = XStringAlignment.Center
};

// And you use that font and alignment to draw a string:
gfx.DrawString(
    "Hello, World!", font, XBrushes.Black,
    new XRect(0, 0, page.Width, page.Height),
    stringFormat);

// When drawing is done, write the file:
string filename = "HelloWorld.pdf";
document.Save(filename);
```

# Hello World

Is the obligatory "Hello World" program.


## Code

```cs
using System;
using System.Diagnostics;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace HelloWorld
{
    /// <summary>
    /// This sample is the obligatory Hello World program.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PdfSharpCore";
            
            // Create an empty page
            PdfPage page = document.AddPage();
            
            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);
            
            // Create a font
            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            
            // Draw the text
            gfx.DrawString(
                "Hello, World!", font, XBrushes.Black,
                new XRect(0, 0, page.Width, page.Height),
                XStringFormats.Center);
            
            // Save the document...
            const string filename = "HelloWorld.pdf";
            document.Save(filename);
        }
    }
}
```

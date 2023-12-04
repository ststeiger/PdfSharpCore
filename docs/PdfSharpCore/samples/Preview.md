# Preview

This sample shows how to render graphics in both a preview and a PDF document.

## Code

This is the source code of the Render method that is called for screen, PDF, and printer output:

```cs
XRect rect;
XPen pen;
double x = 50, y = 100;
XFont fontH1 = new XFont("Times", 18, XFontStyle.Bold);
XFont font = new XFont("Times", 12);
XFont fontItalic = new XFont("Times", 12, XFontStyle.BoldItalic);
double ls = font.GetHeight(gfx);
 
// Draw some text
gfx.DrawString("Create PDF on the fly with PdfSharpCore",
fontH1, XBrushes.Black, x, x);
gfx.DrawString("With PdfSharpCore you can use the same code to draw graphic, " +
"text and images on different targets.", font, XBrushes.Black, x, y);
y += ls;
gfx.DrawString("The object used for drawing is the XGraphics object.",
font, XBrushes.Black, x, y);
y += 2 * ls;
 
// Draw an arc
pen = new XPen(XColors.Red, 4);
pen.DashStyle = XDashStyle.Dash;
gfx.DrawArc(pen, x + 20, y, 100, 60, 150, 120);
 
// Draw a star
XGraphicsState gs = gfx.Save();
gfx.TranslateTransform(x + 140, y + 30);
for (int idx = 0; idx < 360; idx += 10)
{
    gfx.RotateTransform(10);
    gfx.DrawLine(XPens.DarkGreen, 0, 0, 30, 0);
}
gfx.Restore(gs);
 
// Draw a rounded rectangle
rect = new XRect(x + 230, y, 100, 60);
pen = new XPen(XColors.DarkBlue, 2.5);
XColor color1 = XColor.FromKnownColor(KnownColor.DarkBlue);
XColor color2 = XColors.Red;
XLinearGradientBrush lbrush = new XLinearGradientBrush(rect, color1, color2,
XLinearGradientMode.Vertical);
gfx.DrawRoundedRectangle(pen, lbrush, rect, new XSize(10, 10));
 
// Draw a pie
pen = new XPen(XColors.DarkOrange, 1.5);
pen.DashStyle = XDashStyle.Dot;
gfx.DrawPie(pen, XBrushes.Blue, x + 360, y, 100, 60, -130, 135);
 
// Draw some more text
y += 60 + 2 * ls;
gfx.DrawString("With XGraphics you can draw on a PDF page as well as " +
"on any System.Drawing.Graphics object.", font, XBrushes.Black, x, y);
y += ls * 1.1;
gfx.DrawString("Use the same code to", font, XBrushes.Black, x, y);
x += 10;
y += ls * 1.1;
gfx.DrawString("• draw on a newly created PDF page", font, XBrushes.Black, x, y);
y += ls;
gfx.DrawString("• draw above or beneath of the content of an existing PDF page",
font, XBrushes.Black, x, y);
y += ls;
gfx.DrawString("• draw in a window", font, XBrushes.Black, x, y);
y += ls;
gfx.DrawString("• draw on a printer", font, XBrushes.Black, x, y);
y += ls;
gfx.DrawString("• draw in a bitmap image", font, XBrushes.Black, x, y);
x -= 10;
y += ls * 1.1;
gfx.DrawString("You can also import an existing PDF page and use it like " +
"an image, e.g. draw it on another PDF page.", font, XBrushes.Black, x, y);
y += ls * 1.1 * 2;
gfx.DrawString("Imported PDF pages are neither drawn nor printed; create a " +
"PDF file to see or print them!", fontItalic, XBrushes.Firebrick, x, y);
y += ls * 1.1;
gfx.DrawString("Below this text is a PDF form that will be visible when " +
"viewed or printed with a PDF viewer.", fontItalic, XBrushes.Firebrick, x, y);
y += ls * 1.1;
XGraphicsState state = gfx.Save();
XRect rcImage = new XRect(100, y, 100, 100 * Math.Sqrt(2));
gfx.DrawRectangle(XBrushes.Snow, rcImage);
gfx.DrawImage(XPdfForm.FromFile("../../../../../PDFs/SomeLayout.pdf"), rcImage);
gfx.Restore(state);
```

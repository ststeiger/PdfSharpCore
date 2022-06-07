# Graphics

Shows some of the capabilities of the XGraphics class.
You'll find code snippets for the following graphical primitives:

* [Lines and curves](#lines-and-curves)
* [Shapes](#shapes)
* [Graphical paths](#paths-class)
* [Text and fonts](#text-class)
* [Images and external PDF forms](#images) 


##  Main program

This is the main function. It creates a PDF document and adds some sample pages listed below.

```cs
static void Main()
{
    // Create a temporary file
    string filename = String.Format("{0}_tempfile.pdf", Guid.NewGuid().ToString("D").ToUpper());
    s_document = new PdfDocument();
    s_document.Info.Title = "PdfSharpCore XGraphic Sample";
    s_document.Info.Author = "Stefan Lange";
    s_document.Info.Subject = "Created with code snippets that show the use of graphical functions";
    s_document.Info.Keywords = "PdfSharpCore, XGraphics";
    
    // Create demonstration pages
    new LinesAndCurves().DrawPage(s_document.AddPage());
    new Shapes().DrawPage(s_document.AddPage());
    new Paths().DrawPage(s_document.AddPage());
    new Text().DrawPage(s_document.AddPage());
    new Images().DrawPage(s_document.AddPage());

    // Save the s_document...
    s_document.Save(filename);
}
```


## Lines and Curves

Shows how to draw lines and curves.

```cs
public void DrawPage(PdfPage page)
{
    XGraphics gfx = XGraphics.FromPdfPage(page);
    DrawTitle(page, gfx, "Lines & Curves");
    
    DrawLine(gfx, 1);
    DrawLines(gfx, 2);
    DrawBezier(gfx, 3);
    DrawBeziers(gfx, 4);
    DrawCurve(gfx, 5);
    DrawArc(gfx, 6);
}
```

### Draw simple lines

```cs
void DrawLine(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawLine");
    
    gfx.DrawLine(XPens.DarkGreen, 0, 0, 250, 0);
    gfx.DrawLine(XPens.Gold, 15, 7, 230, 15);
    
    XPen pen = new XPen(XColors.Navy, 4);
    gfx.DrawLine(pen, 0, 20, 250, 20);
    
    pen = new XPen(XColors.Firebrick, 6);
    pen.DashStyle = XDashStyle.Dash;
    gfx.DrawLine(pen, 0, 40, 250, 40);

    pen.Width = 7.3;
    pen.DashStyle = XDashStyle.DashDotDot;
    gfx.DrawLine(pen, 0, 60, 250, 60);
    
    pen = new XPen(XColors.Goldenrod, 10);
    pen.LineCap = XLineCap.Flat;
    gfx.DrawLine(pen, 10, 90, 240, 90);
    gfx.DrawLine(XPens.Black, 10, 90, 240, 90);
    
    pen = new XPen(XColors.Goldenrod, 10);
    pen.LineCap = XLineCap.Square;
    gfx.DrawLine(pen, 10, 110, 240, 110);
    gfx.DrawLine(XPens.Black, 10, 110, 240, 110);

    pen = new XPen(XColors.Goldenrod, 10);
    pen.LineCap = XLineCap.Round;
    gfx.DrawLine(pen, 10, 130, 240, 130);
    gfx.DrawLine(XPens.Black, 10, 130, 240, 130);
    
    EndBox(gfx);
}
```

### Draw a polyline

```cs
void DrawLines(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawLines");
    
    XPen pen = new XPen(XColors.DarkSeaGreen, 6);
    pen.LineCap = XLineCap.Round;
    pen.LineJoin = XLineJoin.Bevel;
    XPoint[] points = new XPoint[]
    {
        new XPoint(20, 30), new XPoint(60, 120),
        new XPoint(90, 20), new XPoint(170, 90),
        new XPoint(230, 40)
    };
    gfx.DrawLines(pen, points);
    
    EndBox(gfx);
}
```

### Draw a single Bézier curve

```cs
void DrawBezier(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawBezier");
    
    gfx.DrawBezier(new XPen(XColors.DarkRed, 5), 20, 110, 40, 10, 170, 90, 230, 20);
    
    EndBox(gfx);
}
```

### Draw two Bézier curves

```cs
void DrawBeziers(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawBeziers");
    
    XPoint[] points = new XPoint[]
    {
        new XPoint(20, 30), new XPoint(40, 120),
        new XPoint(80, 20), new XPoint(110, 90),
        new XPoint(180, 40), new XPoint(210, 40),
        new XPoint(220, 80)
    };
    XPen pen = new XPen(XColors.Firebrick, 4);
    gfx.DrawBeziers(pen, points);
    
    EndBox(gfx);
}
```

### Draw a cardinal spline

```cs
void DrawCurve(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawCurve");
    
    XPoint[] points = new XPoint[]
    {
        new XPoint(20, 30), new XPoint(60, 120),
        new XPoint(90, 20), new XPoint(170, 90),
        new XPoint(230, 40)
    };
    XPen pen = new XPen(XColors.RoyalBlue, 3.5);
    gfx.DrawCurve(pen, points, 1);
    
    EndBox(gfx);
}
```

### Draw an arc

```cs
void DrawArc(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawArc");
    
    XPen pen = new XPen(XColors.Plum, 4.7);
    gfx.DrawArc(pen, 0, 0, 250, 140, 190, 200);
    
    EndBox(gfx);
}
```


## Shapes

Shows how to draw graphical shapes.

```cs
public void DrawPage(PdfPage page)
{
    XGraphics gfx = XGraphics.FromPdfPage(page);
    DrawTitle(page, gfx, "Shapes");

    DrawRectangle(gfx, 1);
    DrawRoundedRectangle(gfx, 2);
    DrawEllipse(gfx, 3);
    DrawPolygon(gfx, 4);
    DrawPie(gfx, 5);
    DrawClosedCurve(gfx, 6);
}
```

### Draw rectangles

```cs
void DrawRectangle(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawRectangle");
    
    XPen pen = new XPen(XColors.Navy, Math.PI);

    gfx.DrawRectangle(pen, 10, 0, 100, 60);
    gfx.DrawRectangle(XBrushes.DarkOrange, 130, 0, 100, 60);
    gfx.DrawRectangle(pen, XBrushes.DarkOrange, 10, 80, 100, 60);
    gfx.DrawRectangle(pen, XBrushes.DarkOrange, 150, 80, 60, 60);
    
    EndBox(gfx);
}
```

### Draw rounded rectangles

```cs
void DrawRoundedRectangle(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawRoundedRectangle");
    
    XPen pen = new XPen(XColors.RoyalBlue, Math.PI);
    
    gfx.DrawRoundedRectangle(pen, 10, 0, 100, 60, 30, 20);
    gfx.DrawRoundedRectangle(XBrushes.Orange, 130, 0, 100, 60, 30, 20);
    gfx.DrawRoundedRectangle(pen, XBrushes.Orange, 10, 80, 100, 60, 30, 20);
    gfx.DrawRoundedRectangle(pen, XBrushes.Orange, 150, 80, 60, 60, 20, 20);
    
    EndBox(gfx);
}
```

### Draw ellipses

```cs
void DrawEllipse(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawEllipse");
    
    XPen pen = new XPen(XColors.DarkBlue, 2.5);
    
    gfx.DrawEllipse(pen, 10, 0, 100, 60);
    gfx.DrawEllipse(XBrushes.Goldenrod, 130, 0, 100, 60);
    gfx.DrawEllipse(pen, XBrushes.Goldenrod, 10, 80, 100, 60);
    gfx.DrawEllipse(pen, XBrushes.Goldenrod, 150, 80, 60, 60);
    
    EndBox(gfx);
}
```

### Draw polygons

```cs
void DrawPolygon(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawPolygon");
    
    XPen pen = new XPen(XColors.DarkBlue, 2.5);
    
    gfx.DrawPolygon(pen, XBrushes.LightCoral, GetPentagram(50, new XPoint(60, 70)), XFillMode.Winding);
    gfx.DrawPolygon(pen, XBrushes.LightCoral, GetPentagram(50, new XPoint(180, 70)), XFillMode.Alternate);
    
    EndBox(gfx);
}
```

### Draw pies

```cs
void DrawPie(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawPie");
    
    XPen pen = new XPen(XColors.DarkBlue, 2.5);
    
    gfx.DrawPie(pen, 10, 0, 100, 90, -120, 75);
    gfx.DrawPie(XBrushes.Gold, 130, 0, 100, 90, -160, 150);
    gfx.DrawPie(pen, XBrushes.Gold, 10, 50, 100, 90, 80, 70);
    gfx.DrawPie(pen, XBrushes.Gold, 150, 80, 60, 60, 35, 290);
    
    EndBox(gfx);
}
```

### Draw a closed cardinal spline

```cs
void DrawClosedCurve(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawClosedCurve");
    
    XPen pen = new XPen(XColors.DarkBlue, 2.5);
    gfx.DrawClosedCurve(
        pen, XBrushes.SkyBlue,
        new XPoint[]
        {
            new XPoint(10, 120), new XPoint(80, 30),
            new XPoint(220, 20), new XPoint(170, 110),
            new XPoint(100, 90)
        },
        XFillMode.Winding, 0.7);

    EndBox(gfx);
}
```

### Paths class

Shows how to draw graphical paths.

```cs
public void DrawPage(PdfPage page)
{
    XGraphics gfx = XGraphics.FromPdfPage(page);
    
    DrawTitle(page, gfx, "Paths");
    
    DrawPathOpen(gfx, 1);
    DrawPathClosed(gfx, 2);
    DrawPathAlternateAndWinding(gfx, 3);
    DrawGlyphs(gfx, 5);
    DrawClipPath(gfx, 6);
}
```

### Stroke an open path

```cs
void DrawPathOpen(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawPath (open)");
    
    XPen pen = new XPen(XColors.Navy, Math.PI);
    pen.DashStyle = XDashStyle.Dash;
    
    XGraphicsPath path = new XGraphicsPath();
    path.AddLine(10, 120, 50, 60);
    path.AddArc(50, 20, 110, 80, 180, 180);
    path.AddLine(160, 60, 220, 100);
    gfx.DrawPath(pen, path);
    
    EndBox(gfx);
}
```

### Stroke a closed path

```cs
void DrawPathClosed(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawPath (closed)");
    
    XPen pen = new XPen(XColors.Navy, Math.PI);
    pen.DashStyle = XDashStyle.Dash;
    
    XGraphicsPath path = new XGraphicsPath();
    path.AddLine(10, 120, 50, 60);
    path.AddArc(50, 20, 110, 80, 180, 180);
    path.AddLine(160, 60, 220, 100);
    path.CloseFigure();
    gfx.DrawPath(pen, path);
    
    EndBox(gfx);
}
```

### Draw an alternating and a winding path

```cs
void DrawPathAlternateAndWinding(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawPath (alternate / winding)");
    
    XPen pen = new XPen(XColors.Navy, 2.5);
    
    // Alternate fill mode
    XGraphicsPath path = new XGraphicsPath();
    path.FillMode = XFillMode.Alternate;
    path.AddLine(10, 130, 10, 40);
    path.AddBeziers(
        new XPoint[]
        {
            new XPoint(10, 40), new XPoint(30, 0),
            new XPoint(40, 20), new XPoint(60, 40),
            new XPoint(80, 60), new XPoint(100, 60),
            new XPoint(120, 40)
        }
    );
    path.AddLine(120, 40, 120, 130);
    path.CloseFigure();
    path.AddEllipse(40, 80, 50, 40);
    gfx.DrawPath(pen, XBrushes.DarkOrange, path);
    
    // Winding fill mode
    path = new XGraphicsPath();
    path.FillMode = XFillMode.Winding;
    path.AddLine(130, 130, 130, 40);
    path.AddBeziers(
        new XPoint[]
        {
            new XPoint(130, 40), new XPoint(150, 0),
            new XPoint(160, 20), new XPoint(180, 40),
            new XPoint(200, 60), new XPoint(220, 60),
            new XPoint(240, 40)
        }
    );
    path.AddLine(240, 40, 240, 130);
    path.CloseFigure();
    path.AddEllipse(160, 80, 50, 40);
    gfx.DrawPath(pen, XBrushes.DarkOrange, path);
    
    EndBox(gfx);
}
```

### Convert text to path

```cs
void DrawGlyphs(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "Draw Glyphs");
    
    XGraphicsPath path = new XGraphicsPath();
    path.AddString(
        "Hello!", new XFontFamily("Times New Roman"),
        XFontStyle.BoldItalic, 100,
        new XRect(0, 0, 250, 140),
        XStringFormats.Center
    );
    
    gfx.DrawPath(new XPen(XColors.Purple, 2.3), XBrushes.DarkOrchid, path);
    
    EndBox(gfx);
}
```

### Clip through path

```cs
void DrawClipPath(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "Clip through Path");
    
    XGraphicsPath path = new XGraphicsPath();
    path.AddString(
        "Clip!", new XFontFamily("Verdana"),
        XFontStyle.Bold, 90,
        new XRect(0, 0, 250, 140),
        XStringFormats.Center
    );
    
    gfx.IntersectClip(path);
    
    // Draw a beam of dotted lines
    XPen pen = XPens.DarkRed.Clone();
    pen.DashStyle = XDashStyle.Dot;
    for (double r = 0; r <= 90; r += 0.5)
    {
        gfx.DrawLine(
            pen, 0, 0,
            250 * Math.Cos(r / 90 * Math.PI),
            250 * Math.Sin(r / 90 * Math.PI)
        );
    }

    EndBox(gfx);
}
```

## Text class

Shows how to handle text.

```cs
public void DrawPage(PdfPage page)
{
    XGraphics gfx = XGraphics.FromPdfPage(page);
    DrawTitle(page, gfx, "Text");
    
    DrawText(gfx, 1);
    DrawTextAlignment(gfx, 2);
    MeasureText(gfx, 3);
}
```

### Draw text in different styles

```cs
    void DrawText(XGraphics gfx, int number)
    {
        BeginBox(gfx, number, "Text Styles");
    
        const string facename = "Times New Roman";
        
        //XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
        XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.WinAnsi, PdfFontEmbedding.Default);
        
        XFont fontRegular = new XFont(facename, 20, XFontStyle.Regular, options);
        XFont fontBold = new XFont(facename, 20, XFontStyle.Bold, options);
        XFont fontItalic = new XFont(facename, 20, XFontStyle.Italic, options);
        XFont fontBoldItalic = new XFont(facename, 20, XFontStyle.BoldItalic, options);
        
        // The default alignment is baseline left (that differs from GDI+)
        gfx.DrawString("Times (regular)", fontRegular, XBrushes.DarkSlateGray, 0, 30);
        gfx.DrawString("Times (bold)", fontBold, XBrushes.DarkSlateGray, 0, 65);
        gfx.DrawString("Times (italic)", fontItalic, XBrushes.DarkSlateGray, 0, 100);
        gfx.DrawString("Times (bold italic)", fontBoldItalic, XBrushes.DarkSlateGray, 0, 135);
        
        EndBox(gfx);
    }
```

### Show how to align text in the layout rectangle

```cs
void DrawTextAlignment(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "Text Alignment");
    XRect rect = new XRect(0, 0, 250, 140);
    
    XFont font = new XFont("Verdana", 10);
    XBrush brush = XBrushes.Purple;
    XStringFormat format = new XStringFormat();
    
    gfx.DrawRectangle(XPens.YellowGreen, rect);
    gfx.DrawLine(XPens.YellowGreen, rect.Width / 2, 0, rect.Width / 2, rect.Height);
    gfx.DrawLine(XPens.YellowGreen, 0, rect.Height / 2, rect.Width, rect.Height / 2);
    
    gfx.DrawString("TopLeft", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Center;
    gfx.DrawString("TopCenter", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Far;
    gfx.DrawString("TopRight", font, brush, rect, format);
    
    format.LineAlignment = XLineAlignment.Center;
    format.Alignment = XStringAlignment.Near;
    gfx.DrawString("CenterLeft", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Center;
    gfx.DrawString("Center", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Far;
    gfx.DrawString("CenterRight", font, brush, rect, format);
    
    format.LineAlignment = XLineAlignment.Far;
    format.Alignment = XStringAlignment.Near;
    gfx.DrawString("BottomLeft", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Center;
    gfx.DrawString("BottomCenter", font, brush, rect, format);
    
    format.Alignment = XStringAlignment.Far;
    gfx.DrawString("BottomRight", font, brush, rect, format);
    
    EndBox(gfx);
}
```

### Show how to get text metric information

```cs
void MeasureText(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "Measure Text");
    
    const XFontStyle style = XFontStyle.Regular;
    XFont font = new XFont("Times New Roman", 95, style);
    
    const string text = "Hallo";
    const double x = 20, y = 100;
    XSize size = gfx.MeasureString(text, font);
    
    double lineSpace = font.GetHeight(gfx);
    int cellSpace = font.FontFamily.GetLineSpacing(style);
    int cellAscent = font.FontFamily.GetCellAscent(style);
    int cellDescent = font.FontFamily.GetCellDescent(style);
    int cellLeading = cellSpace - cellAscent - cellDescent;
    
    // Get effective ascent
    double ascent = lineSpace * cellAscent / cellSpace;
    gfx.DrawRectangle(XBrushes.Bisque, x, y - ascent, size.Width, ascent);
    
    // Get effective descent
    double descent = lineSpace * cellDescent / cellSpace;
    gfx.DrawRectangle(XBrushes.LightGreen, x, y, size.Width, descent);
    
    // Get effective leading
    double leading = lineSpace * cellLeading / cellSpace;
    gfx.DrawRectangle(XBrushes.Yellow, x, y + descent, size.Width, leading);
    
    // Draw text half transparent
    XColor color = XColors.DarkSlateBlue;
    color.A = 0.6;
    gfx.DrawString(text, font, new XSolidBrush(color), x, y);
    
    EndBox(gfx);
}
```


## Images

Shows how to draw images.

```cs
public void DrawPage(PdfPage page)
{
    XGraphics gfx = XGraphics.FromPdfPage(page);
    DrawTitle(page, gfx, "Images");
    
    DrawImage(gfx, 1);
    DrawImageScaled(gfx, 2);
    DrawImageRotated(gfx, 3);
    DrawImageSheared(gfx, 4);
    DrawGif(gfx, 5);
    DrawPng(gfx, 6);
    DrawTiff(gfx, 7);
    DrawFormXObject(gfx, 8);
}
```

### Draw an image in original size

```cs
void DrawImage(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawImage (original)");
    
    XImage image = XImage.FromFile(jpegSamplePath);
    
    // Left position in point
    double x = (250 - image.PixelWidth * 72 / image.HorizontalResolution) / 2;
    gfx.DrawImage(image, x, 0);
    
    EndBox(gfx);
}
```

### Draw an image scaled

```cs
void DrawImageScaled(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawImage (scaled)");
    
    XImage image = XImage.FromFile(jpegSamplePath);
    gfx.DrawImage(image, 0, 0, 250, 140);
    
    EndBox(gfx);
}
```

### Draw an image transformed

```cs
void DrawImageRotated(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawImage (rotated)");
    
    XImage image = XImage.FromFile(jpegSamplePath);
    
    const double dx = 250, dy = 140;
    
    gfx.TranslateTransform(dx / 2, dy / 2);
    gfx.ScaleTransform(0.7);
    gfx.RotateTransform(-25);
    gfx.TranslateTransform(-dx / 2, -dy / 2);
    
    //XMatrix matrix = new XMatrix();  //XMatrix.Identity;
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, 0, width, height);
    
    EndBox(gfx);
}
```

### Draw an image transformed

```cs
void DrawImageSheared(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawImage (sheared)");
    
    XImage image = XImage.FromFile(jpegSamplePath);
    
    const double dx = 250, dy = 140;
    
    gfx.TranslateTransform(dx / 2, dy / 2);
    gfx.ScaleTransform(-0.7, 0.7);
    gfx.ShearTransform(-0.4, -0.3);
    gfx.TranslateTransform(-dx / 2, -dy / 2);
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, 0, width, height);
    
    EndBox(gfx);
}
```

### Draw a GIF image with transparency

```cs
void DrawGif(XGraphics gfx, int number)
{
    this.backColor = XColors.LightGoldenrodYellow;
    this.borderPen = new XPen(XColor.FromArgb(202, 121, 74), this.borderWidth);
    BeginBox(gfx, number, "DrawImage (GIF)");
    
    XImage image = XImage.FromFile(gifSamplePath);
    
    const double dx = 250, dy = 140;
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);
    
    EndBox(gfx);
}
```

### Draw a PNG image with transparency

```cs
void DrawPng(XGraphics gfx, int number)
{
    BeginBox(gfx, number, "DrawImage (PNG)");
    
    XImage image = XImage.FromFile(pngSamplePath);
    
    const double dx = 250, dy = 140;
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);
    
    EndBox(gfx);
}
```

### Draw a TIFF image with transparency

```cs
void DrawTiff(XGraphics gfx, int number)
{
    XColor oldBackColor = this.backColor;
    this.backColor = XColors.LightGoldenrodYellow;
    BeginBox(gfx, number, "DrawImage (TIFF)");
    
    XImage image = XImage.FromFile(tiffSamplePath);
    
    const double dx = 250, dy = 140;
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);
    
    EndBox(gfx);
    this.backColor = oldBackColor;
}
```

### Draw a form XObject (a page from an external PDF file)

```cs
void DrawFormXObject(XGraphics gfx, int number)
{
    //this.backColor = XColors.LightSalmon;
    BeginBox(gfx, number, "DrawImage (Form XObject)");
    
    XImage image = XImage.FromFile(pdfSamplePath);
    
    const double dx = 250, dy = 140;
    
    gfx.TranslateTransform(dx / 2, dy / 2);
    gfx.ScaleTransform(0.35);
    gfx.TranslateTransform(-dx / 2, -dy / 2);
    
    double width = image.PixelWidth * 72 / image.HorizontalResolution;
    double height = image.PixelHeight * 72 / image.HorizontalResolution;
    
    gfx.DrawImage(image, (dx - width) / 2, (dy - height) / 2, width, height);
    
    EndBox(gfx);
}
```

## Helper Functions

Shows how to draw the page title and the rounded boxes.

### Draw the page title and footer

```cs
public void DrawTitle(PdfPage page, XGraphics gfx, string title)
{
    XRect rect = new XRect(new XPoint(), gfx.PageSize);
    rect.Inflate(-10, -15);
    XFont font = new XFont("Verdana", 14, XFontStyle.Bold);
    gfx.DrawString(title, font, XBrushes.MidnightBlue, rect, XStringFormats.TopCenter);
    
    rect.Offset(0, 5);
    font = new XFont("Verdana", 8, XFontStyle.Italic);
    XStringFormat format = new XStringFormat();
    format.Alignment = XStringAlignment.Near;
    format.LineAlignment = XLineAlignment.Far;
    gfx.DrawString("Created with " + PdfSharp.ProductVersionInfo.Producer, font, XBrushes.DarkOrchid, rect, format);
    
    font = new XFont("Verdana", 8);
    format.Alignment = XStringAlignment.Center;
    gfx.DrawString(Program.s_document.PageCount.ToString(), font, XBrushes.DarkOrchid, rect, format);
    
    Program.s_document.Outlines.Add(title, page, true);
}
```

### Draw the box around the samples

```cs
public void BeginBox(XGraphics gfx, int number, string title)
{
    const int dEllipse = 15;
    XRect rect = new XRect(0, 20, 300, 200);
    if (number % 2 == 0)
        rect.X = 300 - 5;
    rect.Y = 40 + ((number - 1) / 2) * (200 - 5);
    rect.Inflate(-10, -10);
    XRect rect2 = rect;
    rect2.Offset(this.borderWidth, this.borderWidth);
    gfx.DrawRoundedRectangle(
        new XSolidBrush(this.shadowColor),
        rect2,
        new XSize(dEllipse + 8, dEllipse + 8)
    );
    XLinearGradientBrush brush = new XLinearGradientBrush(
        rect, this.backColor, this.backColor2, XLinearGradientMode.Vertical);
    gfx.DrawRoundedRectangle(this.borderPen, brush, rect, new XSize(dEllipse, dEllipse));
    rect.Inflate(-5, -5);
    
    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
    gfx.DrawString(title, font, XBrushes.Navy, rect, XStringFormats.TopCenter);
    
    rect.Inflate(-10, -5);
    rect.Y += 20;
    rect.Height -= 20;
    
    this.state = gfx.Save();
    gfx.TranslateTransform(rect.X, rect.Y);
}

public void EndBox(XGraphics gfx)
{
    gfx.Restore(this.state);
}
```

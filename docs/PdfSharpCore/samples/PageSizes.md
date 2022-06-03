# Page Sizes

This sample shows a document with different page sizes.

Note: You can set the size of a page to any size using the `Width` and `Height` properties. This sample just shows the predefined sizes.


## Code

This is the whole source code needed to create the PDF file:

```cs
// Create a new PDF document
PdfDocument document = new PdfDocument();
 
// Create a font
XFont font = new XFont("Times", 25, XFontStyle.Bold);
 
PageSize[] pageSizes = (PageSize[])Enum.GetValues(typeof(PageSize));
foreach (PageSize pageSize in pageSizes)
{
    if (pageSize == PageSize.Undefined)
        continue;
    
    // One page in Portrait...
    PdfPage page = document.AddPage();
    page.Size = pageSize;
    XGraphics gfx = XGraphics.FromPdfPage(page);
    gfx.DrawString(pageSize.ToString(), font, XBrushes.DarkRed,
    new XRect(0, 0, page.Width, page.Height),
    XStringFormats.Center);
    
    // ... and one in Landscape orientation.
    page = document.AddPage();
    page.Size = pageSize;
    page.Orientation = PageOrientation.Landscape;
    gfx = XGraphics.FromPdfPage(page);
    gfx.DrawString(pageSize + " (landscape)", font,
    XBrushes.DarkRed, new XRect(0, 0, page.Width, page.Height),
    XStringFormats.Center);
}
 
// Save the document...
const string filename = "PageSizes_tempfile.pdf";
document.Save(filename);
```

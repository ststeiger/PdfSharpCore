# Bookmarks

This sample shows how to create bookmarks. Bookmarks are called outlines in the PDF reference manual, that's why you deal with the class PdfOutline.

Acrobat uses the term "bookmark" in its English version and "Lesezeichen" in the German version.


## Code

This is the source code that demonstrates the creation of bookmarks:

```cs
// Create a new PDF document
PdfDocument document = new PdfDocument();
 
// Create a font
XFont font = new XFont("Verdana", 16);
 
// Create first page
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);
gfx.DrawString("Page 1", font, XBrushes.Black, 20, 50, XStringFormats.Default);
 
// Create the root bookmark. You can set the style and the color.
PdfOutline outline = document.Outlines.Add("Root", page, true, PdfOutlineStyle.Bold, XColors.Red);
 
// Create some more pages
for (int idx = 2; idx <= 5; idx++)
{
    page = document.AddPage();
    gfx = XGraphics.FromPdfPage(page);
    
    string text = "Page " + idx;
    gfx.DrawString(text, font, XBrushes.Black, 20, 50, XStringFormats.Default);
    
    // Create a sub bookmark
    outline.Outlines.Add(text, page, true);
}
 
// Save the document...
const string filename = "Bookmarks_tempfile.pdf";
document.Save(filename);
```

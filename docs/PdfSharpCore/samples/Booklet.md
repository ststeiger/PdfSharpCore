# Booklet

Shows how to produce a booklet by placing two pages of an existing document on one landscape orientated page of a new document (for duplex printing).


## Code

Here is the code that does the work:

```cs
// Create the output document
PdfDocument outputDocument = new PdfDocument();
 
// Show single pages
// (Note: one page contains two pages from the source document.
//  If the number of pages of the source document can not be
//  divided by 4, the first pages of the output document will
//  each contain only one page from the source document.)
outputDocument.PageLayout = PdfPageLayout.SinglePage;
 
XGraphics gfx;
 
// Open the external document as XPdfForm object
XPdfForm form = XPdfForm.FromFile(filename);
// Determine width and height
double extWidth = form.PixelWidth;
double extHeight = form.PixelHeight;
 
int inputPages = form.PageCount;
int sheets = inputPages / 4;
if (sheets * 4 < inputPages)
sheets += 1;
int allpages = 4 * sheets;
int vacats = allpages - inputPages;
 
for (int idx = 1; idx <= sheets; idx += 1)
{
    // Front page of a sheet:
    // Add a new page to the output document
    PdfPage page = outputDocument.AddPage();
    page.Orientation = PageOrientation.Landscape;
    page.Width = 2 * extWidth;
    page.Height = extHeight;
    double width = page.Width;
    double height = page.Height;
    
    gfx = XGraphics.FromPdfPage(page);
    
    // Skip if left side has to remain blank
    XRect box;
    if (vacats > 0)
        vacats -= 1;
    else
    {
        // Set page number (which is one-based) for left side
        form.PageNumber = allpages + 2 * (1 - idx);
        box = new XRect(0, 0, width / 2, height);
        // Draw the page identified by the page number like an image
        gfx.DrawImage(form, box);
    }
    
    // Set page number (which is one-based) for right side
    form.PageNumber = 2 * idx - 1;
    box = new XRect(width / 2, 0, width / 2, height);
    // Draw the page identified by the page number like an image
    gfx.DrawImage(form, box);
    
    // Back page of a sheet
    page = outputDocument.AddPage();
    page.Orientation = PageOrientation.Landscape;
    page.Width = 2 * extWidth;
    page.Height = extHeight;
    
    gfx = XGraphics.FromPdfPage(page);
    
    // Set page number (which is one-based) for left side
    form.PageNumber = 2 * idx;
    box = new XRect(0, 0, width / 2, height);
    // Draw the page identified by the page number like an image
    gfx.DrawImage(form, box);
    
    // Skip if right side has to remain blank
    if (vacats > 0)
        vacats -= 1;
    else
    {
        // Set page number (which is one-based) for right side
        form.PageNumber = allpages + 1 - 2 * idx;
        box = new XRect(width / 2, 0, width / 2, height);
        // Draw the page identified by the page number like an image
        gfx.DrawImage(form, box);
    }
}
 
// Save the document...
filename = "Booklet.pdf";
outputDocument.Save(filename);
```

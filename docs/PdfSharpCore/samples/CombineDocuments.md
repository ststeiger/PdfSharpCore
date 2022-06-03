# Combine Documents

This sample shows how to create a new document from two existing PDF files. The pages are inserted alternately from two external documents. This may be useful for visual comparison.

Two different techniques are demonstrated:
* How to import a page from an external document. This technique includes all annotations of the imported page.
* How to import a page as a PDF form object. This technique treats the pages of external documents like images that can be transformed and placed everywhere.

## Variant 1

Imports pages from an external document. Note that this technique imports the whole page including the hyperlinks.

```cs
// Open the input files
PdfDocument inputDocument1 = PdfReader.Open(filename1, PdfDocumentOpenMode.Import);
PdfDocument inputDocument2 = PdfReader.Open(filename2, PdfDocumentOpenMode.Import);
 
// Create the output document
PdfDocument outputDocument = new PdfDocument();
 
// Show consecutive pages facing. Requires Acrobat 5 or higher.
outputDocument.PageLayout = PdfPageLayout.TwoColumnLeft;
 
XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
XStringFormat format = new XStringFormat();
format.Alignment = XStringAlignment.Center;
format.LineAlignment = XLineAlignment.Far;
XGraphics gfx;
XRect box;
int count = Math.Max(inputDocument1.PageCount, inputDocument2.PageCount);
for (int idx = 0; idx < count; idx++)
{
    // Get page from 1st document
    PdfPage page1 = inputDocument1.PageCount > idx ?
    inputDocument1.Pages[idx] : new PdfPage();
    
    // Get page from 2nd document
    PdfPage page2 = inputDocument2.PageCount > idx ?
    inputDocument2.Pages[idx] : new PdfPage();
    
    // Add both pages to the output document
    page1 = outputDocument.AddPage(page1);
    page2 = outputDocument.AddPage(page2);
    
    // Write document file name and page number on each page
    gfx = XGraphics.FromPdfPage(page1);
    box = page1.MediaBox.ToXRect();
    box.Inflate(0, -10);
    gfx.DrawString(String.Format("{0} • {1}", filename1, idx + 1),
    font, XBrushes.Red, box, format);
    
    gfx = XGraphics.FromPdfPage(page2);
    box = page2.MediaBox.ToXRect();
    box.Inflate(0, -10);
    gfx.DrawString(String.Format("{0} • {1}", filename2, idx + 1),
    font, XBrushes.Red, box, format);
}
 
// Save the document...
const string filename = "CompareDocument1_tempfile.pdf";
outputDocument.Save(filename);
```


## Variant 2

Imports the pages as form X objects. Note that this technique copies only the visual content and the hyperlinks do not work.

```cs
// Create the output document
PdfDocument outputDocument = new PdfDocument();

// Show consecutive pages facing
outputDocument.PageLayout = PdfPageLayout.TwoPageLeft;
 
XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
XStringFormat format = new XStringFormat();
format.Alignment = XStringAlignment.Center;
format.LineAlignment = XLineAlignment.Far;
XGraphics gfx;
XRect box;
 
// Open the external documents as XPdfForm objects. Such objects are
// treated like images. By default the first page of the document is
// referenced by a new XPdfForm.
XPdfForm form1 = XPdfForm.FromFile(filename1);
XPdfForm form2 = XPdfForm.FromFile(filename2);
 
int count = Math.Max(form1.PageCount, form2.PageCount);
for (int idx = 0; idx < count; idx++)
{
    // Add two new pages to the output document
    PdfPage page1 = outputDocument.AddPage();
    PdfPage page2 = outputDocument.AddPage();
    
    if (form1.PageCount > idx)
    {
        // Get a graphics object for page1
        gfx = XGraphics.FromPdfPage(page1);
        
        // Set page number (which is one-based)
        form1.PageNumber = idx + 1;
        
        // Draw the page identified by the page number like an image
        gfx.DrawImage(form1, new XRect(0, 0, form1.PointWidth, form1.PointHeight));
        
        // Write document file name and page number on each page
        box = page1.MediaBox.ToXRect();
        box.Inflate(0, -10);
        gfx.DrawString(String.Format("{0} • {1}", filename1, idx + 1),
        font, XBrushes.Red, box, format);
    }
    
    // Same as above for second page
    if (form2.PageCount > idx)
    {
        gfx = XGraphics.FromPdfPage(page2);
        
        form2.PageNumber = idx + 1;
        gfx.DrawImage(form2, new XRect(0, 0, form2.PointWidth, form2.PointHeight));
        
        box = page2.MediaBox.ToXRect();
        box.Inflate(0, -10);
        gfx.DrawString(String.Format("{0} • {1}", filename2, idx + 1),
        font, XBrushes.Red, box, format);
    }
}
 
// Save the document...
const string filename = "CompareDocument2_tempfile.pdf";
outputDocument.Save(filename);
```

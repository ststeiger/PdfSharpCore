# Multiple Pages

This sample shows one way to create a PDF document with multiple pages.

When you program reaches the end of a page, you just have to create a new page by calling the `AddPage()` method of the PdfDocument class. Then you create a new XGraphics object for the new page and use it to draw on the second page, beginning at the top.

Experience shows that users sometimes have difficulties to modify there code with support for a second page. They call `AddPage()`, but do not store the return value. They do not create a new XGraphics object and continue to draw on the first page. Or they try to create a new XGraphics object, but pass the first page as a parameter and receive an error message.

If you know right from the start that you will or may need more than one page, then take this into account right from the start and your program will be readable and easy to maintain.

Note:  Consider using MigraDocCore instead of PdfSharpCore for large documents. You can use many attributes to format text and you get the line breaks and page breaks for free.


# Code

The class LayoutHelper takes care of the line position and creates pages as needed:

```cs
public class LayoutHelper
{
    private readonly PdfDocument _document;
    private readonly XUnit _topPosition;
    private readonly XUnit _bottomMargin;
    private XUnit _currentPosition;
    
    public LayoutHelper(PdfDocument document, XUnit topPosition, XUnit bottomMargin)
    {
        _document = document;
        _topPosition = topPosition;
        _bottomMargin = bottomMargin;
        // Set a value outside the page - a new page will be created on the first request.
        _currentPosition = bottomMargin + 10000;
    }
    
    public XUnit GetLinePosition(XUnit requestedHeight)
    {
        return GetLinePosition(requestedHeight, -1f);
    }
    
    public XUnit GetLinePosition(XUnit requestedHeight, XUnit requiredHeight)
    {
        XUnit required = requiredHeight == -1f ? requestedHeight : requiredHeight;
        if (_currentPosition + required > _bottomMargin)
            CreatePage();
        XUnit result = _currentPosition;
        _currentPosition += requestedHeight;
        return result;
    }
    
    public XGraphics Gfx { get; private set; }
    public PdfPage Page { get; private set; }
    
    void CreatePage()
    {
        Page = _document.AddPage();
        Page.Size = PageSize.A4;
        Gfx = XGraphics.FromPdfPage(Page);
        _currentPosition = _topPosition;
    }
}
```

And sample code that shows the LayoutHelper class at work. The sample uses short texts that will always fit into a single line. Adding line breaks to texts that do not fit into a single line is beyond the scope of this sample.

I wrote it before: Consider using MigraDocCore instead of PdfSharpCore for large documents. You can use many attributes to format text and you get the line breaks and page breaks for free.

```cs
PdfDocument document = new PdfDocument();
 
// Sample uses DIN A4, page height is 29.7 cm. We use margins of 2.5 cm.
LayoutHelper helper = new LayoutHelper(document, XUnit.FromCentimeter(2.5), XUnit.FromCentimeter(29.7 - 2.5));
XUnit left = XUnit.FromCentimeter(2.5);
 
// Random generator with seed value, so created document will always be the same.
Random rand = new Random(42);
 
const int headerFontSize = 20;
const int normalFontSize = 10;
 
XFont fontHeader = new XFont("Verdana", headerFontSize, XFontStyle.BoldItalic);
XFont fontNormal = new XFont("Verdana", normalFontSize, XFontStyle.Regular);
 
const int totalLines = 666;
bool washeader = false;
for (int line = 0; line < totalLines; ++line)
{
    bool isHeader = line == 0 || !washeader && line < totalLines - 1 && rand.Next(15) == 0;
    washeader = isHeader;
    // We do not want a single header at the bottom of the page,
    // so if we have a header we require space for header and a normal text line.
    XUnit top = helper.GetLinePosition(isHeader ? headerFontSize + 5: normalFontSize + 2, isHeader ? headerFontSize + 5 + normalFontSize : normalFontSize);
    
    helper.Gfx.DrawString(isHeader ? "Sed massa libero, semper a nisi nec" : "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
    isHeader ? fontHeader : fontNormal, XBrushes.Black, left, top, XStringFormats.TopLeft);
}
 
// Save the document...
const string filename = "MultiplePages.pdf";
document.Save(filename);
```

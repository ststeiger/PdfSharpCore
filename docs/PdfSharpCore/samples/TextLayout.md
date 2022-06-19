# Text Layout

This sample shows how to layout text with the TextFormatter class.

TextFormatter was provided because it was one of the "most wanted" features. But it is better and easier to use MigraDocCore to format paragraphs...


## Code

This is the whole source code needed to create the PDF file:

```cs
const string text =.
    "Facin exeraessisit la consenim iureet dignibh eu facilluptat vercil dunt autpat. " +
    "Ecte magna faccum dolor sequisc iliquat, quat, quipiss equipit accummy niate magna " +
    "facil iure eraesequis am velit, quat atis dolore dolent luptat nulla adio odipissectet " +
    "lan venis do essequatio conulla facillandrem zzriusci bla ad minim inis nim velit eugait " +
    "aut aut lor at ilit ut nulla ate te eugait alit augiamet ad magnim iurem il eu feuissi.\n" +
    "Guer sequis duis eu feugait luptat lum adiamet, si tate dolore mod eu facidunt adignisl in " +
    "henim dolorem nulla faccum vel inis dolutpatum iusto od min ex euis adio exer sed del " +
    "dolor ing enit veniamcon vullutat praestrud molenis ciduisim doloborem ipit nulla consequisi.\n" +
    "Nos adit pratetu eriurem delestie del ut lumsandreet nis exerilisit wis nos alit venit praestrud " +
    "dolor sum volore facidui blaor erillaortis ad ea augue corem dunt nis  iustinciduis euisi.\n" +
    "Ut ulputate volore min ut nulpute dolobor sequism olorperilit autatie modit wisl illuptat dolore " +
    "min ut in ute doloboreet ip ex et am dunt at.";
 
PdfDocument document = new PdfDocument();
 
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);
XFont font = new XFont("Times New Roman", 10, XFontStyle.Bold);
XTextFormatter tf = new XTextFormatter(gfx);
 
XRect rect = new XRect(40, 100, 250, 220);
gfx.DrawRectangle(XBrushes.SeaShell, rect);
//tf.Alignment = ParagraphAlignment.Left;
tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);
 
rect = new XRect(310, 100, 250, 220);
gfx.DrawRectangle(XBrushes.SeaShell, rect);
tf.Alignment = XParagraphAlignment.Right;
tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);
 
rect = new XRect(40, 400, 250, 220);
gfx.DrawRectangle(XBrushes.SeaShell, rect);
tf.Alignment = XParagraphAlignment.Center;
tf.DrawString(text, font, XBrushes.Black, rect, XStringFormats.TopLeft);
 
rect = new XRect(310, 400, 250, 220);
gfx.DrawRectangle(XBrushes.SeaShell, rect);
tf.Alignment = XParagraphAlignment.Justify;
```

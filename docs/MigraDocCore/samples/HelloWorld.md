# Hello World

Is the obligatory "Hello World" program for MigraDocCore documents.


## The Main method:

```cs
static void Main(string[] args)
{
    // Create a MigraDocCore document
    Document document = CreateDocument();
    document.UseCmykColor = true;
    
    // ===== Unicode encoding and font program embedding in MigraDocCore is demonstrated here =====
    
    // A flag indicating whether to create a Unicode PDF or a WinAnsi PDF file.
    // This setting applies to all fonts used in the PDF document.
    // This setting has no effect on the RTF renderer.
    const bool unicode = false;
    
    // An enum indicating whether to embed fonts or not.
    // This setting applies to all font programs used in the document.
    // This setting has no effect on the RTF renderer.
    // (The term 'font program' is used by Adobe for a file containing a font. Technically a 'font file'
    // is a collection of small programs and each program renders the glyph of a character when executed.
    // Using a font in PdfSharpCore may lead to the embedding of one or more font programs, because each outline
    // (regular, bold, italic, bold+italic, ...) has its own font program)
    const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
    
    // ========================================================================================
    
    // Create a renderer for the MigraDocCore document.
    PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
    
    // Associate the MigraDocCore document with a renderer
    pdfRenderer.Document = document;
    
    // Layout and render document to PDF
    pdfRenderer.RenderDocument();
    
    // Save the document...
    const string filename = "HelloWorld.pdf";
    pdfRenderer.PdfDocument.Save(filename);
}
```

The CreateDocument method:

```cs
/// <summary>
/// Creates an absolutely minimalistic document.
/// </summary>
static Document CreateDocument()
{
    // Create a new MigraDocCore document
    Document document = new Document();
    
    // Add a section to the document
    Section section = document.AddSection();
    
    // Add a paragraph to the section
    Paragraph paragraph = section.AddParagraph();
    paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
    
    // Add some text to the paragraph
    paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);
    
    return document;
}
```

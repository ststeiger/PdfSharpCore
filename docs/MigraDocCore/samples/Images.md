# Images

Shows how to use images in MigraDocCore documents.


## Code

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
    
    // Add some text to the paragraph
    paragraph.AddFormattedText("Hello, World!", TextFormat.Italic);
    section.AddImage("../../SomeImage.png");
    
    return document;
}
```

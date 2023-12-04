# MigraDocCore

MigraDocCore is a document generator.
It supports almost anything you find in any good word processor.
You just add paragraphs, tables, charts, arrange all this in sections, use bookmarks to create links, tables of contents, indexes, etc.
MigraDocCore will do the layout creating page breaks as needed.
MigraDocCore will create PDF documents.

* [Features](#features)
* [First steps](#first-steps)
* [Samples](samples/index.md)
* [FAQ](faq.md)


## Features

* Create perfect documents “on the fly”
* Import data from various sources via XML files or direct interfaces (any data source that can be used with .NET)
* Integrates easily with existing applications and systems
* Various options for page layout, text formatting, and document design
* Dynamic tables and business charts
* Re-usable building blocks consisting of text and / or code
* Documents with navigation (hyperlinks and / or bookmarks)


## First steps

Both PdfSharpCore and MigraDocCore provide a lot of `AddXxx` functions.
Typically these functions return the newly created objects. Once you’ve learned the basic principles it’s quite easy to work with.
Intellisense helps a lot then.

We’ll discuss a few lines of the [Hello World](samples/HelloWorld.md) sample here.

```cs
// We start with a new document:
Document document = new Document();

// With MigraDocCore, we don’t add pages, we add sections (at least one):
Section section = document.AddSection();

// Adding text is simple:
section.AddParagraph("Hello, World!");

// Adding empty paragraphs is even simpler:
section.AddParagraph();

// Store the newly created object for modification:
Paragraph paragraph = section.AddParagraph();
paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
paragraph.AddFormattedText("Hello, World!", TextFormat.Underline);

// AddFormattedText also returns an object:
FormattedText ft = paragraph.AddFormattedText("Small text", TextFormat.Bold);
ft.Font.Size = 6;

// And there’s much more that can be added: AddTable, AddImage, AddHyperlink, AddBookmark, AddPageField, AddPageBreak, ...

// With MigraDocCore you can create PDF or RTF. Just select the appropriate renderer:
PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false,
PdfFontEmbedding.Always);

// Pass the document to the renderer:
pdfRenderer.Document = document;

// Let the renderer do its job:
pdfRenderer.RenderDocument();

// Save the PDF to a file:
string filename = "HelloWorld.pdf";
pdfRenderer.PdfDocument.Save(filename);
```

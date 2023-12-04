# Hello MigraDocCore

Shows various features of MigraDocCore including table of contents, tables, bookmarks, text formatting and font styles, charts, ...

* [Styles](#define-styles)
* [Cover](#define-cover)
* [Table of Contents](#define-table-of-contents)
* [Content Section](#define-content-section)
* [Paragraphs](#define-paragraphs)
* [Tables](#define-tables)
* [Charts](#define-charts) 


## The Main method:

```cs
static void Main()
{
    // Create a MigraDocCore document
    Document document = Documents.CreateDocument();
    
    //string ddl = MigraDocCore.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
    MigraDocCore.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDocCore.mdddl");
    
    PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
    renderer.Document = document;
    
    renderer.RenderDocument();
    
    // Save the document...
    string filename = "HelloMigraDoc.pdf";
    renderer.PdfDocument.Save(filename);
}
```


## CreateDocument is the method that creates the content:

```cs
public static Document CreateDocument()
{
    // Create a new MigraDocCore document
    Document document = new Document();
    document.Info.Title = "Hello, MigraDocCore";
    document.Info.Subject = "Demonstrates an excerpt of the capabilities of MigraDocCore.";
    document.Info.Author = "Stefan Lange";
    
    Styles.DefineStyles(document);
    Cover.DefineCover(document);
    TableOfContents.DefineTableOfContents(document);
    DefineContentSection(document); 
    Paragraphs.DefineParagraphs(document);
    Tables.DefineTables(document);
    Charts.DefineCharts(document);

    return document;
}
```

### Define Styles

```cs
    /// <summary>
    /// Defines the styles used in the document.
    /// </summary>
    public static void DefineStyles(Document document)
    {
    // Get the predefined style Normal.
    Style style = document.Styles["Normal"];
    // Because all styles are derived from Normal, the next line changes the
    // font of the whole document. Or, more exactly, it changes the font of
    // all styles and paragraphs that do not redefine the font.
    style.Font.Name = "Times New Roman";
    
    // Heading1 to Heading9 are predefined styles with an outline level. An outline level
    // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks)
    // in PDF.
    
    style = document.Styles["Heading1"];
    style.Font.Name = "Tahoma";
    style.Font.Size = 14;
    style.Font.Bold = true;
    style.Font.Color = Colors.DarkBlue;
    style.ParagraphFormat.PageBreakBefore = true;
    style.ParagraphFormat.SpaceAfter = 6;
    
    style = document.Styles["Heading2"];
    style.Font.Size = 12;
    style.Font.Bold = true;
    style.ParagraphFormat.PageBreakBefore = false;
    style.ParagraphFormat.SpaceBefore = 6;
    style.ParagraphFormat.SpaceAfter = 6;
    
    style = document.Styles["Heading3"];
    style.Font.Size = 10;
    style.Font.Bold = true;
    style.Font.Italic = true;
    style.ParagraphFormat.SpaceBefore = 6;
    style.ParagraphFormat.SpaceAfter = 3;
    
    style = document.Styles[StyleNames.Header];
    style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
    
    style = document.Styles[StyleNames.Footer];
    style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
    
    // Create a new style called TextBox based on style Normal
    style = document.Styles.AddStyle("TextBox", "Normal");
    style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
    style.ParagraphFormat.Borders.Width = 2.5;
    style.ParagraphFormat.Borders.Distance = "3pt";
    style.ParagraphFormat.Shading.Color = Colors.SkyBlue;
    
    // Create a new style called TOC based on style Normal
    style = document.Styles.AddStyle("TOC", "Normal");
    style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
    style.ParagraphFormat.Font.Color = Colors.Blue;
}
```

### Define Cover

```cs
/// <summary>
/// Defines the cover page.
/// </summary>
public static void DefineCover(Document document)
{
    Section section = document.AddSection();
    
    Paragraph paragraph = section.AddParagraph();
    paragraph.Format.SpaceAfter = "3cm";
    
    Image image = section.AddImage("../../images/Logo landscape.png");
    image.Width = "10cm";
    
    paragraph = section.AddParagraph("A sample document that demonstrates the\ncapabilities of MigraDocCore");
    paragraph.Format.Font.Size = 16;
    paragraph.Format.Font.Color = Colors.DarkRed;
    paragraph.Format.SpaceBefore = "8cm";
    paragraph.Format.SpaceAfter = "3cm";
    
    paragraph = section.AddParagraph("Rendering date: ");
    paragraph.AddDateField();
}
```

### Define Table of Contents

```cs
/// <summary>
/// Defines the table of contents page.
/// </summary>
public static void DefineTableOfContents(Document document)
{
    Section section = document.LastSection;
    
    section.AddPageBreak();
    Paragraph paragraph = section.AddParagraph("Table of Contents");
    paragraph.Format.Font.Size = 14;
    paragraph.Format.Font.Bold = true;
    paragraph.Format.SpaceAfter = 24;
    paragraph.Format.OutlineLevel = OutlineLevel.Level1;
    
    paragraph = section.AddParagraph();
    paragraph.Style = "TOC";
    Hyperlink hyperlink = paragraph.AddHyperlink("Paragraphs");
    hyperlink.AddText("Paragraphs\t");
    hyperlink.AddPageRefField("Paragraphs");
    
    paragraph = section.AddParagraph();
    paragraph.Style = "TOC";
    hyperlink = paragraph.AddHyperlink("Tables");
    hyperlink.AddText("Tables\t");
    hyperlink.AddPageRefField("Tables");
    
    paragraph = section.AddParagraph();
    paragraph.Style = "TOC";
    hyperlink = paragraph.AddHyperlink("Charts");
    hyperlink.AddText("Charts\t");
    hyperlink.AddPageRefField("Charts");
}
```

### Define Content Section

```cs
/// <summary>
/// Defines page setup, headers, and footers.
/// </summary>
static void DefineContentSection(Document document)
{
    Section section = document.AddSection();
    section.PageSetup.OddAndEvenPagesHeaderFooter = true;
    section.PageSetup.StartingNumber = 1;
    
    HeaderFooter header = section.Headers.Primary;
    header.AddParagraph("\tOdd Page Header");
    
    header = section.Headers.EvenPage;
    header.AddParagraph("Even Page Header");
    
    // Create a paragraph with centered page number. See definition of style "Footer".
    Paragraph paragraph = new Paragraph();
    paragraph.AddTab();
    paragraph.AddPageField();
    
    // Add paragraph to footer for odd pages.
    section.Footers.Primary.Add(paragraph);
    // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
    // not belong to more than one other object. If you forget cloning an exception is thrown.
    section.Footers.EvenPage.Add(paragraph.Clone());
}
```

### Define Paragraphs

```cs
public static void DefineParagraphs(Document document)
{
    Paragraph paragraph = document.LastSection.AddParagraph("Paragraph Layout Overview", "Heading1");
    paragraph.AddBookmark("Paragraphs");
    
    DemonstrateAlignment(document);
    DemonstrateIndent(document);
    DemonstrateFormattedText(document);
    DemonstrateBordersAndShading(document);
}
 
static void DemonstrateAlignment(Document document)
{
    document.LastSection.AddParagraph("Alignment", "Heading2");

    document.LastSection.AddParagraph("Left Aligned", "Heading3");
    
    Paragraph paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Alignment = ParagraphAlignment.Left;
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("Right Aligned", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Alignment = ParagraphAlignment.Right;
    paragraph.AddText(FillerText.Text);

    document.LastSection.AddParagraph("Centered", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Alignment = ParagraphAlignment.Center;
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("Justified", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Alignment = ParagraphAlignment.Justify;
    paragraph.AddText(FillerText.MediumText);
}
 
static void DemonstrateIndent(Document document)
{
    document.LastSection.AddParagraph("Indent", "Heading2");
    
    document.LastSection.AddParagraph("Left Indent", "Heading3");
    
    Paragraph paragraph = document.LastSection.AddParagraph();
    paragraph.Format.LeftIndent = "2cm";
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("Right Indent", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.RightIndent = "1in";
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("First Line Indent", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.FirstLineIndent = "12mm";
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("First Line Negative Indent", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.LeftIndent = "1.5cm";
    paragraph.Format.FirstLineIndent = "-1.5cm";
    paragraph.AddText(FillerText.Text);
}
 
static void DemonstrateFormattedText(Document document)
{
    document.LastSection.AddParagraph("Formatted Text", "Heading2");
    
    //document.LastSection.AddParagraph("Left Aligned", "Heading3");
    
    Paragraph paragraph = document.LastSection.AddParagraph();
    paragraph.AddText("Text can be formatted ");
    paragraph.AddFormattedText("bold", TextFormat.Bold);
    paragraph.AddText(", ");
    paragraph.AddFormattedText("italic", TextFormat.Italic);
    paragraph.AddText(", or ");
    paragraph.AddFormattedText("bold & italic", TextFormat.Bold | TextFormat.Italic);
    paragraph.AddText(".");
    paragraph.AddLineBreak();
    paragraph.AddText("You can set the ");
    FormattedText formattedText = paragraph.AddFormattedText("size ");
    formattedText.Size = 15;
    paragraph.AddText("the ");
    formattedText = paragraph.AddFormattedText("color ");
    formattedText.Color = Colors.Firebrick;
    paragraph.AddText("the ");
    formattedText = paragraph.AddFormattedText("font", new Font("Verdana"));
    paragraph.AddText(".");
    paragraph.AddLineBreak();
    paragraph.AddText("You can set the ");
    formattedText = paragraph.AddFormattedText("subscript");
    formattedText.Subscript = true;
    paragraph.AddText(" or ");
    formattedText = paragraph.AddFormattedText("superscript");
    formattedText.Superscript = true;
    paragraph.AddText(".");
}
 
static void DemonstrateBordersAndShading(Document document)
{
    document.LastSection.AddPageBreak();
    document.LastSection.AddParagraph("Borders and Shading", "Heading2");
    
    document.LastSection.AddParagraph("Border around Paragraph", "Heading3");
    
    Paragraph paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Borders.Width = 2.5;
    paragraph.Format.Borders.Color = Colors.Navy;
    paragraph.Format.Borders.Distance = 3;
    paragraph.AddText(FillerText.MediumText);
    
    document.LastSection.AddParagraph("Shading", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Format.Shading.Color = Colors.LightCoral;
    paragraph.AddText(FillerText.Text);
    
    document.LastSection.AddParagraph("Borders & Shading", "Heading3");
    
    paragraph = document.LastSection.AddParagraph();
    paragraph.Style = "TextBox";
    paragraph.AddText(FillerText.MediumText);
}
```

### Define Tables

```cs
public static void DefineTables(Document document)
{
    Paragraph paragraph = document.LastSection.AddParagraph("Table Overview", "Heading1");
    paragraph.AddBookmark("Tables");
    
    DemonstrateSimpleTable(document);
    DemonstrateAlignment(document);
    DemonstrateCellMerge(document);
}
 
public static void DemonstrateSimpleTable(Document document)
{
    document.LastSection.AddParagraph("Simple Tables", "Heading2");
    
    Table table = new Table();
    table.Borders.Width = 0.75;
    
    Column column = table.AddColumn(Unit.FromCentimeter(2));
    column.Format.Alignment = ParagraphAlignment.Center;
    
    table.AddColumn(Unit.FromCentimeter(5));
    
    Row row = table.AddRow();
    row.Shading.Color = Colors.PaleGoldenrod;
    Cell cell = row.Cells[0];
    cell.AddParagraph("Itemus");
    cell = row.Cells[1];
    cell.AddParagraph("Descriptum");
    
    row = table.AddRow();
    cell = row.Cells[0];
    cell.AddParagraph("1");
    cell = row.Cells[1];
    cell.AddParagraph(FillerText.ShortText);
    
    row = table.AddRow();
    cell = row.Cells[0];
    cell.AddParagraph("2");
    cell = row.Cells[1];
    cell.AddParagraph(FillerText.Text);
    
    table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
    
    document.LastSection.Add(table);
}
 
public static void DemonstrateAlignment(Document document)
{
    document.LastSection.AddParagraph("Cell Alignment", "Heading2");
    
    Table table = document.LastSection.AddTable();
    table.Borders.Visible = true;
    table.Format.Shading.Color = Colors.LavenderBlush;
    table.Shading.Color = Colors.Salmon;
    table.TopPadding = 5;
    table.BottomPadding = 5;
    
    Column column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Left;
    
    column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Center;
    
    column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Right;
    
    table.Rows.Height = 35;
    
    Row row = table.AddRow();
    row.VerticalAlignment = VerticalAlignment.Top;
    row.Cells[0].AddParagraph("Text");
    row.Cells[1].AddParagraph("Text");
    row.Cells[2].AddParagraph("Text");
    
    row = table.AddRow();
    row.VerticalAlignment = VerticalAlignment.Center;
    row.Cells[0].AddParagraph("Text");
    row.Cells[1].AddParagraph("Text");
    row.Cells[2].AddParagraph("Text");
    
    row = table.AddRow();
    row.VerticalAlignment = VerticalAlignment.Bottom;
    row.Cells[0].AddParagraph("Text");
    row.Cells[1].AddParagraph("Text");
    row.Cells[2].AddParagraph("Text");
}
 
public static void DemonstrateCellMerge(Document document)
{
    document.LastSection.AddParagraph("Cell Merge", "Heading2");
    
    Table table = document.LastSection.AddTable();
    table.Borders.Visible = true;
    table.TopPadding = 5;
    table.BottomPadding = 5;
    
    Column column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Left;
    
    column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Center;
    
    column = table.AddColumn();
    column.Format.Alignment = ParagraphAlignment.Right;
    
    table.Rows.Height = 35;
    
    Row row = table.AddRow();
    row.Cells[0].AddParagraph("Merge Right");
    row.Cells[0].MergeRight = 1;
    
    row = table.AddRow();
    row.VerticalAlignment = VerticalAlignment.Bottom;
    row.Cells[0].MergeDown = 1;
    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
    row.Cells[0].AddParagraph("Merge Down");
    
    table.AddRow();
}
```

### Define Charts

```cs
public static void DefineCharts(Document document)
{
    Paragraph paragraph = document.LastSection.AddParagraph("Chart Overview", "Heading1");
    paragraph.AddBookmark("Charts");
    
    document.LastSection.AddParagraph("Sample Chart", "Heading2");
    
    Chart chart = new Chart();
    chart.Left = 0;
    
    chart.Width = Unit.FromCentimeter(16);
    chart.Height = Unit.FromCentimeter(12);
    Series series = chart.SeriesCollection.AddSeries();
    series.ChartType = ChartType.Column2D;
    series.Add(new double[]{1, 17, 45, 5, 3, 20, 11, 23, 8, 19});
    series.HasDataLabel = true;
    
    series = chart.SeriesCollection.AddSeries();
    series.ChartType = ChartType.Line;
    series.Add(new double[]{41, 7, 5, 45, 13, 10, 21, 13, 18, 9});
    
    XSeries xseries = chart.XValues.AddXSeries();
    xseries.Add("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N");
    
    chart.XAxis.MajorTickMark = TickMarkType.Outside;
    chart.XAxis.Title.Caption = "X-Axis";
    
    chart.YAxis.MajorTickMark = TickMarkType.Outside;
    chart.YAxis.HasMajorGridlines = true;
    
    chart.PlotArea.LineFormat.Color = Colors.DarkGray;
    chart.PlotArea.LineFormat.Width = 1;
    
    document.LastSection.Add(chart);
}
```

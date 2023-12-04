# Invoice

Shows how to create a simple invoice of a fictional book store.
The invoice document is created with the MigraDocCore document object model and then rendered to PDF with PdfSharpCore.


## Creating the Document

```cs
public Document CreateDocument()
{
    // Create a new MigraDocCore document
    this.document = new Document();
    this.document.Info.Title = "A sample invoice";
    this.document.Info.Subject = "Demonstrates how to create an invoice.";
    this.document.Info.Author = "Stefan Lange";
    
    DefineStyles();
    CreatePage();
    FillContent();
    
    return this.document;
}
```


## Defining the Styles

Styles define how the text will look:

```cs
void DefineStyles()
{
    // Get the predefined style Normal.
    Style style = this.document.Styles["Normal"];
    // Because all styles are derived from Normal, the next line changes the
    // font of the whole document. Or, more exactly, it changes the font of
    // all styles and paragraphs that do not redefine the font.
    style.Font.Name = "Verdana";
    
    style = this.document.Styles[StyleNames.Header];
    style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);
    
    style = this.document.Styles[StyleNames.Footer];
    style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);
    
    // Create a new style called Table based on style Normal
    style = this.document.Styles.AddStyle("Table", "Normal");
    style.Font.Name = "Verdana";
    style.Font.Name = "Times New Roman";
    style.Font.Size = 9;
    
    // Create a new style called Reference based on style Normal
    style = this.document.Styles.AddStyle("Reference", "Normal");
    style.ParagraphFormat.SpaceBefore = "5mm";
    style.ParagraphFormat.SpaceAfter = "5mm";
    style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
}
```


## Create Page

Create the page with invoice table, header, footer:

```cs
void CreatePage()
{
    // Each MigraDocCore document needs at least one section.
    Section section = this.document.AddSection();
    
    // Put a logo in the header
    Image image = section.Headers.Primary.AddImage("../../PowerBooks.png");
    image.Height = "2.5cm";
    image.LockAspectRatio = true;
    image.RelativeVertical = RelativeVertical.Line;
    image.RelativeHorizontal = RelativeHorizontal.Margin;
    image.Top = ShapePosition.Top;
    image.Left = ShapePosition.Right;
    image.WrapFormat.Style = WrapStyle.Through;
    
    // Create footer
    Paragraph paragraph = section.Footers.Primary.AddParagraph();
    paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
    paragraph.Format.Font.Size = 9;
    paragraph.Format.Alignment = ParagraphAlignment.Center;
    
    // Create the text frame for the address
    this.addressFrame = section.AddTextFrame();
    this.addressFrame.Height = "3.0cm";
    this.addressFrame.Width = "7.0cm";
    this.addressFrame.Left = ShapePosition.Left;
    this.addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
    this.addressFrame.Top = "5.0cm";
    this.addressFrame.RelativeVertical = RelativeVertical.Page;
    
    // Put sender in address frame
    paragraph = this.addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
    paragraph.Format.Font.Name = "Times New Roman";
    paragraph.Format.Font.Size = 7;
    paragraph.Format.SpaceAfter = 3;
    
    // Add the print date field
    paragraph = section.AddParagraph();
    paragraph.Format.SpaceBefore = "8cm";
    paragraph.Style = "Reference";
    paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
    paragraph.AddTab();
    paragraph.AddText("Cologne, ");
    paragraph.AddDateField("dd.MM.yyyy");
    
    // Create the item table
    this.table = section.AddTable();
    this.table.Style = "Table";
    this.table.Borders.Color = TableBorder;
    this.table.Borders.Width = 0.25;
    this.table.Borders.Left.Width = 0.5;
    this.table.Borders.Right.Width = 0.5;
    this.table.Rows.LeftIndent = 0;
    
    // Before you can add a row, you must define the columns
    Column column = this.table.AddColumn("1cm");
    column.Format.Alignment = ParagraphAlignment.Center;
    
    column = this.table.AddColumn("2.5cm");
    column.Format.Alignment = ParagraphAlignment.Right;
    
    column = this.table.AddColumn("3cm");
    column.Format.Alignment = ParagraphAlignment.Right;
    
    column = this.table.AddColumn("3.5cm");
    column.Format.Alignment = ParagraphAlignment.Right;
    
    column = this.table.AddColumn("2cm");
    column.Format.Alignment = ParagraphAlignment.Center;
    
    column = this.table.AddColumn("4cm");
    column.Format.Alignment = ParagraphAlignment.Right;
    
    // Create the header of the table
    Row row = table.AddRow();
    row.HeadingFormat = true;
    row.Format.Alignment = ParagraphAlignment.Center;
    row.Format.Font.Bold = true;
    row.Shading.Color = TableBlue;
    row.Cells[0].AddParagraph("Item");
    row.Cells[0].Format.Font.Bold = false;
    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
    row.Cells[0].MergeDown = 1;
    row.Cells[1].AddParagraph("Title and Author");
    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[1].MergeRight = 3;
    row.Cells[5].AddParagraph("Extended Price");
    row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
    row.Cells[5].MergeDown = 1;
    
    row = table.AddRow();
    row.HeadingFormat = true;
    row.Format.Alignment = ParagraphAlignment.Center;
    row.Format.Font.Bold = true;
    row.Shading.Color = TableBlue;
    row.Cells[1].AddParagraph("Quantity");
    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[2].AddParagraph("Unit Price");
    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[3].AddParagraph("Discount (%)");
    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
    row.Cells[4].AddParagraph("Taxable");
    row.Cells[4].Format.Alignment = ParagraphAlignment.Left;
    
    this.table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
}
```


## Fill in the Content

This routine adds the dynamic data to the invoice:

```cs
void FillContent()
{
    // Fill address in address text frame
    XPathNavigator item = SelectItem("/invoice/to");
    Paragraph paragraph = this.addressFrame.AddParagraph();
    paragraph.AddText(GetValue(item, "name/singleName"));
    paragraph.AddLineBreak();
    paragraph.AddText(GetValue(item, "address/line1"));
    paragraph.AddLineBreak();
    paragraph.AddText(GetValue(item, "address/postalCode") + " " + GetValue(item, "address/city"));
    
    // Iterate the invoice items
    double totalExtendedPrice = 0;
    XPathNodeIterator iter = this.navigator.Select("/invoice/items/*");
    while (iter.MoveNext())
    {
        item = iter.Current;
        double quantity = GetValueAsDouble(item, "quantity");
        double price = GetValueAsDouble(item, "price");
        double discount = GetValueAsDouble(item, "discount");
        
        // Each item fills two rows
        Row row1 = this.table.AddRow();
        Row row2 = this.table.AddRow();
        row1.TopPadding = 1.5;
        row1.Cells[0].Shading.Color = TableGray;
        row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
        row1.Cells[0].MergeDown = 1;
        row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
        row1.Cells[1].MergeRight = 3;
        row1.Cells[5].Shading.Color = TableGray;
        row1.Cells[5].MergeDown = 1;
        
        row1.Cells[0].AddParagraph(GetValue(item, "itemNumber"));
        paragraph = row1.Cells[1].AddParagraph();
        paragraph.AddFormattedText(GetValue(item, "title"), TextFormat.Bold);
        paragraph.AddFormattedText(" by ", TextFormat.Italic);
        paragraph.AddText(GetValue(item, "author"));
        row2.Cells[1].AddParagraph(GetValue(item, "quantity"));
        row2.Cells[2].AddParagraph(price.ToString("0.00") + " €");
        row2.Cells[3].AddParagraph(discount.ToString("0.0"));
        row2.Cells[4].AddParagraph();
        row2.Cells[5].AddParagraph(price.ToString("0.00"));
        double extendedPrice = quantity * price;
        extendedPrice = extendedPrice * (100 - discount) / 100;
        row1.Cells[5].AddParagraph(extendedPrice.ToString("0.00") + " €");
        row1.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
        totalExtendedPrice += extendedPrice;
        
        this.table.SetEdge(0, this.table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
    }
    
    // Add an invisible row as a space line to the table
    Row row = this.table.AddRow();
    row.Borders.Visible = false;
    
    // Add the total price row
    row = this.table.AddRow();
    row.Cells[0].Borders.Visible = false;
    row.Cells[0].AddParagraph("Total Price");
    row.Cells[0].Format.Font.Bold = true;
    row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
    row.Cells[0].MergeRight = 4;
    row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");
    
    // Add the VAT row
    row = this.table.AddRow();
    row.Cells[0].Borders.Visible = false;
    row.Cells[0].AddParagraph("VAT (19%)");
    row.Cells[0].Format.Font.Bold = true;
    row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
    row.Cells[0].MergeRight = 4;
    row.Cells[5].AddParagraph((0.19 * totalExtendedPrice).ToString("0.00") + " €");
    
    // Add the additional fee row
    row = this.table.AddRow();
    row.Cells[0].Borders.Visible = false;
    row.Cells[0].AddParagraph("Shipping and Handling");
    row.Cells[5].AddParagraph(0.ToString("0.00") + " €");
    row.Cells[0].Format.Font.Bold = true;
    row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
    row.Cells[0].MergeRight = 4;
    
    // Add the total due row
    row = this.table.AddRow();
    row.Cells[0].AddParagraph("Total Due");
    row.Cells[0].Borders.Visible = false;
    row.Cells[0].Format.Font.Bold = true;
    row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
    row.Cells[0].MergeRight = 4;
    totalExtendedPrice += 0.19 * totalExtendedPrice;
    row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");
    
    // Set the borders of the specified cell range
    this.table.SetEdge(5, this.table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);
    
    // Add the notes paragraph
    paragraph = this.document.LastSection.AddParagraph();
    paragraph.Format.SpaceBefore = "1cm";
    paragraph.Format.Borders.Width = 0.75;
    paragraph.Format.Borders.Distance = 3;
    paragraph.Format.Borders.Color = TableBorder;
    paragraph.Format.Shading.Color = TableGray;
    item = SelectItem("/invoice");
    paragraph.AddText(GetValue(item, "notes"));
}
```

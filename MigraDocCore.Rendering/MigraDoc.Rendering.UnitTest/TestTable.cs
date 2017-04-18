using System;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.IO;

namespace MigraDocCore.Rendering.UnitTest
{
    /// <summary>
    /// Summary description for TestTable.
    /// </summary>
    public class TestTable
    {
        public static void Borders(string outputFile)
        {
            Document document = new Document();
            Section sec = document.Sections.AddSection();
            sec.AddParagraph("A paragraph before.");
            Table table = sec.AddTable();
            table.Borders.Visible = true;
            table.AddColumn();
            table.AddColumn();
            table.Rows.HeightRule = RowHeightRule.Exactly;
            table.Rows.Height = 14;
            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            cell.Borders.Visible = true;
            cell.Borders.Left.Width = 8;
            cell.Borders.Right.Width = 2;
            cell.AddParagraph("First Cell");

            row = table.AddRow();
            cell = row.Cells[1];
            cell.AddParagraph("Last Cell within this table");
            cell.Borders.Bottom.Width = 15;
            cell.Shading.Color = Colors.LightBlue;
            sec.AddParagraph("A Paragraph afterwards");
            PdfDocumentRenderer printer = new PdfDocumentRenderer()
            {
                Document = document
            };
            printer.RenderDocument();
            printer.PdfDocument.Save(outputFile);
        }

        public static void CellMerge(string outputFile)
        {
            Document document = new Document();
            Section sec = document.Sections.AddSection();
            sec.AddParagraph("A paragraph before.");
            Table table = sec.AddTable();
            table.Borders.Visible = true;
            table.AddColumn();
            table.AddColumn();
            Row row = table.AddRow();
            Cell cell = row.Cells[0];
            cell.MergeRight = 1;
            cell.Borders.Visible = true;
            cell.Borders.Left.Width = 8;
            cell.Borders.Right.Width = 2;
            cell.AddParagraph("First Cell");

            row = table.AddRow();
            cell = row.Cells[1];
            cell.AddParagraph("Last Cell within this row");
            cell.MergeDown = 1;
            cell.Borders.Bottom.Width = 15;
            cell.Borders.Right.Width = 30;
            cell.Shading.Color = Colors.LightBlue;
            row = table.AddRow();
            sec.AddParagraph("A Paragraph afterwards");
            PdfDocumentRenderer printer = new PdfDocumentRenderer()
            {
                Document = document
            };
            printer.RenderDocument();
            printer.PdfDocument.Save(outputFile);
        }


        public static void VerticalAlign(string outputFile)
        {
            Document document = new Document();
            Section sec = document.Sections.AddSection();
            sec.AddParagraph("A paragraph before.");
            Table table = sec.AddTable();
            table.Borders.Visible = true;
            table.AddColumn();
            table.AddColumn();
            Row row = table.AddRow();
            row.HeightRule = RowHeightRule.Exactly;
            row.Height = 70;
            row.VerticalAlignment = VerticalAlignment.Center;
            row[0].AddParagraph("First Cell");
            row[1].AddParagraph("Second Cell");
            sec.AddParagraph("A Paragraph afterwards.");


            PdfDocumentRenderer printer = new PdfDocumentRenderer()
            {
                Document = document
            };
            printer.RenderDocument();
            printer.PdfDocument.Save(outputFile);
        }

    }
}

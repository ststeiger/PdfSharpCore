#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@PdfSharpCore.com)
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//   David Stephensen (mailto:David.Stephensen@PdfSharpCore.com)
//
// Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;

namespace MigraDocCore.DocumentObjectModel.Visitors
{
  /// <summary>
  /// Summary description for VisitorBase.
  /// </summary>
  public abstract class VisitorBase : DocumentObjectVisitor
  {
    public VisitorBase()
    {
    }

    public override void Visit(DocumentObject documentObject)
    {
      IVisitable visitable = documentObject as IVisitable;
      if (visitable != null)
        visitable.AcceptVisitor(this, true);
    }

    protected void FlattenParagraphFormat(ParagraphFormat format, ParagraphFormat refFormat)
    {
      if (format.alignment.IsNull)
        format.alignment = refFormat.alignment;

      if (format.firstLineIndent.IsNull)
        format.firstLineIndent = refFormat.firstLineIndent;

      if (format.leftIndent.IsNull)
        format.leftIndent = refFormat.leftIndent;

      if (format.rightIndent.IsNull)
        format.rightIndent = refFormat.rightIndent;

      if (format.spaceBefore.IsNull)
        format.spaceBefore = refFormat.spaceBefore;

      if (format.spaceAfter.IsNull)
        format.spaceAfter = refFormat.spaceAfter;

      if (format.lineSpacingRule.IsNull)
        format.lineSpacingRule = refFormat.lineSpacingRule;
      if (format.lineSpacing.IsNull)
        format.lineSpacing = refFormat.lineSpacing;

      if (format.widowControl.IsNull)
        format.widowControl = refFormat.widowControl;

      if (format.keepTogether.IsNull)
        format.keepTogether = refFormat.keepTogether;

      if (format.keepWithNext.IsNull)
        format.keepWithNext = refFormat.keepWithNext;

      if (format.pageBreakBefore.IsNull)
        format.pageBreakBefore = refFormat.pageBreakBefore;

      if (format.outlineLevel.IsNull)
        format.outlineLevel = refFormat.outlineLevel;

      if (format.font == null)
      {
        if (refFormat.font != null)
        {
          //The font is cloned here to avoid parent problems
          format.font = refFormat.font.Clone();
          format.font.parent = format;
        }
      }
      else if (refFormat.font != null)
        FlattenFont(format.font, refFormat.font);

      if (format.shading == null)
      {
        if (refFormat.shading != null)
        {
          format.shading = refFormat.shading.Clone();
          format.shading.parent = format;
        }
        //        format.shading = refFormat.shading;
      }
      else if (refFormat.shading != null)
        FlattenShading(format.shading, refFormat.shading);

      if (format.borders == null)
        format.borders = refFormat.borders;
      else if (refFormat.borders != null)
        FlattenBorders(format.borders, refFormat.borders);

      //      if (format.tabStops == null)
      //        format.tabStops = refFormat.tabStops;
      if (refFormat.tabStops != null)
        FlattenTabStops(format.TabStops, refFormat.tabStops);

      if (refFormat.listInfo != null)
        FlattenListInfo(format.ListInfo, refFormat.listInfo);
    }

    protected void FlattenListInfo(ListInfo listInfo, ListInfo refListInfo)
    {
      if (listInfo.continuePreviousList.IsNull)
        listInfo.continuePreviousList = refListInfo.continuePreviousList;
      if (listInfo.listType.IsNull)
        listInfo.listType = refListInfo.listType;
      if (listInfo.numberPosition.IsNull)
        listInfo.numberPosition = refListInfo.numberPosition;
    }

    protected void FlattenFont(Font font, Font refFont)
    {
      if (font.name.IsNull)
        font.name = refFont.name;
      if (font.size.IsNull)
        font.size = refFont.size;
      if (font.color.IsNull)
        font.color = refFont.color;
      if (font.underline.IsNull)
        font.underline = refFont.underline;
      if (font.bold.IsNull)
        font.bold = refFont.bold;
      if (font.italic.IsNull)
        font.italic = refFont.italic;
      if (font.superscript.IsNull)
        font.superscript = refFont.superscript;
      if (font.subscript.IsNull)
        font.subscript = refFont.subscript;
    }

    protected void FlattenShading(Shading shading, Shading refShading)
    {
      //fClear?
      if (shading.visible.IsNull)
        shading.visible = refShading.visible;
      if (shading.color.IsNull)
        shading.color = refShading.color;
    }

    protected Border FlattenedBorderFromBorders(Border border, Borders parentBorders)
    {
      if (border == null)
        border = new Border(parentBorders);

      if (border.visible.IsNull)
        border.visible = parentBorders.visible;

      if (border.style.IsNull)
        border.style = parentBorders.style;

      if (border.width.IsNull)
        border.width = parentBorders.width;

      if (border.color.IsNull)
        border.color = parentBorders.color;

      return border;
    }

    protected void FlattenBorders(Borders borders, Borders refBorders)
    {
      if (borders.visible.IsNull)
        borders.visible = refBorders.visible;
      if (borders.width.IsNull)
        borders.width = refBorders.width;
      if (borders.style.IsNull)
        borders.style = refBorders.style;
      if (borders.color.IsNull)
        borders.color = refBorders.color;

      if (borders.distanceFromBottom.IsNull)
        borders.distanceFromBottom = refBorders.distanceFromBottom;
      if (borders.distanceFromRight.IsNull)
        borders.distanceFromRight = refBorders.distanceFromRight;
      if (borders.distanceFromLeft.IsNull)
        borders.distanceFromLeft = refBorders.distanceFromLeft;
      if (borders.distanceFromTop.IsNull)
        borders.distanceFromTop = refBorders.distanceFromTop;

      if (refBorders.left != null)
      {
        FlattenBorder(borders.Left, refBorders.left);
        FlattenedBorderFromBorders(borders.left, borders);
      }
      if (refBorders.right != null)
      {
        FlattenBorder(borders.Right, refBorders.right);
        FlattenedBorderFromBorders(borders.right, borders);
      }
      if (refBorders.top != null)
      {
        FlattenBorder(borders.Top, refBorders.top);
        FlattenedBorderFromBorders(borders.top, borders);
      }
      if (refBorders.bottom != null)
      {
        FlattenBorder(borders.Bottom, refBorders.bottom);
        FlattenedBorderFromBorders(borders.bottom, borders);
      }
    }

    protected void FlattenBorder(Border border, Border refBorder)
    {
      if (border.visible.IsNull)
        border.visible = refBorder.visible;
      if (border.width.IsNull)
        border.width = refBorder.width;
      if (border.style.IsNull)
        border.style = refBorder.style;
      if (border.color.IsNull)
        border.color = refBorder.color;
    }

    protected void FlattenTabStops(TabStops tabStops, TabStops refTabStops)
    {
      if (!tabStops.fClearAll)
      {
        foreach (TabStop refTabStop in refTabStops)
        {
          if (tabStops.GetTabStopAt(refTabStop.Position) == null && refTabStop.AddTab)
            tabStops.AddTabStop(refTabStop.Position, refTabStop.Alignment, refTabStop.Leader);
        }
      }

      for (int i = 0; i < tabStops.Count; i++)
      {
        TabStop tabStop = tabStops[i];
        if (!tabStop.AddTab)
          tabStops.RemoveObjectAt(i);
      }
      //Die TabStopCollection ist so wie sie jetzt ist vollständig.
      //Sie darf daher nichts erben, d.h. :
      tabStops.fClearAll = true;
    }

    protected void FlattenPageSetup(PageSetup pageSetup, PageSetup refPageSetup)
    {
      Unit dummyUnit;
      if (pageSetup.pageWidth.IsNull && pageSetup.pageHeight.IsNull)
      {
        if (pageSetup.pageFormat.IsNull)
        {
          pageSetup.pageWidth = refPageSetup.pageWidth;
          pageSetup.pageHeight = refPageSetup.pageHeight;
          pageSetup.pageFormat = refPageSetup.pageFormat;
        }
        else
          PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup.pageWidth, out pageSetup.pageHeight);
      }
      else
      {
        if (pageSetup.pageWidth.IsNull)
        {
          if (pageSetup.pageFormat.IsNull)
            pageSetup.pageHeight = refPageSetup.pageHeight;
          else
            PageSetup.GetPageSize(pageSetup.PageFormat, out dummyUnit, out pageSetup.pageHeight);
        }
        else if (pageSetup.pageHeight.IsNull)
        {
          if (pageSetup.pageFormat.IsNull)
            pageSetup.pageWidth = refPageSetup.pageWidth;
          else
            PageSetup.GetPageSize(pageSetup.PageFormat, out pageSetup.pageWidth, out dummyUnit);
        }
      }
      //      if (pageSetup.pageWidth.IsNull)
      //        pageSetup.pageWidth = refPageSetup.pageWidth;
      //      if (pageSetup.pageHeight.IsNull)
      //        pageSetup.pageHeight = refPageSetup.pageHeight;
      //      if (pageSetup.pageFormat.IsNull)
      //        pageSetup.pageFormat = refPageSetup.pageFormat;
      if (pageSetup.sectionStart.IsNull)
        pageSetup.sectionStart = refPageSetup.sectionStart;
      if (pageSetup.orientation.IsNull)
        pageSetup.orientation = refPageSetup.orientation;
      if (pageSetup.topMargin.IsNull)
        pageSetup.topMargin = refPageSetup.topMargin;
      if (pageSetup.bottomMargin.IsNull)
        pageSetup.bottomMargin = refPageSetup.bottomMargin;
      if (pageSetup.leftMargin.IsNull)
        pageSetup.leftMargin = refPageSetup.leftMargin;
      if (pageSetup.rightMargin.IsNull)
        pageSetup.rightMargin = refPageSetup.rightMargin;
      if (pageSetup.headerDistance.IsNull)
        pageSetup.headerDistance = refPageSetup.headerDistance;
      if (pageSetup.footerDistance.IsNull)
        pageSetup.footerDistance = refPageSetup.footerDistance;
      if (pageSetup.oddAndEvenPagesHeaderFooter.IsNull)
        pageSetup.oddAndEvenPagesHeaderFooter = refPageSetup.oddAndEvenPagesHeaderFooter;
      if (pageSetup.differentFirstPageHeaderFooter.IsNull)
        pageSetup.differentFirstPageHeaderFooter = refPageSetup.differentFirstPageHeaderFooter;
      if (pageSetup.mirrorMargins.IsNull)
        pageSetup.mirrorMargins = refPageSetup.mirrorMargins;
      if (pageSetup.horizontalPageBreak.IsNull)
        pageSetup.horizontalPageBreak = refPageSetup.horizontalPageBreak;
    }

    protected void FlattenHeaderFooter(HeaderFooter headerFooter, bool isHeader)
    {
    }

    protected void FlattenFillFormat(FillFormat fillFormat)
    {
    }

    protected void FlattenLineFormat(LineFormat lineFormat, LineFormat refLineFormat)
    {
      if (refLineFormat != null)
      {
        if (lineFormat.width.IsNull)
          lineFormat.width = refLineFormat.width;
      }
    }

    protected void FlattenAxis(Axis axis)
    {
      if (axis == null)
        return;

      LineFormat refLineFormat = new LineFormat();
      refLineFormat.width = 0.15;
      if (axis.hasMajorGridlines.Value && axis.majorGridlines != null)
        FlattenLineFormat(axis.majorGridlines.lineFormat, refLineFormat);
      if (axis.hasMinorGridlines.Value && axis.minorGridlines != null)
        FlattenLineFormat(axis.minorGridlines.lineFormat, refLineFormat);

      refLineFormat.width = 0.4;
      if (axis.lineFormat != null)
        FlattenLineFormat(axis.lineFormat, refLineFormat);

      //      axis.majorTick;
      //      axis.majorTickMark;
      //      axis.minorTick;
      //      axis.minorTickMark;

      //      axis.maximumScale;
      //      axis.minimumScale;

      //      axis.tickLabels;
      //      axis.title;
    }

    protected void FlattenPlotArea(PlotArea plotArea)
    {
    }

    protected void FlattenDataLabel(DataLabel dataLabel)
    {
    }


    #region Chart
    internal override void VisitChart(Chart chart)
    {
      Document document = chart.Document;
      if (chart.style.IsNull)
        chart.style.Value = Style.DefaultParagraphName;
      Style style = document.Styles[chart.style.Value];
      if (chart.format == null)
      {
        chart.format = style.paragraphFormat.Clone();
        chart.format.parent = chart;
      }
      else
        FlattenParagraphFormat(chart.format, style.paragraphFormat);


      FlattenLineFormat(chart.lineFormat, null);
      FlattenFillFormat(chart.fillFormat);

      FlattenAxis(chart.xAxis);
      FlattenAxis(chart.yAxis);
      FlattenAxis(chart.zAxis);

      FlattenPlotArea(chart.plotArea);

      //      if (this.hasDataLabel.Value)
      FlattenDataLabel(chart.dataLabel);

    }
    #endregion

    #region Document
    internal override void VisitDocument(Document document)
    {
    }

    internal override void VisitDocumentElements(DocumentElements elements)
    {
    }
    #endregion

    #region Format
    internal override void VisitStyle(Style style)
    {
      Style baseStyle = style.GetBaseStyle();
      if (baseStyle != null && baseStyle.paragraphFormat != null)
      {
        if (style.paragraphFormat == null)
          style.paragraphFormat = baseStyle.paragraphFormat;
        else
          FlattenParagraphFormat(style.paragraphFormat, baseStyle.paragraphFormat);
      }
    }

    internal override void VisitStyles(Styles styles)
    {
    }
    #endregion

    #region Paragraph
    internal override void VisitFootnote(Footnote footnote)
    {
      Document document = footnote.Document;

      ParagraphFormat format = null;

      Style style = document.styles[footnote.style.Value];
      if (style != null)
        format = ParagraphFormatFromStyle(style);
      else
      {
        footnote.Style = "Footnote";
        format = document.styles[footnote.Style].paragraphFormat;
      }

      if (footnote.format == null)
      {
        footnote.format = format.Clone();
        footnote.format.parent = footnote;
      }
      else
        FlattenParagraphFormat(footnote.format, format);

    }

    internal override void VisitParagraph(Paragraph paragraph)
    {
      Document document = paragraph.Document;

      ParagraphFormat format = null;

      DocumentObject currentElementHolder = GetDocumentElementHolder(paragraph);
      Style style = document.styles[paragraph.style.Value];
      if (style != null)
        format = ParagraphFormatFromStyle(style);

      else if (currentElementHolder is Cell)
      {
        paragraph.style = ((Cell)currentElementHolder).style;
        format = ((Cell)currentElementHolder).format;
      }
      else if (currentElementHolder is HeaderFooter)
      {
        HeaderFooter currHeaderFooter = ((HeaderFooter)currentElementHolder);
        if (currHeaderFooter.IsHeader)
        {
          paragraph.Style = "Header";
          format = document.styles["Header"].paragraphFormat;
        }
        else
        {
          paragraph.Style = "Footer";
          format = document.styles["Footer"].paragraphFormat;
        }

        if (currHeaderFooter.format != null)
          FlattenParagraphFormat(paragraph.Format, currHeaderFooter.format);
      }
      else if (currentElementHolder is Footnote)
      {
        paragraph.Style = "Footnote";
        format = document.styles["Footnote"].paragraphFormat;
      }
      else if (currentElementHolder is TextArea)
      {
        paragraph.style = ((TextArea)currentElementHolder).style;
        format = ((TextArea)currentElementHolder).format;
      }
      else
      {
        if (paragraph.style.Value != "")
          paragraph.Style = "InvalidStyleName";
        else
          paragraph.Style = "Normal";
        format = document.styles[paragraph.Style].paragraphFormat;
      }

      if (paragraph.format == null)
      {
        paragraph.format = format.Clone();
        paragraph.format.parent = paragraph;
      }
      else
        FlattenParagraphFormat(paragraph.format, format);
    }
    #endregion

    #region Section
    internal override void VisitHeaderFooter(HeaderFooter headerFooter)
    {
      Document document = headerFooter.Document;
      string styleString;
      if (headerFooter.IsHeader)
        styleString = "Header";
      else
        styleString = "Footer";

      ParagraphFormat format;
      Style style = document.styles[headerFooter.style.Value];
      if (style != null)
        format = ParagraphFormatFromStyle(style);
      else
      {
        format = document.styles[styleString].paragraphFormat;
        headerFooter.Style = styleString;
      }

      if (headerFooter.format == null)
      {
        headerFooter.format = format.Clone();
        headerFooter.format.parent = headerFooter;
      }
      else
        FlattenParagraphFormat(headerFooter.format, format);
    }

    internal override void VisitHeadersFooters(HeadersFooters headersFooters)
    {
    }

    internal override void VisitSection(Section section)
    {
      Section prevSec = section.PreviousSection();
      PageSetup prevPageSetup = PageSetup.DefaultPageSetup;
      if (prevSec != null)
      {
        prevPageSetup = prevSec.pageSetup;

        if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.Primary))
          section.Headers.primary = prevSec.Headers.primary;
        if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
          section.Headers.evenPage = prevSec.Headers.evenPage;
        if (!section.Headers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
          section.Headers.firstPage = prevSec.Headers.firstPage;

        if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.Primary))
          section.Footers.primary = prevSec.Footers.primary;
        if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.EvenPage))
          section.Footers.evenPage = prevSec.Footers.evenPage;
        if (!section.Footers.HasHeaderFooter(HeaderFooterIndex.FirstPage))
          section.Footers.firstPage = prevSec.Footers.firstPage;
      }

      if (section.pageSetup == null)
        section.pageSetup = prevPageSetup;
      else
        FlattenPageSetup(section.pageSetup, prevPageSetup);
    }

    internal override void VisitSections(Sections sections)
    {
    }
    #endregion

    #region Shape
    internal override void VisitTextFrame(TextFrame textFrame)
    {
      if (textFrame.height.IsNull)
        textFrame.height = Unit.FromInch(1);
      if (textFrame.width.IsNull)
        textFrame.width = Unit.FromInch(1);
    }
    #endregion

    #region Table
    internal override void VisitCell(Cell cell)
    {
      // format, shading and borders are already processed.
    }

    internal override void VisitColumns(Columns columns)
    {
      foreach (Column col in columns)
      {
        if (col.width.IsNull)
          col.width = columns.width;

        if (col.width.IsNull)
          col.width = "2.5cm";
      }
    }

    internal override void VisitRow(Row row)
    {
      foreach (Cell cell in row.Cells)
      {
        if (cell.verticalAlignment.IsNull)
          cell.verticalAlignment = row.verticalAlignment;
      }
    }

    internal override void VisitRows(Rows rows)
    {
      foreach (Row row in rows)
      {
        if (row.height.IsNull)
          row.height = rows.height;
        if (row.heightRule.IsNull)
          row.heightRule = rows.heightRule;
        if (row.verticalAlignment.IsNull)
          row.verticalAlignment = rows.verticalAlignment;
      }
    }
    /// <summary>
    /// Returns a paragraph format object initialized by the given style.
    /// It differs from style.ParagraphFormat if style is a character style.
    /// </summary>
    ParagraphFormat ParagraphFormatFromStyle(Style style)
    {
      if (style.Type == StyleType.Character)
      {
        Document doc = style.Document;
        ParagraphFormat format = style.paragraphFormat.Clone();
        FlattenParagraphFormat(format, doc.Styles.Normal.ParagraphFormat);
        return format;
      }
      else
        return style.paragraphFormat;
    }

    internal override void VisitTable(Table table)
    {
      Document document = table.Document;

      if (table.leftPadding.IsNull)
        table.leftPadding = Unit.FromMillimeter(1.2);
      if (table.rightPadding.IsNull)
        table.rightPadding = Unit.FromMillimeter(1.2);

      ParagraphFormat format;
      Style style = document.styles[table.style.Value];
      if (style != null)
        format = ParagraphFormatFromStyle(style);
      else
      {
        table.Style = "Normal";
        format = document.styles.Normal.paragraphFormat;
      }

      if (table.format == null)
      {
        table.format = format.Clone();
        table.format.parent = table;
      }
      else
        FlattenParagraphFormat(table.format, format);

      int rows = table.Rows.Count;
      int clms = table.Columns.Count;

      for (int idxclm = 0; idxclm < clms; idxclm++)
      {
        Column column = table.Columns[idxclm];
        ParagraphFormat colFormat;
        style = document.styles[column.style.Value];
        if (style != null)
          colFormat = ParagraphFormatFromStyle(style);
        else
        {
          column.style = table.style;
          colFormat = table.Format;
        }

        if (column.format == null)
        {
          column.format = colFormat.Clone();
          column.format.parent = column;
          if (column.format.shading == null && table.format.shading != null)
            column.format.shading = table.format.shading;
        }
        else
          FlattenParagraphFormat(column.format, colFormat);

        if (column.leftPadding.IsNull)
          column.leftPadding = table.leftPadding;
        if (column.rightPadding.IsNull)
          column.rightPadding = table.rightPadding;

        if (column.shading == null)
          column.shading = table.shading;

        else if (table.shading != null)
          FlattenShading(column.shading, table.shading);

        if (column.borders == null)
          column.borders = table.borders;
        else if (table.borders != null)
          FlattenBorders(column.borders, table.borders);
      }

      for (int idxrow = 0; idxrow < rows; idxrow++)
      {
        Row row = table.Rows[idxrow];

        ParagraphFormat rowFormat;
        style = document.styles[row.style.Value];
        if (style != null)
        {
          rowFormat = ParagraphFormatFromStyle(style);
        }
        else
        {
          row.style = table.style;
          rowFormat = table.Format;
        }

        for (int idxclm = 0; idxclm < clms; idxclm++)
        {
          Column column = table.Columns[idxclm];
          Cell cell = row[idxclm];

          ParagraphFormat cellFormat;
          Style cellStyle = document.styles[cell.style.Value];
          if (cellStyle != null)
          {
            cellFormat = ParagraphFormatFromStyle(cellStyle);

            if (cell.format == null)
              cell.format = cellFormat;
            else
              FlattenParagraphFormat(cell.format, cellFormat);
          }
          else
          {
            if (row.format != null)
              FlattenParagraphFormat(cell.Format, row.format);

            if (style != null)
            {
              cell.style = row.style;
              FlattenParagraphFormat(cell.Format, rowFormat);
            }
            else
            {
              cell.style = column.style;
              FlattenParagraphFormat(cell.Format, column.format);
            }
          }

          if (cell.format.shading == null && table.format.shading != null)
            cell.format.shading = table.format.shading;

          if (cell.shading == null)
            cell.shading = row.shading;
          else if (row.shading != null)
            FlattenShading(cell.shading, row.shading);
          if (cell.shading == null)
            cell.shading = column.shading;
          else if (column.shading != null)
            FlattenShading(cell.shading, column.shading);
          if (cell.borders == null)
            cell.borders = row.borders;
          else if (row.borders != null)
            FlattenBorders(cell.borders, row.borders);
          if (cell.borders == null)
            cell.borders = column.borders;
          else if (column.borders != null)
            FlattenBorders(cell.borders, column.borders);
        }

        if (row.format == null)
        {
          row.format = rowFormat.Clone();
          row.format.parent = row;
          if (row.format.shading == null && table.format.shading != null)
            row.format.shading = table.format.shading;
        }
        else
          FlattenParagraphFormat(row.format, rowFormat);

        if (row.topPadding.IsNull)
          row.topPadding = table.topPadding;
        if (row.bottomPadding.IsNull)
          row.bottomPadding = table.bottomPadding;

        if (row.shading == null)
          row.shading = table.shading;
        else if (table.shading != null)
          FlattenShading(row.shading, table.shading);

        if (row.borders == null)
          row.borders = table.borders;
        else if (table.borders != null)
          FlattenBorders(row.borders, table.borders);
      }
    }
    #endregion


    internal override void VisitLegend(Legend legend)
    {
      ParagraphFormat parentFormat;
      if (!legend.style.IsNull)
      {
        Style style = legend.Document.Styles[legend.Style];
        if (style == null)
          style = legend.Document.Styles["InvalidStyleName"];

        parentFormat = style.paragraphFormat;
      }
      else
      {
        TextArea textArea = (TextArea)GetDocumentElementHolder(legend);
        legend.style = textArea.style;
        parentFormat = textArea.format;
      }
      if (legend.format == null)
        legend.Format = parentFormat.Clone();
      else
        FlattenParagraphFormat(legend.format, parentFormat);
    }

    internal override void VisitTextArea(TextArea textArea)
    {
      if (textArea == null || textArea.elements == null)
        return;

      Document document = textArea.Document;

      ParagraphFormat parentFormat;

      if (!textArea.style.IsNull)
      {
        Style style = textArea.Document.Styles[textArea.Style];
        if (style == null)
          style = textArea.Document.Styles["InvalidStyleName"];

        parentFormat = style.paragraphFormat;
      }
      else
      {
        Chart chart = (Chart)textArea.parent;
        parentFormat = chart.format;
        textArea.style = chart.style;
      }

      if (textArea.format == null)
        textArea.Format = parentFormat.Clone();
      else
        FlattenParagraphFormat(textArea.format, parentFormat);

      FlattenFillFormat(textArea.fillFormat);
      FlattenLineFormat(textArea.lineFormat, null);
    }


    private DocumentObject GetDocumentElementHolder(DocumentObject docObj)
    {
      DocumentElements docEls = (DocumentElements)docObj.parent;
      return docEls.parent;
    }
  }
}

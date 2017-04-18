#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
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
using System.Collections;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.IO;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Renders a table to an XGraphics object.
  /// </summary>
  internal class TableRenderer : Renderer
  {
    internal TableRenderer(XGraphics gfx, Table documentObject, FieldInfos fieldInfos)
      :
      base(gfx, documentObject, fieldInfos)
    {
      this.table = (Table)documentObject;
    }

    internal TableRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
      :
      base(gfx, renderInfo, fieldInfos)
    {
      this.table = (Table)this.renderInfo.DocumentObject;
    }

    internal override LayoutInfo InitialLayoutInfo
    {
      get
      {
        LayoutInfo layoutInfo = new LayoutInfo();
        layoutInfo.KeepTogether = this.table.KeepTogether;
        layoutInfo.KeepWithNext = false;
        layoutInfo.MarginBottom = 0;
        layoutInfo.MarginLeft = 0;
        layoutInfo.MarginTop = 0;
        layoutInfo.MarginRight = 0;
        return layoutInfo;
      }
    }


    void InitRendering()
    {
      TableFormatInfo formatInfo = (TableFormatInfo)this.renderInfo.FormatInfo;
      this.bottomBorderMap = formatInfo.bottomBorderMap;
      this.connectedRowsMap = formatInfo.connectedRowsMap;
      this.formattedCells = formatInfo.formattedCells;

      this.currRow = formatInfo.startRow;
      this.startRow = formatInfo.startRow;
      this.endRow = formatInfo.endRow;

      this.mergedCells = formatInfo.mergedCells;
      this.lastHeaderRow = formatInfo.lastHeaderRow;
      this.startX = this.renderInfo.LayoutInfo.ContentArea.X;
      this.startY = this.renderInfo.LayoutInfo.ContentArea.Y;
    }

    /// <summary>
    /// 
    /// </summary>
    void RenderHeaderRows()
    {
      if (this.lastHeaderRow < 0)
        return;

      foreach (Cell cell in this.mergedCells)
      {
        if (cell.Row.Index <= this.lastHeaderRow)
          RenderCell(cell);
      }
    }

    void RenderCell(Cell cell)
    {
      Rectangle innerRect = GetInnerRect(CalcStartingHeight(), cell);
      RenderShading(cell, innerRect);
      RenderContent(cell, innerRect);
      RenderBorders(cell, innerRect);
    }

    void RenderShading(Cell cell, Rectangle innerRect)
    {
      ShadingRenderer shadeRenderer = new ShadingRenderer(this.gfx, cell.Shading);
      shadeRenderer.Render(innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
    }

    void RenderBorders(Cell cell, Rectangle innerRect)
    {
      XUnit leftPos = innerRect.X;
      XUnit rightPos = leftPos + innerRect.Width;
      XUnit topPos = innerRect.Y;
      XUnit bottomPos = innerRect.Y + innerRect.Height;
      Borders mergedBorders = this.mergedCells.GetEffectiveBorders(cell);

      BordersRenderer bordersRenderer = new BordersRenderer(mergedBorders, this.gfx);
      XUnit bottomWidth = bordersRenderer.GetWidth(BorderType.Bottom);
      XUnit leftWidth = bordersRenderer.GetWidth(BorderType.Left);
      XUnit topWidth = bordersRenderer.GetWidth(BorderType.Top);
      XUnit rightWidth = bordersRenderer.GetWidth(BorderType.Right);

      bordersRenderer.RenderVertically(BorderType.Right, rightPos, topPos, bottomPos + bottomWidth - topPos);
      bordersRenderer.RenderVertically(BorderType.Left, leftPos - leftWidth, topPos, bottomPos + bottomWidth - topPos);
      bordersRenderer.RenderHorizontally(BorderType.Bottom, leftPos - leftWidth, bottomPos, rightPos + rightWidth + leftWidth - leftPos);
      bordersRenderer.RenderHorizontally(BorderType.Top, leftPos - leftWidth, topPos - topWidth, rightPos + rightWidth + leftWidth - leftPos);

      RenderDiagonalBorders(mergedBorders, innerRect);
    }

    void RenderDiagonalBorders(Borders mergedBorders, Rectangle innerRect)
    {
      BordersRenderer bordersRenderer = new BordersRenderer(mergedBorders, this.gfx);
      bordersRenderer.RenderDiagonally(BorderType.DiagonalDown, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
      bordersRenderer.RenderDiagonally(BorderType.DiagonalUp, innerRect.X, innerRect.Y, innerRect.Width, innerRect.Height);
    }

    void RenderContent(Cell cell, Rectangle innerRect)
    {
      FormattedCell formattedCell = (FormattedCell)this.formattedCells[cell];
      RenderInfo[] renderInfos = formattedCell.GetRenderInfos();

      if (renderInfos == null)
        return;

      VerticalAlignment verticalAlignment = cell.VerticalAlignment;
      XUnit contentHeight = formattedCell.ContentHeight;
      XUnit innerHeight = innerRect.Height;
      XUnit targetX = innerRect.X + cell.Column.LeftPadding;

      XUnit targetY;
      if (verticalAlignment == VerticalAlignment.Bottom)
      {
        targetY = innerRect.Y + innerRect.Height;
        targetY -= cell.Row.BottomPadding;
        targetY -= contentHeight;
      }
      else if (verticalAlignment == VerticalAlignment.Center)
      {
        targetY = innerRect.Y + cell.Row.TopPadding;
        targetY += innerRect.Y + innerRect.Height - cell.Row.BottomPadding;
        targetY -= contentHeight;
        targetY /= 2;
      }
      else
        targetY = innerRect.Y + cell.Row.TopPadding;

      RenderByInfos(targetX, targetY, renderInfos);
    }



    Rectangle GetInnerRect(XUnit startingHeight, Cell cell)
    {
      BordersRenderer bordersRenderer = new BordersRenderer(this.mergedCells.GetEffectiveBorders(cell), this.gfx);
      FormattedCell formattedCell = (FormattedCell)this.formattedCells[cell];
      XUnit width = formattedCell.InnerWidth;

      XUnit y = this.startY;
      if (cell.Row.Index > this.lastHeaderRow)
        y += startingHeight;
      else
        y += CalcMaxTopBorderWidth(0);

      XUnit upperBorderPos = (XUnit)this.bottomBorderMap[cell.Row.Index];

      y += upperBorderPos;
      if (cell.Row.Index > this.lastHeaderRow)
        y -= (XUnit)this.bottomBorderMap[this.startRow];

      XUnit lowerBorderPos = (XUnit)this.bottomBorderMap[cell.Row.Index + cell.MergeDown + 1];


      XUnit height = lowerBorderPos - upperBorderPos;
      height -= bordersRenderer.GetWidth(BorderType.Bottom);

      XUnit x = this.startX;
      for (int clmIdx = 0; clmIdx < cell.Column.Index; ++clmIdx)
      {
        x += this.table.Columns[clmIdx].Width;
      }
      x += LeftBorderOffset;

      return new Rectangle(x, y, width, height);
    }

    internal override void Render()
    {
      InitRendering();
      RenderHeaderRows();
      if (startRow < this.table.Rows.Count)
      {
        Cell cell = this.table[startRow, 0];

        int cellIdx = this.mergedCells.BinarySearch(this.table[startRow, 0], new CellComparer());
        while (cellIdx < this.mergedCells.Count)
        {
          cell = (Cell)this.mergedCells[cellIdx];
          if (cell.Row.Index > this.endRow)
            break;

          RenderCell(cell);
          ++cellIdx;
        }
      }
    }

    void InitFormat(Area area, FormatInfo previousFormatInfo)
    {
      TableFormatInfo prevTableFormatInfo = (TableFormatInfo)previousFormatInfo;
      TableRenderInfo tblRenderInfo = new TableRenderInfo();
      tblRenderInfo.table = this.table;

      this.renderInfo = tblRenderInfo;

      if (prevTableFormatInfo != null)
      {
        this.mergedCells = prevTableFormatInfo.mergedCells;
        this.formattedCells = prevTableFormatInfo.formattedCells;
        this.bottomBorderMap = prevTableFormatInfo.bottomBorderMap;
        this.lastHeaderRow = prevTableFormatInfo.lastHeaderRow;
        this.connectedRowsMap = prevTableFormatInfo.connectedRowsMap;
        this.startRow = prevTableFormatInfo.endRow + 1;
      }
      else
      {
        this.mergedCells = new MergedCellList(this.table);
        FormatCells();
        CalcLastHeaderRow();
        CreateConnectedRows();
        CreateBottomBorderMap();
        if (this.doHorizontalBreak)
        {
          CalcLastHeaderColumn();
          CreateConnectedColumns();
        }
        this.startRow = this.lastHeaderRow + 1;
      }
      ((TableFormatInfo)tblRenderInfo.FormatInfo).mergedCells = this.mergedCells;
      ((TableFormatInfo)tblRenderInfo.FormatInfo).formattedCells = this.formattedCells;
      ((TableFormatInfo)tblRenderInfo.FormatInfo).bottomBorderMap = this.bottomBorderMap;
      ((TableFormatInfo)tblRenderInfo.FormatInfo).connectedRowsMap = this.connectedRowsMap;
      ((TableFormatInfo)tblRenderInfo.FormatInfo).lastHeaderRow = this.lastHeaderRow;
    }

    void FormatCells()
    {
      this.formattedCells = new SortedList(new CellComparer());
      foreach (Cell cell in this.mergedCells)
      {
        FormattedCell formattedCell = new FormattedCell(cell, this.documentRenderer, this.mergedCells.GetEffectiveBorders(cell), this.fieldInfos, 0, 0);
        formattedCell.Format(this.gfx);
        this.formattedCells.Add(cell, formattedCell);
      }
    }

    /// <summary>
    /// Formats (measures) the table.
    /// </summary>
    /// <param name="area">The area on which to fit the table.</param>
    /// <param name="previousFormatInfo"></param>
    internal override void Format(Area area, FormatInfo previousFormatInfo)
    {
      DocumentElements elements = DocumentRelations.GetParent(this.table) as DocumentElements;
      if (elements != null)
      {
        Section section = DocumentRelations.GetParent(elements) as Section;
        if (section != null)
          this.doHorizontalBreak = section.PageSetup.HorizontalPageBreak;
      }

      this.renderInfo = new TableRenderInfo();
      InitFormat(area, previousFormatInfo);

      // Don't take any Rows higher then MaxElementHeight
      XUnit topHeight = this.CalcStartingHeight();
      XUnit probeHeight = topHeight;
      XUnit offset = 0;
      if (this.startRow > this.lastHeaderRow + 1 &&
        this.startRow < this.table.Rows.Count)
        offset = (XUnit)this.bottomBorderMap[this.startRow] - topHeight;
      else
        offset = -CalcMaxTopBorderWidth(0);

      int probeRow = this.startRow;
      XUnit currentHeight = 0;
      XUnit startingHeight = 0;
      bool isEmpty = false;

      while (probeRow < this.table.Rows.Count)
      {
        bool firstProbe = probeRow == this.startRow;
        probeRow = (int)this.connectedRowsMap[probeRow];
        // Don't take any Rows higher then MaxElementHeight
        probeHeight = (XUnit)this.bottomBorderMap[probeRow + 1] - offset;
        if (firstProbe && probeHeight > MaxElementHeight - Tolerance)
            probeHeight = MaxElementHeight - Tolerance;

        //The height for the first new row(s) + headerrows:
        if (startingHeight == 0)
        {
          if (probeHeight > area.Height)
          {
            isEmpty = true;
            break;
          }
          startingHeight = probeHeight;
        }

        if (probeHeight > area.Height)
          break;

        else
        {
          this.currRow = probeRow;
          currentHeight = probeHeight;
          ++probeRow;
        }
      }
      if (!isEmpty)
      {
        TableFormatInfo formatInfo = (TableFormatInfo)this.renderInfo.FormatInfo;
        formatInfo.startRow = this.startRow;
        formatInfo.isEnding = currRow >= this.table.Rows.Count - 1;
        formatInfo.endRow = this.currRow;
      }
      FinishLayoutInfo(area, currentHeight, startingHeight);
    }

    void FinishLayoutInfo(Area area, XUnit currentHeight, XUnit startingHeight)
    {
      LayoutInfo layoutInfo = this.renderInfo.LayoutInfo;
      layoutInfo.StartingHeight = startingHeight;
      //REM: Trailing height would have to be calculated in case tables had a keep with next property.
      layoutInfo.TrailingHeight = 0;
      if (this.currRow >= 0)
      {
        layoutInfo.ContentArea = new Rectangle(area.X, area.Y, 0, currentHeight);
        XUnit width = LeftBorderOffset;
        foreach (Column clm in this.table.Columns)
        {
          width += clm.Width;
        }
        layoutInfo.ContentArea.Width = width;
      }
      layoutInfo.MinWidth = layoutInfo.ContentArea.Width;

      if (!this.table.Rows.IsNull("LeftIndent"))
        layoutInfo.Left = this.table.Rows.LeftIndent.Point;

      else if (this.table.Rows.Alignment == RowAlignment.Left)
      {
        if (table.Columns.Count > 0) // Errors in Wiki syntax can lead to tables w/o columns ...
        {
          XUnit leftOffset = LeftBorderOffset;
          leftOffset += table.Columns[0].LeftPadding;
          layoutInfo.Left = -leftOffset;
        }
#if DEBUG
        else
          table.GetType();
#endif
      }

      switch (this.table.Rows.Alignment)
      {
        case RowAlignment.Left:
          layoutInfo.HorizontalAlignment = ElementAlignment.Near;
          break;

        case RowAlignment.Right:
          layoutInfo.HorizontalAlignment = ElementAlignment.Far;
          break;

        case RowAlignment.Center:
          layoutInfo.HorizontalAlignment = ElementAlignment.Center;
          break;
      }
    }

    XUnit LeftBorderOffset
    {
      get
      {
        if (this.leftBorderOffset < 0)
        {
          if (table.Rows.Count > 0 && table.Columns.Count > 0)
          {
            Borders borders = this.mergedCells.GetEffectiveBorders(table[0, 0]);
            BordersRenderer bordersRenderer = new BordersRenderer(borders, this.gfx);
            this.leftBorderOffset = bordersRenderer.GetWidth(BorderType.Left);
          }
          else
            this.leftBorderOffset = 0;
        }
        return this.leftBorderOffset;
      }
    }
    private XUnit leftBorderOffset = -1;

    /// <summary>
    /// Calcs either the height of the header rows or the height of the uppermost top border.
    /// </summary>
    /// <returns></returns>
    XUnit CalcStartingHeight()
    {
      XUnit height = 0;
      if (this.lastHeaderRow >= 0)
      {
        height = (XUnit)this.bottomBorderMap[this.lastHeaderRow + 1];
        height += CalcMaxTopBorderWidth(0);
      }
      else
      {
        if (this.table.Rows.Count > this.startRow)
          height = CalcMaxTopBorderWidth(this.startRow);
      }

      return height;
    }


    void CalcLastHeaderColumn()
    {
      this.lastHeaderColumn = -1;
      foreach (Column clm in this.table.Columns)
      {
        if (clm.HeadingFormat)
          this.lastHeaderColumn = clm.Index;
        else break;
      }
      if (this.lastHeaderColumn >= 0)
        this.lastHeaderRow = CalcLastConnectedColumn(this.lastHeaderColumn);

      //Ignore heading format if all the table is heading:
      if (this.lastHeaderRow == this.table.Rows.Count - 1)
        this.lastHeaderRow = -1;

    }

    void CalcLastHeaderRow()
    {
      this.lastHeaderRow = -1;
      foreach (Row row in this.table.Rows)
      {
        if (row.HeadingFormat)
          this.lastHeaderRow = row.Index;
        else break;
      }
      if (this.lastHeaderRow >= 0)
        this.lastHeaderRow = CalcLastConnectedRow(this.lastHeaderRow);

      //Ignore heading format if all the table is heading:
      if (this.lastHeaderRow == this.table.Rows.Count - 1)
        this.lastHeaderRow = -1;

    }

    void CreateConnectedRows()
    {
      this.connectedRowsMap = new SortedList();
      foreach (Cell cell in this.mergedCells)
      {
        if (!this.connectedRowsMap.ContainsKey(cell.Row.Index))
        {
          int lastConnectedRow = CalcLastConnectedRow(cell.Row.Index);
          this.connectedRowsMap[cell.Row.Index] = lastConnectedRow;
        }
      }
    }

    void CreateConnectedColumns()
    {
      this.connectedColumnsMap = new SortedList();
      foreach (Cell cell in this.mergedCells)
      {
        if (!this.connectedColumnsMap.ContainsKey(cell.Column.Index))
        {
          int lastConnectedColumn = CalcLastConnectedColumn(cell.Column.Index);
          this.connectedColumnsMap[cell.Column.Index] = lastConnectedColumn;
        }
      }
    }

    void CreateBottomBorderMap()
    {
      this.bottomBorderMap = new SortedList();
      this.bottomBorderMap.Add(0, XUnit.FromPoint(0));
      while (!this.bottomBorderMap.ContainsKey(this.table.Rows.Count))
      {
        CreateNextBottomBorderPosition();
      }
    }

    /// <summary>
    /// Calculates the top border width for the first row that is rendered or formatted.
    /// </summary>
    /// <param name="row">The row index.</param>
    XUnit CalcMaxTopBorderWidth(int row)
    {
      XUnit maxWidth = 0;
      if (this.table.Rows.Count > row)
      {
        int cellIdx = this.mergedCells.BinarySearch(this.table[row, 0], new CellComparer());
        Cell rowCell = this.mergedCells[cellIdx];
        while (cellIdx < this.mergedCells.Count)
        {
          rowCell = this.mergedCells[cellIdx];
          if (rowCell.Row.Index > row)
            break;

          if (!rowCell.IsNull("Borders"))
          {
            BordersRenderer bordersRenderer = new BordersRenderer(rowCell.Borders, this.gfx);
            XUnit width = 0;
            width = bordersRenderer.GetWidth(BorderType.Top);
            if (width > maxWidth)
              maxWidth = width;
          }
          ++cellIdx;
        }
      }
      return maxWidth;
    }

    /// <summary>
    /// Creates the next bottom border position.
    /// </summary>
    void CreateNextBottomBorderPosition()
    {
      int lastIdx = bottomBorderMap.Count - 1;
      int lastBorderRow = (int)bottomBorderMap.GetKey(lastIdx);
      XUnit lastPos = (XUnit)bottomBorderMap.GetByIndex(lastIdx);
      Cell minMergedCell = GetMinMergedCell(lastBorderRow);
      FormattedCell minMergedFormattedCell = (FormattedCell)this.formattedCells[minMergedCell];
      XUnit maxBottomBorderPosition = lastPos + minMergedFormattedCell.InnerHeight;
      maxBottomBorderPosition += CalcBottomBorderWidth(minMergedCell);

      foreach (Cell cell in this.mergedCells)
      {
        if (cell.Row.Index > minMergedCell.Row.Index + minMergedCell.MergeDown)
          break;

        if (cell.Row.Index + cell.MergeDown == minMergedCell.Row.Index + minMergedCell.MergeDown)
        {
          FormattedCell formattedCell = (FormattedCell)this.formattedCells[cell];
          XUnit topBorderPos = (XUnit)this.bottomBorderMap[cell.Row.Index];
          XUnit bottomBorderPos = topBorderPos + formattedCell.InnerHeight;
          bottomBorderPos += CalcBottomBorderWidth(cell);
          if (bottomBorderPos > maxBottomBorderPosition)
            maxBottomBorderPosition = bottomBorderPos;
        }
      }
      this.bottomBorderMap.Add(minMergedCell.Row.Index + minMergedCell.MergeDown + 1, maxBottomBorderPosition);
    }

    /// <summary>
    /// Calculates bottom border width of a cell.
    /// </summary>
    /// <param name="cell">The cell the bottom border of the row that is probed.</param>
    /// <returns>The calculated border width.</returns>
    XUnit CalcBottomBorderWidth(Cell cell)
    {
      Borders borders = this.mergedCells.GetEffectiveBorders(cell);
      if (borders != null)
      {
        BordersRenderer bordersRenderer = new BordersRenderer(borders, this.gfx);
        return bordersRenderer.GetWidth(BorderType.Bottom);
      }
      return 0;
    }

    /// <summary>
    /// Gets the first cell in the given row that is merged down minimally.
    /// </summary>
    /// <param name="row">The row to prope.</param>
    /// <returns>The first cell with minimal vertical merge.</returns>
    Cell GetMinMergedCell(int row)
    {
      int minMerge = this.table.Rows.Count;
      Cell minCell = null;
      foreach (Cell cell in this.mergedCells)
      {
        if (cell.Row.Index == row)
        {
          if (cell.MergeDown == 0)
          {
            minCell = cell;
            break;
          }
          else if (cell.MergeDown < minMerge)
          {
            minMerge = cell.MergeDown;
            minCell = cell;
          }
        }
        else if (cell.Row.Index > row)
          break;
      }
      return minCell;
    }


    /// <summary>
    /// Calculates the last row that is connected with the given row.
    /// </summary>
    /// <param name="row">The row that is probed for downward connection.</param>
    /// <returns>The last row that is connected with the given row.</returns>
    int CalcLastConnectedRow(int row)
    {
      int lastConnectedRow = row;
      foreach (Cell cell in this.mergedCells)
      {
        if (cell.Row.Index <= lastConnectedRow)
        {
          int downConnection = Math.Max(cell.Row.KeepWith, cell.MergeDown);
          if (lastConnectedRow < cell.Row.Index + downConnection)
            lastConnectedRow = cell.Row.Index + downConnection;
        }
      }
      return lastConnectedRow;
    }

    /// <summary>
    /// Calculates the last column that is connected with the specified column.
    /// </summary>
    /// <param name="column">The column that is probed for downward connection.</param>
    /// <returns>The last column that is connected with the given column.</returns>
    int CalcLastConnectedColumn(int column)
    {
      int lastConnectedColumn = column;
      foreach (Cell cell in this.mergedCells)
      {
        if (cell.Column.Index <= lastConnectedColumn)
        {
          int rightConnection = Math.Max(cell.Column.KeepWith, cell.MergeRight);
          if (lastConnectedColumn < cell.Column.Index + rightConnection)
            lastConnectedColumn = cell.Column.Index + rightConnection;
        }
      }
      return lastConnectedColumn;
    }



    Table table;
    MergedCellList mergedCells;
    SortedList formattedCells;
    SortedList bottomBorderMap;
    SortedList connectedRowsMap;
    SortedList connectedColumnsMap;

    int lastHeaderRow;
    int lastHeaderColumn;
    int startRow;
    int currRow;
    int endRow = -1;

    bool doHorizontalBreak = false;
    XUnit startX;
    XUnit startY;

  }
}

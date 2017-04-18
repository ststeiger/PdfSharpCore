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
using MigraDocCore.DocumentObjectModel;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.IO;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Represents a formatted cell.
  /// </summary>
  internal class FormattedCell : IAreaProvider
  {
    internal FormattedCell(Cell cell, DocumentRenderer documentRenderer, Borders cellBorders, FieldInfos fieldInfos, XUnit xOffset, XUnit yOffset)
    {
      this.cell = cell;
      this.fieldInfos = fieldInfos;
      this.yOffset = yOffset;
      this.xOffset = xOffset;
      this.bordersRenderer = new BordersRenderer(cellBorders, null);
      this.documentRenderer = documentRenderer;
    }

    bool isFirstArea = true;
    Area IAreaProvider.GetNextArea()
    {
      if (this.isFirstArea)
      {
        Rectangle rect = CalcContentRect();
        this.isFirstArea = false;
        return rect;
      }
      return null;
    }

    Area IAreaProvider.ProbeNextArea()
    {
      return null;
    }

    internal void Format(XGraphics gfx)
    {
      this.gfx = gfx;
      this.formatter = new TopDownFormatter(this, this.documentRenderer, this.cell.Elements);
      this.formatter.FormatOnAreas(gfx, false);
      this.contentHeight = CalcContentHeight(this.documentRenderer);
    }

    private Rectangle CalcContentRect()
    {
      Column column = this.cell.Column;
      XUnit width = InnerWidth;
      width -= column.LeftPadding.Point;
      Column rightColumn = this.cell.Table.Columns[column.Index + this.cell.MergeRight];
      width -= rightColumn.RightPadding.Point;

      XUnit height = double.MaxValue;
      return new Rectangle(this.xOffset, this.yOffset, width, height);
    }

    internal XUnit ContentHeight
    {
      get { return this.contentHeight; }
    }

    internal XUnit InnerHeight
    {
      get
      {
        Row row = this.cell.Row;
        XUnit verticalPadding = row.TopPadding.Point;
        verticalPadding += row.BottomPadding.Point;

        switch (row.HeightRule)
        {
          case RowHeightRule.Exactly:
            return row.Height.Point;

          case RowHeightRule.Auto:
            return verticalPadding + this.contentHeight;

          case RowHeightRule.AtLeast:
          default:
            return Math.Max(row.Height, verticalPadding + this.contentHeight);
        }
      }
    }

    internal XUnit InnerWidth
    {
      get
      {
        XUnit width = 0;
        int cellColumnIdx = this.cell.Column.Index;
        for (int toRight = 0; toRight <= this.cell.MergeRight; ++toRight)
        {
          int columnIdx = cellColumnIdx + toRight;
          width += this.cell.Table.Columns[columnIdx].Width;
        }
        width -= this.bordersRenderer.GetWidth(BorderType.Right);

        return width;
      }
    }

    FieldInfos IAreaProvider.AreaFieldInfos
    {
      get
      {
        return this.fieldInfos;
      }
    }

    void IAreaProvider.StoreRenderInfos(System.Collections.ArrayList renderInfos)
    {
      this.renderInfos = renderInfos;
    }

    bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
    {
      return false;
    }

    bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
    {
      return false;
    }

    bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
    {
      return false;
    }

    private XUnit CalcContentHeight(DocumentRenderer documentRenderer)
    {
      XUnit height = RenderInfo.GetTotalHeight(GetRenderInfos());
      if (height == 0)
      {
        height = ParagraphRenderer.GetLineHeight(this.cell.Format, this.gfx, documentRenderer);
        height += this.cell.Format.SpaceBefore;
        height += this.cell.Format.SpaceAfter;
      }
      return height;
    }

    XUnit contentHeight = 0;

    internal RenderInfo[] GetRenderInfos()
    {
      if (this.renderInfos != null)
        return (RenderInfo[])this.renderInfos.ToArray(typeof(RenderInfo));

      return null;
    }

    private FieldInfos fieldInfos;
    private ArrayList renderInfos;
    private XUnit xOffset;
    private XUnit yOffset;
    private Cell cell;
    private TopDownFormatter formatter;
    BordersRenderer bordersRenderer;
    XGraphics gfx;
    DocumentRenderer documentRenderer;
  }
}

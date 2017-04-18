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
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Shapes;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Represents a formatted text frame.
  /// </summary>
  internal class FormattedTextFrame : IAreaProvider
  {
    internal FormattedTextFrame(TextFrame textframe, DocumentRenderer documentRenderer, FieldInfos fieldInfos)
    {
      this.textframe = textframe;
      this.fieldInfos = fieldInfos;
      this.documentRenderer = documentRenderer;
    }

    internal void Format(XGraphics gfx)
    {
      this.gfx = gfx;
      this.isFirstArea = true;
      this.formatter = new TopDownFormatter(this, this.documentRenderer, this.textframe.Elements);
      this.formatter.FormatOnAreas(gfx, false);
      this.contentHeight = RenderInfo.GetTotalHeight(GetRenderInfos());
    }

    Area IAreaProvider.GetNextArea()
    {
      if (this.isFirstArea)
        return CalcContentRect();

      return null;
    }

    Area IAreaProvider.ProbeNextArea()
    {
      return null;
    }

    FieldInfos IAreaProvider.AreaFieldInfos
    {
      get
      {
        return this.fieldInfos;
      }
    }

    void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
    {
      this.renderInfos = renderInfos;
    }

    bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
    {
      return false;
    }

    internal RenderInfo[] GetRenderInfos()
    {
      if (this.renderInfos != null)
        return (RenderInfo[])this.renderInfos.ToArray(typeof(RenderInfo));

      return null;
    }

    Rectangle CalcContentRect()
    {
      LineFormatRenderer lfr = new LineFormatRenderer(this.textframe.LineFormat, this.gfx);
      XUnit lineWidth = lfr.GetWidth();
      XUnit width;
      XUnit xOffset = lineWidth / 2;
      XUnit yOffset = lineWidth / 2;

      if (this.textframe.Orientation == TextOrientation.Horizontal ||
        this.textframe.Orientation == TextOrientation.HorizontalRotatedFarEast)
      {
        width = this.textframe.Width.Point;
        xOffset += this.textframe.MarginLeft;
        yOffset += this.textframe.MarginTop;
        width -= xOffset;
        width -= this.textframe.MarginRight + lineWidth / 2;
      }
      else
      {
        width = this.textframe.Height.Point;
        if (this.textframe.Orientation == TextOrientation.Upward)
        {
          xOffset += this.textframe.MarginBottom;
          yOffset += this.textframe.MarginLeft;
          width -= xOffset;
          width -= this.textframe.MarginTop + lineWidth / 2;
        }
        else
        {
          xOffset += this.textframe.MarginTop;
          yOffset += this.textframe.MarginRight;
          width -= xOffset;
          width -= this.textframe.MarginBottom + lineWidth / 2;
        }
      }
      XUnit height = double.MaxValue;
      return new Rectangle(xOffset, yOffset, width, height);
    }

    XUnit ContentHeight
    {
      get { return this.contentHeight; }
    }

    bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
    {
      return false;
    }

    bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
    {
      Rectangle rect = CalcContentRect();
      switch (layoutInfo.HorizontalAlignment)
      {
        case ElementAlignment.Near:
          if (layoutInfo.Left != 0)
          {
            layoutInfo.ContentArea.X += layoutInfo.Left;
            return true;
          }
          return false;

        case ElementAlignment.Far:
          XUnit xPos = rect.X + rect.Width;
          xPos -= layoutInfo.ContentArea.Width;
          xPos -= layoutInfo.MarginRight;
          layoutInfo.ContentArea.X = xPos;
          return true;

        case ElementAlignment.Center:
          xPos = rect.Width;
          xPos -= layoutInfo.ContentArea.Width;
          xPos = rect.X + xPos / 2;
          layoutInfo.ContentArea.X = xPos;
          return true;
      }
      return false;
    }

    private TextFrame textframe;
    private FieldInfos fieldInfos;
    private TopDownFormatter formatter;
    private ArrayList renderInfos;
    private XGraphics gfx;
    private bool isFirstArea;
    private XUnit contentHeight;
    private DocumentRenderer documentRenderer;
  }
}

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
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Represents a formatted text area.
  /// </summary>
  internal class FormattedTextArea : IAreaProvider
  {
    internal FormattedTextArea(DocumentRenderer documentRenderer, TextArea textArea, FieldInfos fieldInfos)
    {
      this.textArea = textArea;
      this.fieldInfos = fieldInfos;
      this.documentRenderer = documentRenderer;
    }

    internal void Format(XGraphics gfx)
    {
      this.gfx = gfx;
      this.isFirstArea = true;
      this.formatter = new TopDownFormatter(this, this.documentRenderer, this.textArea.Elements);
      this.formatter.FormatOnAreas(gfx, false);
    }

    internal XUnit InnerWidth
    {
      set { this.innerWidth = value; }
      get
      {
        if (double.IsNaN(this.innerWidth))
        {
          if (!this.textArea.IsNull("Width"))
            this.innerWidth = textArea.Width.Point;
          else
            this.innerWidth = CalcInherentWidth();
        }
        return this.innerWidth;
      }
    }
    XUnit innerWidth = double.NaN;

    internal XUnit InnerHeight
    {
      get
      {
        if (this.textArea.IsNull("Height"))
          return this.ContentHeight + this.textArea.TopPadding + this.textArea.BottomPadding;
        return this.textArea.Height.Point;
      }
    }


    XUnit CalcInherentWidth()
    {
      XUnit inherentWidth = 0;
      foreach (DocumentObject obj in this.textArea.Elements)
      {
        Renderer renderer = Renderer.Create(this.gfx, this.documentRenderer, obj, this.fieldInfos);
        if (renderer != null)
        {
          renderer.Format(new Rectangle(0, 0, double.MaxValue, double.MaxValue), null);
          inherentWidth = Math.Max(renderer.RenderInfo.LayoutInfo.MinWidth, inherentWidth);
        }
      }
      inherentWidth += this.textArea.LeftPadding;
      inherentWidth += this.textArea.RightPadding;
      return inherentWidth;
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

    internal XUnit ContentHeight
    {
      get
      {
        return RenderInfo.GetTotalHeight(this.GetRenderInfos());
      }
    }

    Rectangle CalcContentRect()
    {
      XUnit width = this.InnerWidth - this.textArea.LeftPadding - this.textArea.RightPadding;
      XUnit height = double.MaxValue;
      return new Rectangle(0, 0, width, height);
    }

    bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
    {
      return false;
    }

    bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
    {
      return false;
    }

    internal TextArea textArea;
    private FieldInfos fieldInfos;
    private TopDownFormatter formatter;
    private ArrayList renderInfos;
    private XGraphics gfx;
    private bool isFirstArea;
    DocumentRenderer documentRenderer;
  }
}

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
using MigraDocCore.DocumentObjectModel;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Renders a page break to an XGraphics object.
  /// </summary>
  internal class PageBreakRenderer : Renderer
  {
    /// <summary>
    /// Initializes a ParagraphRenderer object for formatting.
    /// </summary>
    /// <param name="gfx">The XGraphics object to do measurements on.</param>
    /// <param name="pageBreak">The page break.</param>
    /// <param name="fieldInfos">The field infos.</param>
    internal PageBreakRenderer(XGraphics gfx, PageBreak pageBreak, FieldInfos fieldInfos)
      : base(gfx, pageBreak, fieldInfos)
    {
      this.pageBreak = pageBreak;
    }

    /// <summary>
    /// Initializes a ParagraphRenderer object for rendering.
    /// </summary>
    /// <param name="gfx">The XGraphics object to render on.</param>
    /// <param name="renderInfo">The render info object containing information necessary for rendering.</param>
    /// <param name="fieldInfos">The field infos.</param>
    internal PageBreakRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
      : base(gfx, renderInfo, fieldInfos)
    {
      this.renderInfo = renderInfo;
    }

    internal override void Format(Area area, FormatInfo previousFormatInfo)
    {
      PageBreakRenderInfo pbRenderInfo = new PageBreakRenderInfo();
      pbRenderInfo.pageBreakFormatInfo = new PageBreakFormatInfo();
      this.renderInfo = pbRenderInfo;

      pbRenderInfo.LayoutInfo.PageBreakBefore = true;
      pbRenderInfo.LayoutInfo.ContentArea = new Rectangle(area.Y, area.Y, 0, 0);
      pbRenderInfo.pageBreak = this.pageBreak;
    }

    internal override void Render()
    {
      //Nothing to do here.
    }

    internal override LayoutInfo InitialLayoutInfo
    {
      get
      {
        LayoutInfo layoutInfo = new LayoutInfo();
        layoutInfo.PageBreakBefore = true;
        return layoutInfo;
      }
    }
    PageBreak pageBreak;
  }
}

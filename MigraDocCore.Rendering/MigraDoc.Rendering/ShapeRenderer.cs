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
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Renders a shape to an XGraphics object.
  /// </summary>
  internal abstract class ShapeRenderer : Renderer
  {

    internal ShapeRenderer(XGraphics gfx, Shape shape, FieldInfos fieldInfos)
      : base(gfx, shape, fieldInfos)
    {
      this.shape = shape;
      LineFormat lf = (LineFormat)this.shape.GetValue("LineFormat", GV.ReadOnly);
      this.lineFormatRenderer = new LineFormatRenderer(lf, gfx);
    }

    internal ShapeRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
      : base(gfx, renderInfo, fieldInfos)
    {
      this.shape = (Shape)renderInfo.DocumentObject;
      LineFormat lf = (LineFormat)this.shape.GetValue("LineFormat", GV.ReadOnly);
      this.lineFormatRenderer = new LineFormatRenderer(lf, gfx);
      FillFormat ff = (FillFormat)this.shape.GetValue("FillFormat", GV.ReadOnly);
      this.fillFormatRenderer = new FillFormatRenderer(ff, gfx);
    }

    internal override LayoutInfo InitialLayoutInfo
    {
      get
      {
        LayoutInfo layoutInfo = new LayoutInfo();

        layoutInfo.MarginTop = this.shape.WrapFormat.DistanceTop.Point;
        layoutInfo.MarginLeft = this.shape.WrapFormat.DistanceLeft.Point;
        layoutInfo.MarginBottom = this.shape.WrapFormat.DistanceBottom.Point;
        layoutInfo.MarginRight = this.shape.WrapFormat.DistanceRight.Point;
        layoutInfo.KeepTogether = true;
        layoutInfo.KeepWithNext = false;
        layoutInfo.PageBreakBefore = false;
        layoutInfo.VerticalReference = GetVerticalReference();
        layoutInfo.HorizontalReference = GetHorizontalReference();
        layoutInfo.Floating = GetFloating();
        if (layoutInfo.Floating == Floating.TopBottom &&!this.shape.Top.Position.IsEmpty)
        {
          layoutInfo.MarginTop = Math.Max(layoutInfo.MarginTop, this.shape.Top.Position);
        }
        return layoutInfo;
      }
    }

    Floating GetFloating()
    {
      if (this.shape.RelativeVertical != RelativeVertical.Line &&
          this.shape.RelativeVertical != RelativeVertical.Paragraph)
        return Floating.None;

      switch (this.shape.WrapFormat.Style)
      {
        case WrapStyle.None:
        case WrapStyle.Through:
          return Floating.None;
      }
      return Floating.TopBottom;
    }

    /// <summary>
    /// Gets the shape width including line width.
    /// </summary>
    protected virtual XUnit ShapeWidth
    {
      get
      {
        return this.shape.Width + this.lineFormatRenderer.GetWidth();
      }
    }

    /// <summary>
    /// Gets the shape height including line width.
    /// </summary>
    protected virtual XUnit ShapeHeight
    {
      get { return this.shape.Height + this.lineFormatRenderer.GetWidth(); }
    }

    /// <summary>
    /// Formats the shape.
    /// </summary>
    /// <param name="area">The area to fit in the shape.</param>
    /// <param name="previousFormatInfo"></param>
    internal override void Format(Area area, FormatInfo previousFormatInfo)
    {
      Floating floating = GetFloating();
      bool fits = floating == Floating.None || this.ShapeHeight <= area.Height;
      ((ShapeFormatInfo)this.renderInfo.FormatInfo).fits = fits;
      FinishLayoutInfo(area);
    }


    void FinishLayoutInfo(Area area)
    {
      LayoutInfo layoutInfo = this.renderInfo.LayoutInfo;
      Area contentArea = new Rectangle(area.X, area.Y, ShapeWidth, ShapeHeight);
      layoutInfo.ContentArea = contentArea;
      layoutInfo.MarginTop = this.shape.WrapFormat.DistanceTop.Point;
      layoutInfo.MarginLeft = this.shape.WrapFormat.DistanceLeft.Point;
      layoutInfo.MarginBottom = this.shape.WrapFormat.DistanceBottom.Point;
      layoutInfo.MarginRight = this.shape.WrapFormat.DistanceRight.Point;
      layoutInfo.KeepTogether = true;
      layoutInfo.KeepWithNext = false;
      layoutInfo.PageBreakBefore = false;
      layoutInfo.MinWidth = this.ShapeWidth;

      if (this.shape.Top.ShapePosition == ShapePosition.Undefined)
        layoutInfo.Top = this.shape.Top.Position.Point;

      layoutInfo.VerticalAlignment = GetVerticalAlignment();
      layoutInfo.HorizontalAlignment = GetHorizontalAlignment();

      if (this.shape.Left.ShapePosition == ShapePosition.Undefined)
        layoutInfo.Left = this.shape.Left.Position.Point;

      layoutInfo.HorizontalReference = GetHorizontalReference();
      layoutInfo.VerticalReference = GetVerticalReference();
      layoutInfo.Floating = GetFloating();
    }

    HorizontalReference GetHorizontalReference()
    {
      switch (this.shape.RelativeHorizontal)
      {
        case RelativeHorizontal.Margin:
          return HorizontalReference.PageMargin;
        case RelativeHorizontal.Page:
          return HorizontalReference.Page;
      }
      return HorizontalReference.AreaBoundary;
    }

    VerticalReference GetVerticalReference()
    {
      switch (this.shape.RelativeVertical)
      {
        case RelativeVertical.Margin:
          return VerticalReference.PageMargin;

        case RelativeVertical.Page:
          return VerticalReference.Page;
      }
      return VerticalReference.PreviousElement;
    }

    ElementAlignment GetVerticalAlignment()
    {
      switch (this.shape.Top.ShapePosition)
      {
        case ShapePosition.Center:
          return ElementAlignment.Center;

        case ShapePosition.Bottom:
          return ElementAlignment.Far;
      }
      return ElementAlignment.Near;
    }

    protected void RenderFilling()
    {
      Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
      this.fillFormatRenderer.Render(contentArea.X, contentArea.Y, contentArea.Width, contentArea.Height);
    }

    protected void RenderLine()
    {
      Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
      XUnit lineWidth = this.lineFormatRenderer.GetWidth();
      XUnit width = contentArea.Width - lineWidth;
      XUnit height = contentArea.Height - lineWidth;
      this.lineFormatRenderer.Render(contentArea.X, contentArea.Y, width, height);
    }

    ElementAlignment GetHorizontalAlignment()
    {
      switch (this.shape.Left.ShapePosition)
      {
        case ShapePosition.Center:
          return ElementAlignment.Center;

        case ShapePosition.Right:
          return ElementAlignment.Far;
        
        case ShapePosition.Outside:
          return ElementAlignment.Outside;

        case ShapePosition.Inside:
          return ElementAlignment.Inside;
      }
      return ElementAlignment.Near;
    }
    protected LineFormatRenderer lineFormatRenderer;
    protected FillFormatRenderer fillFormatRenderer;
    protected Shape shape;
  }
}

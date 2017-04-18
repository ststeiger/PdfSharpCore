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
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Shapes;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Renders fill formats.
  /// </summary>
  internal class FillFormatRenderer
  {
    public FillFormatRenderer(FillFormat fillFormat, XGraphics gfx)
    {
      this.gfx = gfx;
      this.fillFormat = fillFormat;
    }

    internal void Render(XUnit x, XUnit y, XUnit width, XUnit height)
    {
      XBrush brush = GetBrush();

      if (brush == null)
        return;

      this.gfx.DrawRectangle(brush, x.Point, y.Point, width.Point, height.Point);
    }

    private bool IsVisible()
    {
      if (!this.fillFormat.IsNull("Visible"))
        return this.fillFormat.Visible;
      else
        return !this.fillFormat.IsNull("Color");
    }

    private XBrush GetBrush()
    {
      if (this.fillFormat == null || !IsVisible())
        return null;

#if noCMYK
      return new XSolidBrush(XColor.FromArgb(this.fillFormat.Color.Argb));
#else
      return new XSolidBrush(ColorHelper.ToXColor(this.fillFormat.Color, this.fillFormat.Document.UseCmykColor));
#endif
    }
    private XGraphics gfx;
    private FillFormat fillFormat;
  }
}

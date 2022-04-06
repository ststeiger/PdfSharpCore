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

#define CACHE_FONTS_

using System;
using System.Collections;
using System.Text;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Helps measuring and handling fonts.
  /// </summary>
  internal class FontHandler
  {
      /// <summary>
    /// Converts an DOM Font to an XFont.
    /// </summary>
    internal static XFont FontToXFont(Font font, XPrivateFontCollection pfc, 
      PdfFontEncoding encoding)
    {
      XFont xFont = null;
      // #PFC
      XPdfFontOptions options = null;
      options = new XPdfFontOptions(encoding);
      XFontStyle style = GetXStyle(font);

      if (xFont == null)
        xFont = new XFont(font.Name, font.Size, style, options);

      return xFont;
    }

    internal static XFontStyle GetXStyle(Font font)
    {
      XFontStyle style = XFontStyle.Regular;
      if (font.Bold)
      {
        if (font.Italic)
          style = XFontStyle.BoldItalic;
        else
          style = XFontStyle.Bold;
      }
      else if (font.Italic)
        style = XFontStyle.Italic;

      return style;
    }

    internal static XUnit GetDescent(XFont font)
    {
      XUnit descent = font.Metrics.Descent;
      descent *= font.Size;
      descent /= font.FontFamily.GetEmHeight(font.Style);
      return descent;
    }

    internal static XUnit GetAscent(XFont font)
    {
      XUnit ascent = font.Metrics.Ascent;
      ascent *= font.Size;
      ascent /= font.FontFamily.GetEmHeight(font.Style);
      return ascent;
    }

    internal static double GetSubSuperScaling(XFont font)
    {
      return 0.8 * GetAscent(font) / font.GetHeight();
    }

    internal static XFont ToSubSuperFont(XFont font)
    {
      double size = font.Size * GetSubSuperScaling(font);

      // #PFC
      return new XFont(font.Name, size, font.Style, font.PdfOptions);
    }

    internal static XBrush FontColorToXBrush(Font font)
    {
      return new XSolidBrush(ColorHelper.ToXColor(font.Color, font.Document.UseCmykColor));
    }
  }
}

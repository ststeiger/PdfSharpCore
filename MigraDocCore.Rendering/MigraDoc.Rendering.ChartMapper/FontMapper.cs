#region MigraDoc - Creating Documents on the Fly
//
// Authors:
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
using PdfSharpCore.Charting;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering.ChartMapper
{
  internal class FontMapper
  {
    private FontMapper()
    {
    }

    void MapObject(Font font, DocumentObjectModel.Font domFont)
    {
      font.Bold = domFont.Bold;
      if (domFont.Color.IsEmpty)
        font.Color = XColor.Empty;
      else
      {
#if noCMYK
        font.Color = XColor.FromArgb((int)domFont.Color.Argb);
#else
        font.Color = ColorHelper.ToXColor(domFont.Color, domFont.Document.UseCmykColor);
#endif
      }
      font.Italic = domFont.Italic;
      if (!domFont.IsNull("Name"))
        font.Name = domFont.Name;
      if (!domFont.IsNull("Size"))
        font.Size = domFont.Size.Point;
      font.Subscript = domFont.Subscript;
      font.Superscript = domFont.Superscript;
      font.Underline = (Underline)domFont.Underline;
    }

    internal static void Map(Font font, DocumentObjectModel.Document domDocument, string domStyleName)
    {
      DocumentObjectModel.Style domStyle = domDocument.Styles[domStyleName];
      if (domStyle != null)
      {
        FontMapper mapper = new FontMapper();
        mapper.MapObject(font, domStyle.Font);
      }
    }

    internal static void Map(Font font, DocumentObjectModel.Font domFont)
    {
      FontMapper mapper = new FontMapper();
      mapper.MapObject(font, domFont);
    }
  }
}

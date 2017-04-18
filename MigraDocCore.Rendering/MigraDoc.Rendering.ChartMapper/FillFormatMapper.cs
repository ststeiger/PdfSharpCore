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
  internal class FillFormatMapper
  {
    private FillFormatMapper()
    {
    }

    void MapObject(FillFormat fillFormat, DocumentObjectModel.Shapes.FillFormat domFillFormat)
    {
      if (domFillFormat.Color.IsEmpty)
        fillFormat.Color = XColor.Empty;
      else
      {
#if noCMYK
        fillFormat.Color = XColor.FromArgb((int)domFillFormat.Color.Argb);
#else
        fillFormat.Color = ColorHelper.ToXColor(domFillFormat.Color, domFillFormat.Document.UseCmykColor);
#endif
      }
      fillFormat.Visible = domFillFormat.Visible;
    }

    internal static void Map(FillFormat fillFormat, DocumentObjectModel.Shapes.FillFormat domFillFormat)
    {
      FillFormatMapper mapper = new FillFormatMapper();
      mapper.MapObject(fillFormat, domFillFormat);
    }
  }
}

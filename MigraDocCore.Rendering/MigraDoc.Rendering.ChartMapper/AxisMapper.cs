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

namespace MigraDocCore.Rendering.ChartMapper
{
  /// <summary>
  /// The AxisMapper class.
  /// </summary>
  public class AxisMapper
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AxisMapper"/> class.
    /// </summary>
    public AxisMapper()
    {
    }

    void MapObject(Axis axis, DocumentObjectModel.Shapes.Charts.Axis domAxis)
    {
      if (!domAxis.IsNull("TickLabels.Format"))
        axis.TickLabels.Format = domAxis.TickLabels.Format;
      if (!domAxis.IsNull("TickLabels.Style"))
        FontMapper.Map(axis.TickLabels.Font, domAxis.TickLabels.Document, domAxis.TickLabels.Style);
      if (!domAxis.IsNull("TickLabels.Font"))
        FontMapper.Map(axis.TickLabels.Font, domAxis.TickLabels.Font);

      if (!domAxis.IsNull("MajorTickMark"))
        axis.MajorTickMark = (TickMarkType)domAxis.MajorTickMark;
      if (!domAxis.IsNull("MinorTickMark"))
        axis.MinorTickMark = (TickMarkType)domAxis.MinorTickMark;

      if (!domAxis.IsNull("MajorTick"))
        axis.MajorTick = domAxis.MajorTick;
      if (!domAxis.IsNull("MinorTick"))
        axis.MinorTick = domAxis.MinorTick;

      if (!domAxis.IsNull("Title"))
      {
        axis.Title.Caption = domAxis.Title.Caption;
        if (!domAxis.IsNull("Title.Style"))
          FontMapper.Map(axis.Title.Font, domAxis.Title.Document, domAxis.Title.Style);
        if (!domAxis.IsNull("Title.Font"))
          FontMapper.Map(axis.Title.Font, domAxis.Title.Font);
        axis.Title.Orientation = domAxis.Title.Orientation.Value;
        axis.Title.Alignment = (HorizontalAlignment)domAxis.Title.Alignment;
        axis.Title.VerticalAlignment = (VerticalAlignment)domAxis.Title.VerticalAlignment;
      }

      axis.HasMajorGridlines = domAxis.HasMajorGridlines;
      axis.HasMinorGridlines = domAxis.HasMinorGridlines;

      if (!domAxis.IsNull("MajorGridlines") && !domAxis.MajorGridlines.IsNull("LineFormat"))
        LineFormatMapper.Map(axis.MajorGridlines.LineFormat, domAxis.MajorGridlines.LineFormat);
      if (!domAxis.IsNull("MinorGridlines") && !domAxis.MinorGridlines.IsNull("LineFormat"))
        LineFormatMapper.Map(axis.MinorGridlines.LineFormat, domAxis.MinorGridlines.LineFormat);

      if (!domAxis.IsNull("MaximumScale"))
        axis.MaximumScale = domAxis.MaximumScale;
      if (!domAxis.IsNull("MinimumScale"))
        axis.MinimumScale = domAxis.MinimumScale;

      if (!domAxis.IsNull("LineFormat"))
        LineFormatMapper.Map(axis.LineFormat, domAxis.LineFormat);
    }

    internal static void Map(Axis axis, DocumentObjectModel.Shapes.Charts.Axis domAxis)
    {
      AxisMapper mapper = new AxisMapper();
      mapper.MapObject(axis, domAxis);
    }
  }
}

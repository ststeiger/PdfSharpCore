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
  /// The PlotAreaMapper class.
  /// </summary>
  public class PlotAreaMapper
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotAreaMapper"/> class.
    /// </summary>
    public PlotAreaMapper()
    {
    }

    void MapObject(PlotArea plotArea, DocumentObjectModel.Shapes.Charts.PlotArea domPlotArea)
    {
      plotArea.BottomPadding = domPlotArea.BottomPadding.Point;
      plotArea.RightPadding = domPlotArea.RightPadding.Point;
      plotArea.LeftPadding = domPlotArea.LeftPadding.Point;
      plotArea.TopPadding = domPlotArea.TopPadding.Point;

      if (!domPlotArea.IsNull("LineFormat"))
        LineFormatMapper.Map(plotArea.LineFormat, domPlotArea.LineFormat);
      if (!domPlotArea.IsNull("FillFormat"))
        FillFormatMapper.Map(plotArea.FillFormat, domPlotArea.FillFormat);
    }

    internal static void Map(PlotArea plotArea, DocumentObjectModel.Shapes.Charts.PlotArea domPlotArea)
    {
      PlotAreaMapper mapper = new PlotAreaMapper();
      mapper.MapObject(plotArea, domPlotArea);
    }
  }
}

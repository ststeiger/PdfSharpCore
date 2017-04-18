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
  /// <summary>
  /// Maps charts from the DocumentObjectModel to charts from Pdf.Charting.
  /// </summary>
  public class ChartMapper
  {
    /// <summary>
    /// Initializes a new instance of the chart mapper object.
    /// </summary>
    public ChartMapper()
    {
    }

    private ChartFrame MapObject(DocumentObjectModel.Shapes.Charts.Chart domChart)
    {
      ChartFrame chartFrame = new ChartFrame();
      chartFrame.Size = new XSize(domChart.Width.Point, domChart.Height.Point);
      chartFrame.Location = new XPoint(domChart.Left.Position.Point, domChart.Top.Position.Point);

      Chart chart = new Chart((ChartType)domChart.Type);

      if (!domChart.IsNull("XAxis"))
        AxisMapper.Map(chart.XAxis, domChart.XAxis);
      if (!domChart.IsNull("YAxis"))
        AxisMapper.Map(chart.YAxis, domChart.YAxis);

      PlotAreaMapper.Map(chart.PlotArea, domChart.PlotArea);

      SeriesCollectionMapper.Map(chart.SeriesCollection, domChart.SeriesCollection);

      LegendMapper.Map(chart, domChart);

      chart.DisplayBlanksAs = (BlankType)domChart.DisplayBlanksAs;
      chart.HasDataLabel = domChart.HasDataLabel;
      if (!domChart.IsNull("DataLabel"))
        DataLabelMapper.Map(chart.DataLabel, domChart.DataLabel);

      if (!domChart.IsNull("Style"))
        FontMapper.Map(chart.Font, domChart.Document, domChart.Style);
      if (!domChart.IsNull("Format.Font"))
        FontMapper.Map(chart.Font, domChart.Format.Font);
      if (!domChart.IsNull("XValues"))
        XValuesMapper.Map(chart.XValues, domChart.XValues);

      chartFrame.Add(chart);
      return chartFrame;
    }

    /// <summary>
    /// Maps the specified DOM chart.
    /// </summary>
    /// <param name="domChart">The DOM chart.</param>
    /// <returns></returns>
    public static ChartFrame Map(DocumentObjectModel.Shapes.Charts.Chart domChart)
    {
      ChartMapper mapper = new ChartMapper();
      return mapper.MapObject(domChart);
    }
  }
}

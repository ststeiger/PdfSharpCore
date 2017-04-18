#region PDFsharp Charting - A .NET charting library based on PDFsharp
//
// Authors:
//   Niklas Schneider (mailto:Niklas.Schneider@PdfSharpCore.com)
//
// Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
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

namespace PdfSharpCore.Charting.Renderers
{
  /// <summary>
  /// Represents an area chart renderer.
  /// </summary>
  internal class AreaChartRenderer : ColumnLikeChartRenderer
  {
    /// <summary>
    /// Initializes a new instance of the AreaChartRenderer class with the
    /// specified renderer parameters.
    /// </summary>
    internal AreaChartRenderer(RendererParameters parms)
      : base(parms)
    { }

    /// <summary>
    /// Returns an initialized and renderer specific rendererInfo.
    /// </summary>
    internal override RendererInfo Init()
    {
      ChartRendererInfo cri = new ChartRendererInfo();
      cri.chart = (Chart)this.rendererParms.DrawingItem;
      this.rendererParms.RendererInfo = cri;

      InitSeriesRendererInfo();

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      cri.legendRendererInfo = (LegendRendererInfo)lr.Init();

      AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
      cri.xAxisRendererInfo = (AxisRendererInfo)xar.Init();

      AxisRenderer yar = new VerticalYAxisRenderer(this.rendererParms);
      cri.yAxisRendererInfo = (AxisRendererInfo)yar.Init();

      PlotArea plotArea = cri.chart.PlotArea;
      PlotAreaRenderer renderer = new AreaPlotAreaRenderer(this.rendererParms);
      cri.plotAreaRendererInfo = (PlotAreaRendererInfo)renderer.Init();

      return cri;
    }

    /// <summary>
    /// Layouts and calculates the space used by the line chart.
    /// </summary>
    internal override void Format()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Format();

      // axes
      AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
      xar.Format();

      AxisRenderer yar = new VerticalYAxisRenderer(this.rendererParms);
      yar.Format();

      // Calculate rects and positions.
      CalcLayout();

      // Calculated remaining plot area, now it's safe to format.
      PlotAreaRenderer renderer = new AreaPlotAreaRenderer(this.rendererParms);
      renderer.Format();
    }

    /// <summary>
    /// Draws the column chart.
    /// </summary>
    internal override void Draw()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Draw();

      // Draw wall.
      WallRenderer wr = new WallRenderer(this.rendererParms);
      wr.Draw();

      // Draw gridlines.
      GridlinesRenderer glr = new ColumnLikeGridlinesRenderer(this.rendererParms);
      glr.Draw();

      PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(this.rendererParms);
      pabr.Draw();

      PlotAreaRenderer renderer = new AreaPlotAreaRenderer(this.rendererParms);
      renderer.Draw();

      // Draw axes.
      if (cri.xAxisRendererInfo.axis != null)
      {
        AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
        xar.Draw();
      }

      if (cri.yAxisRendererInfo.axis != null)
      {
        AxisRenderer yar = new VerticalYAxisRenderer(this.rendererParms);
        yar.Draw();
      }
    }

    /// <summary>
    /// Initializes all necessary data to draw a series for a area chart.
    /// </summary>
    private void InitSeriesRendererInfo()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      SeriesCollection seriesColl = cri.chart.SeriesCollection;
      cri.seriesRendererInfos = new SeriesRendererInfo[seriesColl.Count];
      for (int idx = 0; idx < seriesColl.Count; ++idx)
      {
        SeriesRendererInfo sri = new SeriesRendererInfo();
        sri.series = seriesColl[idx];
        cri.seriesRendererInfos[idx] = sri;
      }

      InitSeries();
    }

    /// <summary>
    /// Initializes all necessary data to draw a series for a area chart.
    /// </summary>
    internal void InitSeries()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      int seriesIndex = 0;
      foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
      {
        sri.LineFormat = Converter.ToXPen(sri.series.lineFormat, XColors.Black, ChartRenderer.DefaultSeriesLineWidth);
        sri.FillFormat = Converter.ToXBrush(sri.series.fillFormat, ColumnColors.Item(seriesIndex++));

        sri.pointRendererInfos = new PointRendererInfo[sri.series.seriesElements.Count];
        for (int pointIdx = 0; pointIdx < sri.pointRendererInfos.Length; ++pointIdx)
        {
          PointRendererInfo pri = new PointRendererInfo();
          Point point = sri.series.seriesElements[pointIdx];
          pri.point = point;
          if (point != null)
          {
            pri.LineFormat = sri.LineFormat;
            pri.FillFormat = sri.FillFormat;
            if (point.lineFormat != null && !point.lineFormat.color.IsEmpty)
              pri.LineFormat = new XPen(point.lineFormat.color, point.lineFormat.width);
            if (point.fillFormat != null && !point.lineFormat.color.IsEmpty)
              pri.FillFormat = new XSolidBrush(point.fillFormat.color);
          }
          sri.pointRendererInfos[pointIdx] = pri;
        }
      }
    }
  }
}

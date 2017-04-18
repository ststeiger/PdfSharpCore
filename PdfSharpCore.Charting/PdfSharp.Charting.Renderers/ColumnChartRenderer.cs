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
  /// Represents a column chart renderer.
  /// </summary>
  internal class ColumnChartRenderer : ColumnLikeChartRenderer
  {
    /// <summary>
    /// Initializes a new instance of the ColumnChartRenderer class with the
    /// specified renderer parameters.
    /// </summary>
    internal ColumnChartRenderer(RendererParameters parms) : base(parms)
    {
    }

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

      AxisRenderer yar = GetYAxisRenderer();
      cri.yAxisRendererInfo = (AxisRendererInfo)yar.Init();

      PlotArea plotArea = cri.chart.PlotArea;
      PlotAreaRenderer renderer = GetPlotAreaRenderer();
      cri.plotAreaRendererInfo = (PlotAreaRendererInfo)renderer.Init();

      DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
      dlr.Init();

      return cri;
    }
    
    /// <summary>
    /// Layouts and calculates the space used by the column chart.
    /// </summary>
    internal override void Format()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Format();

      // axes
      AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
      xar.Format();

      AxisRenderer yar = GetYAxisRenderer();
      yar.Format();

      // Calculate rects and positions.
      CalcLayout();

      // Calculated remaining plot area, now it's safe to format.
      PlotAreaRenderer renderer = GetPlotAreaRenderer();
      renderer.Format();

      DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
      dlr.Format();
    }

    /// <summary>
    /// Draws the column chart.
    /// </summary>
    internal override void Draw()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      
      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Draw();

      WallRenderer wr = new WallRenderer(this.rendererParms);
      wr.Draw();

      GridlinesRenderer glr = new ColumnLikeGridlinesRenderer(this.rendererParms);
      glr.Draw();

      PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(this.rendererParms);
      pabr.Draw();

      PlotAreaRenderer renderer = GetPlotAreaRenderer();
      renderer.Draw();

      DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
      dlr.Draw();

      if (cri.xAxisRendererInfo.axis != null)
      {
        AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
        xar.Draw();
      }

      if (cri.yAxisRendererInfo.axis != null)
      {
        AxisRenderer yar = GetYAxisRenderer();
        yar.Draw();
      }
    }

    /// <summary>
    /// Returns the specific plot area renderer.
    /// </summary>
    private PlotAreaRenderer GetPlotAreaRenderer()
    {
      Chart chart = (Chart)this.rendererParms.DrawingItem;
      switch (chart.type)
      {
        case ChartType.Column2D:
          return new ColumnClusteredPlotAreaRenderer(this.rendererParms);

        case ChartType.ColumnStacked2D:
          return new ColumnStackedPlotAreaRenderer(this.rendererParms);
      }
      return null;
    }

    /// <summary>
    /// Returns the specific y axis renderer.
    /// </summary>
    private YAxisRenderer GetYAxisRenderer()
    {
      Chart chart = (Chart)this.rendererParms.DrawingItem;
      switch (chart.type)
      {
        case ChartType.Column2D:
          return new VerticalYAxisRenderer(this.rendererParms);

        case ChartType.ColumnStacked2D:
          return new VerticalStackedYAxisRenderer(this.rendererParms);
      }
      return null;
    }

    /// <summary>
    /// Initializes all necessary data to draw all series for a column chart.
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
    /// Initializes all necessary data to draw all series for a column chart.
    /// </summary>
    internal void InitSeries()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;

      int seriesIndex = 0;
      foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
      {
        sri.LineFormat = Converter.ToXPen(sri.series.lineFormat, XColors.Black, ChartRenderer.DefaultSeriesLineWidth);
        sri.FillFormat = Converter.ToXBrush(sri.series.fillFormat, ColumnColors.Item(seriesIndex++));

        sri.pointRendererInfos = new ColumnRendererInfo[sri.series.seriesElements.Count];
        for (int pointIdx = 0; pointIdx < sri.pointRendererInfos.Length; ++pointIdx)
        {
          PointRendererInfo pri = new ColumnRendererInfo();
          Point point = sri.series.seriesElements[pointIdx];
          pri.point = point;
          if (point != null)
          {
            pri.LineFormat = sri.LineFormat;
            pri.FillFormat = sri.FillFormat;
            if (point.lineFormat != null)
              pri.LineFormat = Converter.ToXPen(point.lineFormat, sri.LineFormat);
            if (point.fillFormat != null && !point.fillFormat.color.IsEmpty)
              pri.FillFormat = new XSolidBrush(point.fillFormat.color);
          }
          sri.pointRendererInfos[pointIdx] = pri;
        }
      }
    }
  }
}
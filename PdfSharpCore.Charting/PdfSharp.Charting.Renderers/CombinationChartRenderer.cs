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
using System.Collections;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting.Renderers
{
  /// <summary>
  /// Represents a renderer for combinations of charts.
  /// </summary>
  internal class CombinationChartRenderer : ChartRenderer
  {
    /// <summary>
    /// Initializes a new instance of the CombinationChartRenderer class with the
    /// specified renderer parameters.
    /// </summary>
    internal CombinationChartRenderer(RendererParameters parms) : base(parms)
    {
    }

    /// <summary>
    /// Returns an initialized and renderer specific rendererInfo.
    /// </summary>
    internal override RendererInfo Init()
    {
      CombinationRendererInfo cri = new CombinationRendererInfo();
      cri.chart = (Chart)this.rendererParms.DrawingItem;
      this.rendererParms.RendererInfo = cri;

      InitSeriesRendererInfo();
      DistributeSeries();

      if (cri.areaSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.areaSeriesRendererInfos;
        AreaChartRenderer renderer = new AreaChartRenderer(this.rendererParms);
        renderer.InitSeries();
      }
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        ColumnChartRenderer renderer = new ColumnChartRenderer(this.rendererParms);
        renderer.InitSeries();
      }
      if (cri.lineSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.lineSeriesRendererInfos;
        LineChartRenderer renderer = new LineChartRenderer(this.rendererParms);
        renderer.InitSeries();
      }
      cri.seriesRendererInfos = cri.commonSeriesRendererInfos;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      cri.legendRendererInfo = (LegendRendererInfo)lr.Init();

      AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
      cri.xAxisRendererInfo = (AxisRendererInfo)xar.Init();

      AxisRenderer yar = new VerticalYAxisRenderer(this.rendererParms);
      cri.yAxisRendererInfo = (AxisRendererInfo)yar.Init();

      PlotArea plotArea = cri.chart.PlotArea;
      PlotAreaRenderer apar = new AreaPlotAreaRenderer(this.rendererParms);
      cri.plotAreaRendererInfo = (PlotAreaRendererInfo)apar.Init();

      // Draw data labels.
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
        dlr.Init();
      }

      return cri;
    }
    
    /// <summary>
    /// Layouts and calculates the space used by the combination chart.
    /// </summary>
    internal override void Format()
    {
      CombinationRendererInfo cri = (CombinationRendererInfo)this.rendererParms.RendererInfo;
      cri.seriesRendererInfos = cri.commonSeriesRendererInfos;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Format();

      // axes
      AxisRenderer xar = new HorizontalXAxisRenderer(this.rendererParms);
      xar.Format();

      AxisRenderer yar = new VerticalYAxisRenderer(this.rendererParms);
      yar.Format();

      // Calculate rects and positions.
      XRect chartRect = LayoutLegend();
      cri.xAxisRendererInfo.X = chartRect.Left + cri.yAxisRendererInfo.Width;
      cri.xAxisRendererInfo.Y = chartRect.Bottom - cri.xAxisRendererInfo.Height;
      cri.xAxisRendererInfo.Width = chartRect.Width - cri.yAxisRendererInfo.Width;
      cri.yAxisRendererInfo.X = chartRect.Left;
      cri.yAxisRendererInfo.Y = chartRect.Top;
      cri.yAxisRendererInfo.Height = chartRect.Height - cri.xAxisRendererInfo.Height;
      cri.plotAreaRendererInfo.X = cri.xAxisRendererInfo.X;
      cri.plotAreaRendererInfo.Y = cri.yAxisRendererInfo.InnerRect.Y;
      cri.plotAreaRendererInfo.Width = cri.xAxisRendererInfo.Width;
      cri.plotAreaRendererInfo.Height = cri.yAxisRendererInfo.InnerRect.Height;

      // Calculated remaining plot area, now it's safe to format.
      PlotAreaRenderer renderer;
      if (cri.areaSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.areaSeriesRendererInfos;
        renderer = new AreaPlotAreaRenderer(this.rendererParms);
        renderer.Format();
      }
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        //TODO Check for Clustered- or StackedPlotAreaRenderer
        renderer = new ColumnClusteredPlotAreaRenderer(this.rendererParms);
        renderer.Format();
      }
      if (cri.lineSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.lineSeriesRendererInfos;
        renderer = new LinePlotAreaRenderer(this.rendererParms);
        renderer.Format();
      }

      // Draw data labels.
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
        dlr.Format();
      }
    }

    /// <summary>
    /// Draws the column chart.
    /// </summary>
    internal override void Draw()
    {
      CombinationRendererInfo cri = (CombinationRendererInfo)this.rendererParms.RendererInfo;
      cri.seriesRendererInfos = cri.commonSeriesRendererInfos;

      LegendRenderer lr = new ColumnLikeLegendRenderer(this.rendererParms);
      lr.Draw();

      WallRenderer wr = new WallRenderer(this.rendererParms);
      wr.Draw();

      GridlinesRenderer glr = new ColumnLikeGridlinesRenderer(this.rendererParms);
      glr.Draw();

      PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(this.rendererParms);
      pabr.Draw();

      PlotAreaRenderer renderer;
      if (cri.areaSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.areaSeriesRendererInfos;
        renderer = new AreaPlotAreaRenderer(this.rendererParms);
        renderer.Draw();
      }
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        //TODO Check for Clustered- or StackedPlotAreaRenderer
        renderer = new ColumnClusteredPlotAreaRenderer(this.rendererParms);
        renderer.Draw();
      }
      if (cri.lineSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.lineSeriesRendererInfos;
        renderer = new LinePlotAreaRenderer(this.rendererParms);
        renderer.Draw();
      }

      // Draw data labels.
      if (cri.columnSeriesRendererInfos != null)
      {
        cri.seriesRendererInfos = cri.columnSeriesRendererInfos;
        DataLabelRenderer dlr = new ColumnDataLabelRenderer(this.rendererParms);
        dlr.Draw();
      }

      // Draw axes.
      cri.seriesRendererInfos = cri.commonSeriesRendererInfos;
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
    /// Initializes all necessary data to draw series for a combination chart.
    /// </summary>
    private void InitSeriesRendererInfo()
    {
      CombinationRendererInfo cri = (CombinationRendererInfo)this.rendererParms.RendererInfo;
      SeriesCollection seriesColl = cri.chart.SeriesCollection;
      cri.seriesRendererInfos = new SeriesRendererInfo[seriesColl.Count];
      for (int idx = 0; idx < seriesColl.Count; ++idx)
      {
        SeriesRendererInfo sri = new SeriesRendererInfo();
        sri.series = seriesColl[idx];
        cri.seriesRendererInfos[idx] = sri;
      }
    }

    /// <summary>
    /// Sort all series renderer info dependent on their chart type.
    /// </summary>
    private void DistributeSeries()
    {
      CombinationRendererInfo cri = (CombinationRendererInfo)this.rendererParms.RendererInfo;

      ArrayList areaSeries = new ArrayList();
      ArrayList columnSeries = new ArrayList();
      ArrayList lineSeries = new ArrayList();
      foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
      {
        switch (sri.series.chartType)
        {
          case ChartType.Area2D:
            areaSeries.Add(sri);
            break;

          case ChartType.Column2D:
            columnSeries.Add(sri);
            break;

          case ChartType.Line:
            lineSeries.Add(sri);
            break;

          default:
            throw new InvalidOperationException(PSCSR.InvalidChartTypeForCombination(sri.series.chartType));
        }
      }

      cri.commonSeriesRendererInfos = cri.seriesRendererInfos;
      if (areaSeries.Count > 0)
      {
        cri.areaSeriesRendererInfos = new SeriesRendererInfo[columnSeries.Count];
        areaSeries.CopyTo(cri.areaSeriesRendererInfos);
      }
      if (columnSeries.Count > 0)
      {
        cri.columnSeriesRendererInfos = new SeriesRendererInfo[columnSeries.Count];
        columnSeries.CopyTo(cri.columnSeriesRendererInfos);
      }
      if (lineSeries.Count > 0)
      {
        cri.lineSeriesRendererInfos = new SeriesRendererInfo[columnSeries.Count];
        lineSeries.CopyTo(cri.lineSeriesRendererInfos);
      }
    }
  }
}
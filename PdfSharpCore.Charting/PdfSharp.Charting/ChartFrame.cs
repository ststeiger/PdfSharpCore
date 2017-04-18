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
using PdfSharpCore.Charting.Renderers;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Represents the frame which holds one or more charts.
  /// </summary>
  public class ChartFrame
  {
    /// <summary>
    /// Initializes a new instance of the ChartFrame class.
    /// </summary>
    public ChartFrame()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ChartFrame class with the specified rectangle.
    /// </summary>
    public ChartFrame(XRect rect)
    {
      this.location = rect.Location;
      this.size = rect.Size;
    }

    /// <summary>
    /// Gets or sets the location of the ChartFrame.
    /// </summary>
    public XPoint Location
    {
      get { return this.location; }
      set { this.location = value; }
    }
    XPoint location;

    /// <summary>
    /// Gets or sets the size of the ChartFrame.
    /// </summary>
    public XSize Size
    {
      get { return this.size; }
      set { this.size = value; }
    }
    XSize size;

    /// <summary>
    /// Adds a chart to the ChartFrame.
    /// </summary>
    public void Add(Chart chart)
    {
      if (this.chartList == null)
        this.chartList = new ArrayList();
      this.chartList.Add(chart);
    }

    /// <summary>
    /// Draws all charts inside the ChartFrame.
    /// </summary>
    public void Draw(XGraphics gfx)
    {
      // Draw frame of ChartFrame. First shadow frame.
      int dx = 5;
      int dy = 5;
      gfx.DrawRoundedRectangle(XBrushes.Gainsboro,
                               this.location.X + dx, this.location.Y + dy,
                               this.size.Width, this.size.Height, 20, 20);

      XRect chartRect = new XRect(this.location.X, this.location.Y, this.size.Width, this.size.Height);
      XLinearGradientBrush brush = new XLinearGradientBrush(chartRect, XColor.FromArgb(0xFFD0DEEF), XColors.White,
                                                            XLinearGradientMode.Vertical);
      XPen penBorder = new XPen(XColors.SteelBlue, 2.5);
      gfx.DrawRoundedRectangle(penBorder, brush,
                               this.location.X, this.location.Y, this.size.Width, this.size.Height,
                               15, 15);

      XGraphicsState state = gfx.Save();
      gfx.TranslateTransform(this.location.X, this.location.Y);

      // Calculate rectangle for all charts. Y-Position will be moved for each chart.
      int charts = this.chartList.Count;
      uint dxChart = 20;
      uint dyChart = 20;
      uint dyBetweenCharts = 30;
      XRect rect = new XRect(dxChart, dyChart,
        this.size.Width - 2 * dxChart,
        (this.size.Height - (charts - 1) * dyBetweenCharts - 2 * dyChart) / charts);

      // draw each chart in list
      foreach (Chart chart in this.chartList)
      {
        RendererParameters parms = new RendererParameters(gfx, rect);
        parms.DrawingItem = chart;

        ChartRenderer renderer = GetChartRenderer(chart, parms);
        renderer.Init();
        renderer.Format();
        renderer.Draw();

        rect.Y += rect.Height + dyBetweenCharts;
      }
      gfx.Restore(state);

//      // Calculate rectangle for all charts. Y-Position will be moved for each chart.
//      int charts = this.chartList.Count;
//      uint dxChart = 0;
//      uint dyChart = 0;
//      uint dyBetweenCharts = 0;
//      XRect rect = new XRect(dxChart, dyChart,
//        this.size.Width - 2 * dxChart,
//        (this.size.Height - (charts - 1) * dyBetweenCharts - 2 * dyChart) / charts);
//
//      // draw each chart in list
//      foreach (Chart chart in this.chartList)
//      {
//        RendererParameters parms = new RendererParameters(gfx, rect);
//        parms.DrawingItem = chart;
//
//        ChartRenderer renderer = GetChartRenderer(chart, parms);
//        renderer.Init();
//        renderer.Format();
//        renderer.Draw();
//
//        rect.Y += rect.Height + dyBetweenCharts;
//      }
    }

    /// <summary>
    /// Draws first chart only.
    /// </summary>
    public void DrawChart(XGraphics gfx)
    {
      XGraphicsState state = gfx.Save();
      gfx.TranslateTransform(this.location.X, this.location.Y);

      if (this.chartList.Count > 0)
      {
        XRect chartRect = new XRect(0, 0, this.size.Width, this.size.Height);
        Chart chart = (Chart)this.chartList[0];
        RendererParameters parms = new RendererParameters(gfx, chartRect);
        parms.DrawingItem = chart;

        ChartRenderer renderer = GetChartRenderer(chart, parms);
        renderer.Init();
        renderer.Format();
        renderer.Draw();
      }
      gfx.Restore(state);
    }

    /// <summary>
    /// Returns the chart renderer appropriate for the chart.
    /// </summary>
    private ChartRenderer GetChartRenderer(Chart chart, RendererParameters parms)
    {
      ChartType chartType = chart.Type;
      bool useCombinationRenderer = false;
      foreach (Series series in chart.seriesCollection)
      {
        if (series.chartType != chartType)
        {
          useCombinationRenderer = true;
          break;
        }
      }

      if (useCombinationRenderer)
        return new CombinationChartRenderer(parms);

      switch (chartType)
      {
        case ChartType.Line:
          return new LineChartRenderer(parms);

        case ChartType.Column2D:
        case ChartType.ColumnStacked2D:
          return new ColumnChartRenderer(parms);

        case ChartType.Bar2D:
        case ChartType.BarStacked2D:
          return new BarChartRenderer(parms);

        case ChartType.Area2D:
          return new AreaChartRenderer(parms);

        case ChartType.Pie2D:
        case ChartType.PieExploded2D:
          return new PieChartRenderer(parms);
      }

      return null;
    }

    /// <summary>
    /// Holds the charts which will be drawn inside the ChartFrame.
    /// </summary>
    ArrayList chartList;
  }
}

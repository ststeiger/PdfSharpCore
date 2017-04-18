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
using System.Globalization;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting.Renderers
{
  /// <summary>
  /// Represents the legend renderer specific to pie charts.
  /// </summary>
  internal class PieLegendRenderer : LegendRenderer
  {
    /// <summary>
    /// Initializes a new instance of the PieLegendRenderer class with the specified renderer
    /// parameters.
    /// </summary>
    internal PieLegendRenderer(RendererParameters parms)
      : base(parms)
    { }

    /// <summary>
    /// Initializes the legend's renderer info. Each data point will be represented through
    /// a legend entry renderer info.
    /// </summary>
    internal override RendererInfo Init()
    {
      LegendRendererInfo lri = null;
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      if (cri.chart.legend != null)
      {
        lri = new LegendRendererInfo();
        lri.legend = cri.chart.legend;

        lri.Font = Converter.ToXFont(lri.legend.font, cri.DefaultFont);
        lri.FontColor = new XSolidBrush(XColors.Black);

        if (lri.legend.lineFormat != null)
          lri.BorderPen = Converter.ToXPen(lri.legend.lineFormat, XColors.Black, DefaultLineWidth, XDashStyle.Solid);

        XSeries xseries = null;
        if (cri.chart.xValues != null)
          xseries = cri.chart.xValues[0];

        int index = 0;
        SeriesRendererInfo sri = cri.seriesRendererInfos[0];
        lri.Entries = new LegendEntryRendererInfo[sri.pointRendererInfos.Length];
        foreach (PointRendererInfo pri in sri.pointRendererInfos)
        {
          LegendEntryRendererInfo leri = new LegendEntryRendererInfo();
          leri.seriesRendererInfo = sri;
          leri.legendRendererInfo = lri;
          leri.EntryText = string.Empty;
          if (xseries != null)
          {
            if (xseries.Count > index)
              leri.EntryText = xseries[index].Value;
          }
          else
            leri.EntryText = (index + 1).ToString(CultureInfo.InvariantCulture); // create default/dummy entry
          leri.MarkerPen = pri.LineFormat;
          leri.MarkerBrush = pri.FillFormat;

          lri.Entries[index++] = leri;
        }
      }
      return lri;
    }
  }
}

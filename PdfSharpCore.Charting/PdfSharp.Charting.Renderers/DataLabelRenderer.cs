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
  /// Represents a data label renderer.
  /// </summary>
  internal abstract class DataLabelRenderer : Renderer
  {
    /// <summary>
    /// Initializes a new instance of the DataLabelRenderer class with the
    /// specified renderer parameters.
    /// </summary>
    internal DataLabelRenderer(RendererParameters parms) : base(parms)
    {
    }

    /// <summary>
    /// Creates a data label rendererInfo.
    /// Does not return any renderer info.
    /// </summary>
    internal override RendererInfo Init()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
      {
        if (cri.chart.hasDataLabel || cri.chart.dataLabel != null ||
            sri.series.hasDataLabel || sri.series.dataLabel != null)
        {
          DataLabelRendererInfo dlri = new DataLabelRendererInfo();

          DataLabel dl = sri.series.dataLabel;
          if (dl == null)
            dl = cri.chart.dataLabel;
          if (dl == null)
          {
            dlri.Format = "0";
            dlri.Font = cri.DefaultDataLabelFont;
            dlri.FontColor = new XSolidBrush(XColors.Black);
            dlri.Position = DataLabelPosition.InsideEnd;
            if (cri.chart.type == ChartType.Pie2D || cri.chart.type == ChartType.PieExploded2D)
              dlri.Type = DataLabelType.Percent;
            else
              dlri.Type = DataLabelType.Value;
          }
          else
          {
            dlri.Format = dl.Format.Length > 0 ? dl.Format : "0";
            dlri.Font = Converter.ToXFont(dl.font, cri.DefaultDataLabelFont);
            dlri.FontColor = Converter.ToXBrush(dl.font, XColors.Black);
            if (dl.positionInitialized)
              dlri.Position = dl.position;
            else
              dlri.Position = DataLabelPosition.OutsideEnd;
            if (dl.typeInitialized)
              dlri.Type = dl.type;
            else
            {
              if (cri.chart.type == ChartType.Pie2D || cri.chart.type == ChartType.PieExploded2D)
                dlri.Type = DataLabelType.Percent;
              else
                dlri.Type = DataLabelType.Value;
            }
          }

          sri.dataLabelRendererInfo = dlri;
        }
      }

      return null;
    }

    /// <summary>
    /// Calculates the specific positions for each data label.
    /// </summary>
    internal abstract void CalcPositions();
  }
}
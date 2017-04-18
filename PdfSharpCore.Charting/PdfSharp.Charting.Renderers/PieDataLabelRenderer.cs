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
  /// Represents a data label renderer for pie charts.
  /// </summary>
  internal class PieDataLabelRenderer : DataLabelRenderer
  {
    /// <summary>
    /// Initializes a new instance of the PieDataLabelRenderer class with the
    /// specified renderer parameters.
    /// </summary>
    internal PieDataLabelRenderer(RendererParameters parms) : base(parms)
    {
    }
    
    /// <summary>
    /// Calculates the space used by the data labels.
    /// </summary>
    internal override void Format()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      if (cri.seriesRendererInfos.Length == 0)
        return;

      SeriesRendererInfo sri = cri.seriesRendererInfos[0];
      if (sri.dataLabelRendererInfo == null)
        return;

      double sumValues = sri.SumOfPoints;
      XGraphics gfx = this.rendererParms.Graphics;

      sri.dataLabelRendererInfo.Entries = new DataLabelEntryRendererInfo[sri.pointRendererInfos.Length];
      int index = 0;
      foreach (SectorRendererInfo sector in sri.pointRendererInfos)
      {
        DataLabelEntryRendererInfo dleri = new DataLabelEntryRendererInfo();
        if (sri.dataLabelRendererInfo.Type != DataLabelType.None)
        {
          if (sri.dataLabelRendererInfo.Type == DataLabelType.Percent)
          {
            double percent = 100 / (sumValues / Math.Abs(sector.point.value));
            dleri.Text = percent.ToString(sri.dataLabelRendererInfo.Format) + "%";
          }
          else if (sri.dataLabelRendererInfo.Type == DataLabelType.Value)
            dleri.Text = sector.point.value.ToString(sri.dataLabelRendererInfo.Format);

          if (dleri.Text.Length > 0)
            dleri.Size = gfx.MeasureString(dleri.Text, sri.dataLabelRendererInfo.Font);
        }

        sri.dataLabelRendererInfo.Entries[index++] = dleri;
      }
    }

    /// <summary>
    /// Draws the data labels of the pie chart.
    /// </summary>
    internal override void Draw()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      if (cri.seriesRendererInfos.Length == 0)
        return;

      SeriesRendererInfo sri = cri.seriesRendererInfos[0];
      if (sri.dataLabelRendererInfo == null)
        return;

      if (sri != null)
      {
        XGraphics gfx = this.rendererParms.Graphics;
        XFont font = sri.dataLabelRendererInfo.Font;
        XBrush fontColor = sri.dataLabelRendererInfo.FontColor;
        XStringFormat format = XStringFormats.Center;
        format.LineAlignment = XLineAlignment.Center;
        foreach (DataLabelEntryRendererInfo dataLabel in sri.dataLabelRendererInfo.Entries)
        {
          if (dataLabel.Text != null)
            gfx.DrawString(dataLabel.Text, font, fontColor, dataLabel.Rect, format);
        }
      }
    }

    /// <summary>
    /// Calculates the data label positions specific for pie charts.
    /// </summary>
    internal override void CalcPositions()
    {
      ChartRendererInfo cri = (ChartRendererInfo)this.rendererParms.RendererInfo;
      XGraphics gfx = this.rendererParms.Graphics;

      if (cri.seriesRendererInfos.Length > 0)
      {
        SeriesRendererInfo sri = cri.seriesRendererInfos[0];
        if (sri != null && sri.dataLabelRendererInfo != null)
        {
          int sectorIndex = 0;
          foreach (SectorRendererInfo sector in sri.pointRendererInfos)
          {
            // Determine output rectangle
            double midAngle = sector.StartAngle + sector.SweepAngle / 2;
            double radMidAngle = midAngle / 180 * Math.PI;
            XPoint origin = new XPoint(sector.Rect.X + sector.Rect.Width / 2,
              sector.Rect.Y + sector.Rect.Height / 2);
            double radius = sector.Rect.Width / 2;
            double halfradius = radius / 2;

            DataLabelEntryRendererInfo dleri = sri.dataLabelRendererInfo.Entries[sectorIndex++];
            switch (sri.dataLabelRendererInfo.Position)
            {
              case DataLabelPosition.OutsideEnd:
                // Outer border of the circle.
                dleri.X = origin.X + (radius * Math.Cos(radMidAngle));
                dleri.Y = origin.Y + (radius * Math.Sin(radMidAngle));
                if (dleri.X < origin.X)
                  dleri.X -= dleri.Width;
                if (dleri.Y < origin.Y)
                  dleri.Y -= dleri.Height;
                break;

              case DataLabelPosition.InsideEnd:
                // Inner border of the circle.
                dleri.X = origin.X + (radius * Math.Cos(radMidAngle));
                dleri.Y = origin.Y + (radius * Math.Sin(radMidAngle));
                if (dleri.X > origin.X)
                  dleri.X -= dleri.Width;
                if (dleri.Y > origin.Y)
                  dleri.Y -= dleri.Height;
                break;

              case DataLabelPosition.Center:
                // Centered
                dleri.X = origin.X + (halfradius * Math.Cos(radMidAngle));
                dleri.Y = origin.Y + (halfradius * Math.Sin(radMidAngle));
                dleri.X -= dleri.Width / 2;
                dleri.Y -= dleri.Height / 2;
                break;

              case DataLabelPosition.InsideBase:
                // Aligned at the base/center of the circle
                dleri.X = origin.X;
                dleri.Y = origin.Y;
                if (dleri.X < origin.X)
                  dleri.X -= dleri.Width;
                if (dleri.Y < origin.Y)
                  dleri.Y -= dleri.Height;
                break;
            }
          }
        }
      }
    }
  }
}
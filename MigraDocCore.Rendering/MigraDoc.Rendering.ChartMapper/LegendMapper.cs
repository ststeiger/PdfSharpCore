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
  internal class LegendMapper
  {
    private LegendMapper()
    {
    }

    void MapObject(Chart chart, DocumentObjectModel.Shapes.Charts.Chart domChart)
    {
      DocumentObjectModel.Shapes.Charts.Legend domLegend = null;
      DocumentObjectModel.Shapes.Charts.TextArea textArea = null;

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.BottomArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Bottom;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.BottomArea;
        }
      }

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.RightArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Right;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.RightArea;
        }
      }

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.LeftArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Left;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.LeftArea;
        }
      }

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.TopArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Top;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.TopArea;
        }
      }

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.HeaderArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Top;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.HeaderArea;
        }
      }

      foreach (DocumentObjectModel.DocumentObject domObj in domChart.FooterArea.Elements)
      {
        if (domObj is DocumentObjectModel.Shapes.Charts.Legend)
        {
          chart.Legend.Docking = PdfSharpCore.Charting.DockingType.Bottom;
          domLegend = domObj as DocumentObjectModel.Shapes.Charts.Legend;
          textArea = domChart.FooterArea;
        }
      }

      if (domLegend != null)
      {
        if (!domLegend.IsNull("LineFormat"))
          LineFormatMapper.Map(chart.Legend.LineFormat, domLegend.LineFormat);
        if (!textArea.IsNull("Style"))
          FontMapper.Map(chart.Legend.Font, textArea.Document, textArea.Style);
        if (!domLegend.IsNull("Format.Font"))
          FontMapper.Map(chart.Legend.Font, domLegend.Format.Font);
      }
    }

    internal static void Map(Chart chart, DocumentObjectModel.Shapes.Charts.Chart domChart)
    {
      LegendMapper mapper = new LegendMapper();
      mapper.MapObject(chart, domChart);
    }
  }
}

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
using PdfSharpCore.Drawing;
using PdfSharpCore.Charting;

namespace MigraDocCore.Rendering.ChartMapper
{
  /// <summary>
  /// The SeriesCollectionMapper class.
  /// </summary>
  public class SeriesCollectionMapper
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SeriesCollectionMapper"/> class.
    /// </summary>
    public SeriesCollectionMapper()
    {
    }
    
    void MapObject(SeriesCollection seriesCollection, DocumentObjectModel.Shapes.Charts.SeriesCollection domSeriesCollection)
    {
      foreach (DocumentObjectModel.Shapes.Charts.Series domSeries in domSeriesCollection)
      {
        Series series = seriesCollection.AddSeries();
        series.Name = domSeries.Name;

        if (domSeries.IsNull("ChartType"))
        {
          DocumentObjectModel.Shapes.Charts.Chart chart = (DocumentObjectModel.Shapes.Charts.Chart)DocumentObjectModel.DocumentRelations.GetParentOfType(domSeries, typeof(DocumentObjectModel.Shapes.Charts.Chart));
          series.ChartType = (ChartType)chart.Type;
        }
        else
          series.ChartType = (ChartType)domSeries.ChartType;

        if (!domSeries.IsNull("DataLabel"))
          DataLabelMapper.Map(series.DataLabel, domSeries.DataLabel);
        if (!domSeries.IsNull("LineFormat"))
          LineFormatMapper.Map(series.LineFormat, domSeries.LineFormat);
        if (!domSeries.IsNull("FillFormat"))
          FillFormatMapper.Map(series.FillFormat, domSeries.FillFormat);

        series.HasDataLabel = domSeries.HasDataLabel;
        if (domSeries.MarkerBackgroundColor.IsEmpty)
          series.MarkerBackgroundColor = XColor.Empty;
        else
        {
#if noCMYK
          series.MarkerBackgroundColor = XColor.FromArgb(domSeries.MarkerBackgroundColor.Argb);
#else
          series.MarkerBackgroundColor = 
            ColorHelper.ToXColor(domSeries.MarkerBackgroundColor, domSeries.Document.UseCmykColor);
#endif
        }
        if (domSeries.MarkerForegroundColor.IsEmpty)
          series.MarkerForegroundColor = XColor.Empty;
        else
        {
#if noCMYK
          series.MarkerForegroundColor = XColor.FromArgb(domSeries.MarkerForegroundColor.Argb);
#else
          series.MarkerForegroundColor = 
            ColorHelper.ToXColor(domSeries.MarkerForegroundColor, domSeries.Document.UseCmykColor);
#endif
        }
        series.MarkerSize = domSeries.MarkerSize.Point;
        if (!domSeries.IsNull("MarkerStyle"))
          series.MarkerStyle = (MarkerStyle)domSeries.MarkerStyle;

        foreach (DocumentObjectModel.Shapes.Charts.Point domPoint in domSeries.Elements)
        {
          if (domPoint != null)
          {
            Point point = series.Add(domPoint.Value);
            FillFormatMapper.Map(point.FillFormat, domPoint.FillFormat);
            LineFormatMapper.Map(point.LineFormat, domPoint.LineFormat);
          }
          else
            series.Add(double.NaN);
        }
      }
    }

    internal static void Map(SeriesCollection seriesCollection, DocumentObjectModel.Shapes.Charts.SeriesCollection domSeriesCollection)
    {
      SeriesCollectionMapper mapper = new SeriesCollectionMapper();
      mapper.MapObject(seriesCollection, domSeriesCollection);
    }
  }
}

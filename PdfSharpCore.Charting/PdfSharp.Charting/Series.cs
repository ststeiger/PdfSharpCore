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
using System.ComponentModel;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Represents a series of data on the chart.
  /// </summary>
  public class Series : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the Series class.
    /// </summary>
    public Series()
    {
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Series Clone()
    {
      return (Series)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Series series = (Series)base.DeepCopy();
      if (series.seriesElements != null)
      {
        series.seriesElements = series.seriesElements.Clone();
        series.seriesElements.parent = series;
      }
      if (series.lineFormat != null)
      {
        series.lineFormat = series.lineFormat.Clone();
        series.lineFormat.parent = series;
      }
      if (series.fillFormat != null)
      {
        series.fillFormat = series.fillFormat.Clone();
        series.fillFormat.parent = series;
      }
      if (series.dataLabel != null)
      {
        series.dataLabel = series.dataLabel.Clone();
        series.dataLabel.parent = series;
      }
      return series;
    }

    /// <summary>
    /// Adds a blank to the series.
    /// </summary>
    public void AddBlank()
    {
      this.Elements.AddBlank();
    }

    /// <summary>
    /// Adds a real value to the series.
    /// </summary>
    public Point Add(double value)
    {
      return this.Elements.Add(value);
    }

    /// <summary>
    /// Adds an array of real values to the series.
    /// </summary>
    public void Add(params double[] values)
    {
      this.Elements.Add(values);
    }
    #endregion

    #region Properties
    /// <summary>
    /// The actual value container of the series.
    /// </summary>
    public SeriesElements Elements
    {
      get
      {
        if (this.seriesElements == null)
          this.seriesElements = new SeriesElements(this);

        return this.seriesElements;
      }
    }
    internal SeriesElements seriesElements;

    /// <summary>
    /// Gets or sets the name of the series which will be used in the legend.
    /// </summary>
    public string Name
    {
      get {return this.name;}
      set {this.name = value;}
    }
    internal string name = String.Empty;

    /// <summary>
    /// Gets the line format of the border of each data.
    /// </summary>
    public LineFormat LineFormat
    {
      get
      {
        if (this.lineFormat == null)
          this.lineFormat = new LineFormat(this);

        return this.lineFormat;
      }
    }
    internal LineFormat lineFormat;

    /// <summary>
    /// Gets the background filling of the data.
    /// </summary>
    public FillFormat FillFormat
    {
      get
      {
        if (this.fillFormat == null)
          this.fillFormat = new FillFormat(this);

        return this.fillFormat;
      }
    }
    internal FillFormat fillFormat;

    /// <summary>
    /// Gets or sets the size of the marker in a line chart.
    /// </summary>
    public XUnit MarkerSize
    {
      get {return this.markerSize;}
      set {this.markerSize = value;}
    }
    internal XUnit markerSize;

    /// <summary>
    /// Gets or sets the style of the marker in a line chart.
    /// </summary>
    public MarkerStyle MarkerStyle
    {
      get {return this.markerStyle;}
      set
      {
        if (!Enum.IsDefined(typeof(MarkerStyle), value))
          throw new InvalidEnumArgumentException("value", (int)value, typeof(MarkerStyle));

        this.markerStyle = value;
        this.markerStyleInitialized = true;
      }
    }
    internal MarkerStyle markerStyle;
    internal bool markerStyleInitialized;

    /// <summary>
    /// Gets or sets the foreground color of the marker in a line chart.
    /// </summary>
    public XColor MarkerForegroundColor
    {
      get {return this.markerForegroundColor;}
      set {this.markerForegroundColor = value;}
    }
    internal XColor markerForegroundColor = XColor.Empty;

    /// <summary>
    /// Gets or sets the background color of the marker in a line chart.
    /// </summary>
    public XColor MarkerBackgroundColor
    {
      get {return this.markerBackgroundColor;}
      set {this.markerBackgroundColor = value;}
    }
    internal XColor markerBackgroundColor = XColor.Empty;

    /// <summary>
    /// Gets or sets the chart type of the series if it's intended to be different than the
    /// global chart type.
    /// </summary>
    public ChartType ChartType
    {
      get {return this.chartType;}
      set
      {
        if (!Enum.IsDefined(typeof(ChartType), value))
          throw new InvalidEnumArgumentException("value", (int)value, typeof(ChartType));

        this.chartType = value;
      }
    }
    internal ChartType chartType;

    /// <summary>
    /// Gets the DataLabel of the series.
    /// </summary>
    public DataLabel DataLabel
    {
      get 
      {
        if (this.dataLabel == null)
          this.dataLabel = new DataLabel(this);

        return this.dataLabel;
      }
    }
    internal DataLabel dataLabel;

    /// <summary>
    /// Gets or sets whether the series has a DataLabel.
    /// </summary>
    public bool HasDataLabel
    {
      get {return this.hasDataLabel;}
      set {this.hasDataLabel = value;}
    }
    internal bool hasDataLabel;

    /// <summary>
    /// Gets the element count of the series.
    /// </summary>
    public int Count
    {
      get 
      {
        if (this.seriesElements != null)
          return this.seriesElements.Count;

        return 0;
      }
    }
    #endregion
  }
}

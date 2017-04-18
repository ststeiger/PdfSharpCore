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

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Represents a series of data on the X-Axis.
  /// </summary>
  public class XSeries : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the XSeries class.
    /// </summary>
    public XSeries()
    {
      xSeriesElements = new XSeriesElements();
    }

    /// <summary>
    /// Gets the xvalue at the specified index.
    /// </summary>
    public XValue this[int index]
    {
      get {return (XValue)this.xSeriesElements[index];}
    }

    /// <summary>
    /// The actual value container of the XSeries.
    /// </summary>
    protected XSeriesElements xSeriesElements;

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new XSeries Clone()
    {
      return (XSeries)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      XSeries xSeries = (XSeries)base.DeepCopy();
      if (xSeries.xSeriesElements != null)
      {
        xSeries.xSeriesElements = xSeries.xSeriesElements.Clone();
        xSeries.xSeriesElements.parent = xSeries;
      }
      return xSeries;
    }

    /// <summary>
    /// Adds a blank to the XSeries.
    /// </summary>
    public void AddBlank()
    {
      this.xSeriesElements.AddBlank();
    }

    /// <summary>
    /// Adds a value to the XSeries.
    /// </summary>
    public XValue Add(string value)
    {
      return this.xSeriesElements.Add(value);
    }

    /// <summary>
    /// Adds an array of values to the XSeries.
    /// </summary>
    public void Add(params string[] values)
    {
      this.xSeriesElements.Add(values);
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
      return this.xSeriesElements.GetEnumerator();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the number of xvalues actually contained in the xseries.
    /// </summary>
    public int Count
    {
      get {return this.xSeriesElements.Count;}
    }
    #endregion
  }
}

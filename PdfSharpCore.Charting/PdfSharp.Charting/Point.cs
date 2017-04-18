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

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Represents a formatted value on the data series.
  /// </summary>
  public class Point : ChartObject
  {    
    /// <summary>
    /// Initializes a new instance of the Point class.
    /// </summary>
    internal Point()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Point class with a real value.
    /// </summary>
    public Point(double value) : this() 
    {
      this.Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the Point class with a real value.
    /// </summary>
    public Point(string value) : this() 
    {
      // = "34.5 23.9"
      this.Value = 0;
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Point Clone()
    {
      return (Point)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Point point = (Point)base.DeepCopy();
      if (point.lineFormat != null)
      {
        point.lineFormat = point.lineFormat.Clone();
        point.lineFormat.parent = point;
      }
      if (point.fillFormat != null)
      {
        point.fillFormat = point.fillFormat.Clone();
        point.fillFormat.parent = point;
      }
      return point;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the line format of the data point's border.
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
    /// Gets the filling format of the data point.
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
    /// The actual value of the data point.
    /// </summary>
    public double Value
    {
      get {return this.value;}
      set {this.value = value;}
    }
    internal double value;
    #endregion
  }
}

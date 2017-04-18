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
using System.Diagnostics;
using System.ComponentModel;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// This class represents an axis in a chart.
  /// </summary>
  public class Axis : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the Axis class with the specified parent.
    /// </summary>
    internal Axis(DocumentObject parent) : base(parent) {}
    
    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Axis Clone()
    {
      return (Axis)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Axis axis = (Axis)base.DeepCopy();
      if (axis.title != null)
      {
        axis.title = axis.title.Clone();
        axis.title.parent = axis;
      }
      if (axis.tickLabels != null)
      {
        axis.tickLabels = axis.tickLabels.Clone();
        axis.tickLabels.parent = axis;
      }
      if (axis.lineFormat != null)
      {
        axis.lineFormat = axis.lineFormat.Clone();
        axis.lineFormat.parent = axis;
      }
      if (axis.majorGridlines != null)
      {
        axis.majorGridlines = axis.majorGridlines.Clone();
        axis.majorGridlines.parent = axis;
      }
      if (axis.minorGridlines != null)
      {
        axis.minorGridlines = axis.minorGridlines.Clone();
        axis.minorGridlines.parent = axis;
      }
      return axis;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the title of the axis.
    /// </summary>
    public AxisTitle Title
    {
      get
      {
        if (this.title == null)
          this.title = new AxisTitle(this);

        return this.title;
      }
    }
    internal AxisTitle title;

    /// <summary>
    /// Gets or sets the minimum value of the axis.
    /// </summary>
    public double MinimumScale
    {
      get {return this.minimumScale;}
      set {this.minimumScale = value;}
    }
    internal double minimumScale = double.NaN;

    /// <summary>
    /// Gets or sets the maximum value of the axis.
    /// </summary>
    public double MaximumScale
    {
      get {return this.maximumScale;}
      set {this.maximumScale = value;}
    }
    internal double maximumScale = double.NaN;

    /// <summary>
    /// Gets or sets the interval of the primary tick.
    /// </summary>
    public double MajorTick
    {
      get {return this.majorTick;}
      set {this.majorTick = value;}
    }
    internal double majorTick = double.NaN;

    /// <summary>
    /// Gets or sets the interval of the secondary tick.
    /// </summary>
    public double MinorTick
    {
      get {return this.minorTick;}
      set {this.minorTick = value;}
    }
    internal double minorTick = double.NaN;

    /// <summary>
    /// Gets or sets the type of the primary tick mark.
    /// </summary>
    public TickMarkType MajorTickMark
    {
      get {return this.majorTickMark;}
      set
      {
        if (!Enum.IsDefined(typeof(TickMarkType), value))
          throw new InvalidEnumArgumentException("value", (int)value, typeof(TickMarkType));
        this.majorTickMark = value;
        this.majorTickMarkInitialized = true;
      }
    }
    internal TickMarkType majorTickMark;
    internal bool majorTickMarkInitialized;

    /// <summary>
    /// Gets or sets the type of the secondary tick mark.
    /// </summary>
    public TickMarkType MinorTickMark
    {
      get {return this.minorTickMark;}
      set
      {
        if (!Enum.IsDefined(typeof(TickMarkType), value))
          throw new InvalidEnumArgumentException("value", (int)value, typeof(TickMarkType));
        this.minorTickMark = value;
        this.minorTickMarkInitialized = true;
      }
    }
    internal TickMarkType minorTickMark;
    internal bool minorTickMarkInitialized;

    /// <summary>
    /// Gets the label of the primary tick.
    /// </summary>
    public TickLabels TickLabels
    {
      get
      {
        if (this.tickLabels == null)
          this.tickLabels = new TickLabels(this);

        return this.tickLabels;
      }
    }
    internal TickLabels tickLabels;

    /// <summary>
    /// Gets the format of the axis line.
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
    /// Gets the primary gridline object.
    /// </summary>
    public Gridlines MajorGridlines
    {
      get
      {
        if (this.majorGridlines == null)
          this.majorGridlines = new Gridlines(this);

        return this.majorGridlines;
      }
    }
    internal Gridlines majorGridlines;
    
    /// <summary>
    /// Gets the secondary gridline object.
    /// </summary>
    public Gridlines MinorGridlines
    {
      get
      {
        if (this.minorGridlines == null)
          this.minorGridlines = new Gridlines(this);

        return this.minorGridlines;
      }
    }
    internal Gridlines minorGridlines;
    
    /// <summary>
    /// Gets or sets, whether the axis has a primary gridline object.
    /// </summary>
    public bool HasMajorGridlines
    {
      get {return this.hasMajorGridlines;}
      set {this.hasMajorGridlines = value;}
    }
    internal bool hasMajorGridlines;

    /// <summary>
    /// Gets or sets, whether the axis has a secondary gridline object.
    /// </summary>
    public bool HasMinorGridlines
    {
      get {return this.hasMinorGridlines;}
      set {this.hasMinorGridlines = value;}
    }
    internal bool hasMinorGridlines;
    #endregion
  }
}

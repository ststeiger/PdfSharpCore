#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@PdfSharpCore.com)
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
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
using System.ComponentModel;
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// This class represents an axis in a chart.
  /// </summary>
  public class Axis : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the Axis class.
    /// </summary>
    public Axis()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Axis class with the specified parent.
    /// </summary>
    internal Axis(DocumentObject parent) : base(parent) { }

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
      set
      {
        SetParent(value);
        this.title = value;
      }
    }
    [DV]
    internal AxisTitle title;

    /// <summary>
    /// Gets or sets the minimum value of the axis.
    /// </summary>
    public double MinimumScale
    {
      get { return this.minimumScale.Value; }
      set { this.minimumScale.Value = value; }
    }
    [DV]
    internal NDouble minimumScale = NDouble.NullValue;

    /// <summary>
    /// Gets or sets the maximum value of the axis.
    /// </summary>
    public double MaximumScale
    {
      get { return this.maximumScale.Value; }
      set { this.maximumScale.Value = value; }
    }
    [DV]
    internal NDouble maximumScale = NDouble.NullValue;

    /// <summary>
    /// Gets or sets the interval of the primary tick.
    /// </summary>
    public double MajorTick
    {
      get { return this.majorTick.Value; }
      set { this.majorTick.Value = value; }
    }
    [DV]
    internal NDouble majorTick = NDouble.NullValue;

    /// <summary>
    /// Gets or sets the interval of the secondary tick.
    /// </summary>
    public double MinorTick
    {
      get { return this.minorTick.Value; }
      set { this.minorTick.Value = value; }
    }
    [DV]
    internal NDouble minorTick = NDouble.NullValue;

    /// <summary>
    /// Gets or sets the type of the primary tick mark.
    /// </summary>
    public TickMarkType MajorTickMark
    {
      get { return (TickMarkType)this.majorTickMark.Value; }
      set { this.majorTickMark.Value = (int)value; }
    }
    [DV(Type = typeof(TickMarkType))]
    internal NEnum majorTickMark = NEnum.NullValue(typeof(TickMarkType));

    /// <summary>
    /// Gets or sets the type of the secondary tick mark.
    /// </summary>
    public TickMarkType MinorTickMark
    {
      get { return (TickMarkType)this.minorTickMark.Value; }
      set { this.minorTickMark.Value = (int)value; }
    }
    [DV(Type = typeof(TickMarkType))]
    internal NEnum minorTickMark = NEnum.NullValue(typeof(TickMarkType));

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
      set
      {
        SetParent(value);
        this.tickLabels = value;
      }
    }
    [DV]
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
      set
      {
        SetParent(value);
        this.lineFormat = value;
      }
    }
    [DV]
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
      set
      {
        SetParent(value);
        this.majorGridlines = value;
      }
    }
    [DV]
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
      set
      {
        SetParent(value);
        this.minorGridlines = value;
      }
    }
    [DV]
    internal Gridlines minorGridlines;

    /// <summary>
    /// Gets or sets, whether the axis has a primary gridline object.
    /// </summary>
    public bool HasMajorGridlines
    {
      get { return this.hasMajorGridlines.Value; }
      set { this.hasMajorGridlines.Value = value; }
    }
    [DV]
    internal NBool hasMajorGridlines = NBool.NullValue;

    /// <summary>
    /// Gets or sets, whether the axis has a secondary gridline object.
    /// </summary>
    public bool HasMinorGridlines
    {
      get { return this.hasMinorGridlines.Value; }
      set { this.hasMinorGridlines.Value = value; }
    }
    [DV]
    internal NBool hasMinorGridlines = NBool.NullValue;
    #endregion

    /// <summary>
    /// Determines whether the specified gridlines object is a MajorGridlines or an MinorGridlines.
    /// </summary>
    internal string CheckGridlines(Gridlines gridlines)
    {
      if ((this.majorGridlines != null) && (gridlines == this.majorGridlines))
        return "MajorGridlines";
      if ((this.minorGridlines != null) && (gridlines == this.minorGridlines))
        return "MinorGridlines";

      return "";
    }

    #region Internal
    /// <summary>
    /// Converts Axis into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      Chart chartObject = this.parent as Chart;

      serializer.WriteLine("\\" + chartObject.CheckAxis(this));
      int pos = serializer.BeginAttributes();

      if (!this.minimumScale.IsNull)
        serializer.WriteSimpleAttribute("MinimumScale", this.MinimumScale);
      if (!this.maximumScale.IsNull)
        serializer.WriteSimpleAttribute("MaximumScale", this.MaximumScale);
      if (!this.majorTick.IsNull)
        serializer.WriteSimpleAttribute("MajorTick", this.MajorTick);
      if (!this.minorTick.IsNull)
        serializer.WriteSimpleAttribute("MinorTick", this.MinorTick);
      if (!this.hasMajorGridlines.IsNull)
        serializer.WriteSimpleAttribute("HasMajorGridLines", this.HasMajorGridlines);
      if (!this.hasMinorGridlines.IsNull)
        serializer.WriteSimpleAttribute("HasMinorGridLines", this.HasMinorGridlines);
      if (!this.majorTickMark.IsNull)
        serializer.WriteSimpleAttribute("MajorTickMark", this.MajorTickMark);
      if (!this.minorTickMark.IsNull)
        serializer.WriteSimpleAttribute("MinorTickMark", this.MinorTickMark);

      if (!this.IsNull("Title"))
        this.title.Serialize(serializer);

      if (!this.IsNull("LineFormat"))
        this.lineFormat.Serialize(serializer);

      if (!this.IsNull("MajorGridlines"))
        this.majorGridlines.Serialize(serializer);

      if (!this.IsNull("MinorGridlines"))
        this.minorGridlines.Serialize(serializer);

      if (!this.IsNull("TickLabels"))
        this.tickLabels.Serialize(serializer);

      serializer.EndAttributes(pos);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Axis));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

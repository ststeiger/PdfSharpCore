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
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// Represents charts with different types.
  /// </summary>
  public class Chart : Shape, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the Chart class.
    /// </summary>
    public Chart()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Chart class with the specified parent.
    /// </summary>
    internal Chart(DocumentObject parent) : base(parent) { }

    /// <summary>
    /// Initializes a new instance of the Chart class with the specified chart type.
    /// </summary>
    public Chart(ChartType type)
      : this()
    {
      this.Type = type;
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Chart Clone()
    {
      return (Chart)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Chart chart = (Chart)base.DeepCopy();
      if (chart.format != null)
      {
        chart.format = chart.format.Clone();
        chart.format.parent = chart;
      }
      if (chart.xAxis != null)
      {
        chart.xAxis = chart.xAxis.Clone();
        chart.xAxis.parent = chart;
      }
      if (chart.yAxis != null)
      {
        chart.yAxis = chart.yAxis.Clone();
        chart.yAxis.parent = chart;
      }
      if (chart.zAxis != null)
      {
        chart.zAxis = chart.zAxis.Clone();
        chart.zAxis.parent = chart;
      }
      if (chart.seriesCollection != null)
      {
        chart.seriesCollection = chart.seriesCollection.Clone();
        chart.seriesCollection.parent = chart;
      }
      if (chart.xValues != null)
      {
        chart.xValues = chart.xValues.Clone();
        chart.xValues.parent = chart;
      }
      if (chart.headerArea != null)
      {
        chart.headerArea = chart.headerArea.Clone();
        chart.headerArea.parent = chart;
      }
      if (chart.bottomArea != null)
      {
        chart.bottomArea = chart.bottomArea.Clone();
        chart.bottomArea.parent = chart;
      }
      if (chart.topArea != null)
      {
        chart.topArea = chart.topArea.Clone();
        chart.topArea.parent = chart;
      }
      if (chart.footerArea != null)
      {
        chart.footerArea = chart.footerArea.Clone();
        chart.footerArea.parent = chart;
      }
      if (chart.leftArea != null)
      {
        chart.leftArea = chart.leftArea.Clone();
        chart.leftArea.parent = chart;
      }
      if (chart.rightArea != null)
      {
        chart.rightArea = chart.rightArea.Clone();
        chart.rightArea.parent = chart;
      }
      if (chart.plotArea != null)
      {
        chart.plotArea = chart.plotArea.Clone();
        chart.plotArea.parent = chart;
      }
      if (chart.dataLabel != null)
      {
        chart.dataLabel = chart.dataLabel.Clone();
        chart.dataLabel.parent = chart;
      }
      return chart;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the base type of the chart.
    /// ChartType of the series can be overwritten.
    /// </summary>
    public ChartType Type
    {
      get { return (ChartType)this.type.Value; }
      set { this.type.Value = (int)value; }
    }
    [DV(Type = typeof(ChartType))]
    internal NEnum type = NEnum.NullValue(typeof(ChartType));

    /// <summary>
    /// Gets or sets the default style name of the whole chart.
    /// </summary>
    public string Style
    {
      get { return this.style.Value; }
      set { this.style.Value = value; }
    }
    [DV]
    internal NString style = NString.NullValue;

    /// <summary>
    /// Gets the default paragraph format of the whole chart.
    /// </summary>
    public ParagraphFormat Format
    {
      get
      {
        if (this.format == null)
          this.format = new ParagraphFormat(this);

        return this.format;
      }
      set
      {
        SetParent(value);
        this.format = value;
      }
    }
    [DV]
    internal ParagraphFormat format;

    /// <summary>
    /// Gets the X-Axis of the Chart.
    /// </summary>
    public Axis XAxis
    {
      get
      {
        if (this.xAxis == null)
          this.xAxis = new Axis(this);

        return this.xAxis;
      }
      set
      {
        SetParent(value);
        this.xAxis = value;
      }
    }
    [DV]
    internal Axis xAxis;

    /// <summary>
    /// Gets the Y-Axis of the Chart.
    /// </summary>
    public Axis YAxis
    {
      get
      {
        if (this.yAxis == null)
          this.yAxis = new Axis(this);

        return this.yAxis;
      }
      set
      {
        SetParent(value);
        this.yAxis = value;
      }
    }
    [DV]
    internal Axis yAxis;

    /// <summary>
    /// Gets the Z-Axis of the Chart.
    /// </summary>
    public Axis ZAxis
    {
      get
      {
        if (this.zAxis == null)
          this.zAxis = new Axis(this);

        return this.zAxis;
      }
      set
      {
        SetParent(value);
        this.zAxis = value;
      }
    }
    [DV]
    internal Axis zAxis;

    /// <summary>
    /// Gets the collection of the data series.
    /// </summary>
    public SeriesCollection SeriesCollection
    {
      get
      {
        if (this.seriesCollection == null)
          this.seriesCollection = new SeriesCollection(this);

        return this.seriesCollection;
      }
      set
      {
        SetParent(value);
        this.seriesCollection = value;
      }
    }
    [DV(ItemType = typeof(Series))]
    internal SeriesCollection seriesCollection;

    /// <summary>
    /// Gets the collection of the values written on the X-Axis.
    /// </summary>
    public XValues XValues
    {
      get
      {
        if (this.xValues == null)
          this.xValues = new XValues(this);

        return this.xValues;
      }
      set
      {
        SetParent(value);
        this.xValues = value;
      }
    }
    [DV(ItemType = typeof(Series))]
    internal XValues xValues;

    /// <summary>
    /// Gets the header area of the chart.
    /// </summary>
    public TextArea HeaderArea
    {
      get
      {
        if (this.headerArea == null)
          this.headerArea = new TextArea(this);

        return this.headerArea;
      }
      set
      {
        SetParent(value);
        this.headerArea = value;
      }
    }
    [DV]
    internal TextArea headerArea;

    /// <summary>
    /// Gets the bottom area of the chart.
    /// </summary>
    public TextArea BottomArea
    {
      get
      {
        if (this.bottomArea == null)
          this.bottomArea = new TextArea(this);

        return this.bottomArea;
      }
      set
      {
        SetParent(value);
        this.bottomArea = value;
      }
    }
    [DV]
    internal TextArea bottomArea;

    /// <summary>
    /// Gets the top area of the chart.
    /// </summary>
    public TextArea TopArea
    {
      get
      {
        if (this.topArea == null)
          this.topArea = new TextArea(this);

        return this.topArea;
      }
      set
      {
        SetParent(value);
        this.topArea = value;
      }
    }
    [DV]
    internal TextArea topArea;

    /// <summary>
    /// Gets the footer area of the chart.
    /// </summary>
    public TextArea FooterArea
    {
      get
      {
        if (this.footerArea == null)
          this.footerArea = new TextArea(this);

        return this.footerArea;
      }
      set
      {
        SetParent(value);
        this.footerArea = value;
      }
    }
    [DV]
    internal TextArea footerArea;

    /// <summary>
    /// Gets the left area of the chart.
    /// </summary>
    public TextArea LeftArea
    {
      get
      {
        if (this.leftArea == null)
          this.leftArea = new TextArea(this);

        return this.leftArea;
      }
      set
      {
        SetParent(value);
        this.leftArea = value;
      }
    }
    [DV]
    internal TextArea leftArea;

    /// <summary>
    /// Gets the right area of the chart.
    /// </summary>
    public TextArea RightArea
    {
      get
      {
        if (this.rightArea == null)
          this.rightArea = new TextArea(this);

        return this.rightArea;
      }
      set
      {
        SetParent(value);
        this.rightArea = value;
      }
    }
    [DV]
    internal TextArea rightArea;

    /// <summary>
    /// Gets the plot (drawing) area of the chart.
    /// </summary>
    public PlotArea PlotArea
    {
      get
      {
        if (this.plotArea == null)
          this.plotArea = new PlotArea(this);

        return this.plotArea;
      }
      set
      {
        SetParent(value);
        this.plotArea = value;
      }
    }
    [DV]
    internal PlotArea plotArea;

    /// <summary>
    /// Gets or sets a value defining how blanks in the data series should be shown.
    /// </summary>
    public BlankType DisplayBlanksAs
    {
      get { return (BlankType)this.displayBlanksAs.Value; }
      set { this.displayBlanksAs.Value = (int)value; }
    }
    [DV(Type = typeof(BlankType))]
    internal NEnum displayBlanksAs = NEnum.NullValue(typeof(BlankType));

    /// <summary>
    /// Gets or sets whether XAxis Labels should be merged.
    /// </summary>
    public bool PivotChart
    {
      get { return this.pivotChart.Value; }
      set { this.pivotChart.Value = value; }
    }
    [DV]
    internal NBool pivotChart = NBool.NullValue;

    /// <summary>
    /// Gets the DataLabel of the chart.
    /// </summary>
    public DataLabel DataLabel
    {
      get
      {
        if (this.dataLabel == null)
          this.dataLabel = new DataLabel(this);

        return this.dataLabel;
      }
      set
      {
        SetParent(value);
        this.dataLabel = value;
      }
    }
    [DV]
    internal DataLabel dataLabel;

    /// <summary>
    /// Gets or sets whether the chart has a DataLabel.
    /// </summary>
    public bool HasDataLabel
    {
      get { return this.hasDataLabel.Value; }
      set { this.hasDataLabel.Value = value; }
    }
    [DV]
    internal NBool hasDataLabel = NBool.NullValue;
    #endregion

    /// <summary>
    /// Determines the type of the given axis.
    /// </summary>
    internal string CheckAxis(Axis axis)
    {
      if ((this.xAxis != null) && (axis == this.xAxis))
        return "xaxis";
      if ((this.yAxis != null) && (axis == this.yAxis))
        return "yaxis";
      if ((this.zAxis != null) && (axis == this.zAxis))
        return "zaxis";

      return "";
    }

    /// <summary>
    /// Determines the type of the given textarea.
    /// </summary>
    internal string CheckTextArea(TextArea textArea)
    {
      if ((this.headerArea != null) && (textArea == this.headerArea))
        return "headerarea";
      if ((this.footerArea != null) && (textArea == this.footerArea))
        return "footerarea";
      if ((this.leftArea != null) && (textArea == this.leftArea))
        return "leftarea";
      if ((this.rightArea != null) && (textArea == this.rightArea))
        return "rightarea";
      if ((this.topArea != null) && (textArea == this.topArea))
        return "toparea";
      if ((this.bottomArea != null) && (textArea == this.bottomArea))
        return "bottomarea";

      return "";
    }

    #region Internal
    /// <summary>
    /// Converts Chart into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteLine("\\chart(" + this.Type + ")");
      int pos = serializer.BeginAttributes();

      base.Serialize(serializer);
      if (!this.displayBlanksAs.IsNull)
        serializer.WriteSimpleAttribute("DisplayBlanksAs", this.DisplayBlanksAs);
      if (!this.pivotChart.IsNull)
        serializer.WriteSimpleAttribute("PivotChart", this.PivotChart);
      if (!this.hasDataLabel.IsNull)
        serializer.WriteSimpleAttribute("HasDataLabel", this.HasDataLabel);

      if (!this.style.IsNull)
        serializer.WriteSimpleAttribute("Style", this.Style);
      if (!this.IsNull("Format"))
        this.format.Serialize(serializer, "Format", null);
      if (!this.IsNull("DataLabel"))
        this.dataLabel.Serialize(serializer);
      serializer.EndAttributes(pos);

      serializer.BeginContent();

      if (!this.IsNull("PlotArea"))
        this.plotArea.Serialize(serializer);
      if (!this.IsNull("HeaderArea"))
        this.headerArea.Serialize(serializer);
      if (!this.IsNull("FooterArea"))
        this.footerArea.Serialize(serializer);
      if (!this.IsNull("TopArea"))
        this.topArea.Serialize(serializer);
      if (!this.IsNull("BottomArea"))
        this.bottomArea.Serialize(serializer);
      if (!this.IsNull("LeftArea"))
        this.leftArea.Serialize(serializer);
      if (!this.IsNull("RightArea"))
        this.rightArea.Serialize(serializer);

      if (!this.IsNull("XAxis"))
        this.xAxis.Serialize(serializer);
      if (!this.IsNull("YAxis"))
        this.yAxis.Serialize(serializer);
      if (!this.IsNull("ZAxis"))
        this.zAxis.Serialize(serializer);

      if (!this.IsNull("SeriesCollection"))
        this.seriesCollection.Serialize(serializer);
      if (!this.IsNull("XValues"))
        this.xValues.Serialize(serializer);

      serializer.EndContent();
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitChart(this);
      if (visitChildren)
      {
        if (this.bottomArea != null)
          ((IVisitable)this.bottomArea).AcceptVisitor(visitor, visitChildren);

        if (this.footerArea != null)
          ((IVisitable)this.footerArea).AcceptVisitor(visitor, visitChildren);

        if (this.headerArea != null)
          ((IVisitable)this.headerArea).AcceptVisitor(visitor, visitChildren);

        if (this.leftArea != null)
          ((IVisitable)this.leftArea).AcceptVisitor(visitor, visitChildren);

        if (this.rightArea != null)
          ((IVisitable)this.rightArea).AcceptVisitor(visitor, visitChildren);

        if (this.topArea != null)
          ((IVisitable)this.topArea).AcceptVisitor(visitor, visitChildren);
      }
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Chart));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

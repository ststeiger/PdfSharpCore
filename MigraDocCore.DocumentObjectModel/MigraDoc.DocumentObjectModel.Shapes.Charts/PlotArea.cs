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
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// Represents the area where the actual chart is drawn.
  /// </summary>
  public class PlotArea : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the PlotArea class.
    /// </summary>
    internal PlotArea()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PlotArea class with the specified parent.
    /// </summary>
    internal PlotArea(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new PlotArea Clone()
    {
      return (PlotArea)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      PlotArea plotArea = (PlotArea)base.DeepCopy();
      if (plotArea.lineFormat != null)
      {
        plotArea.lineFormat = plotArea.lineFormat.Clone();
        plotArea.lineFormat.parent = plotArea;
      }
      if (plotArea.fillFormat != null)
      {
        plotArea.fillFormat = plotArea.fillFormat.Clone();
        plotArea.fillFormat.parent = plotArea;
      }
      return plotArea;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the line format of the plot area's border.
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
    /// Gets the background filling of the plot area.
    /// </summary>
    public FillFormat FillFormat
    {
      get
      {
        if (this.fillFormat == null)
          this.fillFormat = new FillFormat(this);

        return this.fillFormat;
      }
      set
      {
        SetParent(value);
        this.fillFormat = value;
      }
    }
    [DV]
    internal FillFormat fillFormat;

    /// <summary>
    /// Gets or sets the left padding of the area.
    /// </summary>
    public Unit LeftPadding
    {
      get { return this.leftPadding; }
      set { this.leftPadding = value; }
    }
    [DV]
    internal Unit leftPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the right padding of the area.
    /// </summary>
    public Unit RightPadding
    {
      get { return this.rightPadding; }
      set { this.rightPadding = value; }
    }
    [DV]
    internal Unit rightPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the top padding of the area.
    /// </summary>
    public Unit TopPadding
    {
      get { return this.topPadding; }
      set { this.topPadding = value; }
    }
    [DV]
    internal Unit topPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the bottom padding of the area.
    /// </summary>
    public Unit BottomPadding
    {
      get { return this.bottomPadding; }
      set { this.bottomPadding = value; }
    }
    [DV]
    internal Unit bottomPadding = Unit.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts PlotArea into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteLine("\\plotarea");
      int pos = serializer.BeginAttributes();

      if (!this.topPadding.IsNull)
        serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);
      if (!this.leftPadding.IsNull)
        serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);
      if (!this.rightPadding.IsNull)
        serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);
      if (!this.bottomPadding.IsNull)
        serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);

      if (!this.IsNull("LineFormat"))
        this.lineFormat.Serialize(serializer);
      if (!this.IsNull("FillFormat"))
        this.fillFormat.Serialize(serializer);

      serializer.EndAttributes(pos);

      serializer.BeginContent();
      serializer.EndContent();
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(PlotArea));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

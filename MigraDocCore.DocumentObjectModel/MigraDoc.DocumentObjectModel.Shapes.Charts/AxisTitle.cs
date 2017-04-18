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
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Tables;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// Represents the title of an axis.
  /// </summary>
  public class AxisTitle : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the AxisTitle class.
    /// </summary>
    public AxisTitle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AxisTitle class with the specified parent.
    /// </summary>
    internal AxisTitle(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new AxisTitle Clone()
    {
      return (AxisTitle)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      AxisTitle axisTitle = (AxisTitle)base.DeepCopy();
      if (axisTitle.font != null)
      {
        axisTitle.font = axisTitle.font.Clone();
        axisTitle.font.parent = axisTitle;
      }
      return axisTitle;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the style name of the axis.
    /// </summary>
    public string Style
    {
      get { return this.style.Value; }
      set { this.style.Value = value; }
    }
    [DV]
    internal NString style = NString.NullValue;

    /// <summary>
    /// Gets or sets the caption of the title.
    /// </summary>
    public string Caption
    {
      get { return this.caption.Value; }
      set { this.caption.Value = value; }
    }
    [DV]
    internal NString caption = NString.NullValue;

    /// <summary>
    /// Gets the font object of the title.
    /// </summary>
    public Font Font
    {
      get
      {
        if (this.font == null)
          this.font = new Font(this);

        return this.font;
      }
      set
      {
        SetParent(value);
        this.font = value;
      }
    }
    [DV]
    internal Font font;

    /// <summary>
    /// Gets or sets the orientation of the caption.
    /// </summary>
    public Unit Orientation
    {
      get { return this.orientation; }
      set { this.orientation = value; }
    }
    [DV]
    internal Unit orientation = Unit.NullValue;

    /// <summary>
    /// Gets or sets the alignment of the caption.
    /// </summary>
    public HorizontalAlignment Alignment
    {
      get { return (HorizontalAlignment)this.alignment.Value; }
      set { this.alignment.Value = (int)value; }
    }
    [DV(Type = typeof(HorizontalAlignment))]
    internal NEnum alignment = NEnum.NullValue(typeof(HorizontalAlignment));

    /// <summary>
    /// Gets or sets the alignment of the caption.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
      get { return (VerticalAlignment)this.verticalAlignment.Value; }
      set { this.verticalAlignment.Value = (int)value; }
    }
    [DV(Type = typeof(VerticalAlignment))]
    internal NEnum verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));
    #endregion

    #region Internal
    /// <summary>
    /// Converts AxisTitle into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int pos = serializer.BeginContent("Title");

      if (!this.style.IsNull)
        serializer.WriteSimpleAttribute("Style", this.Style);

      if (!this.IsNull("Font"))
        this.font.Serialize(serializer);

      if (!this.orientation.IsNull)
        serializer.WriteSimpleAttribute("Orientation", this.Orientation);

      if (!this.alignment.IsNull)
        serializer.WriteSimpleAttribute("Alignment", this.Alignment);

      if (!this.verticalAlignment.IsNull)
        serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);

      if (!this.caption.IsNull)
        serializer.WriteSimpleAttribute("Caption", this.Caption);

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
          meta = new Meta(typeof(AxisTitle));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

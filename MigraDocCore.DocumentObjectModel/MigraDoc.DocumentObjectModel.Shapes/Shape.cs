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


namespace MigraDocCore.DocumentObjectModel.Shapes
{
  /// <summary>
  /// Base Class for all positionable Classes.
  /// </summary>
  public class Shape : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the Shape class.
    /// </summary>
    public Shape()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Shape class with the specified parent.
    /// </summary>
    internal Shape(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Shape Clone()
    {
      return (Shape)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Shape shape = (Shape)base.DeepCopy();
      if (shape.wrapFormat != null)
      {
        shape.wrapFormat = shape.wrapFormat.Clone();
        shape.wrapFormat.parent = shape;
      }
      if (shape.lineFormat != null)
      {
        shape.lineFormat = shape.lineFormat.Clone();
        shape.lineFormat.parent = shape;
      }
      if (shape.fillFormat != null)
      {
        shape.fillFormat = shape.fillFormat.Clone();
        shape.fillFormat.parent = shape;
      }
      return shape;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the wrapping format of the shape.
    /// </summary>
    public WrapFormat WrapFormat
    {
      get
      {
        if (this.wrapFormat == null)
          this.wrapFormat = new WrapFormat(this);

        return this.wrapFormat;
      }
      set
      {
        SetParent(value);
        this.wrapFormat = value;
      }
    }
    [DV]
    internal WrapFormat wrapFormat;

    /// <summary>
    /// Gets or sets the reference point of the Top property.
    /// </summary>
    public RelativeVertical RelativeVertical
    {
      get { return (RelativeVertical)this.relativeVertical.Value; }
      set { this.relativeVertical.Value = (int)value; }
    }
    [DV(Type = typeof(RelativeVertical))]
    internal NEnum relativeVertical = NEnum.NullValue(typeof(RelativeVertical));

    /// <summary>
    /// Gets or sets the reference point of the Left property.
    /// </summary>
    public RelativeHorizontal RelativeHorizontal
    {
      get { return (RelativeHorizontal)this.relativeHorizontal.Value; }
      set { this.relativeHorizontal.Value = (int)value; }
    }
    [DV(Type = typeof(RelativeHorizontal))]
    internal NEnum relativeHorizontal = NEnum.NullValue(typeof(RelativeHorizontal));

    /// <summary>
    /// Gets or sets the position of the top side of the shape.
    /// </summary>
    public TopPosition Top
    {
      get { return this.top; }
      set { this.top = value; }
    }
    [DV]
    internal TopPosition top = TopPosition.NullValue;

    /// <summary>
    /// Gets or sets the position of the left side of the shape.
    /// </summary>
    public LeftPosition Left
    {
      get { return this.left; }
      set { this.left = value; }
    }
    [DV]
    internal LeftPosition left = LeftPosition.NullValue;

    /// <summary>
    /// Gets the line format of the shape's border.
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
    /// Gets the background filling format of the shape.
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
    /// Gets or sets the height of the shape.
    /// </summary>
    public Unit Height
    {
      get { return this.height; }
      set { this.height = value; }
    }
    [DV]
    internal Unit height = Unit.NullValue;

    /// <summary>
    /// Gets or sets the width of the shape.
    /// </summary>
    public Unit Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    [DV]
    internal Unit width = Unit.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts Shape into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      if (!this.height.IsNull)
        serializer.WriteSimpleAttribute("Height", this.Height);
      if (!this.width.IsNull)
        serializer.WriteSimpleAttribute("Width", this.Width);
      if (!this.relativeHorizontal.IsNull)
        serializer.WriteSimpleAttribute("RelativeHorizontal", this.RelativeHorizontal);
      if (!this.relativeVertical.IsNull)
        serializer.WriteSimpleAttribute("RelativeVertical", this.RelativeVertical);
      if (!this.IsNull("Left"))
        this.left.Serialize(serializer);
      if (!this.IsNull("Top"))
        this.top.Serialize(serializer);
      if (!this.IsNull("WrapFormat"))
        this.wrapFormat.Serialize(serializer);
      if (!this.IsNull("LineFormat"))
        this.lineFormat.Serialize(serializer);
      if (!this.IsNull("FillFormat"))
        this.fillFormat.Serialize(serializer);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Shape));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

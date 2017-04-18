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
  /// Define how the shape should be wrapped between the texts.
  /// </summary>
  public class WrapFormat : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the WrapFormat class.
    /// </summary>
    public WrapFormat()
    {
    }

    /// <summary>
    /// Initializes a new instance of the WrapFormat class with the specified parent.
    /// </summary>
    internal WrapFormat(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new WrapFormat Clone()
    {
      return (WrapFormat)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the wrapping style.
    /// </summary>
    public WrapStyle Style
    {
      get { return (WrapStyle)this.style.Value; }
      set { this.style.Value = (int)value; }
    }
    [DV(Type = typeof(WrapStyle))]
    internal NEnum style = NEnum.NullValue(typeof(WrapStyle));

    /// <summary>
    /// Gets or sets the distance between the top side of the shape with the adjacent text.
    /// </summary>
    public Unit DistanceTop
    {
      get { return this.distanceTop; }
      set { this.distanceTop = value; }
    }
    [DV]
    protected Unit distanceTop = Unit.NullValue;

    /// <summary>
    /// Gets or sets the distance between the bottom side of the shape with the adjacent text.
    /// </summary>
    public Unit DistanceBottom
    {
      get { return this.distanceBottom; }
      set { this.distanceBottom = value; }
    }
    [DV]
    protected Unit distanceBottom = Unit.NullValue;

    /// <summary>
    /// Gets or sets the distance between the left side of the shape with the adjacent text.
    /// </summary>
    public Unit DistanceLeft
    {
      get { return this.distanceLeft; }
      set { this.distanceLeft = value; }
    }
    [DV]
    protected Unit distanceLeft = Unit.NullValue;

    /// <summary>
    /// Gets or sets the distance between the right side of the shape with the adjacent text.
    /// </summary>
    public Unit DistanceRight
    {
      get { return this.distanceRight; }
      set { this.distanceRight = value; }
    }
    [DV]
    protected Unit distanceRight = Unit.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts WrapFormat into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int pos = serializer.BeginContent("WrapFormat");
      if (!this.style.IsNull)
        serializer.WriteSimpleAttribute("Style", this.Style);
      if (!this.distanceTop.IsNull)
        serializer.WriteSimpleAttribute("DistanceTop", this.DistanceTop);
      if (!this.distanceLeft.IsNull)
        serializer.WriteSimpleAttribute("DistanceLeft", this.DistanceLeft);
      if (!this.distanceRight.IsNull)
        serializer.WriteSimpleAttribute("DistanceRight", this.DistanceRight);
      if (!this.distanceBottom.IsNull)
        serializer.WriteSimpleAttribute("DistanceBottom", this.DistanceBottom);
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
          meta = new Meta(typeof(WrapFormat));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}
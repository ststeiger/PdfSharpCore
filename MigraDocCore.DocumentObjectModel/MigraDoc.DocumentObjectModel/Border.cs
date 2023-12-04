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
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Represents one border in a borders collection. The type determines its position in a cell,
  /// paragraph etc.
  /// </summary>
  public class Border : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the Border class.
    /// </summary>
    public Border()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Border class with the specified parent.
    /// </summary>
    internal Border(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Border Clone()
    {
      return (Border)DeepCopy();
    }

    /// <summary>
    /// Clears the Border object. Additionally 'Border = null'
    /// is written to the DDL stream when serialized.
    /// </summary>
    public void Clear()
    {
      this.fClear.Value = true;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether the border visible is.
    /// </summary>
    public bool Visible
    {
      get { return this.visible.Value; }
      set { this.visible.Value = value; }
    }
    [DV]
    internal NBool visible = NBool.NullValue;

    /// <summary>
    /// Gets or sets the line style of the border.
    /// </summary>
    public BorderStyle Style
    {
      get { return (BorderStyle)this.style.Value; }
      set { this.style.Value = (int)value; }
    }
    [DV(Type = typeof(BorderStyle))]
    internal NEnum style = NEnum.NullValue(typeof(BorderStyle));

    /// <summary>
    /// Gets or sets the line width of the border.
    /// </summary>
    public Unit Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    [DV]
    internal Unit width = Unit.NullValue;

    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    public Color Color
    {
      get { return this.color; }
      set { this.color = value; }
    }
    [DV]
    internal Color color = Color.Empty;

    /// <summary>
    /// Gets the name of this border ("top", "bottom"....).
    /// </summary>
    public string Name
    {
      get { return ((Borders)parent).GetMyName(this); }
    }

    /// <summary>
    /// Gets the information if the border is marked as cleared. Additionally 'xxx = null'
    /// is written to the DDL stream when serialized.
    /// </summary>
    public bool BorderCleared
    {
      get { return this.fClear.Value; }
    }
    internal NBool fClear = new NBool(false);
    #endregion

    #region Internal
    /// <summary>
    /// Converts Border into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      throw new Exception("A Border cannot be serialized alone.");
    }

    /// <summary>
    /// Converts Border into DDL.
    /// </summary>
    internal void Serialize(Serializer serializer, string name, Border refBorder)
    {
      if (this.fClear.Value)
        serializer.WriteLine(name + " = null");

      int pos = serializer.BeginContent(name);

      if (!this.visible.IsNull && (refBorder == null || (this.Visible != refBorder.Visible)))
        serializer.WriteSimpleAttribute("Visible", this.Visible);

      if (!this.style.IsNull && (refBorder == null || (this.Style != refBorder.Style)))
        serializer.WriteSimpleAttribute("Style", this.Style);

      if (!this.width.IsNull && (refBorder == null || (this.Width != refBorder.Width)))
        serializer.WriteSimpleAttribute("Width", this.Width);

      if (!this.color.IsNull && (refBorder == null || (this.Color != refBorder.Color)))
        serializer.WriteSimpleAttribute("Color", this.Color);

      serializer.EndContent(pos);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Border));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

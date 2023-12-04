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
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Shading represents the background color of a document object.
  /// </summary>
  public sealed class Shading : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the Shading class.
    /// </summary>
    public Shading()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Shading class with the specified parent.
    /// </summary>
    internal Shading(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Shading Clone()
    {
      return (Shading)DeepCopy();
    }

    /// <summary>
    /// Clears the Shading object. Additionally 'Shading = null'
    /// is written to the DDL stream when serialized.
    /// </summary>
    public void Clear()
    {
      this.isCleared = true;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether the shading is visible.
    /// </summary>
    public bool Visible
    {
      get { return this.visible.Value; }
      set { this.visible.Value = value; }
    }
    [DV]
    internal NBool visible = NBool.NullValue;

    /// <summary>
    /// Gets or sets the shading color.
    /// </summary>
    public Color Color
    {
      get { return this.color; }
      set { this.color = value; }
    }
    [DV]
    internal Color color = Color.Empty;

    /// <summary>
    /// Gets the information if the shading is marked as cleared. Additionally 'Shading = null'
    /// is written to the DDL stream when serialized.
    /// </summary>
    public bool IsCleared
    {
      get { return this.isCleared; }
    }
    internal bool isCleared = false;
    #endregion

    #region Internal
    /// <summary>
    /// Converts Shading into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      if (isCleared)
        serializer.WriteLine("Shading = null");

      int pos = serializer.BeginContent("Shading");

      if (!this.visible.IsNull)
        serializer.WriteSimpleAttribute("Visible", this.Visible);

      if (!this.color.IsNull)
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
          meta = new Meta(typeof(Shading));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

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
using System.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;

namespace MigraDocCore.DocumentObjectModel.Shapes
{
  /// <summary>
  /// Represents a barcode in the document or paragraph. !!!Still under Construction!!!
  /// </summary>
  public class Barcode : Shape
  {
    /// <summary>
    /// Initializes a new instance of the Barcode class.
    /// </summary>
    internal Barcode()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Barcode class with the specified parent.
    /// </summary>
    internal Barcode(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Barcode Clone()
    {
      return (Barcode)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the text orientation for the barcode content.
    /// </summary>
    public TextOrientation Orientation
    {
      get { return (TextOrientation)this.orientation.Value; }
      set { this.orientation.Value = (int)value; }
    }
    [DV(Type = typeof(TextOrientation))]
    internal NEnum orientation = NEnum.NullValue(typeof(TextOrientation));

    /// <summary>
    /// Gets or sets the type of the barcode.
    /// </summary>
    public BarcodeType Type
    {
      get { return (BarcodeType)this.type.Value; }
      set { this.type.Value = (int)value; }
    }
    [DV(Type = typeof(BarcodeType))]
    internal NEnum type = NEnum.NullValue(typeof(BarcodeType));

    /// <summary>
    /// Gets or sets a value indicating whether bars shall appear beside the barcode
    /// </summary>
    public bool BearerBars
    {
      get { return this.bearerBars.Value; }
      set { this.bearerBars.Value = value; }
    }
    [DV]
    internal NBool bearerBars = NBool.NullValue;

    /// <summary>
    /// Gets or sets the a value indicating whether the barcode's code is rendered.
    /// </summary>
    public bool Text
    {
      get { return this.text.Value; }
      set { this.text.Value = value; }
    }
    [DV]
    internal NBool text = NBool.NullValue;

    /// <summary>
    /// Gets or sets code the barcode represents.
    /// </summary>
    public string Code
    {
      get { return this.code.Value; }
      set { this.code.Value = value; }
    }
    [DV]
    internal NString code = NString.NullValue;

    /// <summary>
    /// ???
    /// </summary>
    public double LineRatio
    {
      get { return this.lineRatio.Value; }
      set { this.lineRatio.Value = value; }
    }
    [DV]
    internal NDouble lineRatio = NDouble.NullValue;

    /// <summary>
    /// ???
    /// </summary>
    public double LineHeight
    {
      get { return this.lineHeight.Value; }
      set { this.lineHeight.Value = value; }
    }
    [DV]
    internal NDouble lineHeight = NDouble.NullValue;

    /// <summary>
    /// ???
    /// </summary>
    public double NarrowLineWidth
    {
      get { return this.narrowLineWidth.Value; }
      set { this.narrowLineWidth.Value = value; }
    }
    [DV]
    internal NDouble narrowLineWidth = NDouble.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts Barcode into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      if (this.code.Value == "")
        throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "BookmarkField"));

      serializer.WriteLine("\\barcode(\"" + this.Code + "\")");

      int pos = serializer.BeginAttributes();

      base.Serialize(serializer);

      if (!this.orientation.IsNull)
        serializer.WriteSimpleAttribute("Orientation", this.Orientation);
      if (!this.bearerBars.IsNull)
        serializer.WriteSimpleAttribute("BearerBars", this.BearerBars);
      if (!this.text.IsNull)
        serializer.WriteSimpleAttribute("Text", this.Text);
      if (!this.type.IsNull)
        serializer.WriteSimpleAttribute("Type", this.Type);
      if (!this.lineRatio.IsNull)
        serializer.WriteSimpleAttribute("LineRatio", this.LineRatio);
      if (!this.lineHeight.IsNull)
        serializer.WriteSimpleAttribute("LineHeight", this.LineHeight);
      if (!this.narrowLineWidth.IsNull)
        serializer.WriteSimpleAttribute("NarrowLineWidth", this.NarrowLineWidth);

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
          meta = new Meta(typeof(Barcode));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

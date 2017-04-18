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
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;

namespace MigraDocCore.DocumentObjectModel.Fields
{
  /// <summary>
  /// NumericFieldBase serves as a base for Numeric fields, which are: 
  /// NumPagesField, PageField, PageRefField, SectionField, SectionPagesField
  /// </summary>
  public abstract class NumericFieldBase : DocumentObject
  {
    protected static string[] validFormatStrings =
    {
      "",
      "ROMAN",
      "roman",
      "ALPHABETIC",
      "alphabetic"
    };

    /// <summary>
    /// Initializes a new instance of the NumericFieldBase class.
    /// </summary>
    internal NumericFieldBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the NumericFieldBase class with the specified parent.
    /// </summary>
    internal NumericFieldBase(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new NumericFieldBase Clone()
    {
      return (NumericFieldBase)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      NumericFieldBase numericFieldBase = (NumericFieldBase)base.DeepCopy();
      return numericFieldBase;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the format of the number.
    /// </summary>
    public string Format
    {
      get { return this.format.Value; }
      set
      {
        if (IsValidFormat(value))
          this.format.Value = value;
        else
          throw new ArgumentException(DomSR.InvalidFieldFormat(value));
      }
    }
    [DV]
    internal NString format = NString.NullValue;
    #endregion

    /// <summary>
    /// Determines whether the format is valid for numeric fields.
    /// </summary>
    protected bool IsValidFormat(string format)
    {
      foreach (string name in validFormatStrings)
      {
        if (name == this.Format)
          return true;
      }
      return false;
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    public override bool IsNull()
    {
      return false;
    }
  }
}

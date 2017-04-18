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
  /// InfoField is used to reference one of the DocumentInfo fields in the document.
  /// </summary>
  public class InfoField : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the InfoField class.
    /// </summary>
    internal InfoField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the InfoField class with the specified parent.
    /// </summary>
    internal InfoField(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new InfoField Clone()
    {
      return (InfoField)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the name of the information to be shown in the field.
    /// </summary>
    public string Name
    {
      get { return this.name.Value; }
      set
      {
        if (IsValidName(value))
          this.name.Value = value;
        else
          throw new ArgumentException(DomSR.InvalidInfoFieldName(value));
      }
    }
    [DV]
    internal NString name = NString.NullValue;
    #endregion

    /// <summary>
    /// Determines whether the name is a valid InfoFieldType.
    /// </summary>
    private bool IsValidName(string name)
    {
      foreach (string validName in validNames)
      {
        if (String.Compare(validName, name, true) == 0)
          return true;
      }
      return false;
    }
    private static string[] validNames = Enum.GetNames(typeof(InfoFieldType));

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    public override bool IsNull()
    {
      return false;
    }
    #region Internal
    /// <summary>
    /// Converts InfoField into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      string str = "\\field(Info)";
      if (this.Name == "")
        throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "InfoField"));
      str += "[Name = \"" + this.Name + "\"]";

      serializer.Write(str);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(InfoField));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

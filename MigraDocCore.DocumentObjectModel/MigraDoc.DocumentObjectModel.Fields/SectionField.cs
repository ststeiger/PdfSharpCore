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

namespace MigraDocCore.DocumentObjectModel.Fields
{
  /// <summary>
  /// SectionField is used to reference the number of the current section.
  /// </summary>
  public class SectionField : NumericFieldBase
  {
    /// <summary>
    /// Initializes a new instance of the SectionField class.
    /// </summary>
    internal SectionField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SectionField class with the specified parent.
    /// </summary>
    internal SectionField(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new SectionField Clone()
    {
      return (SectionField)DeepCopy();
    }
    #endregion

    #region Internal
    /// <summary>
    /// Converts SectionField into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      string str = "\\field(Section)";

      if (this.format.Value != "")
        str += "[Format = \"" + this.Format + "\"]";
      else
        str += "[]"; //Has to be appended to avoid confusion with '[' in directly following text.

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
          meta = new Meta(typeof(SectionField));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

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
  /// A PageBreak is used to put following elements on a new page.
  /// </summary>
  public class PageBreak : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the PageBreak class.
    /// </summary>
    public PageBreak()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PageBreak class with the specified parent.
    /// </summary>
    internal PageBreak(DocumentObject parent) : base(parent) { }

    //#region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new PageBreak Clone()
    {
      return (PageBreak)DeepCopy();
    }
    //#endregion

    //#region Internal
    /// <summary>
    /// Converts PageBreak into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteLine("\\pagebreak");
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(PageBreak));
        return meta;
      }
    }
    static Meta meta;
    //#endregion
  }
}

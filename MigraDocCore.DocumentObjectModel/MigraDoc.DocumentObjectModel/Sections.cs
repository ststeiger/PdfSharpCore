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
using MigraDocCore.DocumentObjectModel.Visitors;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Represents the collection of document sections.
  /// </summary>
  public class Sections : DocumentObjectCollection, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the Sections class.
    /// </summary>
    public Sections()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Sections class with the specified parent.
    /// </summary>
    internal Sections(DocumentObject parent) : base(parent) { }

    /// <summary>
    /// Gets a section by its index. First section has index 0.
    /// </summary>
    public new Section this[int index]
    {
      get { return base[index] as Section; }
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Sections Clone()
    {
      return (Sections)DeepCopy();
    }

    /// <summary>
    /// Adds a new section.
    /// </summary>
    public Section AddSection()
    {
      Section section = new Section();
      this.Add(section);
      return section;
    }
    #endregion

    #region Internal
    /// <summary>
    /// Converts Sections into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int count = Count;
      for (int index = 0; index < count; ++index)
      {
        Section section = this[index];
        section.Serialize(serializer);
      }
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitSections(this);
      foreach (Section section in this)
        ((IVisitable)section).AcceptVisitor(visitor, visitChildren);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Sections));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

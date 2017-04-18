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
  /// Represents the collection of HeaderFooter objects.
  /// </summary>
  public class HeadersFooters : DocumentObject, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the HeadersFooters class.
    /// </summary>
    public HeadersFooters()
    {
    }

    /// <summary>
    /// Initializes a new instance of the HeadersFooters class with the specified parent.
    /// </summary>
    public HeadersFooters(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new HeadersFooters Clone()
    {
      return (HeadersFooters)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      HeadersFooters headersFooters = (HeadersFooters)base.DeepCopy();
      if (headersFooters.evenPage != null)
      {
        headersFooters.evenPage = headersFooters.evenPage.Clone();
        headersFooters.evenPage.parent = headersFooters;
      }
      if (headersFooters.firstPage != null)
      {
        headersFooters.firstPage = headersFooters.firstPage.Clone();
        headersFooters.firstPage.parent = headersFooters;
      }
      if (headersFooters.primary != null)
      {
        headersFooters.primary = headersFooters.primary.Clone();
        headersFooters.primary.parent = headersFooters;
      }
      return headersFooters;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Returns true if this collection contains headers, false otherwise.
    /// </summary>
    public bool IsHeader
    {
      get
      {
        Section sec = (Section)this.parent;
        return sec.headers == this;
      }
    }

    /// <summary>
    /// Returns true if this collection contains footers, false otherwise.
    /// </summary>
    public bool IsFooter
    {
      get { return !IsHeader; }
    }

    /// <summary>
    /// Determines whether a particular header or footer exists.
    /// </summary>
    public bool HasHeaderFooter(HeaderFooterIndex index)
    {
      return !this.IsNull(index.ToString());
    }

    /// <summary>
    /// Gets or sets the even page HeaderFooter of the HeadersFooters object.
    /// </summary>
    public HeaderFooter EvenPage
    {
      get
      {
        if (this.evenPage == null)
          this.evenPage = new HeaderFooter(this);

        return this.evenPage;
      }
      set
      {
        SetParent(value);
        this.evenPage = value;
      }
    }
    [DV]
    internal HeaderFooter evenPage;

    /// <summary>
    /// Gets or sets the first page HeaderFooter of the HeadersFooters object.
    /// </summary>
    public HeaderFooter FirstPage
    {
      get
      {
        if (this.firstPage == null)
          this.firstPage = new HeaderFooter(this);

        return this.firstPage;
      }
      set
      {
        SetParent(value);
        this.firstPage = value;
      }
    }
    [DV]
    internal HeaderFooter firstPage;

    /// <summary>
    /// Gets or sets the primary HeaderFooter of the HeadersFooters object.
    /// </summary>
    public HeaderFooter Primary
    {
      get
      {
        if (this.primary == null)
          this.primary = new HeaderFooter(this);

        return this.primary;
      }
      set
      {
        SetParent(value);
        this.primary = value;
      }
    }
    [DV]
    internal HeaderFooter primary;
    #endregion

    #region Internal
    /// <summary>
    /// Converts HeadersFooters into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      bool hasPrimary = HasHeaderFooter(HeaderFooterIndex.Primary);
      bool hasEvenPage = HasHeaderFooter(HeaderFooterIndex.EvenPage);
      bool hasFirstPage = HasHeaderFooter(HeaderFooterIndex.FirstPage);

      // \primary...
      if (hasPrimary)
        Primary.Serialize(serializer, "primary");

      // \even... 
      if (hasEvenPage)
        EvenPage.Serialize(serializer, "evenpage");

      // \firstpage...
      if (hasFirstPage)
        FirstPage.Serialize(serializer, "firstpage");
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitHeadersFooters(this);

      if (visitChildren)
      {
        if (HasHeaderFooter(HeaderFooterIndex.Primary))
          ((IVisitable)this.primary).AcceptVisitor(visitor, visitChildren);
        if (HasHeaderFooter(HeaderFooterIndex.EvenPage))
          ((IVisitable)this.evenPage).AcceptVisitor(visitor, visitChildren);
        if (HasHeaderFooter(HeaderFooterIndex.FirstPage))
          ((IVisitable)this.firstPage).AcceptVisitor(visitor, visitChildren);
      }
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(HeadersFooters));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

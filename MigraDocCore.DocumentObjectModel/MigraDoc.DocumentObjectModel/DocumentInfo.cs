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
  /// Contains information about document content, author etc.
  /// </summary>
  public class DocumentInfo : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the DocumentInfo class.
    /// </summary>
    public DocumentInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DocumentInfo class with the specified parent.
    /// </summary>
    internal DocumentInfo(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new DocumentInfo Clone()
    {
      return (DocumentInfo)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the document title.
    /// </summary>
    public string Title
    {
      get { return this.title.Value; }
      set { this.title.Value = value; }
    }
    [DV]
    internal NString title = NString.NullValue;

    /// <summary>
    /// Gets or sets the document author.
    /// </summary>
    public string Author
    {
      get { return this.author.Value; }
      set { this.author.Value = value; }
    }
    [DV]
    internal NString author = NString.NullValue;

    /// <summary>
    /// Gets or sets keywords related to the document.
    /// </summary>
    public string Keywords
    {
      get { return this.keywords.Value; }
      set { this.keywords.Value = value; }
    }
    [DV]
    internal NString keywords = NString.NullValue;

    /// <summary>
    /// Gets or sets the subject of the document.
    /// </summary>
    public string Subject
    {
      get { return this.subject.Value; }
      set { this.subject.Value = value; }
    }
    [DV]
    internal NString subject = NString.NullValue;

    /// <summary>
    /// Gets or sets a comment associated with this object.
    /// </summary>
    public string Comment
    {
      get { return this.comment.Value; }
      set { this.comment.Value = value; }
    }
    [DV]
    internal NString comment = NString.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts DocumentInfo into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteComment(this.comment.Value);
      int pos = serializer.BeginContent("Info");

      if (this.Title != String.Empty)
        serializer.WriteSimpleAttribute("Title", this.Title);

      if (this.Subject != String.Empty)
        serializer.WriteSimpleAttribute("Subject", this.Subject);

      if (this.Author != String.Empty)
        serializer.WriteSimpleAttribute("Author", this.Author);

      if (this.Keywords != String.Empty)
        serializer.WriteSimpleAttribute("Keywords", this.Keywords);

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
          meta = new Meta(typeof(DocumentInfo));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

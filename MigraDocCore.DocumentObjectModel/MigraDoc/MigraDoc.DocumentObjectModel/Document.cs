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
//using System.Drawing.Text;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.IO;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Represents a MigraDoc document.
  /// </summary>
  public sealed class Document : DocumentObject, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the Document class.
    /// </summary>
    public Document()
    {
      this.styles = new Styles(this);
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Document Clone()
    {
      return (Document)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Document document = (Document)base.DeepCopy();
      if (document.info != null)
      {
        document.info = document.info.Clone();
        document.info.parent = document;
      }
      if (document.styles != null)
      {
        document.styles = document.styles.Clone();
        document.styles.parent = document;
      }
      if (document.sections != null)
      {
        document.sections = document.sections.Clone();
        document.sections.parent = document;
      }
      return document;
    }

    /// <summary>
    /// Internal function used by renderers to bind this instance to it. 
    /// </summary>
    public void BindToRenderer(object renderer)
    {
      //if (this.renderer != null && this.renderer != renderer)
      if (this.renderer != null && renderer != null && !Object.ReferenceEquals(this.renderer, renderer))
      {
        throw new InvalidOperationException("The document is already bound to another renderer. " +
          "A MigraDoc document can be rendered by only one renderer, because the rendering process " +
          "modifies its internal structure. If you want to render a MigraDoc document  on different renderers, " +
          "you must create a copy of it using the Clone function.");
      }
      this.renderer = renderer;
    }
    object renderer;

    /// <summary>
    /// Indicates whether the document is bound to a renderer. A bound document must not be modified anymore.
    /// Modifying it leads to undefined results of the rendering process.
    /// </summary>
    public bool IsBoundToRenderer
    {
      get { return this.renderer != null; }
    }

    /// <summary>
    /// Adds a new section to the document.
    /// </summary>
    public Section AddSection()
    {
      return this.Sections.AddSection();
    }

    /// <summary>
    /// Adds a new style to the document styles.
    /// </summary>
    /// <param name="name">Name of the style.</param>
    /// <param name="baseStyle">Name of the base style.</param>
    public Style AddStyle(string name, string baseStyle)
    {
      if (name == null || baseStyle == null)
        throw new ArgumentNullException(name == null ? "name" : "baseStyle");
      if (name == "" || baseStyle == "")
        throw new ArgumentException(name == "" ? "name" : "baseStyle");

      return this.Styles.AddStyle(name, baseStyle);
    }

    /// <summary>
    /// Adds a new section to the document.
    /// </summary>
    public void Add(Section section)
    {
      this.Sections.Add(section);
    }

    /// <summary>
    /// Adds a new style to the document styles.
    /// </summary>
    public void Add(Style style)
    {
      this.Styles.Add(style);
    }
    #endregion

    #region Properties

    /// <summary>
    /// Gets the last section of the document, or null, if the document has no sections.
    /// </summary>
    public Section LastSection
    {
      get
      {
        return (this.sections != null && this.sections.Count > 0) ?
          this.sections.LastObject as Section : null;
      }
    }

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

    /// <summary>
    /// Gets the document info.
    /// </summary>
    public DocumentInfo Info
    {
      get
      {
        if (this.info == null)
          this.info = new DocumentInfo(this);

        return info;
      }
      set
      {
        SetParent(value);
        this.info = value;
      }
    }
    [DV]
    internal DocumentInfo info;

    /// <summary>
    /// Gets or sets the styles of the document.
    /// </summary>
    public Styles Styles
    {
      get
      {
        if (this.styles == null)
          this.styles = new Styles(this);

        return this.styles;
      }
      set
      {
        SetParent(value);
        this.styles = value;
      }
    }
    [DV]
    internal Styles styles;

    /// <summary>
    /// Gets or sets the default tab stop position.
    /// </summary>
    public Unit DefaultTabStop
    {
      get { return this.defaultTabStop; }
      set { this.defaultTabStop = value; }
    }
    [DV]
    internal Unit defaultTabStop = Unit.NullValue;

    /// <summary>
    /// Gets the default page setup.
    /// </summary>
    public PageSetup DefaultPageSetup
    {
      get { return PageSetup.DefaultPageSetup; }
    }

    /// <summary>
    /// Gets or sets the location of the Footnote.
    /// </summary>
    public FootnoteLocation FootnoteLocation
    {
      get { return (FootnoteLocation)this.footnoteLocation.Value; }
      set { this.footnoteLocation.Value = (int)value; }
    }
    [DV(Type = typeof(FootnoteLocation))]
    internal NEnum footnoteLocation = NEnum.NullValue(typeof(FootnoteLocation));

    /// <summary>
    /// Gets or sets the rule which is used to determine the footnote number on a new page.
    /// </summary>
    public FootnoteNumberingRule FootnoteNumberingRule
    {
      get { return (FootnoteNumberingRule)this.footnoteNumberingRule.Value; }
      set { this.footnoteNumberingRule.Value = (int)value; }
    }
    [DV(Type = typeof(FootnoteNumberingRule))]
    internal NEnum footnoteNumberingRule = NEnum.NullValue(typeof(FootnoteNumberingRule));

    /// <summary>
    /// Gets or sets the type of number which is used for the footnote.
    /// </summary>
    public FootnoteNumberStyle FootnoteNumberStyle
    {
      get { return (FootnoteNumberStyle)this.footnoteNumberStyle.Value; }
      set { this.footnoteNumberStyle.Value = (int)value; }
    }
    [DV(Type = typeof(FootnoteNumberStyle))]
    internal NEnum footnoteNumberStyle = NEnum.NullValue(typeof(FootnoteNumberStyle));

    /// <summary>
    /// Gets or sets the starting number of the footnote.
    /// </summary>
    public int FootnoteStartingNumber
    {
      get { return this.footnoteStartingNumber.Value; }
      set { this.footnoteStartingNumber.Value = value; }
    }
    [DV]
    internal NInt footnoteStartingNumber = NInt.NullValue;

    /// <summary>
    /// Gets or sets the path for images used by the document.
    /// </summary>
    public string ImagePath
    {
      get { return this.imagePath.Value; }
      set { this.imagePath.Value = value; }
    }
    [DV]
    internal NString imagePath = NString.NullValue;

    /// <summary>
    /// Gets or sets a value indicating whether to use the CMYK color model when rendered as PDF.
    /// </summary>
    public bool UseCmykColor
    {
      get { return this.useCmykColor.Value; }
      set { this.useCmykColor.Value = value; }
    }
    [DV]
    internal NBool useCmykColor = NBool.NullValue;

    /// <summary>
    /// Gets the sections of the document.
    /// </summary>
    public Sections Sections
    {
      get
      {
        if (this.sections == null)
          this.sections = new Sections(this);
        return this.sections;
      }
      set
      {
        SetParent(value);
        this.sections = value;
      }
    }
    [DV]
    internal Sections sections;
    #endregion

    /// <summary>
    /// Gets the DDL file name.
    /// </summary>
    public string DdlFile
    {
      get { return this.ddlFile; }
    }
    internal string ddlFile = "";

    #region Internal
    /// <summary>
    /// Converts Document into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteComment(this.comment.Value);
      serializer.WriteLine("\\document");

      int pos = serializer.BeginAttributes();
      if (!this.IsNull("Info"))
        this.Info.Serialize(serializer);
      if (!this.defaultTabStop.IsNull)
        serializer.WriteSimpleAttribute("DefaultTabStop", DefaultTabStop);
      if (!this.footnoteLocation.IsNull)
        serializer.WriteSimpleAttribute("FootnoteLocation", FootnoteLocation);
      if (!this.footnoteNumberingRule.IsNull)
        serializer.WriteSimpleAttribute("FootnoteNumberingRule", FootnoteNumberingRule);
      if (!this.footnoteNumberStyle.IsNull)
        serializer.WriteSimpleAttribute("FootnoteNumberStyle", FootnoteNumberStyle);
      if (!this.footnoteStartingNumber.IsNull)
        serializer.WriteSimpleAttribute("FootnoteStartingNumber", FootnoteStartingNumber);
      if (!this.imagePath.IsNull)
        serializer.WriteSimpleAttribute("ImagePath", ImagePath);
      if (!this.useCmykColor.IsNull)
        serializer.WriteSimpleAttribute("UseCmykColor", UseCmykColor);
      serializer.EndAttributes(pos);

      serializer.BeginContent();
      Styles.Serialize(serializer);

      if (!this.IsNull("Sections"))
        Sections.Serialize(serializer);
      serializer.EndContent();
      serializer.Flush();
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and all it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitDocument(this);
      if (visitChildren)
      {
        ((IVisitable)Styles).AcceptVisitor(visitor, visitChildren);
        ((IVisitable)Sections).AcceptVisitor(visitor, visitChildren);
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
          meta = new Meta(typeof(Document));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

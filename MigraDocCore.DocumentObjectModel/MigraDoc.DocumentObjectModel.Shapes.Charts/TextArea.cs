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
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// An area object in the chart which contain text or legend.
  /// </summary>
  public class TextArea : ChartObject, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the TextArea class.
    /// </summary>
    internal TextArea()
    {
    }

    /// <summary>
    /// Initializes a new instance of the TextArea class with the specified parent.
    /// </summary>
    internal TextArea(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new TextArea Clone()
    {
      return (TextArea)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      TextArea textArea = (TextArea)base.DeepCopy();
      if (textArea.format != null)
      {
        textArea.format = textArea.format.Clone();
        textArea.format.parent = textArea;
      }
      if (textArea.lineFormat != null)
      {
        textArea.lineFormat = textArea.lineFormat.Clone();
        textArea.lineFormat.parent = textArea;
      }
      if (textArea.fillFormat != null)
      {
        textArea.fillFormat = textArea.fillFormat.Clone();
        textArea.fillFormat.parent = textArea;
      }
      if (textArea.elements != null)
      {
        textArea.elements = textArea.elements.Clone();
        textArea.elements.parent = textArea;
      }
      return textArea;
    }

    /// <summary>
    /// Adds a new paragraph to the text area.
    /// </summary>
    public Paragraph AddParagraph()
    {
      return this.Elements.AddParagraph();
    }

    /// <summary>
    /// Adds a new paragraph with the specified text to the text area.
    /// </summary>
    public Paragraph AddParagraph(string paragraphText)
    {
      return this.Elements.AddParagraph(paragraphText);
    }

    /// <summary>
    /// Adds a new table to the text area.
    /// </summary>
    public Table AddTable()
    {
      return this.Elements.AddTable();
    }

    /// <summary>
    /// Adds a new Image to the text area.
    /// </summary>
    public Image AddImage(IImageSource imageSource)
    {
      return this.Elements.AddImage(imageSource);
    }

    /// <summary>
    /// Adds a new legend to the text area.
    /// </summary>
    public Legend AddLegend()
    {
      return this.Elements.AddLegend();
    }

    /// <summary>
    /// Adds a new paragraph to the text area.
    /// </summary>
    public void Add(Paragraph paragraph)
    {
      this.Elements.Add(paragraph);
    }

    /// <summary>
    /// Adds a new table to the text area.
    /// </summary>
    public void Add(Table table)
    {
      this.Elements.Add(table);
    }

    /// <summary>
    /// Adds a new image to the text area.
    /// </summary>
    public void Add(Image image)
    {
      this.Elements.Add(image);
    }

    /// <summary>
    /// Adds a new legend to the text area.
    /// </summary>
    public void Add(Legend legend)
    {
      this.Elements.Add(legend);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the height of the area.
    /// </summary>
    public Unit Height
    {
      get { return this.height; }
      set { this.height = value; }
    }
    [DV]
    internal Unit height = Unit.NullValue;

    /// <summary>
    /// Gets or sets the width of the area.
    /// </summary>
    public Unit Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    [DV]
    internal Unit width = Unit.NullValue;

    /// <summary>
    /// Gets or sets the default style name of the area.
    /// </summary>
    public string Style
    {
      get { return this.style.Value; }
      set { this.style.Value = value; }
    }
    [DV]
    internal NString style = NString.NullValue;

    /// <summary>
    /// Gets or sets the default paragraph format of the area.
    /// </summary>
    public ParagraphFormat Format
    {
      get
      {
        if (this.format == null)
          this.format = new ParagraphFormat(this);

        return this.format;
      }
      set
      {
        SetParent(value);
        this.format = value;
      }
    }
    [DV]
    internal ParagraphFormat format;

    /// <summary>
    /// Gets the line format of the area's border.
    /// </summary>
    public LineFormat LineFormat
    {
      get
      {
        if (this.lineFormat == null)
          this.lineFormat = new LineFormat(this);

        return this.lineFormat;
      }
      set
      {
        SetParent(value);
        this.lineFormat = value;
      }
    }
    [DV]
    internal LineFormat lineFormat;

    /// <summary>
    /// Gets the background filling of the area.
    /// </summary>
    public FillFormat FillFormat
    {
      get
      {
        if (this.fillFormat == null)
          this.fillFormat = new FillFormat(this);

        return this.fillFormat;
      }
      set
      {
        SetParent(value);
        this.fillFormat = value;
      }
    }
    [DV]
    internal FillFormat fillFormat;

    /// <summary>
    /// Gets or sets the left padding of the area.
    /// </summary>
    public Unit LeftPadding
    {
      get { return this.leftPadding; }
      set { this.leftPadding = value; }
    }
    [DV]
    internal Unit leftPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the right padding of the area.
    /// </summary>
    public Unit RightPadding
    {
      get { return this.rightPadding; }
      set { this.rightPadding = value; }
    }
    [DV]
    internal Unit rightPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the top padding of the area.
    /// </summary>
    public Unit TopPadding
    {
      get { return this.topPadding; }
      set { this.topPadding = value; }
    }
    [DV]
    internal Unit topPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the bottom padding of the area.
    /// </summary>
    public Unit BottomPadding
    {
      get { return this.bottomPadding; }
      set { this.bottomPadding = value; }
    }
    [DV]
    internal Unit bottomPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the Vertical alignment of the area.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
      get { return (VerticalAlignment)this.verticalAlignment.Value; }
      set { this.verticalAlignment.Value = (int)value; }
    }
    [DV(Type = typeof(VerticalAlignment))]
    internal NEnum verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));

    /// <summary>
    /// Gets the document objects that creates the text area.
    /// </summary>
    public DocumentElements Elements
    {
      get
      {
        if (this.elements == null)
          this.elements = new DocumentElements(this);

        return this.elements;
      }
      set
      {
        SetParent(value);
        this.elements = value;
      }
    }
    [DV(ItemType = typeof(DocumentObject))]
    internal DocumentElements elements;
    #endregion

    #region Internal
    /// <summary>
    /// Converts TextArea into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      Chart chartObject = this.parent as Chart;

      serializer.WriteLine("\\" + chartObject.CheckTextArea(this));
      int pos = serializer.BeginAttributes();

      if (!this.style.IsNull)
        serializer.WriteSimpleAttribute("Style", this.Style);
      if (!this.IsNull("Format"))
        this.format.Serialize(serializer, "Format", null);

      if (!this.topPadding.IsNull)
        serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);
      if (!this.leftPadding.IsNull)
        serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);
      if (!this.rightPadding.IsNull)
        serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);
      if (!this.bottomPadding.IsNull)
        serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);

      if (!this.width.IsNull)
        serializer.WriteSimpleAttribute("Width", this.Width);
      if (!this.height.IsNull)
        serializer.WriteSimpleAttribute("Height", this.Height);

      if (!this.verticalAlignment.IsNull)
        serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);

      if (!this.IsNull("LineFormat"))
        this.lineFormat.Serialize(serializer);
      if (!this.IsNull("FillFormat"))
        this.fillFormat.Serialize(serializer);

      serializer.EndAttributes(pos);

      serializer.BeginContent();
      if (this.elements != null)
        this.elements.Serialize(serializer);
      serializer.EndContent();
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(TextArea));
        return meta;
      }
    }
    static Meta meta;
    #endregion

    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitTextArea(this);
      if (this.elements != null && visitChildren)
        ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
    }
  }
}

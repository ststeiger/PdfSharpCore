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
using MigraDocCore.DocumentObjectModel.Fields;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// A Hyperlink is used to reference targets in the document (Local), on a drive (File) or a network (Web).
    /// </summary>
    public class Hyperlink : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Hyperlink class.
        /// </summary>
        public Hyperlink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Hyperlink class with the specified parent.
        /// </summary>
        internal Hyperlink(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Hyperlink class with the text the hyperlink shall content.
        /// The type will be treated as Local by default.
        /// </summary>
        internal Hyperlink(string name, string text)
          : this()
        {
            this.Name = name;
            this.Elements.AddText(text);
        }

        /// <summary>
        /// Initializes a new instance of the Hyperlink class with the type and text the hyperlink shall
        /// represent.
        /// </summary>
        internal Hyperlink(string name, HyperlinkType type, string text)
          : this()
        {
            this.Name = name;
            this.Type = type;
            this.Elements.AddText(text);
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Hyperlink Clone()
        {
            return (Hyperlink)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Hyperlink hyperlink = (Hyperlink)base.DeepCopy();
            if (hyperlink.elements != null)
            {
                hyperlink.elements = hyperlink.elements.Clone();
                hyperlink.elements.parent = hyperlink;
            }
            return hyperlink;
        }

        /// <summary>
        /// Adds a text phrase to the hyperlink.
        /// </summary>
        public Text AddText(String text)
        {
            return this.Elements.AddText(text);
        }

        /// <summary>
        /// Adds a single character repeated the specified number of times to the hyperlink.
        /// </summary>
        public Text AddChar(char ch, int count)
        {
            return this.Elements.AddChar(ch, count);
        }

        /// <summary>
        /// Adds a single character to the hyperlink.
        /// </summary>
        public Text AddChar(char ch)
        {
            return this.Elements.AddChar(ch);
        }

        /// <summary>
        /// Adds one or more Symbol objects.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType, int count)
        {
            return this.Elements.AddCharacter(symbolType, count);
        }

        /// <summary>
        /// Adds a Symbol object.
        /// </summary>
        public Character AddCharacter(SymbolName symbolType)
        {
            return this.Elements.AddCharacter(symbolType);
        }

        /// <summary>
        /// Adds one or more Symbol objects defined by a character.
        /// </summary>
        public Character AddCharacter(char ch, int count)
        {
            return this.Elements.AddCharacter(ch, count);
        }

        /// <summary>
        /// Adds a Symbol object defined by a character.
        /// </summary>
        public Character AddCharacter(char ch)
        {
            return this.Elements.AddCharacter(ch);
        }

        /// <summary>
        /// Adds a space character as many as count.
        /// </summary>
        public Character AddSpace(int count)
        {
            return this.Elements.AddSpace(count);
        }

        /// <summary>
        /// Adds a horizontal tab.
        /// </summary>
        public void AddTab()
        {
            this.Elements.AddTab();
        }

        /// <summary>
        /// Adds a new FormattedText.
        /// </summary>
        public FormattedText AddFormattedText()
        {
            return this.Elements.AddFormattedText();
        }

        /// <summary>
        /// Adds a new FormattedText object with the given format.
        /// </summary>
        public FormattedText AddFormattedText(TextFormat textFormat)
        {
            return this.Elements.AddFormattedText(textFormat);
        }

        /// <summary>
        /// Adds a new FormattedText with the given Font.
        /// </summary>
        public FormattedText AddFormattedText(Font font)
        {
            return this.Elements.AddFormattedText(font);
        }

        /// <summary>
        /// Adds a new FormattedText with the given text.
        /// </summary>
        public FormattedText AddFormattedText(string text)
        {
            return this.Elements.AddFormattedText(text);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and format.
        /// </summary>
        public FormattedText AddFormattedText(string text, TextFormat textFormat)
        {
            return this.Elements.AddFormattedText(text, textFormat);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and font.
        /// </summary>
        public FormattedText AddFormattedText(string text, Font font)
        {
            return this.Elements.AddFormattedText(text, font);
        }

        /// <summary>
        /// Adds a new FormattedText object with the given text and style.
        /// </summary>
        public FormattedText AddFormattedText(string text, string style)
        {
            return this.Elements.AddFormattedText(text, style);
        }

        /// <summary>
        /// Adds a new Bookmark.
        /// </summary>
        public BookmarkField AddBookmark(string name)
        {
            return this.Elements.AddBookmark(name);
        }

        /// <summary>
        /// Adds a new PageField.
        /// </summary>
        public PageField AddPageField()
        {
            return this.Elements.AddPageField();
        }

        /// <summary>
        /// Adds a new PageRefField.
        /// </summary>
        public PageRefField AddPageRefField(string name)
        {
            return this.Elements.AddPageRefField(name);
        }

        /// <summary>
        /// Adds a new NumPagesField.
        /// </summary>
        public NumPagesField AddNumPagesField()
        {
            return this.Elements.AddNumPagesField();
        }

        /// <summary>
        /// Adds a new SectionField.
        /// </summary>
        public SectionField AddSectionField()
        {
            return this.Elements.AddSectionField();
        }

        /// <summary>
        /// Adds a new SectionPagesField.
        /// </summary>
        public SectionPagesField AddSectionPagesField()
        {
            return this.Elements.AddSectionPagesField();
        }

        /// <summary>
        /// Adds a new DateField.
        /// </summary>
        public DateField AddDateField()
        {
            return this.Elements.AddDateField();
        }

        /// <summary>
        /// Adds a new DateField.
        /// </summary>
        public DateField AddDateField(string format)
        {
            return this.Elements.AddDateField(format);
        }

        /// <summary>
        /// Adds a new InfoField.
        /// </summary>
        public InfoField AddInfoField(InfoFieldType iType)
        {
            return this.Elements.AddInfoField(iType);
        }

        /// <summary>
        /// Adds a new Footnote with the specified text.
        /// </summary>
        public Footnote AddFootnote(string text)
        {
            return this.Elements.AddFootnote(text);
        }

        /// <summary>
        /// Adds a new Footnote.
        /// </summary>
        public Footnote AddFootnote()
        {
            return this.Elements.AddFootnote();
        }

        /// <summary>
        /// Adds a new Image object
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }
        /// <summary>
        /// Adds a new Bookmark
        /// </summary>
        public void Add(BookmarkField bookmark)
        {
            this.Elements.Add(bookmark);
        }

        /// <summary>
        /// Adds a new PageField
        /// </summary>
        public void Add(PageField pageField)
        {
            this.Elements.Add(pageField);
        }

        /// <summary>
        /// Adds a new PageRefField
        /// </summary>
        public void Add(PageRefField pageRefField)
        {
            this.Elements.Add(pageRefField);
        }

        /// <summary>
        /// Adds a new NumPagesField
        /// </summary>
        public void Add(NumPagesField numPagesField)
        {
            this.Elements.Add(numPagesField);
        }

        /// <summary>
        /// Adds a new SectionField
        /// </summary>
        public void Add(SectionField sectionField)
        {
            this.Elements.Add(sectionField);
        }

        /// <summary>
        /// Adds a new SectionPagesField
        /// </summary>
        public void Add(SectionPagesField sectionPagesField)
        {
            this.Elements.Add(sectionPagesField);
        }

        /// <summary>
        /// Adds a new DateField
        /// </summary>
        public void Add(DateField dateField)
        {
            this.Elements.Add(dateField);
        }

        /// <summary>
        /// Adds a new InfoField
        /// </summary>
        public void Add(InfoField infoField)
        {
            this.Elements.Add(infoField);
        }

        /// <summary>
        /// Adds a new Footnote
        /// </summary>
        public void Add(Footnote footnote)
        {
            this.Elements.Add(footnote);
        }

        /// <summary>
        /// Adds a new Text
        /// </summary>
        public void Add(Text text)
        {
            this.Elements.Add(text);
        }

        /// <summary>
        /// Adds a new FormattedText
        /// </summary>
        public void Add(FormattedText formattedText)
        {
            this.Elements.Add(formattedText);
        }

        /// <summary>
        /// Adds a new Image
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        /// <summary>
        /// Adds a new Character
        /// </summary>
        public void Add(Character character)
        {
            this.Elements.Add(character);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the font object.
        /// </summary>
        public Font Font
        {
            get
            {
                if (this.font == null)
                    this.font = new Font(this);

                return this.font;
            }
            set
            {
                SetParent(value);
                this.font = value;
            }
        }
        [DV]
        internal Font font;

        /// <summary>
        /// Gets or sets the target name of the Hyperlink, e.g. an URL or a bookmark's name.
        /// </summary>
        public string Name
        {
            get { return this.name.Value; }
            set { this.name.Value = value; }
        }
        [DV]
        internal NString name = NString.NullValue;

        /// <summary>
        /// Gets or sets the target type of the Hyperlink.
        /// </summary>
        public HyperlinkType Type
        {
            get { return (HyperlinkType)this.type.Value; }
            set { this.type.Value = (int)value; }
        }
        [DV(Type = typeof(HyperlinkType))]
        internal NEnum type = NEnum.NullValue(typeof(HyperlinkType));

        /// <summary>
        /// Gets the ParagraphElements of the Hyperlink specifying its 'clickable area'.
        /// </summary>
        public ParagraphElements Elements
        {
            get
            {
                if (this.elements == null)
                    this.elements = new ParagraphElements(this);

                return this.elements;
            }
            set
            {
                SetParent(value);
                this.elements = value;
            }
        }
        [DV(ItemType = typeof(DocumentObject))]
        internal ParagraphElements elements;
        #endregion

        #region Internal
        /// <summary>
        /// Converts Hyperlink into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (this.name.Value == string.Empty)
                throw new InvalidOperationException(DomSR.MissingObligatoryProperty("Name", "Hyperlink"));
            serializer.Write("\\hyperlink");
            string str = "[Name = \"" + this.Name.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
            if (!this.type.IsNull)
                str += " Type = " + this.Type;
            str += "]";
            serializer.Write(str);
            serializer.Write("{");
            if (this.elements != null)
                elements.Serialize(serializer);
            serializer.Write("}");
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get
            {
                if (meta == null)
                    meta = new Meta(typeof(Hyperlink));
                return meta;
            }
        }
        static Meta meta;
        #endregion

        #region IDomVisitable Members
        public void AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHyperlink(this);
            if (visitChildren && this.elements != null)
            {
                ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
            }
        }
        #endregion
    }
}

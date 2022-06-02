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
using System.Collections;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Fields;
using MigraDocCore.DocumentObjectModel.Shapes;
using System.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Represents a paragraph which is used to build up a document with text.
    /// </summary>
    public class Paragraph : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Paragraph class.
        /// </summary>
        public Paragraph()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Paragraph class with the specified parent.
        /// </summary>
        internal Paragraph(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Paragraph Clone()
        {
            return (Paragraph)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Paragraph paragraph = (Paragraph)base.DeepCopy();
            if (paragraph.format != null)
            {
                paragraph.format = paragraph.format.Clone();
                paragraph.format.parent = paragraph;
            }
            if (paragraph.elements != null)
            {
                paragraph.elements = paragraph.elements.Clone();
                paragraph.elements.parent = paragraph;
            }
            return paragraph;
        }

        /// <summary>
        /// Adds a text phrase to the paragraph.
        /// </summary>
        public Text AddText(String text)
        {
            return this.Elements.AddText(text);
        }

        /// <summary>
        /// Adds a single character repeated the specified number of times to the paragraph.
        /// </summary>
        public Text AddChar(char ch, int count)
        {
            return this.Elements.AddChar(ch, count);
        }

        /// <summary>
        /// Adds a single character to the paragraph.
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
        /// Adds a line break.
        /// </summary>
        public void AddLineBreak()
        {
            this.Elements.AddLineBreak();
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
        /// Adds a new Hyperlink of Type "Local", 
        /// i.e. the Target is a Bookmark within the Document
        /// </summary>
        public Hyperlink AddHyperlink(string name)
        {
            return this.Elements.AddHyperlink(name);
        }

        /// <summary>
        /// Adds a new Hyperlink
        /// </summary>
        public Hyperlink AddHyperlink(string name, HyperlinkType type)
        {
            return this.Elements.AddHyperlink(name, type);
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
        /// Adds a new Hyperlink
        /// </summary>
        public void Add(Hyperlink hyperlink)
        {
            this.Elements.Add(hyperlink);
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
        /// Gets or sets the style name.
        /// </summary>
        public string Style
        {
            get { return this.style.Value; }
            set { this.style.Value = value; }
        }
        [DV]
        internal NString style = NString.NullValue;

        /// <summary>
        /// Gets or sets the ParagraphFormat object of the paragraph.
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
        /// Gets the collection of document objects that defines the paragraph.
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
        [DV]
        internal ParagraphElements elements;

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
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitParagraph(this);

            if (visitChildren && this.elements != null)
                ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        internal bool SerializeContentOnly
        {
            get { return serializeContentOnly; }
            set { serializeContentOnly = value; }
        }
        bool serializeContentOnly = false;

        /// <summary>
        /// Converts Paragraph into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            if (!serializeContentOnly)
            {
                serializer.WriteComment(this.comment.Value);
                serializer.WriteLine("\\paragraph");

                int pos = serializer.BeginAttributes();

                if (this.style.Value != "")
                    serializer.WriteLine("Style = \"" + this.style.Value + "\"");

                if (!this.IsNull("Format"))
                    this.format.Serialize(serializer, "Format", null);

                serializer.EndAttributes(pos);

                serializer.BeginContent();
                if (!this.IsNull("Elements"))
                    this.Elements.Serialize(serializer);
                serializer.CloseUpLine();
                serializer.EndContent();
            }
            else
            {
                this.Elements.Serialize(serializer);
                serializer.CloseUpLine();
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
                    meta = new Meta(typeof(Paragraph));
                return meta;
            }
        }

        /// <summary>
        /// Returns an array of Paragraphs that are separated by parabreaks. Null if no parabreak is found.
        /// </summary>
        internal Paragraph[] SplitOnParaBreak()
        {
            if (this.elements == null)
                return null;

            int startIdx = 0;
            ArrayList paragraphs = new ArrayList();
            for (int idx = 0; idx < this.Elements.Count; ++idx)
            {
                DocumentObject element = this.Elements[idx];
                if (element is Character)
                {
                    Character character = (Character)element;
                    if (character.SymbolName == SymbolName.ParaBreak)
                    {
                        Paragraph paragraph = new Paragraph();
                        paragraph.Format = this.Format.Clone();
                        paragraph.Style = this.Style;
                        paragraph.Elements = SubsetElements(startIdx, idx - 1);
                        startIdx = idx + 1;
                        paragraphs.Add(paragraph);
                    }
                }
            }
            if (startIdx == 0) //No paragraph breaks given.
                return null;
            else
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Format = this.Format.Clone();
                paragraph.Style = this.Style;
                paragraph.Elements = SubsetElements(startIdx, this.elements.Count - 1);
                paragraphs.Add(paragraph);

                return (Paragraph[])paragraphs.ToArray(typeof(Paragraph));
            }
        }

        /// <summary>
        /// Gets a subset of the paragraphs elements.
        /// </summary>
        /// <param name="startIdx">Start index of the required subset.</param>
        /// <param name="endIdx">End index of the required subset.</param>
        /// <returns>A ParagraphElements object with cloned elements.</returns>
        private ParagraphElements SubsetElements(int startIdx, int endIdx)
        {
            ParagraphElements paragraphElements = new ParagraphElements();
            for (int idx = startIdx; idx <= endIdx; ++idx)
            {
                paragraphElements.Add((DocumentObject)this.elements[idx].Clone());
            }
            return paragraphElements;
        }
        static Meta meta;
        #endregion
    }
}

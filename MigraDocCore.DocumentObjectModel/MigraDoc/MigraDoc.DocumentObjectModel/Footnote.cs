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
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Represents a footnote in a paragraph.
    /// </summary>
    public class Footnote : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Footnote class.
        /// </summary>
        public Footnote()
        {
            //NYI: Nested footnote check!
        }

        /// <summary>
        /// Initializes a new instance of the Footnote class with the specified parent.
        /// </summary>
        internal Footnote(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Footnote class with a text the Footnote shall content.
        /// </summary>
        internal Footnote(string content)
          : this()
        {
            this.Elements.AddParagraph(content);
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Footnote Clone()
        {
            return (Footnote)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Footnote footnote = (Footnote)base.DeepCopy();
            if (footnote.elements != null)
            {
                footnote.elements = footnote.elements.Clone();
                footnote.elements.parent = footnote;
            }
            if (footnote.format != null)
            {
                footnote.format = footnote.format.Clone();
                footnote.format.parent = footnote;
            }
            return footnote;
        }

        /// <summary>
        /// Adds a new paragraph to the footnote.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return this.Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the footnote.
        /// </summary>
        public Paragraph AddParagraph(string text)
        {
            return this.Elements.AddParagraph(text);
        }

        /// <summary>
        /// Adds a new table to the footnote.
        /// </summary>
        public Table AddTable()
        {
            return this.Elements.AddTable();
        }

        /// <summary>
        /// Adds a new image to the footnote.
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }

        /// <summary>
        /// Adds a new paragraph to the footnote.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new table to the footnote.
        /// </summary>
        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the footnote.
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the collection of paragraph elements that defines the footnote.
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

        /// <summary>
        /// Gets or sets the character to be used to mark the footnote.
        /// </summary>
        public string Reference
        {
            get { return this.reference.Value; }
            set { this.reference.Value = value; }
        }
        [DV]
        internal NString reference = NString.NullValue;

        /// <summary>
        /// Gets or sets the style name of the footnote.
        /// </summary>
        public string Style
        {
            get { return this.style.Value; }
            set { this.style.Value = value; }
        }
        [DV]
        internal NString style = NString.NullValue;

        /// <summary>
        /// Gets the format of the footnote.
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
        #endregion

        #region Internal
        /// <summary>
        /// Converts Footnote into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\footnote");

            int pos = serializer.BeginAttributes();
            if (this.reference.Value != string.Empty)
                serializer.WriteSimpleAttribute("Reference", this.Reference);
            if (this.style.Value != string.Empty)
                serializer.WriteSimpleAttribute("Style", this.Style);

            if (!this.IsNull("Format"))
                this.format.Serialize(serializer, "Format", null);

            serializer.EndAttributes(pos);

            pos = serializer.BeginContent();
            if (!this.IsNull("Elements"))
                this.elements.Serialize(serializer);
            serializer.EndContent(pos);
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitFootnote(this);

            if (visitChildren && this.elements != null)
                ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get
            {
                if (meta == null)
                    meta = new Meta(typeof(Footnote));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}

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
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Represents a header or footer object in a section.
    /// </summary>
    public class HeaderFooter : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the HeaderFooter class.
        /// </summary>
        public HeaderFooter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HeaderFooter class with the specified parent.
        /// </summary>
        internal HeaderFooter(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new HeaderFooter Clone()
        {
            return (HeaderFooter)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            HeaderFooter headerFooter = (HeaderFooter)base.DeepCopy();
            if (headerFooter.format != null)
            {
                headerFooter.format = headerFooter.format.Clone();
                headerFooter.format.parent = headerFooter;
            }
            if (headerFooter.elements != null)
            {
                headerFooter.elements = headerFooter.elements.Clone();
                headerFooter.elements.parent = headerFooter;
            }
            return headerFooter;
        }

        /// <summary>
        /// Adds a new paragraph to the header or footer.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return this.Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the header or footer.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return this.Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the header or footer.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return this.Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the header or footer.
        /// </summary>
        public Chart AddChart()
        {
            return this.Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the header or footer.
        /// </summary>
        public Table AddTable()
        {
            return this.Elements.AddTable();
        }

        /// <summary>
        /// Adds a new Image to the header or footer.
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }

        /// <summary>
        /// Adds a new textframe to the header or footer.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return this.Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the header or footer.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the header or footer.
        /// </summary>
        public void Add(Chart chart)
        {
            this.Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the header or footer.
        /// </summary>
        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the header or footer.
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the header or footer.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            this.Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns true if this is a headers, false otherwise.
        /// </summary>
        public bool IsHeader
        {
            get { return ((HeadersFooters)parent).IsHeader; }
        }

        /// <summary>
        /// Returns true if this is a footer, false otherwise.
        /// </summary>
        public bool IsFooter
        {
            get { return ((HeadersFooters)parent).IsFooter; }
        }

        /// <summary>
        /// Returns true if this is a first page header or footer, false otherwise.
        /// </summary>
        public bool IsFirstPage
        {
            get { return ((HeadersFooters)this.parent).firstPage == this; }
        }

        /// <summary>
        /// Returns true if this is an even page header or footer, false otherwise.
        /// </summary>
        public bool IsEvenPage
        {
            get { return ((HeadersFooters)this.parent).evenPage == this; }
        }

        /// <summary>
        /// Returns true if this is a primary header or footer, false otherwise.
        /// </summary>
        public bool IsPrimary
        {
            get { return ((HeadersFooters)this.parent).primary == this; }
        }

        /// <summary>
        /// Gets or sets the style name.
        /// </summary>
        public string Style
        {
            get { return this.style.Value; }
            set
            {
                // Just save style name. 
                Style style = Document.Styles[value];
                if (style != null)
                    this.style.Value = value;
                else
                    throw new ArgumentException("Invalid style name '" + value + "'.");
            }
        }
        [DV]
        internal NString style = NString.NullValue;

        /// <summary>
        /// Gets or sets the paragraph format.
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
        /// Gets the collection of document objects that defines the header or footer.
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
        /// Converts HeaderFooter into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            HeadersFooters headersfooters = this.parent as HeadersFooters;
            if (headersfooters.Primary == this)
                this.Serialize(serializer, "primary");
            else if (headersfooters.EvenPage == this)
                this.Serialize(serializer, "evenpage");
            else if (headersfooters.FirstPage == this)
                this.Serialize(serializer, "firstpage");
        }

        /// <summary>
        /// Converts HeaderFooter into DDL.
        /// </summary>
        internal void Serialize(Serializer serializer, string prefix)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine("\\" + prefix + (IsHeader ? "header" : "footer"));

            int pos = serializer.BeginAttributes();
            if (!IsNull("Format"))
                this.format.Serialize(serializer, "Format", null);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!IsNull("Elements"))
                this.elements.Serialize(serializer);
            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitHeaderFooter(this);

            if (visitChildren && this.elements != null)
                ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
        }

        /// <summary>
        /// Determines whether this instance is null (not set).
        /// </summary>
        public override bool IsNull()
        {
            return false;
        }

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get
            {
                if (meta == null)
                    meta = new Meta(typeof(HeaderFooter));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}

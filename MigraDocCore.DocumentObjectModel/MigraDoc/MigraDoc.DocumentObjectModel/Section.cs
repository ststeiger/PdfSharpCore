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
using System.IO;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// A Section is a collection of document objects sharing the same header, footer, 
    /// and page setup.
    /// </summary>
    public class Section : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Section class.
        /// </summary>
        public Section()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Section class with the specified parent.
        /// </summary>
        internal Section(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Section Clone()
        {
            return (Section)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Section section = (Section)base.DeepCopy();
            if (section.pageSetup != null)
            {
                section.pageSetup = section.pageSetup.Clone();
                section.pageSetup.parent = section;
            }
            if (section.headers != null)
            {
                section.headers = section.headers.Clone();
                section.headers.parent = section;
            }
            if (section.footers != null)
            {
                section.footers = section.footers.Clone();
                section.footers.parent = section;
            }
            if (section.elements != null)
            {
                section.elements = section.elements.Clone();
                section.elements.parent = section;
            }
            return section;
        }

        /// <summary>
        /// Gets the previous section.
        /// </summary>
        public Section PreviousSection()
        {
            Sections sections = Parent as Sections;
            int index = sections.IndexOf(this);
            if (index > 0)
                return sections[index - 1];
            return null;
        }

        /// <summary>
        /// Adds a new paragraph to the section.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return this.Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the section.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return this.Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new paragraph with the specified text and style to the section.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText, string style)
        {
            return this.Elements.AddParagraph(paragraphText, style);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the section.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return this.Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the section.
        /// </summary>
        public Chart AddChart()
        {
            return this.Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the section.
        /// </summary>
        public Table AddTable()
        {
            return this.Elements.AddTable();
        }

        /// <summary>
        /// Adds a manual page break.
        /// </summary>
        public void AddPageBreak()
        {
            this.Elements.AddPageBreak();
        }

        /// <summary>
        /// Adds a new Image to the section.
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }

        /// <summary>
        /// Adds a new textframe to the section.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return this.Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the section.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the section.
        /// </summary>
        public void Add(Chart chart)
        {
            this.Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the section.
        /// </summary>
        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the section.
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the section.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            this.Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the PageSetup object
        /// </summary>
        public PageSetup PageSetup
        {
            get
            {
                if (this.pageSetup == null)
                    this.pageSetup = new PageSetup(this);

                return this.pageSetup;
            }
            set
            {
                SetParent(value);
                this.pageSetup = value;
            }
        }
        [DV]
        internal PageSetup pageSetup;

        /// <summary>
        /// Gets the HeadersFooters collection containing the headers.
        /// </summary>
        public HeadersFooters Headers
        {
            get
            {
                if (this.headers == null)
                    this.headers = new HeadersFooters(this);

                return this.headers;
            }
            set
            {
                SetParent(value);
                this.headers = value;
            }
        }
        [DV]
        internal HeadersFooters headers;

        /// <summary>
        /// Gets the HeadersFooters collection containing the footers.
        /// </summary>
        public HeadersFooters Footers
        {
            get
            {
                if (this.footers == null)
                    this.footers = new HeadersFooters(this);

                return this.footers;
            }
            set
            {
                SetParent(value);
                this.footers = value;
            }
        }
        [DV]
        internal HeadersFooters footers;

        /// <summary>
        /// Gets the document elements that build the section's content.
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
        [DV]
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

        /// <summary>
        /// Gets the last paragraph of this section, or null, if no paragraph exists is this section.
        /// </summary>
        public Paragraph LastParagraph
        {
            get
            {
                int count = this.elements.Count;
                for (int idx = count - 1; idx >= 0; idx--)
                {
                    if (this.elements[idx] is Paragraph)
                        return (Paragraph)this.elements[idx];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the last table of this section, or null, if no table exists is this section.
        /// </summary>
        public Table LastTable
        {
            get
            {
                int count = this.elements.Count;
                for (int idx = count - 1; idx >= 0; idx--)
                {
                    if (this.elements[idx] is Table)
                        return (Table)this.elements[idx];
                }
                return null;
            }
        }
        #endregion

        #region Internal
        /// <summary>
        /// Converts Section into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine("\\section");

            int pos = serializer.BeginAttributes();
            if (!this.IsNull("PageSetup"))
                this.PageSetup.Serialize(serializer);
            serializer.EndAttributes(pos);

            serializer.BeginContent();
            if (!this.IsNull("headers"))
                this.headers.Serialize(serializer);
            if (!this.IsNull("footers"))
                this.footers.Serialize(serializer);
            if (!this.IsNull("elements"))
                this.elements.Serialize(serializer);

            serializer.EndContent();
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitSection(this);

            if (visitChildren && this.headers != null)
                ((IVisitable)this.headers).AcceptVisitor(visitor, visitChildren);
            if (visitChildren && this.footers != null)
                ((IVisitable)this.footers).AcceptVisitor(visitor, visitChildren);

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
                    meta = new Meta(typeof(Section));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}

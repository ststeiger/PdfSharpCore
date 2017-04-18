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
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel.Shapes
{
    /// <summary>
    /// Represents a text frame that can be freely placed.
    /// </summary>
    public class TextFrame : Shape, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the TextFrame class.
        /// </summary>
        public TextFrame()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TextFrame class with the specified parent.
        /// </summary>
        internal TextFrame(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new TextFrame Clone()
        {
            return (TextFrame)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            TextFrame textFrame = (TextFrame)base.DeepCopy();
            if (textFrame.elements != null)
            {
                textFrame.elements = textFrame.elements.Clone();
                textFrame.elements.parent = textFrame;
            }
            return textFrame;
        }

        /// <summary>
        /// Adds a new paragraph to the text frame.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return this.Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the text frame.
        /// </summary>
        public Paragraph AddParagraph(string _paragraphText)
        {
            return this.Elements.AddParagraph(_paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the text frame.
        /// </summary>
        public Chart AddChart(ChartType _type)
        {
            return this.Elements.AddChart(_type);
        }

        /// <summary>
        /// Adds a new chart to the text frame.
        /// </summary>
        public Chart AddChart()
        {
            return this.Elements.AddChart();
        }

        /// <summary>
        /// Adds a new table to the text frame.
        /// </summary>
        public Table AddTable()
        {
            return this.Elements.AddTable();
        }

        /// <summary>
        /// Adds a new Image to the text frame.
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }

        /// <summary>
        /// Adds a new paragraph to the text frame.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the text frame.
        /// </summary>
        public void Add(Chart chart)
        {
            this.Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new table to the text frame.
        /// </summary>
        public void Add(Table table)
        {
            this.Elements.Add(table);
        }

        /// <summary>
        /// Adds a new image to the text frame.
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Margin between the textframes content and its left edge.
        /// </summary>
        public Unit MarginLeft
        {
            get { return this.marginLeft; }
            set { this.marginLeft = value; }
        }
        [DV]
        internal Unit marginLeft = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its right edge.
        /// </summary>
        public Unit MarginRight
        {
            get { return this.marginRight; }
            set { this.marginRight = value; }
        }
        [DV]
        internal Unit marginRight = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its top edge.
        /// </summary>
        public Unit MarginTop
        {
            get { return this.marginTop; }
            set { this.marginTop = value; }
        }
        [DV]
        internal Unit marginTop = Unit.NullValue;

        /// <summary>
        /// Gets or sets the Margin between the textframes content and its bottom edge.
        /// </summary>
        public Unit MarginBottom
        {
            get { return this.marginBottom; }
            set { this.marginBottom = value; }
        }
        [DV]
        internal Unit marginBottom = Unit.NullValue;

        /// <summary>
        /// Gets or sets the text orientation for the texframe content.
        /// </summary>
        public TextOrientation Orientation
        {
            get { return (TextOrientation)this.orientation.Value; }
            set { this.orientation.Value = (int)value; }
        }
        [DV(Type = typeof(TextOrientation))]
        internal NEnum orientation = NEnum.NullValue(typeof(TextOrientation));

        /// <summary>
        /// The document elements that build the textframe's content.
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
        protected DocumentElements elements;
        #endregion

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitTextFrame(this);

            if (visitChildren && this.elements != null)
                ((IVisitable)this.elements).AcceptVisitor(visitor, visitChildren);
        }

        #region Internal
        /// <summary>
        /// Converts TextFrame into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteLine("\\textframe");
            int pos = serializer.BeginAttributes();
            base.Serialize(serializer);
            if (!this.marginLeft.IsNull)
                serializer.WriteSimpleAttribute("MarginLeft", this.MarginLeft);
            if (!this.marginRight.IsNull)
                serializer.WriteSimpleAttribute("MarginRight", this.MarginRight);
            if (!this.marginTop.IsNull)
                serializer.WriteSimpleAttribute("MarginTop", this.MarginTop);
            if (!this.marginBottom.IsNull)
                serializer.WriteSimpleAttribute("MarginBottom", this.MarginBottom);
            if (!this.orientation.IsNull)
                serializer.WriteSimpleAttribute("Orientation", this.Orientation);
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
                    meta = new Meta(typeof(TextFrame));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}
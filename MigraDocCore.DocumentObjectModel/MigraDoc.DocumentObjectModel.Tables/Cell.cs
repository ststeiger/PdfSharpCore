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
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel.Tables
{
    /// <summary>
    /// Represents a cell of a table.
    /// </summary>
    public class Cell : DocumentObject, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        public Cell()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Cell class with the specified parent.
        /// </summary>
        internal Cell(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Cell Clone()
        {
            return (Cell)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Cell cell = (Cell)base.DeepCopy();
            if (cell.format != null)
            {
                cell.format = cell.format.Clone();
                cell.format.parent = cell;
            }
            if (cell.borders != null)
            {
                cell.borders = cell.borders.Clone();
                cell.borders.parent = cell;
            }
            if (cell.shading != null)
            {
                cell.shading = cell.shading.Clone();
                cell.shading.parent = cell;
            }
            if (cell.elements != null)
            {
                cell.elements = cell.elements.Clone();
                cell.elements.parent = cell;
            }
            return cell;
        }

        /// <summary>
        /// Resets the cached values.
        /// </summary>
        internal override void ResetCachedValues()
        {
            this.row = null;
            this.clm = null;
        }

        /// <summary>
        /// Adds a new paragraph to the cell.
        /// </summary>
        public Paragraph AddParagraph()
        {
            return this.Elements.AddParagraph();
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the cell.
        /// </summary>
        public Paragraph AddParagraph(string paragraphText)
        {
            return this.Elements.AddParagraph(paragraphText);
        }

        /// <summary>
        /// Adds a new chart with the specified type to the cell.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            return this.Elements.AddChart(type);
        }

        /// <summary>
        /// Adds a new chart to the cell.
        /// </summary>
        public Chart AddChart()
        {
            return this.Elements.AddChart();
        }

        /// <summary>
        /// Adds a new Image to the cell.
        /// </summary>
        public Image AddImage(IImageSource imageSource)
        {
            return this.Elements.AddImage(imageSource);
        }

        /// <summary>
        /// Adds a new textframe to the cell.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            return this.Elements.AddTextFrame();
        }

        /// <summary>
        /// Adds a new paragraph to the cell.
        /// </summary>
        public void Add(Paragraph paragraph)
        {
            this.Elements.Add(paragraph);
        }

        /// <summary>
        /// Adds a new chart to the cell.
        /// </summary>
        public void Add(Chart chart)
        {
            this.Elements.Add(chart);
        }

        /// <summary>
        /// Adds a new image to the cell.
        /// </summary>
        public void Add(Image image)
        {
            this.Elements.Add(image);
        }

        /// <summary>
        /// Adds a new text frame to the cell.
        /// </summary>
        public void Add(TextFrame textFrame)
        {
            this.Elements.Add(textFrame);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table the cell belongs to.
        /// </summary>
        public Table Table
        {
            get
            {
                if (this.table == null)
                {
                    Cells cls = this.Parent as Cells;
                    if (cls != null)
                        this.table = cls.Table;
                }
                return this.table;
            }
        }
        Table table;

        /// <summary>
        /// Gets the column the cell belongs to.
        /// </summary>
        public Column Column
        {
            get
            {
                if (this.clm == null)
                {
                    Cells cells = this.Parent as Cells;
                    for (int index = 0; index < cells.Count; ++index)
                    {
                        if (cells[index] == this)
                            this.clm = this.Table.Columns[index];
                    }
                }
                return this.clm;
            }
        }
        Column clm;

        /// <summary>
        /// Gets the row the cell belongs to.
        /// </summary>
        public Row Row
        {
            get
            {
                if (this.row == null)
                {
                    Cells cells = this.Parent as Cells;
                    this.row = cells.Row;
                }
                return this.row;
            }
        }
        Row row;

        /// <summary>
        /// Sets or gets the style name.
        /// </summary>
        public string Style
        {
            get { return this.style.Value; }
            set { this.style.Value = value; }
        }
        [DV]
        internal NString style = NString.NullValue;

        /// <summary>
        /// Gets the ParagraphFormat object of the paragraph.
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
        /// Gets or sets the vertical alignment of the cell.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)this.verticalAlignment.Value; }
            set { this.verticalAlignment.Value = (int)value; }
        }
        [DV(Type = typeof(VerticalAlignment))]
        internal NEnum verticalAlignment = NEnum.NullValue(typeof(VerticalAlignment));

        /// <summary>
        /// Gets the Borders object.
        /// </summary>
        public Borders Borders
        {
            get
            {
                if (this.borders == null)
                {
                    if (this.Document == null) // BUG CMYK
                        GetType();
                    this.borders = new Borders(this);
                }
                return this.borders;
            }
            set
            {
                SetParent(value);
                this.borders = value;
            }
        }
        [DV]
        internal Borders borders;

        /// <summary>
        /// Gets the shading object.
        /// </summary>
        public Shading Shading
        {
            get
            {
                if (this.shading == null)
                    this.shading = new Shading(this);

                return this.shading;
            }
            set
            {
                SetParent(value);
                this.shading = value;
            }
        }
        [DV]
        internal Shading shading;

        /// <summary>
        /// Specifies if the Cell should be rendered as a rounded corner.
        /// </summary>
        public RoundedCorner RoundedCorner {
            get { return (RoundedCorner)this.roundedCorner.Value; }
            set { this.roundedCorner.Value = (int)value; }
        }
        [DV(Type = typeof(RoundedCorner))]
        internal NEnum roundedCorner = NEnum.NullValue(typeof(RoundedCorner));

        /// <summary>
        /// Gets or sets the number of cells to be merged right.
        /// </summary>
        public int MergeRight
        {
            get { return this.mergeRight ?? 0; }
            set { this.mergeRight = value; }
        }
        [DV]
        internal int? mergeRight;

        /// <summary>
        /// Gets or sets the number of cells to be merged down.
        /// </summary>
        public int MergeDown
        {
            get { return this.mergeDown ?? 0; }
            set { this.mergeDown = value; }
        }
        [DV]
        internal int? mergeDown;

        /// <summary>
        /// Gets the collection of document objects that defines the cell.
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
        /// Converts Cell into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            serializer.WriteLine("\\cell");

            int pos = serializer.BeginAttributes();

            if (this.style.Value != String.Empty)
                serializer.WriteSimpleAttribute("Style", this.Style);

            if (!this.IsNull("Format"))
                this.format.Serialize(serializer, "Format", null);

            if (this.mergeDown.HasValue)
                serializer.WriteSimpleAttribute("MergeDown", this.MergeDown);

            if (this.mergeRight.HasValue)
                serializer.WriteSimpleAttribute("MergeRight", this.MergeRight);

            if (!this.verticalAlignment.IsNull)
                serializer.WriteSimpleAttribute("VerticalAlignment", this.VerticalAlignment);

            if (!this.IsNull("Borders"))
                this.borders.Serialize(serializer, null);

            if (!this.IsNull("Shading"))
                this.shading.Serialize(serializer);

            if (!this.roundedCorner.IsNull)
                serializer.WriteSimpleAttribute("RoundedCorner", this.RoundedCorner);

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
            visitor.VisitCell(this);

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
                    meta = new Meta(typeof(Cell));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}
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

using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocImage = MigraDocCore.DocumentObjectModel.Shapes.Image;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Represents a collection of document elements.
    /// </summary>
    public class DocumentElements : DocumentObjectCollection, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the DocumentElements class.
        /// </summary>
        public DocumentElements()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DocumentElements class with the specified parent.
        /// </summary>
        internal DocumentElements(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Gets a document object by its index.
        /// </summary>
        public new DocumentObject this[int index]
        {
            get { return base[index]; }
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new DocumentElements Clone()
        {
            return (DocumentElements)DeepCopy();
        }

        /// <summary>
        /// Adds a new paragraph to the collection.
        /// </summary>
        public Paragraph AddParagraph()
        {
            Paragraph paragraph = new Paragraph();
            Add(paragraph);
            return paragraph;
        }

        /// <summary>
        /// Adds a new paragraph with the specified text to the collection.
        /// </summary>
        public Paragraph AddParagraph(string text)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddText(text);
            Add(paragraph);
            return paragraph;
        }

        /// <summary>
        /// Adds a new paragraph with the specified text and style to the collection.
        /// </summary>
        public Paragraph AddParagraph(string text, string style)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.AddText(text);
            paragraph.Style = style;
            Add(paragraph);
            return paragraph;
        }

        /// <summary>
        /// Adds a new table to the collection.
        /// </summary>
        public Table AddTable()
        {
            Table tbl = new Table();
            Add(tbl);
            return tbl;
        }

        /// <summary>
        /// Adds a new legend to the collection.
        /// </summary>
        public Legend AddLegend()
        {
            Legend legend = new Legend();
            Add(legend);
            return legend;
        }

        /// <summary>
        /// Add a manual page break.
        /// </summary>
        public void AddPageBreak()
        {
            PageBreak pageBreak = new PageBreak();
            Add(pageBreak);
        }

        /// <summary>
        /// Adds a new barcode to the collection.
        /// </summary>
        public Barcode AddBarcode()
        {
            Barcode barcode = new Barcode();
            Add(barcode);
            return barcode;
        }

        /// <summary>
        /// Adds a new chart with the specified type to the collection.
        /// </summary>
        public Chart AddChart(ChartType type)
        {
            Chart chart = AddChart();
            chart.Type = type;
            return chart;
        }

        /// <summary>
        /// Adds a new chart with the specified type to the collection.
        /// </summary>
        public Chart AddChart()
        {
            Chart chart = new Chart();
            chart.Type = ChartType.Line;
            Add(chart);
            return chart;
        }

        public MigraDocImage AddImage(IImageSource image)
        {
            MigraDocImage img = new MigraDocImage()
            {
                Source = image,                
            };
            Add(img);
            return img;
        }

        /// <summary>
        /// Adds a new text frame to the collection.
        /// </summary>
        public TextFrame AddTextFrame()
        {
            TextFrame textFrame = new TextFrame();
            Add(textFrame);
            return textFrame;
        }
        #endregion

        #region Internal
        /// <summary>
        /// Converts DocumentElements into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            int count = Count;
            if (count == 1 && this[0] is Paragraph)
            {
                // Omit keyword if paragraph has no attributes set.
                Paragraph paragraph = (Paragraph)this[0];
                if (paragraph.Style == "" && paragraph.IsNull("Format"))
                {
                    paragraph.SerializeContentOnly = true;
                    paragraph.Serialize(serializer);
                    paragraph.SerializeContentOnly = false;
                    return;
                }
            }
            for (int index = 0; index < count; index++)
            {
                DocumentObject documentElement = this[index];
                documentElement.Serialize(serializer);
            }
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitDocumentElements(this);

            foreach (DocumentObject docObject in this)
            {
                if (docObject is IVisitable)
                    ((IVisitable)docObject).AcceptVisitor(visitor, visitChildren);
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
                    meta = new Meta(typeof(DocumentElements));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}

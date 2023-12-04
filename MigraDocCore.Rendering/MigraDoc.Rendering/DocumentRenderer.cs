#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
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
using System.Collections;
using System.IO;
using MigraDocCore.DocumentObjectModel;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering.MigraDoc.Rendering.Resources;

namespace MigraDocCore.Rendering
{
    /// <summary>
    /// Provides methods to render the document or single parts of it to a XGraphics object.
    /// </summary>
    /// <remarks>
    /// One prepared instance of this class can serve to render several output formats.
    /// </remarks>
    public class DocumentRenderer
    {
        /// <summary>
        /// Initializes a new instance of the DocumentRenderer class.
        /// </summary>
        /// <param name="document">The migradoc document to render.</param>
        public DocumentRenderer(Document document)
        {
            this.document = document;
        }

        /// <summary>
        /// Prepares this instance for rendering.
        /// </summary>
        public void PrepareDocument()
        {
            PdfFlattenVisitor visitor = new PdfFlattenVisitor();
            visitor.Visit(this.document);
            this.previousListNumbers = new Hashtable(3);
            this.previousListNumbers[ListType.NumberList1] = 0;
            this.previousListNumbers[ListType.NumberList2] = 0;
            this.previousListNumbers[ListType.NumberList3] = 0;
            this.formattedDocument = new FormattedDocument(this.document, this);
            //REM: Size should not be necessary in this case.
            XGraphics gfx = XGraphics.CreateMeasureContext(new XSize(2000, 2000), XGraphicsUnit.Point, XPageDirection.Downwards);
            //      this.previousListNumber = int.MinValue;
            //gfx.MUH = this.unicode;
            //gfx.MFEH = this.fontEmbedding;

            this.previousListInfo = null;
            this.formattedDocument.Format(gfx);
        }

        /// <summary>
        /// Occurs while the document is being prepared (can be used to show a progress bar).
        /// </summary>
        public event PrepareDocumentProgressEventHandler PrepareDocumentProgress;

        /// <summary>
        /// Allows applications to display a progress indicator while PrepareDocument() is being executed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        internal virtual void OnPrepareDocumentProgress(int value, int maximum)
        {
            if (PrepareDocumentProgress != null)
            {
                // Invokes the delegates. 
                PrepareDocumentProgressEventArgs e = new PrepareDocumentProgressEventArgs(value, maximum);
                PrepareDocumentProgress(this, e);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance supports PrepareDocumentProgress.
        /// </summary>
        public bool HasPrepareDocumentProgress
        {
            get { return PrepareDocumentProgress != null; }
        }

        /// <summary>
        /// Gets the formatted document of this instance.
        /// </summary>
        public FormattedDocument FormattedDocument
        {
            get { return this.formattedDocument; }
        }
        internal FormattedDocument formattedDocument;

        /// <summary>
        /// Renders a MigraDoc document to the specified graphics object.
        /// </summary>
        public void RenderPage(XGraphics gfx, int page)
        {
            RenderPage(gfx, page, PageRenderOptions.All);
        }

        /// <summary>
        /// Renders a MigraDoc document to the specified graphics object.
        /// </summary>
        public void RenderPage(XGraphics gfx, int page, PageRenderOptions options)
        {
            if (this.formattedDocument.IsEmptyPage(page))
                return;

            FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);

            if (this.printDate != DateTime.MinValue)
                fieldInfos.date = this.printDate;
            else
                fieldInfos.date = DateTime.Now;

            if ((options & PageRenderOptions.RenderHeader) == PageRenderOptions.RenderHeader)
                RenderHeader(gfx, page);
            if ((options & PageRenderOptions.RenderFooter) == PageRenderOptions.RenderFooter)
                RenderFooter(gfx, page);

            if ((options & PageRenderOptions.RenderContent) == PageRenderOptions.RenderContent)
            {
                RenderInfo[] renderInfos = this.formattedDocument.GetRenderInfos(page);
                //foreach (RenderInfo renderInfo in renderInfos)
                int count = renderInfos.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    RenderInfo renderInfo = renderInfos[idx];
                    Renderer renderer = Renderer.Create(gfx, this, renderInfo, fieldInfos);
                    renderer.Render();
                }
            }
        }

        /// <summary>
        /// Gets the document objects that get rendered on the specified page.
        /// </summary>
        public DocumentObject[] GetDocumentObjectsFromPage(int page)
        {
            RenderInfo[] renderInfos = this.formattedDocument.GetRenderInfos(page);
            int count = renderInfos != null ? renderInfos.Length : 0;
            DocumentObject[] documentObjects = new DocumentObject[count];
            for (int idx = 0; idx < count; idx++)
                documentObjects[idx] = renderInfos[idx].DocumentObject;
            return documentObjects;
        }

        /// <summary>
        /// Gets the render information for document objects that get rendered on the specified page.
        /// </summary>
        public RenderInfo[] GetRenderInfoFromPage(int page)
        {
            return this.formattedDocument.GetRenderInfos(page);
        }

        /// <summary>
        /// Renders a single object to the specified graphics object at the given point.
        /// </summary>
        /// <param name="graphics">The graphics object to render on.</param>
        /// <param name="xPosition">The left position of the rendered object.</param>
        /// <param name="yPosition">The top position of the rendered object.</param>
        /// <param name="width">The width.</param>
        /// <param name="documentObject">The document object to render. Can be paragraph, table, or shape.</param>
        /// <remarks>This function is still in an experimental state.</remarks>
        public void RenderObject(XGraphics graphics, XUnit xPosition, XUnit yPosition, XUnit width, DocumentObject documentObject)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            if (documentObject == null)
                throw new ArgumentNullException("documentObject");

            if (!(documentObject is Shape) && !(documentObject is Table) &&
                !(documentObject is Paragraph))
                throw new ArgumentException(AppResources.ObjectNotRenderable, "documentObject");

            Renderer renderer = Renderer.Create(graphics, this, documentObject, null);
            renderer.Format(new Rectangle(xPosition, yPosition, width, double.MaxValue), null);

            RenderInfo renderInfo = renderer.RenderInfo;
            renderInfo.LayoutInfo.ContentArea.X = xPosition;
            renderInfo.LayoutInfo.ContentArea.Y = yPosition;

            renderer = Renderer.Create(graphics, this, renderer.RenderInfo, null);
            renderer.Render();
        }

        /// <summary>
        /// Gets or sets the working directory for rendering.
        /// </summary>
        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            set { this.workingDirectory = value; }
        }
        string workingDirectory;

        private void RenderHeader(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedHeader = this.formattedDocument.GetFormattedHeader(page);
            if (formattedHeader == null)
                return;

            Rectangle headerArea = this.formattedDocument.GetHeaderArea(page);
            RenderInfo[] renderInfos = formattedHeader.GetRenderInfos();
            FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                renderer.Render();
            }
        }

        private void RenderFooter(XGraphics graphics, int page)
        {
            FormattedHeaderFooter formattedFooter = this.formattedDocument.GetFormattedFooter(page);
            if (formattedFooter == null)
                return;

            Rectangle footerArea = this.formattedDocument.GetFooterArea(page);
            RenderInfo[] renderInfos = formattedFooter.GetRenderInfos();
            XUnit topY = footerArea.Y + footerArea.Height - RenderInfo.GetTotalHeight(renderInfos);

            FieldInfos fieldInfos = this.formattedDocument.GetFieldInfos(page);
            foreach (RenderInfo renderInfo in renderInfos)
            {
                Renderer renderer = Renderer.Create(graphics, this, renderInfo, fieldInfos);
                XUnit savedY = renderer.RenderInfo.LayoutInfo.ContentArea.Y;
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = topY;
                renderer.Render();
                renderer.RenderInfo.LayoutInfo.ContentArea.Y = savedY;
            }
        }

        internal void AddOutline(int level, string title, PdfPage destinationPage)
        {
            if (level < 1 || destinationPage == null)
                return;

            PdfDocument document = destinationPage.Owner;

            if (document == null)
                return;

            PdfOutlineCollection outlines = document.Outlines;
            while (--level > 0)
            {
                int count = outlines.Count;
                if (count == 0)
                {
                    // You cannot add empty bookmarks to PDF. So we use blank here.
                    PdfOutline outline = outlines.Add(" ", destinationPage, true);
                    outlines = outline.Outlines;
                }
                else
                    outlines = outlines[count - 1].Outlines;
            }
            outlines.Add(title, destinationPage, true);
        }

        internal int NextListNumber(ListInfo listInfo)
        {
            ListType listType = listInfo.ListType;
            bool isNumberList = listType == ListType.NumberList1 ||
              listType == ListType.NumberList2 ||
              listType == ListType.NumberList3;

            int listNumber = int.MinValue;
            if (listInfo == this.previousListInfo)
            {
                if (isNumberList)
                    return (int)this.previousListNumbers[listType];
                return listNumber;
            }

            //bool listTypeChanged = this.previousListInfo == null || this.previousListInfo.ListType != listType;

            if (isNumberList)
            {
                listNumber = 1;
                if (/*!listTypeChanged &&*/ (listInfo.IsNull("ContinuePreviousList") || listInfo.ContinuePreviousList))
                    listNumber = (int)this.previousListNumbers[listType] + 1;

                this.previousListNumbers[listType] = listNumber;
            }
            //      else
            //        listNumber = int.MinValue;

            this.previousListInfo = listInfo;
            return listNumber;
        }
        ListInfo previousListInfo;
        Hashtable previousListNumbers;
        private Document document;
        internal DateTime printDate = DateTime.MinValue;

        /// <summary>
        /// Arguments for the PrepareDocumentProgressEvent which is called while a document is being prepared (you can use this to display a progress bar).
        /// </summary>
        public class PrepareDocumentProgressEventArgs : EventArgs
        {
            /// <summary>
            /// Indicates the current step reached in document preparation.
            /// </summary>
            public int Value;
            /// <summary>
            /// Indicates the final step in document preparation. The quitient of Value and Maximum can be used to calculate a percentage (e. g. for use in a progress bar).
            /// </summary>
            public int Maximum;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrepareDocumentProgressEventArgs"/> class.
            /// </summary>
            /// <param name="value">The current step in document preparation.</param>
            /// <param name="maximum">The latest step in document preparation.</param>
            public PrepareDocumentProgressEventArgs(int value, int maximum)
            {
                this.Value = value;
                this.Maximum = maximum;
            }
        }

        /// <summary>
        /// The event handler that is being called for the PrepareDocumentProgressEvent event.
        /// </summary>
        public delegate void PrepareDocumentProgressEventHandler(object sender, PrepareDocumentProgressEventArgs e);

        internal int ProgressMaximum;
        internal int ProgressCompleted;

        /// <summary>
        /// Gets or sets the private fonts of the document.
        /// </summary>
        public XPrivateFontCollection PrivateFonts
        {
            get { return this.privateFonts; }
            set { this.privateFonts = value; }
        }
        //[DV]
        internal XPrivateFontCollection privateFonts;
    }
}

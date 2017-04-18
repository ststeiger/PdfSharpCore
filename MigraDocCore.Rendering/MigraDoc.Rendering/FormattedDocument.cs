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
using System.Collections.Generic;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.Rendering
{
    /// <summary>
    /// Represents a formatted document.
    /// </summary>
    public class FormattedDocument : IAreaProvider
    {
        enum PagePosition
        {
            First,
            Odd,
            Even
        }

        private struct HeaderFooterPosition
        {
            internal HeaderFooterPosition(int sectionNr, PagePosition pagePosition)
            {
                this.sectionNr = sectionNr;
                this.pagePosition = pagePosition;
            }

            public override bool Equals(object obj)
            {
                if (obj is HeaderFooterPosition)
                {
                    HeaderFooterPosition hfp = (HeaderFooterPosition)obj;
                    return this.sectionNr == hfp.sectionNr && this.pagePosition == hfp.pagePosition;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.sectionNr.GetHashCode() ^ this.pagePosition.GetHashCode();
            }
            internal int sectionNr;
            internal PagePosition pagePosition;
        }

        internal FormattedDocument(Document document, DocumentRenderer documentRenderer)
        {
            this.document = document;
            this.documentRenderer = documentRenderer;
        }

        /// <summary>
        /// Formats the document by performing line breaks and page breaks.
        /// </summary>
        internal void Format(XGraphics gfx)
        {
            this.bookmarks = new Dictionary<string, FieldInfos.BookmarkInfo>();
            this.pageRenderInfos = new Dictionary<int, ArrayList>();
            this.pageInfos = new Dictionary<int, PageInfo>();
            this.pageFieldInfos = new Dictionary<int, FieldInfos>();
            this.formattedHeaders = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            this.formattedFooters = new Dictionary<HeaderFooterPosition, FormattedHeaderFooter>();
            this.gfx = gfx;
            this.currentPage = 0;
            this.sectionNumber = 0;
            this.pageCount = 0;
            this.shownPageNumber = 0;
            this.documentRenderer.ProgressCompleted = 0;
            this.documentRenderer.ProgressMaximum = 0;
            if (this.documentRenderer.HasPrepareDocumentProgress)
            {
                foreach (Section section in this.document.Sections)
                    this.documentRenderer.ProgressMaximum += section.Elements.Count;
            }
            foreach (Section section in this.document.Sections)
            {
                this.isNewSection = true;
                this.currentSection = section;
                ++this.sectionNumber;
                if (NeedsEmptyPage())
                    InsertEmptyPage();

                TopDownFormatter formatter = new TopDownFormatter(this, this.documentRenderer, section.Elements);
                formatter.FormatOnAreas(gfx, true);
                FillSectionPagesInfo();
                this.documentRenderer.ProgressCompleted += section.Elements.Count;
            }
            this.pageCount = this.currentPage;
            FillNumPagesInfo();
        }

        PagePosition CurrentPagePosition
        {
            get
            {
                if (this.isNewSection)
                    return PagePosition.First;
                else if (this.currentPage % 2 == 0)
                    return PagePosition.Even;
                else
                    return PagePosition.Odd;
            }
        }

        void FormatHeadersFooters()
        {
            HeadersFooters headers = (HeadersFooters)this.currentSection.GetValue("Headers", GV.ReadOnly);
            if (headers != null)
            {
                PagePosition pagePos = CurrentPagePosition;
                HeaderFooterPosition hfp = new HeaderFooterPosition(this.sectionNumber, pagePos);
                if (!this.formattedHeaders.ContainsKey(hfp))
                    FormatHeader(hfp, ChooseHeaderFooter(headers, pagePos));
            }

            HeadersFooters footers = (HeadersFooters)this.currentSection.GetValue("Footers", GV.ReadOnly);
            if (footers != null)
            {
                PagePosition pagePos = CurrentPagePosition;
                HeaderFooterPosition hfp = new HeaderFooterPosition(this.sectionNumber, pagePos);
                if (!this.formattedFooters.ContainsKey(hfp))
                    FormatFooter(hfp, ChooseHeaderFooter(footers, pagePos));
            }
        }


        void FormatHeader(HeaderFooterPosition hfp, HeaderFooter header)
        {
            if (header != null && !this.formattedHeaders.ContainsKey(hfp))
            {
                FormattedHeaderFooter formattedHeaderFooter = new FormattedHeaderFooter(header, this.documentRenderer, this.currentFieldInfos);
                formattedHeaderFooter.ContentRect = GetHeaderArea(this.currentSection, this.currentPage);
                formattedHeaderFooter.Format(gfx);
                this.formattedHeaders.Add(hfp, formattedHeaderFooter);
            }
        }


        void FormatFooter(HeaderFooterPosition hfp, HeaderFooter footer)
        {
            if (footer != null && !this.formattedFooters.ContainsKey(hfp))
            {
                FormattedHeaderFooter formattedHeaderFooter = new FormattedHeaderFooter(footer, this.documentRenderer, this.currentFieldInfos);
                formattedHeaderFooter.ContentRect = GetFooterArea(this.currentSection, this.currentPage);
                formattedHeaderFooter.Format(gfx);
                this.formattedFooters.Add(hfp, formattedHeaderFooter);
            }
        }

        /// <summary>
        /// Fills the number pages information after formatting the document.
        /// </summary>
        void FillNumPagesInfo()
        {
            for (int page = 1; page <= this.pageCount; ++page)
            {
                if (IsEmptyPage(page))
                    continue;

                FieldInfos fieldInfos = this.pageFieldInfos[page];
                fieldInfos.numPages = this.pageCount;
            }
        }

        /// <summary>
        /// Fills the section pages information after formatting a section.
        /// </summary>
        void FillSectionPagesInfo()
        {
            for (int page = this.currentPage; page > 0; --page)
            {
                if (IsEmptyPage(page))
                    continue;

                FieldInfos fieldInfos = this.pageFieldInfos[page];
                if (fieldInfos.section != this.sectionNumber)
                    break;

                fieldInfos.sectionPages = this.sectionPages;
            }
        }

        Rectangle CalcContentRect(int page)
        {
            PageSetup pageSetup = this.currentSection.PageSetup;
            XUnit width;
            if (pageSetup.Orientation == Orientation.Portrait)
                width = pageSetup.PageWidth.Point;
            else
                width = pageSetup.PageHeight.Point;

            width -= pageSetup.RightMargin.Point;
            width -= pageSetup.LeftMargin.Point;

            XUnit height;
            if (pageSetup.Orientation == Orientation.Portrait)
                height = pageSetup.PageHeight.Point;
            else
                height = pageSetup.PageWidth.Point;

            height -= pageSetup.TopMargin.Point;
            height -= pageSetup.BottomMargin.Point;
            XUnit x;
            XUnit y;
            y = pageSetup.TopMargin.Point;
            if (pageSetup.MirrorMargins)
                x = page % 2 == 0 ? pageSetup.RightMargin.Point : pageSetup.LeftMargin.Point;
            else
                x = pageSetup.LeftMargin.Point;
            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Gets the rendering information for the page content.
        /// </summary>
        /// <param name="page">The page to render.</param>
        /// <returns>Rendering information for the page content.</returns>
        internal RenderInfo[] GetRenderInfos(int page)
        {
            if (this.pageRenderInfos.ContainsKey(page))
                return (RenderInfo[])(this.pageRenderInfos[page]).ToArray(typeof(RenderInfo));
            return null;
        }
        private Dictionary<int, ArrayList> pageRenderInfos;

        /// <summary>
        /// Gets a formatted headerfooter object for header of the given page.
        /// </summary>
        /// <param name="page">The page the header shall appear on.</param>
        /// <returns>The required header, null if none exists to render.</returns>
        internal FormattedHeaderFooter GetFormattedHeader(int page)
        {
            PagePosition pagePos = page % 2 == 0 ? PagePosition.Even : PagePosition.Odd;

            FieldInfos fieldInfos = this.pageFieldInfos[page];

            if (page == 1)
                pagePos = PagePosition.First;
            else //page > 1
            {
                if (IsEmptyPage(page - 1)) // these empty pages only occur between sections.
                    pagePos = PagePosition.First;
                else
                {
                    FieldInfos prevFieldInfos = this.pageFieldInfos[page - 1];
                    if (fieldInfos.section != prevFieldInfos.section)
                        pagePos = PagePosition.First;
                }
            }
            HeaderFooterPosition hfp = new HeaderFooterPosition(fieldInfos.section, pagePos);
            if (this.formattedHeaders.ContainsKey(hfp))
                return this.formattedHeaders[hfp];
            return null;
        }

        /// <summary>
        /// Gets a formatted headerfooter object for footer of the given page.
        /// </summary>
        /// <param name="page">The page the footer shall appear on.</param>
        /// <returns>The required footer, null if none exists to render.</returns>
        internal FormattedHeaderFooter GetFormattedFooter(int page)
        {
            PagePosition pagePos = page % 2 == 0 ? PagePosition.Even : PagePosition.Odd;

            FieldInfos fieldInfos = this.pageFieldInfos[page];

            if (page == 1)
                pagePos = PagePosition.First;

            else //page > 1
            {
                if (IsEmptyPage(page - 1)) // these empty pages only occur between sections.
                    pagePos = PagePosition.First;
                else
                {
                    FieldInfos prevFieldInfos = this.pageFieldInfos[page - 1];
                    if (fieldInfos.section != prevFieldInfos.section)
                        pagePos = PagePosition.First;
                }
            }
            HeaderFooterPosition hfp = new HeaderFooterPosition(fieldInfos.section, pagePos);
            if (this.formattedFooters.ContainsKey(hfp))
                return this.formattedFooters[hfp];
            return null;
        }

        private Rectangle GetHeaderArea(Section section, int page)
        {
            PageSetup pageSetup = section.PageSetup;
            XUnit xPos;
            if (pageSetup.MirrorMargins && page % 2 == 0)
                xPos = pageSetup.RightMargin.Point;
            else
                xPos = pageSetup.LeftMargin.Point;

            XUnit width;
            if (pageSetup.Orientation == Orientation.Portrait)
                width = pageSetup.PageWidth.Point;
            else
                width = pageSetup.PageHeight.Point;

            width -= pageSetup.LeftMargin + pageSetup.RightMargin;

            XUnit yPos = pageSetup.HeaderDistance.Point;
            XUnit height = pageSetup.TopMargin - pageSetup.HeaderDistance;
            return new Rectangle(xPos, yPos, width, height);
        }

        internal Rectangle GetHeaderArea(int page)
        {
            FieldInfos fieldInfos = this.pageFieldInfos[page];
            Section section = this.document.Sections[fieldInfos.section - 1];
            return GetHeaderArea(section, page);
        }

        internal Rectangle GetFooterArea(int page)
        {
            FieldInfos fieldInfos = this.pageFieldInfos[page];
            Section section = this.document.Sections[fieldInfos.section - 1];
            return GetFooterArea(section, page);
        }

        private Rectangle GetFooterArea(Section section, int page)
        {
            PageSetup pageSetup = section.PageSetup;
            XUnit xPos;
            if (pageSetup.MirrorMargins && page % 2 == 0)
                xPos = pageSetup.RightMargin.Point;
            else
                xPos = pageSetup.LeftMargin.Point;

            XUnit width;
            if (pageSetup.Orientation == Orientation.Portrait)
                width = pageSetup.PageWidth.Point;
            else
                width = pageSetup.PageHeight.Point;
            width -= pageSetup.LeftMargin + pageSetup.RightMargin;
            XUnit yPos;
            if (pageSetup.Orientation == Orientation.Portrait)
                yPos = pageSetup.PageHeight.Point;
            else
                yPos = pageSetup.PageWidth.Point;

            yPos -= pageSetup.BottomMargin.Point;
            XUnit height = pageSetup.BottomMargin - pageSetup.FooterDistance;
            return new Rectangle(xPos, yPos, width, height);
        }


        private HeaderFooter ChooseHeaderFooter(HeadersFooters hfs, PagePosition pagePos)
        {
            if (hfs == null)
                return null;

            PageSetup pageSetup = this.currentSection.PageSetup;

            if (pagePos == PagePosition.First)
            {
                if (pageSetup.DifferentFirstPageHeaderFooter)
                    return (HeaderFooter)hfs.GetValue("FirstPage", GV.ReadOnly);
            }
            if (pagePos == PagePosition.Even || this.currentPage % 2 == 0)
            {
                if (pageSetup.OddAndEvenPagesHeaderFooter)
                    return (HeaderFooter)hfs.GetValue("EvenPage", GV.ReadOnly);
            }
            return (HeaderFooter)hfs.GetValue("Primary", GV.ReadOnly);
        }

        /// <summary>
        /// Gets the number of pages of the document.
        /// </summary>
        public int PageCount
        {
            get { return pageCount; }
        }
        int pageCount;


        /// <summary>
        /// Gets information about the specified page.
        /// </summary>
        /// <param name="page">The page the information is asked for.</param>
        /// <returns>The page information.</returns>
        public PageInfo GetPageInfo(int page)
        {
            if (page < 1 || page > this.pageCount)
                throw new System.ArgumentOutOfRangeException("page");

            return this.pageInfos[page];
        }

        #region IAreaProvider Members

        Area IAreaProvider.GetNextArea()
        {
            if (this.isNewSection)
                this.sectionPages = 0;

            ++this.currentPage;
            ++this.shownPageNumber;
            ++this.sectionPages;
            InitFieldInfos();
            FormatHeadersFooters();
            this.isNewSection = false;
            return CalcContentRect(this.currentPage);
        }
        int currentPage;

        Area IAreaProvider.ProbeNextArea()
        {
            return CalcContentRect(this.currentPage + 1);
        }

        void InitFieldInfos()
        {
            this.currentFieldInfos = new FieldInfos(this.bookmarks);
            this.currentFieldInfos.pyhsicalPageNr = this.currentPage;
            this.currentFieldInfos.section = this.sectionNumber;

            if (this.isNewSection && !this.currentSection.PageSetup.IsNull("StartingNumber"))
                this.shownPageNumber = this.currentSection.PageSetup.StartingNumber;

            this.currentFieldInfos.displayPageNr = this.shownPageNumber;
        }

        void IAreaProvider.StoreRenderInfos(ArrayList renderInfos)
        {
            this.pageRenderInfos.Add(this.currentPage, renderInfos);
            XSize pageSize = CalcPageSize(this.currentSection.PageSetup);
            PageOrientation pageOrientation = CalcPageOrientation(this.currentSection.PageSetup);
            PageInfo pageInfo = new PageInfo(pageSize.Width, pageSize.Height, pageOrientation);
            this.pageInfos.Add(this.currentPage, pageInfo);
            this.pageFieldInfos.Add(this.currentPage, this.currentFieldInfos);
        }

        PageOrientation CalcPageOrientation(PageSetup pageSetup)
        {
            PageOrientation pageOrientation = PageOrientation.Portrait;
            if (this.currentSection.PageSetup.Orientation == Orientation.Landscape)
                pageOrientation = PageOrientation.Landscape;

            return pageOrientation;
        }

        XSize CalcPageSize(PageSetup pageSetup)
        {
            return new XSize(pageSetup.PageWidth.Point, pageSetup.PageHeight.Point);
        }

        bool IAreaProvider.PositionHorizontally(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.HorizontalReference)
            {
                case HorizontalReference.PageMargin:
                case HorizontalReference.AreaBoundary:
                    return PositionHorizontallyToMargin(layoutInfo);

                case HorizontalReference.Page:
                    return PositionHorizontallyToPage(layoutInfo);
            }
            return false;
        }

        //New KlPo 27.08.07
        /// <summary>
        /// Gets the alignment depending on the currentPage for the alignments "Outside" and "Inside".
        /// </summary>
        /// <param name="alignment">The original alignment</param>
        /// <returns>the alignment depending on the currentPage for the alignments "Outside" and "Inside"</returns>
        private ElementAlignment GetCurrentAlignment(ElementAlignment alignment)
        {
            ElementAlignment align = alignment;

            if (align == ElementAlignment.Inside)
            {
                if (currentPage % 2 == 0)
                    align = ElementAlignment.Far;
                else
                    align = ElementAlignment.Near;
            }
            else if (align == ElementAlignment.Outside)
            {
                if (currentPage % 2 == 0)
                    align = ElementAlignment.Near;
                else
                    align = ElementAlignment.Far;
            }
            return align;
        }

        bool PositionHorizontallyToMargin(LayoutInfo layoutInfo)
        {
            Rectangle rect = CalcContentRect(this.currentPage);
            ElementAlignment align = GetCurrentAlignment(layoutInfo.HorizontalAlignment);


            switch (align)
            {
                case ElementAlignment.Near:
                    if (layoutInfo.Left != 0)
                    {
                        layoutInfo.ContentArea.X += layoutInfo.Left;
                        return true;
                    }
                    else if (layoutInfo.MarginLeft != 0)
                    {
                        layoutInfo.ContentArea.X += layoutInfo.MarginLeft;
                        return true;
                    }
                    return false;

                case ElementAlignment.Far:
                    XUnit xPos = rect.X + rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos -= layoutInfo.MarginRight;
                    layoutInfo.ContentArea.X = xPos;
                    return true;

                case ElementAlignment.Center:
                    xPos = rect.Width;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos = rect.X + xPos / 2;
                    layoutInfo.ContentArea.X = xPos;
                    return true;
            }
            return false;
        }

        bool PositionHorizontallyToPage(LayoutInfo layoutInfo)
        {
            XUnit xPos;
            ElementAlignment align = GetCurrentAlignment(layoutInfo.HorizontalAlignment);
            switch (align)
            {
                case ElementAlignment.Near:
#if true
                    // Allow negative offsets (supporting "Anschnitt" for images)
                    if (layoutInfo.HorizontalReference == HorizontalReference.Page ||
                      layoutInfo.HorizontalReference == HorizontalReference.PageMargin)
                        xPos = layoutInfo.MarginLeft; // Ignore layoutInfo.Left if absolute position is specified
                    else
                        xPos = Math.Max(layoutInfo.MarginLeft, layoutInfo.Left);
#else
          //!!!delTHHO 22.10.2008 
          xPos = Math.Max(layoutInfo.MarginLeft, layoutInfo.Left);
#endif
                    layoutInfo.ContentArea.X = xPos;
                    break;

                case ElementAlignment.Far:
                    xPos = this.currentSection.PageSetup.PageWidth.Point;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos -= layoutInfo.MarginRight;
                    layoutInfo.ContentArea.X = xPos;
                    break;

                case ElementAlignment.Center:
                    xPos = this.currentSection.PageSetup.PageWidth.Point;
                    xPos -= layoutInfo.ContentArea.Width;
                    xPos /= 2;
                    layoutInfo.ContentArea.X = xPos;
                    break;
            }
            return true;
        }

        bool PositionVerticallyToMargin(LayoutInfo layoutInfo)
        {
            Rectangle rect = CalcContentRect(this.currentPage);
            XUnit yPos;
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    yPos = rect.Y;
                    //Added KlPo 12.07.07:
                    if (layoutInfo.Top == 0)
                        yPos += layoutInfo.MarginTop;
                    else
                        yPos += layoutInfo.Top;
                    //Removed KlPo 12.07.07
                    //yPos += Math.Max(layoutInfo.Top, layoutInfo.MarginTop);

                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Far:
                    yPos = rect.Y + rect.Height;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos -= layoutInfo.MarginBottom;
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Center:
                    yPos = rect.Height;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos = rect.Y + yPos / 2;
                    layoutInfo.ContentArea.Y = yPos;
                    break;
            }
            return true;
        }

        bool NeedsEmptyPage()
        {
            int nextPage = this.currentPage + 1;
            PageSetup pageSetup = this.currentSection.PageSetup;
            bool startOnEvenPage = pageSetup.SectionStart == BreakType.BreakEvenPage;
            bool startOnOddPage = pageSetup.SectionStart == BreakType.BreakOddPage;

            if (startOnOddPage)
                return nextPage % 2 == 0;
            else if (startOnEvenPage)
                return nextPage % 2 == 1;

            return false;
        }

        void InsertEmptyPage()
        {
            ++this.currentPage;
            ++this.shownPageNumber;
            this.emptyPages.Add(this.currentPage, null);

            XSize pageSize = CalcPageSize(this.currentSection.PageSetup);
            PageOrientation pageOrientation = CalcPageOrientation(this.currentSection.PageSetup);
            PageInfo pageInfo = new PageInfo(pageSize.Width, pageSize.Height, pageOrientation);
            this.pageInfos.Add(this.currentPage, pageInfo);
        }

        bool PositionVerticallyToPage(LayoutInfo layoutInfo)
        {
            XUnit yPos;
            switch (layoutInfo.VerticalAlignment)
            {
                case ElementAlignment.Near:
                    yPos = Math.Max(layoutInfo.MarginTop, layoutInfo.Top);
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Far:
                    yPos = this.currentSection.PageSetup.PageHeight.Point;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos -= layoutInfo.MarginBottom;
                    layoutInfo.ContentArea.Y = yPos;
                    break;

                case ElementAlignment.Center:
                    yPos = this.currentSection.PageSetup.PageHeight.Point;
                    yPos -= layoutInfo.ContentArea.Height;
                    yPos /= 2;
                    layoutInfo.ContentArea.Y = yPos;
                    break;
            }
            return true;
        }

        bool IAreaProvider.PositionVertically(LayoutInfo layoutInfo)
        {
            switch (layoutInfo.VerticalReference)
            {
                case VerticalReference.PreviousElement:
                    return false;

                case VerticalReference.AreaBoundary:
                case VerticalReference.PageMargin:
                    return PositionVerticallyToMargin(layoutInfo);

                case VerticalReference.Page:
                    return PositionVerticallyToPage(layoutInfo);
            }
            return false;
        }

        internal FieldInfos GetFieldInfos(int page)
        {
            return this.pageFieldInfos[page];
        }

        FieldInfos IAreaProvider.AreaFieldInfos
        {
            get
            {
                return this.currentFieldInfos;
            }
        }

        bool IAreaProvider.IsAreaBreakBefore(LayoutInfo layoutInfo)
        {
            return layoutInfo.PageBreakBefore;
        }

        internal bool IsEmptyPage(int page)
        {
            return this.emptyPages.ContainsKey(page);
        }
        #endregion

        Dictionary<string, FieldInfos.BookmarkInfo> bookmarks;
        int sectionPages;
        int shownPageNumber;
        int sectionNumber;
        Section currentSection;
        bool isNewSection;
        FieldInfos currentFieldInfos;
        Dictionary<int, FieldInfos> pageFieldInfos;
        Dictionary<HeaderFooterPosition, FormattedHeaderFooter> formattedHeaders;
        Dictionary<HeaderFooterPosition, FormattedHeaderFooter> formattedFooters;
        DocumentRenderer documentRenderer;
        XGraphics gfx;
        Dictionary<int, PageInfo> pageInfos;
        Dictionary<int, object> emptyPages = new Dictionary<int, object>();
        Document document;
    }
}
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
using System.Diagnostics;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Internals;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Annotations;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Fields;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.Rendering.MigraDoc.Rendering.Resources;
using PdfSharpCore.Fonts;

namespace MigraDocCore.Rendering
{
    internal struct TabOffset
    {
        internal TabOffset(TabLeader leader, XUnit offset)
        {
            this.leader = leader;
            this.offset = offset;
        }
        internal TabLeader leader;
        internal XUnit offset;
    }

    /// <summary>
    /// Summary description for ParagraphRenderer.
    /// </summary>
    internal class ParagraphRenderer : Renderer
    {
        /// <summary>
        /// Process phases of the renderer.
        /// </summary>
        private enum Phase
        {
            Formatting,
            Rendering
        }

        /// <summary>
        /// Results that can occur when processing a paragraph element
        /// during formatting.
        /// </summary>
        private enum FormatResult
        {
            /// <summary>
            /// Ignore the current element during formatting.
            /// </summary>
            Ignore,

            /// <summary>
            /// Continue with the next element within the same line.
            /// </summary>
            Continue,

            /// <summary>
            /// Start a new line from the current object on.
            /// </summary>
            NewLine,

            /// <summary>
            /// Break formatting and continue in a new area (e.g. a new page).
            /// </summary>
            NewArea
        }
        private Phase phase;

        /// <summary>
        /// Initializes a ParagraphRenderer object for formatting.
        /// </summary>
        /// <param name="gfx">The XGraphics object to do measurements on.</param>
        /// <param name="paragraph">The paragraph to format.</param>
        /// <param name="fieldInfos">The field infos.</param>
        internal ParagraphRenderer(XGraphics gfx, Paragraph paragraph, FieldInfos fieldInfos)
          : base(gfx, paragraph, fieldInfos)
        {
            this.paragraph = paragraph;

            ParagraphRenderInfo parRenderInfo = new ParagraphRenderInfo();
            parRenderInfo.paragraph = this.paragraph;
            ((ParagraphFormatInfo)parRenderInfo.FormatInfo).widowControl = this.paragraph.Format.WidowControl;

            this.renderInfo = parRenderInfo;
        }

        /// <summary>
        /// Initializes a ParagraphRenderer object for rendering.
        /// </summary>
        /// <param name="gfx">The XGraphics object to render on.</param>
        /// <param name="renderInfo">The render info object containing information necessary for rendering.</param>
        /// <param name="fieldInfos">The field infos.</param>
        internal ParagraphRenderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
          : base(gfx, renderInfo, fieldInfos)
        {
            this.paragraph = (Paragraph)renderInfo.DocumentObject;
        }

        /// <summary>
        /// Renders the paragraph.
        /// </summary>
        internal override void Render()
        {
            InitRendering();
            if ((int)this.paragraph.Format.OutlineLevel >= 1 && this.gfx.PdfPage != null) // Don't call GetOutlineTitle() in vain
                this.documentRenderer.AddOutline((int)this.paragraph.Format.OutlineLevel, GetOutlineTitle(), this.gfx.PdfPage);

            RenderShading();
            RenderBorders();

            ParagraphFormatInfo parFormatInfo = (ParagraphFormatInfo)this.renderInfo.FormatInfo;
            for (int idx = 0; idx < parFormatInfo.LineCount; ++idx)
            {
                LineInfo lineInfo = parFormatInfo.GetLineInfo(idx);
                this.isLastLine = (idx == parFormatInfo.LineCount - 1);

                this.lastTabPosition = 0;
                if (lineInfo.reMeasureLine)
                    ReMeasureLine(ref lineInfo);

                RenderLine(lineInfo);
            }
        }

        bool IsRenderedField(DocumentObject docObj)
        {
            if (docObj is NumericFieldBase)
                return true;

            if (docObj is DocumentInfo)
                return true;

            if (docObj is DateField)
                return true;

            return false;
        }

        string GetFieldValue(DocumentObject field)
        {
            if (field is NumericFieldBase)
            {
                int number = -1;
                if (field is PageRefField)
                {
                    PageRefField pageRefField = (PageRefField)field;
                    number = this.fieldInfos.GetShownPageNumber(pageRefField.Name);
                    if (number <= 0)
                    {
                        if (this.phase == Phase.Formatting)
                            return "XX";
                        else
                            return string.Format(AppResources.BookmarkNotDefined, pageRefField.Name);
                    }
                }
                else if (field is SectionField)
                {
                    number = this.fieldInfos.section;
                    if (number <= 0)
                        return "XX";
                }
                else if (field is PageField)
                {
                    number = this.fieldInfos.displayPageNr;
                    if (number <= 0)
                        return "XX";
                }
                else if (field is NumPagesField)
                {
                    number = this.fieldInfos.numPages;
                    if (number <= 0)
                        return "XXX";
                }
                else if (field is SectionPagesField)
                {
                    number = this.fieldInfos.sectionPages;
                    if (number <= 0)
                        return "XX";
                }
                return NumberFormatter.Format(number, ((NumericFieldBase)field).Format);
            }
            else if (field is DateField)
            {
                DateTime dt = (this.fieldInfos.date);
                if (dt == DateTime.MinValue)
                    dt = DateTime.Now;

                return this.fieldInfos.date.ToString(((DateField)field).Format);
            }
            else if (field is InfoField)
            {
                return GetDocumentInfo(((InfoField)field).Name);
            }
            else
                Debug.Assert(false, "Given parameter must be a rendered Field");

            return "";
        }

        string GetOutlineTitle()
        {
            ParagraphIterator iter = new ParagraphIterator(this.paragraph.Elements);
            iter = iter.GetFirstLeaf();

            bool ignoreBlank = true;
            string title = "";
            while (iter != null)
            {
                DocumentObject current = iter.Current;
                if (!ignoreBlank && (IsBlank(current) || IsTab(current) || IsLineBreak(current)))
                {
                    title += " ";
                    ignoreBlank = true;
                }
                else if (current is Text)
                {
                    title += ((Text)current).Content;
                    ignoreBlank = false;
                }
                else if (IsRenderedField(current))
                {
                    title += GetFieldValue(current);
                    ignoreBlank = false;
                }
                else if (IsSymbol(current))
                {
                    title += GetSymbol((Character)current);
                    ignoreBlank = false;
                }

                if (title.Length > 64)
                    break;
                iter = iter.GetNextLeaf();
            }
            return title;
        }

        /// <summary>
        /// Gets a layout info with only margin and break information set.
        /// It can be taken before the paragraph is formatted.
        /// </summary>
        /// <remarks>
        /// The following layout information is set properly:<br />
        /// MarginTop, MarginLeft, MarginRight, MarginBottom, KeepTogether, KeepWithNext, PagebreakBefore.
        /// </remarks>
        internal override LayoutInfo InitialLayoutInfo
        {
            get
            {
                LayoutInfo layoutInfo = new LayoutInfo();
                layoutInfo.PageBreakBefore = this.paragraph.Format.PageBreakBefore;
                layoutInfo.MarginTop = this.paragraph.Format.SpaceBefore.Point;
                layoutInfo.MarginBottom = this.paragraph.Format.SpaceAfter.Point;
                //Don't confuse margins with left or right indent.
                //Indents are invisible for the layouter.
                layoutInfo.MarginRight = 0;
                layoutInfo.MarginLeft = 0;
                layoutInfo.KeepTogether = this.paragraph.Format.KeepTogether;
                layoutInfo.KeepWithNext = this.paragraph.Format.KeepWithNext;
                return layoutInfo;
            }
        }

        /// <summary>
        /// Adjusts the current x position to the given tab stop if possible.
        /// </summary>
        /// <returns>True, if the text doesn't fit the line any more and the tab causes a line break.</returns>
        FormatResult FormatTab()
        {
            // For Tabs in Justified context
            if (this.paragraph.Format.Alignment == ParagraphAlignment.Justify)
                this.reMeasureLine = true;
            TabStop nextTabStop = GetNextTabStop();
            this.savedWordWidth = 0;
            if (nextTabStop == null)
                return FormatResult.NewLine;

            bool notFitting = false;
            XUnit xPositionBeforeTab = this.currentXPosition;
            switch (nextTabStop.Alignment)
            {
                case TabAlignment.Left:
                    this.currentXPosition = ProbeAfterLeftAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Right:
                    this.currentXPosition = ProbeAfterRightAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Center:
                    this.currentXPosition = ProbeAfterCenterAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;

                case TabAlignment.Decimal:
                    this.currentXPosition = ProbeAfterDecimalAlignedTab(nextTabStop.Position.Point, out notFitting);
                    break;
            }
            if (!notFitting)
            {
                // For correct right paragraph alignment with tabs
                if (!this.IgnoreHorizontalGrowth)
                    this.currentLineWidth += this.currentXPosition - xPositionBeforeTab;

                this.tabOffsets.Add(new TabOffset(nextTabStop.Leader, this.currentXPosition - xPositionBeforeTab));
                if (this.currentLeaf != null)
                    this.lastTab = this.currentLeaf.Current;
            }

            return notFitting ? FormatResult.NewLine : FormatResult.Continue;
        }

        bool IsLineBreak(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                if (((Character)docObj).SymbolName == SymbolName.LineBreak)
                    return true;
            }
            return false;
        }

        bool IsBlank(DocumentObject docObj)
        {
            if (docObj is Text)
            {
                if (((Text)docObj).Content == " ")
                    return true;
            }
            return false;
        }

        bool IsTab(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                if (((Character)docObj).SymbolName == SymbolName.Tab)
                    return true;
            }
            return false;
        }

        bool IsSoftHyphen(DocumentObject docObj)
        {
            Text text = docObj as Text;
            if (text != null)
                return text.Content == "­";

            return false;
        }

        /// <summary>
        /// Probes the paragraph elements after a left aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterLeftAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;
            this.currentXPosition = this.formattingArea.X + tabStopPosition.Point;

            notFitting = ProbeAfterTab();
            if (!notFitting)
                xPositionAfterTab = this.formattingArea.X + tabStopPosition;

            //--- Restore ---------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterRightAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;

            notFitting = ProbeAfterTab();
            if (!notFitting && xPosition + this.currentLineWidth <= this.formattingArea.X + tabStopPosition)
                xPositionAfterTab = this.formattingArea.X + tabStopPosition - this.currentLineWidth;

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        Hyperlink GetHyperlink()
        {
            DocumentObject elements = DocumentRelations.GetParent(this.currentLeaf.Current);
            DocumentObject parent = DocumentRelations.GetParent(elements);
            while (!(parent is Paragraph))
            {
                if (parent is Hyperlink)
                    return (Hyperlink)parent;
                elements = DocumentRelations.GetParent(parent);
                parent = DocumentRelations.GetParent(elements);
            }
            return null;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterCenterAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------

            XUnit xPositionAfterTab = xPosition;
            notFitting = ProbeAfterTab();

            if (!notFitting)
            {
                if (xPosition + this.currentLineWidth / 2.0 <= this.formattingArea.X + tabStopPosition)
                {
                    Rectangle rect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
                    if (this.formattingArea.X + tabStopPosition + this.currentLineWidth / 2.0 > rect.X + rect.Width - this.RightIndent)
                    {
                        //the text is too long on the right hand side of the tabstop => align to right indent.
                        xPositionAfterTab = rect.X +
                          rect.Width -
                          this.RightIndent -
                          this.currentLineWidth;
                    }
                    else
                        xPositionAfterTab = this.formattingArea.X + tabStopPosition - this.currentLineWidth / 2;
                }
            }

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            return xPositionAfterTab;
        }

        /// <summary>
        /// Probes the paragraph elements after a right aligned tab stop and returns the vertical text position to start at.
        /// </summary>
        /// <param name="tabStopPosition">Position of the tab to probe.</param>
        /// <param name="notFitting">Out parameter determining whether the tab causes a line break.</param>
        /// <returns>The new x-position to restart behind the tab.</returns>
        XUnit ProbeAfterDecimalAlignedTab(XUnit tabStopPosition, out bool notFitting)
        {
            notFitting = false;
            ParagraphIterator savedLeaf = this.currentLeaf;

            //Extra for auto tab after list symbol
            if (IsTab(this.currentLeaf.Current))
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            if (this.currentLeaf == null)
            {
                this.currentLeaf = savedLeaf;
                return this.currentXPosition + tabStopPosition;
            }
            VerticalLineInfo newVerticalInfo = CalcCurrentVerticalInfo();
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, newVerticalInfo.height);
            if (fittingRect == null)
            {
                notFitting = true;
                this.currentLeaf = savedLeaf;
                return this.currentXPosition;
            }

            if (IsPlainText(this.currentLeaf.Current))
            {
                Text text = (Text)this.currentLeaf.Current;
                string word = text.Content;
                int lastIndex = text.Content.LastIndexOfAny(new char[] { ',', '.' });
                if (lastIndex > 0)
                    word = word.Substring(0, lastIndex);

                XUnit wordLength = MeasureString(word);
                notFitting = this.currentXPosition + wordLength >= formattingArea.X + formattingArea.Width + Tolerance;
                if (!notFitting)
                    return this.formattingArea.X + tabStopPosition - wordLength;

                else
                    return this.currentXPosition;
            }
            this.currentLeaf = savedLeaf;
            return ProbeAfterRightAlignedTab(tabStopPosition, out notFitting);
        }

        void SaveBeforeProbing(out ParagraphIterator paragraphIter, out int blankCount, out XUnit wordsWidth, out XUnit xPosition, out XUnit lineWidth, out XUnit blankWidth)
        {
            paragraphIter = this.currentLeaf;
            blankCount = this.currentBlankCount;
            xPosition = this.currentXPosition;
            lineWidth = this.currentLineWidth;
            wordsWidth = this.currentWordsWidth;
            blankWidth = this.savedBlankWidth;
        }

        void RestoreAfterProbing(ParagraphIterator paragraphIter, int blankCount, XUnit wordsWidth, XUnit xPosition, XUnit lineWidth, XUnit blankWidth)
        {
            this.currentLeaf = paragraphIter;
            this.currentBlankCount = blankCount;
            this.currentXPosition = xPosition;
            this.currentLineWidth = lineWidth;
            this.currentWordsWidth = wordsWidth;
            this.savedBlankWidth = blankWidth;
        }

        /// <summary>
        /// Probes the paragraph after a tab.
        /// Caution: This Function resets the word count and line width before doing its work.
        /// </summary>
        /// <returns>True if the tab causes a linebreak.</returns>
        bool ProbeAfterTab()
        {
            this.currentLineWidth = 0;
            this.currentBlankCount = 0;
            //Extra for auto tab after list symbol

            //TODO: KLPO4KLPO: Check if this conditional statement is still required
            if (this.currentLeaf != null && IsTab(this.currentLeaf.Current))
                this.currentLeaf = this.currentLeaf.GetNextLeaf();

            bool wordAppeared = false;
            while (this.currentLeaf != null && !IsLineBreak(this.currentLeaf.Current) && !IsTab(this.currentLeaf.Current))
            {
                FormatResult result = FormatElement(this.currentLeaf.Current);
                if (result != FormatResult.Continue)
                    break;

                wordAppeared = wordAppeared || IsWordLikeElement(this.currentLeaf.Current);
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            return this.currentLeaf != null && !IsLineBreak(this.currentLeaf.Current) &&
              !IsTab(this.currentLeaf.Current) && !wordAppeared;
        }

        /// <summary>
        /// Gets the next tab stop following the current x position.
        /// </summary>
        /// <returns>The searched tab stop.</returns>
        private TabStop GetNextTabStop()
        {
            ParagraphFormat format = this.paragraph.Format;
            TabStops tabStops = format.TabStops;
            XUnit lastPosition = 0;

            foreach (TabStop tabStop in tabStops)
            {
                if (tabStop.Position.Point > this.formattingArea.Width - this.RightIndent + Tolerance)
                    break;

                if (tabStop.Position.Point + this.formattingArea.X > this.currentXPosition + Tolerance) // With Tolerance ...
                    return tabStop;

                lastPosition = tabStop.Position.Point;
            }
            //Automatic tab stop: FirstLineIndent < 0 => automatic tab stop at LeftIndent.

            if (format.FirstLineIndent < 0 || (!format.IsNull("ListInfo") && format.ListInfo.NumberPosition < format.LeftIndent))
            {
                XUnit leftIndent = format.LeftIndent.Point;
                if (this.isFirstLine && this.currentXPosition < leftIndent + this.formattingArea.X)
                    return new TabStop(leftIndent.Point);
            }
            XUnit defaultTabStop = "1.25cm";
            if (!this.paragraph.Document.IsNull("DefaultTabstop"))
                defaultTabStop = this.paragraph.Document.DefaultTabStop.Point;

            XUnit currTabPos = defaultTabStop;
            while (currTabPos + this.formattingArea.X <= this.formattingArea.Width - this.RightIndent)
            {
                if (currTabPos > lastPosition && currTabPos + this.formattingArea.X > this.currentXPosition + Tolerance)
                    return new TabStop(currTabPos.Point);

                currTabPos += defaultTabStop;
            }
            return null;
        }

        /// <summary>
        /// Gets the horizontal position to start a new line.
        /// </summary>
        /// <returns>The position to start the line.</returns>
        XUnit StartXPosition
        {
            get
            {
                XUnit xPos = 0;

                if (this.phase == Phase.Formatting)
                {
                    xPos = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height).X;
                    xPos += this.LeftIndent;
                }
                else //if (phase == Phase.Rendering)
                {
                    Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
                    //next lines for non fitting lines that produce an empty fitting rect:
                    XUnit rectX = contentArea.X;
                    XUnit rectWidth = contentArea.Width;

                    Rectangle fittingRect = contentArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
                    if (fittingRect != null)
                    {
                        rectX = fittingRect.X;
                        rectWidth = fittingRect.Width;
                    }
                    switch (this.paragraph.Format.Alignment)
                    {
                        case ParagraphAlignment.Left:
                        case ParagraphAlignment.Justify:
                            xPos = rectX;
                            xPos += this.LeftIndent;
                            break;

                        case ParagraphAlignment.Right:
                            xPos = rectX + rectWidth - this.RightIndent;
                            xPos -= this.currentLineWidth;
                            break;

                        case ParagraphAlignment.Center:
                            xPos = rectX + (rectWidth + this.LeftIndent - this.RightIndent - this.currentLineWidth) / 2.0;
                            break;
                    }
                }
                return xPos;
            }
        }

        /// <summary>
        /// Renders a single line.
        /// </summary>
        /// <param name="lineInfo"></param>
        void RenderLine(LineInfo lineInfo)
        {
            this.currentVerticalInfo = lineInfo.vertical;
            this.currentLeaf = lineInfo.startIter;
            this.startLeaf = lineInfo.startIter;
            this.endLeaf = lineInfo.endIter;
            this.currentBlankCount = lineInfo.blankCount;
            this.currentLineWidth = lineInfo.lineWidth;
            this.currentWordsWidth = lineInfo.wordsWidth;
            this.currentXPosition = this.StartXPosition;
            this.tabOffsets = lineInfo.tabOffsets;
            this.lastTabPassed = lineInfo.lastTab == null;
            this.lastTab = lineInfo.lastTab;

            this.tabIdx = 0;

            bool ready = this.currentLeaf == null;
            if (this.isFirstLine)
                RenderListSymbol();

            while (!ready)
            {
                if (this.currentLeaf.Current == lineInfo.endIter.Current)
                    ready = true;

                if (this.currentLeaf.Current == lineInfo.lastTab)
                    this.lastTabPassed = true;
                RenderElement(this.currentLeaf.Current);
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
            }
            this.currentYPosition += lineInfo.vertical.height;
            this.isFirstLine = false;
        }

        void ReMeasureLine(ref LineInfo lineInfo)
        {
            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            bool origLastTabPassed = this.lastTabPassed;
            //------------------------------------------
            this.currentLeaf = lineInfo.startIter;
            this.endLeaf = lineInfo.endIter;
            this.formattingArea = this.renderInfo.LayoutInfo.ContentArea;
            this.tabOffsets = new ArrayList();
            this.currentLineWidth = 0;
            this.currentWordsWidth = 0;

            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            if (fittingRect == null)
                GetType();
            if (fittingRect != null)
            {
                this.currentXPosition = fittingRect.X + this.LeftIndent;
                FormatListSymbol();
                bool goOn = true;
                while (goOn && this.currentLeaf != null)
                {
                    if (this.currentLeaf.Current == lineInfo.lastTab)
                        this.lastTabPassed = true;

                    FormatElement(this.currentLeaf.Current);

                    goOn = this.currentLeaf != null && this.currentLeaf.Current != this.endLeaf.Current;
                    if (goOn)
                        this.currentLeaf = this.currentLeaf.GetNextLeaf();
                }
                lineInfo.lineWidth = this.currentLineWidth;
                lineInfo.wordsWidth = this.currentWordsWidth;
                lineInfo.blankCount = this.currentBlankCount;
                lineInfo.tabOffsets = this.tabOffsets;
                lineInfo.reMeasureLine = false;
                this.lastTabPassed = origLastTabPassed;
            }
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
        }

        XUnit CurrentWordDistance
        {
            get
            {
                if (this.phase == Phase.Rendering &&
                  this.paragraph.Format.Alignment == ParagraphAlignment.Justify && this.lastTabPassed)
                {
                    if (this.currentBlankCount >= 1 && !(this.isLastLine && this.renderInfo.FormatInfo.IsEnding))
                    {
                        Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
                        XUnit width = contentArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height).Width;
                        if (this.lastTabPosition > 0)
                        {
                            width -= (this.lastTabPosition -
                            contentArea.X);
                        }
                        else
                            width -= this.LeftIndent;

                        width -= this.RightIndent;
                        return (width - this.currentWordsWidth) / (this.currentBlankCount);
                    }
                }
                return MeasureString(" ");
            }
        }

        void RenderElement(DocumentObject docObj)
        {
            string typeName = docObj.GetType().Name;
            switch (typeName)
            {
                case "Text":
                    if (IsBlank(docObj))
                        RenderBlank();
                    else if (IsSoftHyphen(docObj))
                        RenderSoftHyphen();
                    else
                        RenderText((Text)docObj);
                    break;

                case "Character":
                    RenderCharacter((Character)docObj);
                    break;

                case "DateField":
                    RenderDateField((DateField)docObj);
                    break;

                case "InfoField":
                    RenderInfoField((InfoField)docObj);
                    break;

                case "NumPagesField":
                    RenderNumPagesField((NumPagesField)docObj);
                    break;

                case "PageField":
                    RenderPageField((PageField)docObj);
                    break;

                case "SectionField":
                    RenderSectionField((SectionField)docObj);
                    break;

                case "SectionPagesField":
                    RenderSectionPagesField((SectionPagesField)docObj);
                    break;

                case "BookmarkField":
                    RenderBookmarkField();
                    break;

                case "PageRefField":
                    RenderPageRefField((PageRefField)docObj);
                    break;

                case "Image":
                    RenderImage((Image)docObj);
                    break;
                    //        default:
                    //          throw new NotImplementedException(typeName + " is coming soon...");
            }
        }

        void RenderImage(Image image)
        {
            RenderInfo renderInfo = this.CurrentImageRenderInfo;
            XUnit top = CurrentBaselinePosition;
            Area contentArea = renderInfo.LayoutInfo.ContentArea;
            top -= contentArea.Height;
            RenderByInfos(this.currentXPosition, top, new RenderInfo[] { renderInfo });

            RenderUnderline(contentArea.Width, true);
            RealizeHyperlink(contentArea.Width);

            this.currentXPosition += contentArea.Width;
        }

        void RenderDateField(DateField dateField)
        {
            RenderWord(this.fieldInfos.date.ToString(dateField.Format));
        }

        void RenderInfoField(InfoField infoField)
        {
            RenderWord(GetDocumentInfo(infoField.Name));
        }

        void RenderNumPagesField(NumPagesField numPagesField)
        {
            RenderWord(GetFieldValue(numPagesField));
        }

        void RenderPageField(PageField pageField)
        {
            RenderWord(GetFieldValue(pageField));
        }

        void RenderSectionField(SectionField sectionField)
        {
            RenderWord(GetFieldValue(sectionField));
        }

        void RenderSectionPagesField(SectionPagesField sectionPagesField)
        {
            RenderWord(GetFieldValue(sectionPagesField));
        }

        void RenderBookmarkField()
        {
            RenderUnderline(0, false);
        }

        void RenderPageRefField(PageRefField pageRefField)
        {
            RenderWord(GetFieldValue(pageRefField));
        }

        void RenderCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                case SymbolName.Em:
                case SymbolName.Em4:
                case SymbolName.En:
                    RenderSpace(character);
                    break;
                case SymbolName.LineBreak:
                    RenderLinebreak();
                    break;

                case SymbolName.Tab:
                    RenderTab();
                    break;

                default:
                    RenderSymbol(character);
                    break;
            }
        }

        void RenderSpace(Character character)
        {
            this.currentXPosition += GetSpaceWidth(character);
        }

        void RenderLinebreak()
        {
            this.RenderUnderline(0, false);
            this.RealizeHyperlink(0);
        }

        void RenderSymbol(Character character)
        {
            string sym = GetSymbol(character);
            string completeWord = sym;
            for (int idx = 1; idx < character.Count; ++idx)
                completeWord += sym;

            RenderWord(completeWord);
        }

        void RenderTab()
        {
            TabOffset tabOffset = NextTabOffset();
            RenderUnderline(tabOffset.offset, false);
            RenderTabLeader(tabOffset);
            RealizeHyperlink(tabOffset.offset);
            this.currentXPosition += tabOffset.offset;
            if (this.currentLeaf.Current == this.lastTab)
                this.lastTabPosition = this.currentXPosition;
        }

        void RenderTabLeader(TabOffset tabOffset)
        {
            string leaderString = " ";
            switch (tabOffset.leader)
            {
                case TabLeader.Dashes:
                    leaderString = "-";
                    break;

                case TabLeader.Dots:
                    leaderString = ".";
                    break;

                case TabLeader.Heavy:
                case TabLeader.Lines:
                    leaderString = "_";
                    break;

                case TabLeader.MiddleDot:
                    leaderString = "·";
                    break;

                default:
                    return;
            }
            XUnit leaderWidth = MeasureString(leaderString);
            XUnit xPosition = this.currentXPosition;
            string drawString = "";

            while (xPosition + leaderWidth <= this.currentXPosition + tabOffset.offset)
            {
                drawString += leaderString;
                xPosition += leaderWidth;
            }
            Font font = this.CurrentDomFont;
            XFont xFont = CurrentFont;
            if (font.Subscript || font.Superscript)
                xFont = FontHandler.ToSubSuperFont(xFont);

            this.gfx.DrawString(drawString, xFont, CurrentBrush, this.currentXPosition, CurrentBaselinePosition);
        }

        TabOffset NextTabOffset()
        {
#if false
      TabOffset offset =
        (TabOffset)this.tabOffsets[this.tabIdx];
#else
            TabOffset offset = this.tabOffsets.Count > this.tabIdx ?
              (TabOffset)this.tabOffsets[this.tabIdx] :
              new TabOffset(0, 0);
#endif
            ++this.tabIdx;
            return offset;
        }
        int tabIdx;

        bool IgnoreBlank()
        {
            if (this.currentLeaf == this.startLeaf)
                return true;

            if (this.endLeaf != null && this.currentLeaf.Current == this.endLeaf.Current)
                return true;

            ParagraphIterator nextIter = this.currentLeaf.GetNextLeaf();
            while (nextIter != null && (IsBlank(nextIter.Current) || nextIter.Current is BookmarkField))
            {
                nextIter = nextIter.GetNextLeaf();
            }
            if (nextIter == null)
                return true;

            if (IsTab(nextIter.Current))
                return true;

            ParagraphIterator prevIter = this.currentLeaf.GetPreviousLeaf();
            // Can be null if currentLeaf is the first leaf
            DocumentObject obj = prevIter != null ? prevIter.Current : null;
            while (obj != null && obj is BookmarkField)
            {
                prevIter = prevIter.GetPreviousLeaf();
                if (prevIter != null)
                    obj = prevIter.Current;
                else
                    obj = null;
            }
            if (obj == null)
                return true;

            return IsBlank(obj) || IsTab(obj);
        }

        void RenderBlank()
        {
            if (!IgnoreBlank())
            {
                XUnit wordDistance = this.CurrentWordDistance;
                RenderUnderline(wordDistance, false);
                RealizeHyperlink(wordDistance);
                this.currentXPosition += wordDistance;
            }
            else
            {
                RenderUnderline(0, false);
                RealizeHyperlink(0);
            }
        }

        void RenderSoftHyphen()
        {
            if (this.currentLeaf.Current == this.endLeaf.Current)
                RenderWord("-");
        }

        void RenderText(Text text)
        {
            RenderWord(text.Content);
        }

        void RenderWord(string word)
        {
            Font font = this.CurrentDomFont;
            XFont xFont = CurrentFont;
            if (font.Subscript || font.Superscript)
                xFont = FontHandler.ToSubSuperFont(xFont);

            this.gfx.DrawString(word, xFont, CurrentBrush, this.currentXPosition, CurrentBaselinePosition);
            XUnit wordWidth = MeasureString(word);
            RenderUnderline(wordWidth, true);
            RealizeHyperlink(wordWidth);
            this.currentXPosition += wordWidth;
        }

        void StartHyperlink(XUnit left, XUnit top)
        {
            this.hyperlinkRect = new XRect(left, top, 0, 0);
        }

        void EndHyperlink(Hyperlink hyperlink, XUnit right, XUnit bottom)
        {
            this.hyperlinkRect.Width = right - this.hyperlinkRect.X;
            this.hyperlinkRect.Height = bottom - this.hyperlinkRect.Y;
            PdfPage page = this.gfx.PdfPage;
            if (page != null)
            {
                XRect rect = this.gfx.Transformer.WorldToDefaultPage(this.hyperlinkRect);

                switch (hyperlink.Type)
                {
                    case HyperlinkType.Local:
                        int pageRef = this.fieldInfos.GetPhysicalPageNumber(hyperlink.Name);
                        if (pageRef > 0)
                            page.AddDocumentLink(new PdfRectangle(rect), pageRef);
                        break;

                    case HyperlinkType.Web:
                        page.AddWebLink(new PdfRectangle(rect), hyperlink.Name);
                        break;

                    case HyperlinkType.File:
                        page.AddFileLink(new PdfRectangle(rect), hyperlink.Name);
                        break;
                }
                this.hyperlinkRect = new XRect();
            }
        }

        void RealizeHyperlink(XUnit width)
        {
            XUnit top = this.currentYPosition;
            XUnit left = this.currentXPosition;
            XUnit bottom = top + this.currentVerticalInfo.height;
            XUnit right = left + width;
            Hyperlink hyperlink = GetHyperlink();

            bool hyperlinkChanged = this.currentHyperlink != hyperlink;

            if (hyperlinkChanged)
            {
                if (this.currentHyperlink != null)
                    EndHyperlink(this.currentHyperlink, left, bottom);

                if (hyperlink != null)
                    StartHyperlink(left, top);

                this.currentHyperlink = hyperlink;
            }

            if (this.currentLeaf.Current == this.endLeaf.Current)
            {
                if (this.currentHyperlink != null)
                    EndHyperlink(this.currentHyperlink, right, bottom);

                this.currentHyperlink = null;
            }
        }
        Hyperlink currentHyperlink;
        XRect hyperlinkRect;

        XUnit CurrentBaselinePosition
        {
            get
            {
                VerticalLineInfo verticalInfo = this.currentVerticalInfo;
                XUnit position = this.currentYPosition;

                Font font = CurrentDomFont;
                XFont xFont = CurrentFont;
                if (font.Subscript)
                {
                    position += verticalInfo.inherentlineSpace;
                    position -= FontHandler.GetSubSuperScaling(this.CurrentFont) * FontHandler.GetDescent(xFont);
                }
                else if (font.Superscript)
                {
                    position += FontHandler.GetSubSuperScaling(this.CurrentFont) * (xFont.GetHeight() - FontHandler.GetDescent(xFont));
                }
                else
                    position += verticalInfo.inherentlineSpace - verticalInfo.descent;

                return position;
            }
        }

        XBrush CurrentBrush
        {
            get
            {
                if (this.currentLeaf != null)
                    return FontHandler.FontColorToXBrush(CurrentDomFont);

                return null;
            }
        }

        private void InitRendering()
        {
            this.phase = Phase.Rendering;

            ParagraphFormatInfo parFormatInfo = (ParagraphFormatInfo)this.renderInfo.FormatInfo;
            if (parFormatInfo.LineCount == 0)
                return;
            this.isFirstLine = parFormatInfo.IsStarting;

            LineInfo lineInfo = parFormatInfo.GetFirstLineInfo();
            Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
            this.currentYPosition = contentArea.Y + TopBorderOffset;
            // StL: GetFittingRect liefert manchmal null
            Rectangle rect = contentArea.GetFittingRect(this.currentYPosition, lineInfo.vertical.height);
            if (rect != null)
                this.currentXPosition = rect.X;
            this.currentLineWidth = 0;
        }

        /// <summary>
        /// Initializes this instance for formatting.
        /// </summary>
        /// <param name="area">The area for formatting</param>
        /// <param name="previousFormatInfo">A previous format info.</param>
        /// <returns>False, if nothing of the paragraph will fit the area any more.</returns>
        private bool InitFormat(Area area, FormatInfo previousFormatInfo)
        {
            this.phase = Phase.Formatting;

            this.tabOffsets = new ArrayList();

            ParagraphFormatInfo prevParaFormatInfo = (ParagraphFormatInfo)previousFormatInfo;
            if (previousFormatInfo == null || prevParaFormatInfo.LineCount == 0)
            {
                ((ParagraphFormatInfo)this.renderInfo.FormatInfo).isStarting = true;
                ParagraphIterator parIt = new ParagraphIterator(this.paragraph.Elements);
                this.currentLeaf = parIt.GetFirstLeaf();
                this.isFirstLine = true;
            }
            else
            {
                this.currentLeaf = prevParaFormatInfo.GetLastLineInfo().endIter.GetNextLeaf();
                this.isFirstLine = false;
                ((ParagraphFormatInfo)this.renderInfo.FormatInfo).isStarting = false;
            }

            this.startLeaf = this.currentLeaf;
            this.currentVerticalInfo = CalcCurrentVerticalInfo();
            this.currentYPosition = area.Y + TopBorderOffset;
            this.formattingArea = area;
            Rectangle rect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            if (rect == null)
                return false;

            this.currentXPosition = rect.X + LeftIndent;
            if (this.isFirstLine)
                FormatListSymbol();

            return true;
        }

        /// <summary>
        /// Gets information necessary to render or measure the list symbol.
        /// </summary>
        /// <param name="symbol">The text to list symbol to render or measure</param>
        /// <param name="font">The font to use for rendering or measuring.</param>
        /// <returns>True, if a symbol needs to be rendered.</returns>
        bool GetListSymbol(out string symbol, out XFont font)
        {
            font = null;
            symbol = null;
            ParagraphFormatInfo formatInfo = (ParagraphFormatInfo)this.renderInfo.FormatInfo;
            if (this.phase == Phase.Formatting)
            {
                ParagraphFormat format = this.paragraph.Format;
                if (!format.IsNull("ListInfo"))
                {
                    ListInfo listInfo = format.ListInfo;
                    double size = format.Font.Size;
                    XFontStyle style = FontHandler.GetXStyle(format.Font);

                    switch (listInfo.ListType)
                    {
                        case ListType.BulletList1:
                            symbol = "·";
                            font = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, size, style);
                            break;

                        case ListType.BulletList2:
                            symbol = "o";
                            font = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, size, style);
                            break;

                        case ListType.BulletList3:
                            symbol = "§";
                            font = new XFont(GlobalFontSettings.FontResolver.DefaultFontName, size, style);
                            break;

                        case ListType.NumberList1:
                            symbol = this.documentRenderer.NextListNumber(listInfo) + ".";
                            font = FontHandler.FontToXFont(format.Font, this.documentRenderer.PrivateFonts, this.gfx.MUH);
                            break;

                        case ListType.NumberList2:
                            symbol = this.documentRenderer.NextListNumber(listInfo) + ")";
                            font = FontHandler.FontToXFont(format.Font, this.documentRenderer.PrivateFonts, this.gfx.MUH);
                            break;

                        case ListType.NumberList3:
                            symbol = NumberFormatter.Format(this.documentRenderer.NextListNumber(listInfo), "alphabetic") + ")";
                            font = FontHandler.FontToXFont(format.Font, this.documentRenderer.PrivateFonts, this.gfx.MUH);
                            break;
                    }
                    formatInfo.listFont = font;
                    formatInfo.listSymbol = symbol;
                    return true;
                }
            }
            else
            {
                if (formatInfo.listFont != null && formatInfo.listSymbol != null)
                {
                    font = formatInfo.listFont;
                    symbol = formatInfo.listSymbol;
                    return true;
                }
            }
            return false;
        }

        XUnit LeftIndent
        {
            get
            {
                ParagraphFormat format = this.paragraph.Format;
                XUnit leftIndent = format.LeftIndent.Point;
                if (this.isFirstLine)
                {
                    if (!format.IsNull("ListInfo"))
                    {
                        if (!format.ListInfo.IsNull("NumberPosition"))
                            return format.ListInfo.NumberPosition.Point;
                        else if (format.IsNull("FirstLineIndent"))
                            return 0;
                    }
                    return leftIndent + this.paragraph.Format.FirstLineIndent.Point;
                }
                else
                    return leftIndent;
            }
        }

        XUnit RightIndent
        {
            get
            {
                return this.paragraph.Format.RightIndent.Point;
            }
        }

        /// <summary>
        /// Formats the paragraph by performing line breaks etc.
        /// </summary>
        /// <param name="area">The area in which to render.</param>
        /// <param name="previousFormatInfo">The format info that was obtained on formatting the same paragraph on a previous area.</param>
        internal override void Format(Area area, FormatInfo previousFormatInfo)
        {
            ParagraphFormatInfo formatInfo = ((ParagraphFormatInfo)this.renderInfo.FormatInfo);
            if (!InitFormat(area, previousFormatInfo))
            {
                formatInfo.isStarting = false;
                return;
            }
            formatInfo.isEnding = true;

            FormatResult lastResult = FormatResult.Continue;
            while (this.currentLeaf != null)
            {
                FormatResult result = FormatElement(this.currentLeaf.Current);
                switch (result)
                {
                    case FormatResult.Ignore:
                        this.currentLeaf = this.currentLeaf.GetNextLeaf();
                        break;

                    case FormatResult.Continue:
                        lastResult = result;
                        this.currentLeaf = this.currentLeaf.GetNextLeaf();
                        break;

                    case FormatResult.NewLine:
                        lastResult = result;
                        StoreLineInformation();
                        if (!StartNewLine())
                        {
                            result = FormatResult.NewArea;
                            formatInfo.isEnding = false;
                        }
                        break;
                }
                if (result == FormatResult.NewArea)
                {
                    lastResult = result;
                    formatInfo.isEnding = false;
                    break;
                }
            }
            if (formatInfo.IsEnding && lastResult != FormatResult.NewLine)
                StoreLineInformation();

            formatInfo.imageRenderInfos = this.imageRenderInfos;
            FinishLayoutInfo();
        }

        /// <summary>
        /// Finishes the layout info by calculating starting and trailing heights.
        /// </summary>
        private void FinishLayoutInfo()
        {
            LayoutInfo layoutInfo = this.renderInfo.LayoutInfo;
            ParagraphFormat format = this.paragraph.Format;
            ParagraphFormatInfo parInfo = (ParagraphFormatInfo)this.renderInfo.FormatInfo;
            layoutInfo.MinWidth = this.minWidth;
            layoutInfo.KeepTogether = format.KeepTogether;

            if (parInfo.IsComplete)
            {
                int limitOfLines = 1;
                if (parInfo.widowControl)
                    limitOfLines = 3;

                if (parInfo.LineCount <= limitOfLines)
                    layoutInfo.KeepTogether = true;
            }
            if (parInfo.IsStarting)
            {
                layoutInfo.MarginTop = format.SpaceBefore.Point;
                layoutInfo.PageBreakBefore = format.PageBreakBefore;
            }
            else
            {
                layoutInfo.MarginTop = 0;
                layoutInfo.PageBreakBefore = false;
            }

            if (parInfo.IsEnding)
            {
                layoutInfo.MarginBottom = this.paragraph.Format.SpaceAfter.Point;
                layoutInfo.KeepWithNext = this.paragraph.Format.KeepWithNext;
            }
            else
            {
                layoutInfo.MarginBottom = 0;
                layoutInfo.KeepWithNext = false;
            }
            if (parInfo.LineCount > 0)
            {
                XUnit startingHeight = parInfo.GetFirstLineInfo().vertical.height;
                if (parInfo.isStarting && this.paragraph.Format.WidowControl && parInfo.LineCount >= 2)
                    startingHeight += parInfo.GetLineInfo(1).vertical.height;

                layoutInfo.StartingHeight = startingHeight;

                XUnit trailingHeight = parInfo.GetLastLineInfo().vertical.height;

                if (parInfo.IsEnding && this.paragraph.Format.WidowControl && parInfo.LineCount >= 2)
                    trailingHeight += parInfo.GetLineInfo(parInfo.LineCount - 2).vertical.height;

                layoutInfo.TrailingHeight = trailingHeight;
            }
        }


        private XUnit PopSavedBlankWidth()
        {
            XUnit width = this.savedBlankWidth;
            this.savedBlankWidth = 0;
            return width;
        }

        private void SaveBlankWidth(XUnit blankWidth)
        {
            this.savedBlankWidth = blankWidth;
        }
        private XUnit savedBlankWidth = 0;

        /// <summary>
        /// Processes the elements when formatting.
        /// </summary>
        /// <param name="docObj"></param>
        /// <returns></returns>
        FormatResult FormatElement(DocumentObject docObj)
        {
            switch (docObj.GetType().Name)
            {
                case "Text":
                    if (IsBlank(docObj))
                        return FormatBlank();
                    else if (IsSoftHyphen(docObj))
                        return FormatSoftHyphen();
                    else
                        return FormatText((Text)docObj);

                case "Character":
                    return FormatCharacter((Character)docObj);

                case "DateField":
                    return FormatDateField((DateField)docObj);

                case "InfoField":
                    return FormatInfoField((InfoField)docObj);

                case "NumPagesField":
                    return FormatNumPagesField((NumPagesField)docObj);

                case "PageField":
                    return FormatPageField((PageField)docObj);

                case "SectionField":
                    return FormatSectionField((SectionField)docObj);

                case "SectionPagesField":
                    return FormatSectionPagesField((SectionPagesField)docObj);

                case "BookmarkField":
                    return FormatBookmarkField((BookmarkField)docObj);

                case "PageRefField":
                    return FormatPageRefField((PageRefField)docObj);

                case "Image":
                    return FormatImage((Image)docObj);

                default:
                    return FormatResult.Continue;
            }
        }

        FormatResult FormatImage(Image image)
        {
            XUnit width = this.CurrentImageRenderInfo.LayoutInfo.ContentArea.Width;
            return FormatAsWord(width);
        }

        RenderInfo CalcImageRenderInfo(Image image)
        {
            Renderer renderer = Create(this.gfx, this.documentRenderer, image, this.fieldInfos);
            renderer.Format(new Rectangle(0, 0, double.MaxValue, double.MaxValue), null);

            return renderer.RenderInfo;
        }

        bool IsPlainText(DocumentObject docObj)
        {
            if (docObj is Text)
                return !IsSoftHyphen(docObj) && !IsBlank(docObj);

            return false;
        }

        bool IsSymbol(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                return !IsSpaceCharacter(docObj) && !IsTab(docObj) && !IsLineBreak(docObj);
            }
            return false;
        }

        bool IsSpaceCharacter(DocumentObject docObj)
        {
            if (docObj is Character)
            {
                switch (((Character)docObj).SymbolName)
                {
                    case SymbolName.Blank:
                    case SymbolName.Em:
                    case SymbolName.Em4:
                    case SymbolName.En:
                        return true;
                }
            }
            return false;
        }

        bool IsWordLikeElement(DocumentObject docObj)
        {
            if (IsPlainText(docObj))
                return true;

            if (IsRenderedField(docObj))
                return true;

            if (IsSymbol(docObj))
                return true;


            return false;
        }

        FormatResult FormatBookmarkField(BookmarkField bookmarkField)
        {
            this.fieldInfos.AddBookmark(bookmarkField.Name);
            return FormatResult.Ignore;
        }

        FormatResult FormatPageRefField(PageRefField pageRefField)
        {
            this.reMeasureLine = true;
            string fieldValue = GetFieldValue(pageRefField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatNumPagesField(NumPagesField numPagesField)
        {
            this.reMeasureLine = true;
            string fieldValue = GetFieldValue(numPagesField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatPageField(PageField pageField)
        {
            this.reMeasureLine = true;
            string fieldValue = GetFieldValue(pageField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatSectionField(SectionField sectionField)
        {
            this.reMeasureLine = true;
            string fieldValue = GetFieldValue(sectionField);
            return FormatWord(fieldValue);
        }

        FormatResult FormatSectionPagesField(SectionPagesField sectionPagesField)
        {
            this.reMeasureLine = true;
            string fieldValue = GetFieldValue(sectionPagesField);
            return FormatWord(fieldValue);
        }

        /// <summary>
        /// Helper function for formatting word-like elements like text and fields.
        /// </summary>
        FormatResult FormatWord(string word)
        {
            XUnit width = MeasureString(word);
            return FormatAsWord(width);
        }

        XUnit savedWordWidth = 0;

        /// <summary>
        /// When rendering a justified paragraph, only the part after the last tab stop needs remeasuring.
        /// </summary>
        private bool IgnoreHorizontalGrowth
        {
            get
            {
                return this.phase == Phase.Rendering && this.paragraph.Format.Alignment == ParagraphAlignment.Justify &&
                    !this.lastTabPassed;
            }
        }

        FormatResult FormatAsWord(XUnit width)
        {
            VerticalLineInfo newVertInfo = CalcCurrentVerticalInfo();

            Rectangle rect = this.formattingArea.GetFittingRect(this.currentYPosition, newVertInfo.height + BottomBorderOffset);
            if (rect == null)
                return FormatResult.NewArea;

            if (this.currentXPosition + width <= rect.X + rect.Width - this.RightIndent + Tolerance)
            {
                this.savedWordWidth = width;
                this.currentXPosition += width;
                // For Tabs in justified context
                if (!this.IgnoreHorizontalGrowth)
                    this.currentWordsWidth += width;
                if (this.savedBlankWidth > 0)
                {
                    // For Tabs in justified context
                    if (!this.IgnoreHorizontalGrowth)
                        ++this.currentBlankCount;
                }
                // For Tabs in justified context
                if (!this.IgnoreHorizontalGrowth)
                    this.currentLineWidth += width + PopSavedBlankWidth();
                this.currentVerticalInfo = newVertInfo;
                this.minWidth = Math.Max(this.minWidth, width);
                return FormatResult.Continue;
            }
            else
            {
                savedWordWidth = width;
                return FormatResult.NewLine;
            }
        }

        FormatResult FormatDateField(DateField dateField)
        {
            this.reMeasureLine = true;
            string estimatedFieldValue = DateTime.Now.ToString(dateField.Format);
            return FormatWord(estimatedFieldValue);
        }

        FormatResult FormatInfoField(InfoField infoField)
        {
            string fieldValue = GetDocumentInfo(infoField.Name);
            if (fieldValue != "")
                return FormatWord(fieldValue);

            return FormatResult.Continue;
        }

        string GetDocumentInfo(string name)
        {
            string docInfoValue = "";
            string[] enumNames = Enum.GetNames(typeof(InfoFieldType));
            foreach (string enumName in enumNames)
            {
                if (String.Compare(name, enumName, true) == 0)
                {
                    docInfoValue = paragraph.Document.Info.GetValue(enumName).ToString();
                    break;
                }
            }
            return docInfoValue;
        }

        Area GetShadingArea()
        {
            Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
            ParagraphFormat format = this.paragraph.Format;
            XUnit left = contentArea.X;
            left += format.LeftIndent;
            if (format.FirstLineIndent < 0)
                left += format.FirstLineIndent;

            XUnit top = contentArea.Y;
            XUnit bottom = contentArea.Y + contentArea.Height;
            XUnit right = contentArea.X + contentArea.Width;
            right -= format.RightIndent;

            if (!this.paragraph.Format.IsNull("Borders"))
            {
                Borders borders = format.Borders;
                BordersRenderer bordersRenderer = new BordersRenderer(borders, this.gfx);

                if (this.renderInfo.FormatInfo.IsStarting)
                    top += bordersRenderer.GetWidth(BorderType.Top);
                if (this.renderInfo.FormatInfo.IsEnding)
                    bottom -= bordersRenderer.GetWidth(BorderType.Bottom);

                left -= borders.DistanceFromLeft;
                right += borders.DistanceFromRight;
            }
            return new Rectangle(left, top, right - left, bottom - top);
        }

        void RenderShading()
        {
            if (this.paragraph.Format.IsNull("Shading"))
                return;

            ShadingRenderer shadingRenderer = new ShadingRenderer(this.gfx, this.paragraph.Format.Shading);
            Area area = GetShadingArea();

            shadingRenderer.Render(area.X, area.Y, area.Width, area.Height);
        }


        void RenderBorders()
        {
            if (this.paragraph.Format.IsNull("Borders"))
                return;

            Area shadingArea = GetShadingArea();
            XUnit left = shadingArea.X;
            XUnit top = shadingArea.Y;
            XUnit bottom = shadingArea.Y + shadingArea.Height;
            XUnit right = shadingArea.X + shadingArea.Width;

            Borders borders = this.paragraph.Format.Borders;
            BordersRenderer bordersRenderer = new BordersRenderer(borders, this.gfx);
            XUnit borderWidth = bordersRenderer.GetWidth(BorderType.Left);
            if (borderWidth > 0)
            {
                left -= borderWidth;
                bordersRenderer.RenderVertically(BorderType.Left, left, top, bottom - top);
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Right);
            if (borderWidth > 0)
            {
                bordersRenderer.RenderVertically(BorderType.Right, right, top, bottom - top);
                right += borderWidth;
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Top);
            if (this.renderInfo.FormatInfo.IsStarting && borderWidth > 0)
            {
                top -= borderWidth;
                bordersRenderer.RenderHorizontally(BorderType.Top, left, top, right - left);
            }

            borderWidth = bordersRenderer.GetWidth(BorderType.Bottom);
            if (this.renderInfo.FormatInfo.IsEnding && borderWidth > 0)
            {
                bordersRenderer.RenderHorizontally(BorderType.Bottom, left, bottom, right - left);
            }
        }

        XUnit MeasureString(string word)
        {
            XFont xFont = CurrentFont;
            XUnit width = this.gfx.MeasureString(word, xFont, StringFormat).Width;
            Font font = CurrentDomFont;

            if (font.Subscript || font.Superscript)
                width *= FontHandler.GetSubSuperScaling(xFont);

            return width;
        }

        XUnit GetSpaceWidth(Character character)
        {
            XUnit width = 0;
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                    width = MeasureString(" ");
                    break;
                case SymbolName.Em:
                    width = MeasureString("m");
                    break;
                case SymbolName.Em4:
                    width = 0.25 * MeasureString("m");
                    break;
                case SymbolName.En:
                    width = MeasureString("n");
                    break;
            }
            return width * character.Count;
        }

        void RenderListSymbol()
        {
            string symbol;
            XFont font;
            if (GetListSymbol(out symbol, out font))
            {
                XBrush brush = FontHandler.FontColorToXBrush(this.paragraph.Format.Font);
                this.gfx.DrawString(symbol, font, brush, this.currentXPosition, CurrentBaselinePosition);
                this.currentXPosition += this.gfx.MeasureString(symbol, font, StringFormat).Width;
                TabOffset tabOffset = NextTabOffset();
                this.currentXPosition += tabOffset.offset;
                this.lastTabPosition = this.currentXPosition;
            }
        }

        void FormatListSymbol()
        {
            string symbol;
            XFont font;
            if (GetListSymbol(out symbol, out font))
            {
                this.currentVerticalInfo = CalcVerticalInfo(font);
                this.currentXPosition += this.gfx.MeasureString(symbol, font, StringFormat).Width;
                FormatTab();
            }
        }

        FormatResult FormatSpace(Character character)
        {
            XUnit width = GetSpaceWidth(character);
            return FormatAsWord(width);
        }

        static string GetSymbol(Character character)
        {
            char ch;
            switch (character.SymbolName)
            {
                case SymbolName.Euro:
                    ch = '€';
                    break;

                case SymbolName.Copyright:
                    ch = '©';
                    break;

                case SymbolName.Trademark:
                    ch = '™';
                    break;

                case SymbolName.RegisteredTrademark:
                    ch = '®';
                    break;

                case SymbolName.Bullet:
                    ch = '•';
                    break;

                case SymbolName.Not:
                    ch = '¬';
                    break;
                //REM: Non-breakable blanks are still ignored.
                //        case SymbolName.SymbolNonBreakableBlank:
                //          return "\xA0";
                //          break;

                case SymbolName.EmDash:
                    ch = '—';
                    break;

                case SymbolName.EnDash:
                    ch = '–';
                    break;

                default:
                    char c = character.Char;
                    char[] chars = System.Text.Encoding.UTF8.GetChars(new byte[] { (byte)c });
                    ch = chars[0];
                    break;
            }
            string returnString = "";
            returnString += ch;
            int count = character.Count;
            while (--count > 0)
                returnString += ch;
            return returnString;
        }

        FormatResult FormatSymbol(Character character)
        {
            return FormatWord(GetSymbol(character));
        }

        /// <summary>
        /// Processes (measures) a special character within text.
        /// </summary>
        /// <param name="character">The character to process.</param>
        /// <returns>True if the character should start at a new line.</returns>
        FormatResult FormatCharacter(Character character)
        {
            switch (character.SymbolName)
            {
                case SymbolName.Blank:
                case SymbolName.Em:
                case SymbolName.Em4:
                case SymbolName.En:
                    return FormatSpace(character);

                case SymbolName.LineBreak:
                    return FormatLineBreak();

                case SymbolName.Tab:
                    return FormatTab();

                default:
                    return FormatSymbol(character);
            }
        }

        /// <summary>
        /// Processes (measures) a blank.
        /// </summary>
        /// <returns>True if the blank causes a line break.</returns>
        FormatResult FormatBlank()
        {
            if (IgnoreBlank())
                return FormatResult.Ignore;

            this.savedWordWidth = 0;
            XUnit width = MeasureString(" ");
            VerticalLineInfo newVertInfo = CalcCurrentVerticalInfo();
            Rectangle rect = this.formattingArea.GetFittingRect(this.currentYPosition, newVertInfo.height + BottomBorderOffset);
            if (rect == null)
                return FormatResult.NewArea;

            if (width + currentXPosition <= rect.X + rect.Width + Tolerance)
            {
                this.currentXPosition += width;
                this.currentVerticalInfo = newVertInfo;
                SaveBlankWidth(width);
                return FormatResult.Continue;
            }
            return FormatResult.NewLine;
        }

        FormatResult FormatLineBreak()
        {
            if (this.phase != Phase.Rendering)
                this.currentLeaf = this.currentLeaf.GetNextLeaf();

            this.savedWordWidth = 0;
            return FormatResult.NewLine;
        }

        /// <summary>
        /// Processes a text element during formatting.
        /// </summary>
        /// <param name="text">The text element to measure.</param>
        FormatResult FormatText(Text text)
        {
            return FormatWord(text.Content);
        }

        FormatResult FormatSoftHyphen()
        {
            if (this.currentLeaf.Current == this.startLeaf.Current)
                return FormatResult.Continue;

            ParagraphIterator nextIter = this.currentLeaf.GetNextLeaf();
            ParagraphIterator prevIter = this.currentLeaf.GetPreviousLeaf();
            if (!IsWordLikeElement(prevIter.Current) || !IsWordLikeElement(nextIter.Current))
                return FormatResult.Continue;

            //--- Save ---------------------------------
            ParagraphIterator iter;
            int blankCount;
            XUnit xPosition;
            XUnit lineWidth;
            XUnit wordsWidth;
            XUnit blankWidth;
            SaveBeforeProbing(out iter, out blankCount, out wordsWidth, out xPosition, out lineWidth, out blankWidth);
            //------------------------------------------
            this.currentLeaf = nextIter;
            FormatResult result = FormatElement(nextIter.Current);

            //--- Restore ------------------------------
            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            //------------------------------------------
            if (result == FormatResult.Continue)
                return FormatResult.Continue;

            RestoreAfterProbing(iter, blankCount, wordsWidth, xPosition, lineWidth, blankWidth);
            Rectangle fittingRect = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);

            XUnit hyphenWidth = MeasureString("-");
            if (xPosition + hyphenWidth <= fittingRect.X + fittingRect.Width + Tolerance
                // If one word fits, but not the hyphen, the formatting must continue with the next leaf
                || prevIter.Current == this.startLeaf.Current)
            {
                // For Tabs in justified context
                if (!IgnoreHorizontalGrowth)
                {
                    this.currentWordsWidth += hyphenWidth;
                    this.currentLineWidth += hyphenWidth;
                }
                this.currentLeaf = nextIter;
                return FormatResult.NewLine;
            }
            else
            {
                this.currentWordsWidth -= this.savedWordWidth;
                this.currentLineWidth -= this.savedWordWidth;
                this.currentLineWidth -= GetPreviousBlankWidth(prevIter);
                this.currentLeaf = prevIter;
                return FormatResult.NewLine;
            }
        }

        XUnit GetPreviousBlankWidth(ParagraphIterator beforeIter)
        {
            XUnit width = 0;
            ParagraphIterator savedIter = this.currentLeaf;
            this.currentLeaf = beforeIter.GetPreviousLeaf();
            while (this.currentLeaf != null)
            {
                if (this.currentLeaf.Current is BookmarkField)
                    this.currentLeaf = this.currentLeaf.GetPreviousLeaf();
                else if (IsBlank(this.currentLeaf.Current))
                {
                    if (!IgnoreBlank())
                        width = CurrentWordDistance;

                    break;
                }
                else
                    break;
            }
            this.currentLeaf = savedIter;
            return width;
        }

        void HandleNonFittingLine()
        {
            if (this.currentLeaf != null)
            {
                if (this.savedWordWidth > 0)
                {
                    this.currentWordsWidth = this.savedWordWidth;
                    this.currentLineWidth = this.savedWordWidth;
                }
                this.currentLeaf = this.currentLeaf.GetNextLeaf();
                this.currentYPosition += this.currentVerticalInfo.height;
                this.currentVerticalInfo = new VerticalLineInfo();
            }
        }

        /// <summary>
        /// Starts a new line by resetting measuring values.
        /// Do not call before the first first line is formatted!
        /// </summary>
        /// <returns>True, if the new line may fit the formatting area.</returns>
        bool StartNewLine()
        {
            this.tabOffsets = new ArrayList();
            this.lastTab = null;
            this.lastTabPosition = 0;
            this.currentYPosition += this.currentVerticalInfo.height;
#if true
            Rectangle rect = this.formattingArea.GetFittingRect(currentYPosition, this.currentVerticalInfo.height + BottomBorderOffset);
            if (rect == null)
                return false;

            this.isFirstLine = false;
            this.currentXPosition = StartXPosition; // depends on "currentVerticalInfo"
            this.currentVerticalInfo = new VerticalLineInfo();
            this.currentVerticalInfo = CalcCurrentVerticalInfo();
#else
      if (this.formattingArea.GetFittingRect(currentYPosition, this.currentVerticalInfo.height + BottomBorderOffset) == null)
        return false;

      this.currentVerticalInfo = new VerticalLineInfo();
      this.currentVerticalInfo = CalcCurrentVerticalInfo();
      this.isFirstLine = false;
      this.currentXPosition = this.StartXPosition;
#endif
            this.startLeaf = this.currentLeaf;
            this.currentBlankCount = 0;
            this.currentWordsWidth = 0;
            this.currentLineWidth = 0;
            return true;
        }
        /// <summary>
        /// Stores all line information.
        /// </summary>
        void StoreLineInformation()
        {
            PopSavedBlankWidth();

            XUnit topBorderOffset = TopBorderOffset;
            Area contentArea = this.renderInfo.LayoutInfo.ContentArea;
            if (topBorderOffset > 0)//May only occure for the first line.
                contentArea = this.formattingArea.GetFittingRect(this.formattingArea.Y, topBorderOffset);

            if (contentArea == null)
            {
                contentArea = this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height);
            }
            else
                contentArea = contentArea.Unite(this.formattingArea.GetFittingRect(this.currentYPosition, this.currentVerticalInfo.height));

            XUnit bottomBorderOffset = this.BottomBorderOffset;
            if (bottomBorderOffset > 0)
                contentArea = contentArea.Unite(this.formattingArea.GetFittingRect(this.currentYPosition + this.currentVerticalInfo.height, bottomBorderOffset));

            LineInfo lineInfo = new LineInfo();
            lineInfo.vertical = this.currentVerticalInfo;

            if (this.startLeaf != null && this.startLeaf == this.currentLeaf)
                HandleNonFittingLine();

            lineInfo.lastTab = this.lastTab;
            this.renderInfo.LayoutInfo.ContentArea = contentArea;

            lineInfo.startIter = this.startLeaf;

            if (this.currentLeaf == null)
                lineInfo.endIter = new ParagraphIterator(this.paragraph.Elements).GetLastLeaf();
            else
                lineInfo.endIter = this.currentLeaf.GetPreviousLeaf();

            lineInfo.blankCount = this.currentBlankCount;

            lineInfo.wordsWidth = this.currentWordsWidth;

            lineInfo.lineWidth = this.currentLineWidth;
            lineInfo.tabOffsets = this.tabOffsets;
            lineInfo.reMeasureLine = this.reMeasureLine;

            this.savedWordWidth = 0;
            this.reMeasureLine = false;
            ((ParagraphFormatInfo)this.renderInfo.FormatInfo).AddLineInfo(lineInfo);
        }

        /// <summary>
        /// Gets the top border offset for the first line, else 0.
        /// </summary>
        XUnit TopBorderOffset
        {
            get
            {
                XUnit offset = 0;
                if (this.isFirstLine && !this.paragraph.Format.IsNull("Borders"))
                {
                    offset += paragraph.Format.Borders.DistanceFromTop;
                    if (!paragraph.Format.IsNull("Borders"))
                    {
                        BordersRenderer bordersRenderer = new BordersRenderer(paragraph.Format.Borders, this.gfx);
                        offset += bordersRenderer.GetWidth(BorderType.Top);
                    }
                }
                return offset;
            }
        }

        bool IsLastVisibleLeaf
        {
            get
            {
                // REM: Code is missing here for blanks, bookmarks etc. which might be invisible.
                if (this.currentLeaf.IsLastLeaf)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// Gets the bottom border offset for the last line, else 0.
        /// </summary>
        XUnit BottomBorderOffset
        {
            get
            {
                XUnit offset = 0;
                //while formatting, it is impossible to determine whether we are in the last line until the last visible leaf is reached.
                if ((this.phase == Phase.Formatting && (this.currentLeaf == null || this.IsLastVisibleLeaf))
                  || (this.phase == Phase.Rendering && (this.isLastLine)))
                {
                    if (!this.paragraph.Format.IsNull("Borders"))
                    {
                        offset += paragraph.Format.Borders.DistanceFromBottom;
                        BordersRenderer bordersRenderer = new BordersRenderer(paragraph.Format.Borders, this.gfx);
                        offset += bordersRenderer.GetWidth(BorderType.Bottom);
                    }
                }
                return offset;
            }
        }

        VerticalLineInfo CalcCurrentVerticalInfo()
        {
            return CalcVerticalInfo(this.CurrentFont);
        }

        VerticalLineInfo CalcVerticalInfo(XFont font)
        {
            ParagraphFormat paragraphFormat = this.paragraph.Format;
            LineSpacingRule spacingRule = paragraphFormat.LineSpacingRule;
            XUnit lineHeight = 0;

            XUnit descent = FontHandler.GetDescent(font);
            descent = Math.Max(this.currentVerticalInfo.descent, descent);

            XUnit singleLineSpace = font.GetHeight();
            RenderInfo imageRenderInfo = this.CurrentImageRenderInfo;
            if (imageRenderInfo != null)
                singleLineSpace = singleLineSpace - FontHandler.GetAscent(font) + imageRenderInfo.LayoutInfo.ContentArea.Height;

            XUnit inherentLineSpace = Math.Max(this.currentVerticalInfo.inherentlineSpace, singleLineSpace);
            switch (spacingRule)
            {
                case LineSpacingRule.Single:
                    lineHeight = singleLineSpace;
                    break;

                case LineSpacingRule.OnePtFive:
                    lineHeight = 1.5 * singleLineSpace;
                    break;

                case LineSpacingRule.Double:
                    lineHeight = 2.0 * singleLineSpace;
                    break;

                case LineSpacingRule.Multiple:
                    lineHeight = this.paragraph.Format.LineSpacing * singleLineSpace;
                    break;

                case LineSpacingRule.AtLeast:
                    lineHeight = Math.Max(singleLineSpace, paragraph.Format.LineSpacing);
                    break;

                case LineSpacingRule.Exactly:
                    lineHeight = new XUnit(paragraph.Format.LineSpacing);
                    inherentLineSpace = paragraph.Format.LineSpacing.Point;
                    break;
            }
            lineHeight = Math.Max(this.currentVerticalInfo.height, lineHeight);
            if (this.MaxElementHeight > 0)
                lineHeight = Math.Min(this.MaxElementHeight - Renderer.Tolerance, lineHeight);

            return new VerticalLineInfo(lineHeight, descent, inherentLineSpace);
        }

        /// <summary>
        /// The font used for the current paragraph element.
        /// </summary>
        private XFont CurrentFont
        {
            get { return FontHandler.FontToXFont(CurrentDomFont, this.documentRenderer.PrivateFonts, this.gfx.MUH); }
        }

        private Font CurrentDomFont
        {
            get
            {
                if (this.currentLeaf != null)
                {
                    DocumentObject parent = DocumentRelations.GetParent(this.currentLeaf.Current);
                    parent = DocumentRelations.GetParent(parent);
                    if (parent is FormattedText)
                        return ((FormattedText)parent).Font;
                    else if (parent is Hyperlink)
                        return ((Hyperlink)parent).Font;
                }
                return this.paragraph.Format.Font;
            }
        }

        /// <summary>
        /// Help function to receive a line height on empty paragraphs.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="gfx">The GFX.</param>
        /// <param name="renderer">The renderer.</param>
        internal static XUnit GetLineHeight(ParagraphFormat format, XGraphics gfx, DocumentRenderer renderer)
        {
            XFont font = FontHandler.FontToXFont(format.Font, renderer.PrivateFonts, gfx.MUH);
            XUnit singleLineSpace = font.GetHeight();
            switch (format.LineSpacingRule)
            {
                case LineSpacingRule.Exactly:
                    return format.LineSpacing.Point;

                case LineSpacingRule.AtLeast:
                    return Math.Max(format.LineSpacing.Point, font.GetHeight());

                case LineSpacingRule.Multiple:
                    return format.LineSpacing * format.Font.Size;

                case LineSpacingRule.OnePtFive:
                    return 1.5 * singleLineSpace;

                case LineSpacingRule.Double:
                    return 2.0 * singleLineSpace;

                case LineSpacingRule.Single:
                default:
                    return singleLineSpace;
            }
        }

        void RenderUnderline(XUnit width, bool isWord)
        {
            XPen pen = GetUnderlinePen(isWord);

            bool penChanged = UnderlinePenChanged(pen);
            if (penChanged)
            {
                if (this.currentUnderlinePen != null)
                    EndUnderline(this.currentUnderlinePen, this.currentXPosition);

                if (pen != null)
                    StartUnderline(this.currentXPosition);

                this.currentUnderlinePen = pen;
            }

            if (this.currentLeaf.Current == this.endLeaf.Current)
            {
                if (this.currentUnderlinePen != null)
                    EndUnderline(this.currentUnderlinePen, this.currentXPosition + width);

                this.currentUnderlinePen = null;
            }
        }

        void StartUnderline(XUnit xPosition)
        {
            this.underlineStartPos = xPosition;
        }

        void EndUnderline(XPen pen, XUnit xPosition)
        {
            //Removed KlPo 06.06.07
            //XUnit yPosition = this.currentYPosition + this.currentVerticalInfo.height + pen.Width / 2;
            //yPosition -= 0.66 * this.currentVerticalInfo.descent;

            //New KlPo 
            XUnit yPosition = CurrentBaselinePosition;
            yPosition += 0.33 * this.currentVerticalInfo.descent;
            this.gfx.DrawLine(pen, this.underlineStartPos, yPosition, xPosition, yPosition);
        }

        XPen currentUnderlinePen = null;
        XUnit underlineStartPos;

        bool UnderlinePenChanged(XPen pen)
        {
            if (pen == null && this.currentUnderlinePen == null)
                return false;

            if (pen == null && this.currentUnderlinePen != null)
                return true;

            if (pen != null && this.currentUnderlinePen == null)
                return true;

            if (pen.Color != this.currentUnderlinePen.Color)
                return true;

            return pen.Width != this.currentUnderlinePen.Width;
        }

        RenderInfo CurrentImageRenderInfo
        {
            get
            {
                if (this.currentLeaf != null && this.currentLeaf.Current is Image)
                {
                    Image image = (Image)this.currentLeaf.Current;
                    if (this.imageRenderInfos != null && this.imageRenderInfos.ContainsKey(image))
                        return (RenderInfo)this.imageRenderInfos[image];

                    else
                    {
                        if (this.imageRenderInfos == null)
                            this.imageRenderInfos = new Hashtable();

                        RenderInfo renderInfo = CalcImageRenderInfo(image);
                        this.imageRenderInfos.Add(image, renderInfo);
                        return renderInfo;
                    }
                }
                return null;
            }
        }
        XPen GetUnderlinePen(bool isWord)
        {
            Font font = CurrentDomFont;
            Underline underlineType = font.Underline;
            if (underlineType == Underline.None)
                return null;

            if (underlineType == Underline.Words && !isWord)
                return null;

#if noCMYK
      XPen pen = new XPen(XColor.FromArgb(font.Color.Argb), font.Size / 16);
#else
            XPen pen = new XPen(ColorHelper.ToXColor(font.Color, this.paragraph.Document.UseCmykColor), font.Size / 16);
#endif
            switch (font.Underline)
            {
                case Underline.DotDash:
                    pen.DashStyle = XDashStyle.DashDot;
                    break;

                case Underline.DotDotDash:
                    pen.DashStyle = XDashStyle.DashDotDot;
                    break;

                case Underline.Dash:
                    pen.DashStyle = XDashStyle.Dash;
                    break;

                case Underline.Dotted:
                    pen.DashStyle = XDashStyle.Dot;
                    break;

                case Underline.Single:
                default:
                    pen.DashStyle = XDashStyle.Solid;
                    break;
            }
            return pen;
        }

        private static XStringFormat StringFormat
        {
            get
            {
                if (stringFormat == null)
                {
                    stringFormat = XStringFormats.Default;
                }
                return stringFormat;
            }
        }

        /// <summary>
        /// The paragraph to format or render.
        /// </summary>
        private Paragraph paragraph;
        private XUnit currentWordsWidth;
        private int currentBlankCount;
        private XUnit currentLineWidth;
        private bool isFirstLine;
        private bool isLastLine;
        private VerticalLineInfo currentVerticalInfo = new VerticalLineInfo();
        private Area formattingArea;
        private XUnit currentYPosition;
        private XUnit currentXPosition;
        private ParagraphIterator currentLeaf;
        private ParagraphIterator startLeaf;
        private ParagraphIterator endLeaf;
        private static XStringFormat stringFormat;
        private bool reMeasureLine;
        private XUnit minWidth = 0;
        private Hashtable imageRenderInfos;
        private ArrayList tabOffsets;
        private DocumentObject lastTab = null;
        private bool lastTabPassed = false;
        private XUnit lastTabPosition;
    }
}

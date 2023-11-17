#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharp.com
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
using System.Collections.Generic;
using System.Linq;
using PdfSharpCore.Drawing.Layout.enums;
using PdfSharpCore.Pdf.IO;

namespace PdfSharpCore.Drawing.Layout
{
    /// <summary>
    /// Represents a very simple text formatter.
    /// If this class does not satisfy your needs on formatting paragraphs I recommend to take a look
    /// at MigraDoc Foundation. Alternatively you should copy this class in your own source code and modify it.
    /// </summary>
    public class XTextFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTextFormatter"/> class.
        /// </summary>
        public XTextFormatter(XGraphics gfx)
        {
            if (gfx == null)
                throw new ArgumentNullException("gfx");
            _gfx = gfx;
        }
        readonly XGraphics _gfx;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        string _text;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public XFont Font
        {
            get { return _font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Font");
                _font = value;

                _lineSpace = _font.GetHeight(); // old: _font.GetHeight(_gfx);
                _cyAscent = _lineSpace * _font.CellAscent / _font.CellSpace;
                _cyDescent = _lineSpace * _font.CellDescent / _font.CellSpace;

                // HACK in XTextFormatter
                _spaceWidth = _gfx.MeasureString("x x", value).Width;
                _spaceWidth -= _gfx.MeasureString("xx", value).Width;
            }
        }
        XFont _font;
        double _lineSpace;
        double _cyAscent;
        double _cyDescent;
        double _spaceWidth;
        double _lineHeight;

        // Bounding box of the formatted text after layout
        private XRect _textLayout;

        /// <summary>
        /// Gets or sets the bounding box of the layout.
        /// </summary>
        public XRect LayoutRectangle
        {
            get { return _layoutRectangle; }
            set { _layoutRectangle = value; }
        }
        XRect _layoutRectangle;

        /// <summary>
        /// When true, ignore the height of text areas when rendering multiline strings
        /// </summary>
        public bool AllowVerticalOverflow { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the horizontal alignment of the text.
        /// </summary>
        public XParagraphAlignment Alignment { get; set; } = XParagraphAlignment.Left;
        
        /// <summary>
        /// Gets or sets the vertical alignment of the text.
        /// </summary>
        public XVerticalAlignment VerticalAlignment { get; set; } = XVerticalAlignment.Top;

        /// <summary>
        /// Set vertical and horizontal alignment
        /// </summary>
        /// <param name="alignments"></param>
        public void SetAlignment(TextFormatAlignment alignments)
        {
            Alignment = alignments.Horizontal;
            VerticalAlignment = alignments.Vertical;
        }
        
        
        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XUnit? lineHeight = null)
        {
            DrawString(text, font, brush, layoutRectangle, XStringFormats.TopLeft, lineHeight);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format, XUnit? lineHeight = null)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (format.Alignment != XStringAlignment.Near || format.LineAlignment != XLineAlignment.Near)
                throw new ArgumentException("Only TopLeft alignment is currently implemented.");

            Text = text;
            Font = font;
            LayoutRectangle = layoutRectangle;
            
            _lineHeight = lineHeight?.Point ?? _lineSpace;

            if (text.Length == 0)
                return;

            CreateBlocks();

            CreateLayout();

            double dx = layoutRectangle.Location.X;
            double dy = layoutRectangle.Location.Y + _cyAscent;
            
            if (VerticalAlignment == XVerticalAlignment.Middle)
            {
                dy += layoutRectangle.Height / 2 - _layoutRectangle.Height / 2 - _cyDescent;
            }
            else if (VerticalAlignment == XVerticalAlignment.Bottom)
            {
                dy = layoutRectangle.Location.Y + layoutRectangle.Height - _layoutRectangle.Height + _lineHeight - _cyDescent;
            }
            
            int count = _blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = _blocks[idx];
                if (block.Stop)
                    break;
                if (block.Type == BlockType.LineBreak)
                    continue;
                _gfx.DrawString(block.Text, font, brush, dx + block.Location.X, dy + block.Location.Y);
            }
        }

        void CreateBlocks()
        {
            _blocks.Clear();
            int length = _text.Length;
            bool inNonWhiteSpace = false;
            int startIndex = 0, blockLength = 0;
            for (int idx = 0; idx < length; idx++)
            {
                char ch = _text[idx];

                // Treat CR and CRLF as LF
                if (ch == Chars.CR)
                {
                    if (idx < length - 1 && _text[idx + 1] == Chars.LF)
                        idx++;
                    ch = Chars.LF;
                }
                if (ch == Chars.LF)
                {
                    if (blockLength != 0)
                    {
                        string token = _text.Substring(startIndex, blockLength);
                        _blocks.Add(new Block(token, BlockType.Text,
                          _gfx.MeasureString(token, _font).Width));
                    }
                    startIndex = idx + 1;
                    blockLength = 0;
                    _blocks.Add(new Block(BlockType.LineBreak));
                }
                else if (char.IsWhiteSpace(ch))
                {
                    if (inNonWhiteSpace)
                    {
                        string token = _text.Substring(startIndex, blockLength);
                        _blocks.Add(new Block(token, BlockType.Text,
                          _gfx.MeasureString(token, _font).Width));
                        startIndex = idx + 1;
                        blockLength = 0;
                    }
                    else
                    {
                        blockLength++;
                    }
                }
                else
                {
                    inNonWhiteSpace = true;
                    blockLength++;
                }
            }
            if (blockLength != 0)
            {
                string token = _text.Substring(startIndex, blockLength);
                _blocks.Add(new Block(token, BlockType.Text,
                  _gfx.MeasureString(token, _font).Width));
            }
        }

        void CreateLayout()
        {
            double rectWidth = _layoutRectangle.Width;
            double rectHeight = _layoutRectangle.Height - _cyAscent - _cyDescent;
            int firstIndex = 0;
            double x = 0, y = 0;
            int count = _blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = _blocks[idx];
                if (block.Type == BlockType.LineBreak)
                {
                    if (Alignment == XParagraphAlignment.Justify)
                        _blocks[firstIndex].Alignment = XParagraphAlignment.Left;
                    HorizontalAlignLine(firstIndex, idx - 1, rectWidth);
                    firstIndex = idx + 1;
                    x = 0;
                    y += _lineHeight;
                    if (!AllowVerticalOverflow && y > rectHeight)
                    {
                        block.Stop = true;
                        break;
                    }
                }
                else
                {
                    double width = block.Width;
                    if ((x + width <= rectWidth || x == 0) && block.Type != BlockType.LineBreak)
                    {
                        block.Location = new XPoint(x, y);
                        x += width + _spaceWidth;
                    }
                    else
                    {
                        HorizontalAlignLine(firstIndex, idx - 1, rectWidth);

                        // Begin implicit line break
                        firstIndex = idx;
                        y += _lineHeight;
                        if (!AllowVerticalOverflow && y > rectHeight)
                        {
                            block.Stop = true;
                            break;
                        }
                        block.Location = new XPoint(0, y);
                        x = width + _spaceWidth;
                    }
                }
            }
            if (firstIndex < count && Alignment != XParagraphAlignment.Justify)
                HorizontalAlignLine(firstIndex, count - 1, rectWidth);
            
            var minY = _blocks.Min(b => b.Location.Y);
            var maxY = _blocks.Max(b => b.Location.Y + _lineHeight);
            var minX = _blocks.Min(b => b.Location.X);
            var maxX = _blocks.Max(b => b.Location.X + b.Width);
            _layoutRectangle = new XRect
            {
                X = minX,
                Y = minY,
                Height = maxY - minY,
                Width = maxX - minX
            };
        }

        /// <summary>
        /// Align center, right, or justify.
        /// </summary>
        void HorizontalAlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment blockAlignment = _blocks[firstIndex].Alignment;
            if (Alignment == XParagraphAlignment.Left || blockAlignment == XParagraphAlignment.Left)
                return;

            int count = lastIndex - firstIndex + 1;
            if (count == 0)
                return;

            double totalWidth = -_spaceWidth;
            for (int idx = firstIndex; idx <= lastIndex; idx++)
                totalWidth += _blocks[idx].Width + _spaceWidth;

            double dx = Math.Max(layoutWidth - totalWidth, 0);
            //Debug.Assert(dx >= 0);
            if (Alignment != XParagraphAlignment.Justify)
            {
                if (Alignment == XParagraphAlignment.Center)
                    dx /= 2;
                for (int idx = firstIndex; idx <= lastIndex; idx++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx, 0);
                }
            }
            else if (count > 1) // case: justify
            {
                dx /= count - 1;
                for (int idx = firstIndex + 1, i = 1; idx <= lastIndex; idx++, i++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx * i, 0);
                }
            }
        }

        readonly List<Block> _blocks = new List<Block>();

        // TODO:
        // - more XStringFormat variations
        // - left and right indent
        // - first line indent
        // - margins and paddings
        // - background color
        // - text background color
        // - border style
        // - hyphens, soft hyphens, hyphenation
        // - kerning
        // - change font, size, text color etc.
        // - underline and strike-out variation
        // - super- and sub-script
        // - ...
    }

    public class TextFormatAlignment
    {
        public XParagraphAlignment Horizontal { get; set; } = XParagraphAlignment.Left;
        public XVerticalAlignment Vertical { get; set; } = XVerticalAlignment.Top;
    }
}

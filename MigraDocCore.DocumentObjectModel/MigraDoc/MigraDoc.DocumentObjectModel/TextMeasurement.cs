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
using System.ComponentModel;
using MigraDocCore.DocumentObjectModel.Internals;
using PdfSharpCore.Drawing;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Provides functionality to measure the width of text during document design time.
    /// </summary>
    public sealed class TextMeasurement
    {
        /// <summary>
        /// Initializes a new instance of the TextMeasurement class with the specified font.
        /// </summary>
        public TextMeasurement(XGraphics graphics, Font font)
        {
            this.font = font ?? throw new ArgumentNullException("font");
            this.graphics = graphics;
        }

        /// <summary>
        /// Returns the size of the bounding box of the specified text.
        /// </summary>
        public XSize MeasureString(string text, UnitType unitType)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (!Enum.IsDefined(typeof(UnitType), unitType))
                throw new ArgumentException();

            XSize size = graphics.MeasureString(text, this.xFont);
            switch (unitType)
            {
                case UnitType.Point:
                    break;

                case UnitType.Centimeter:
                    size.Width = (float)(size.Width * 2.54 / 72);
                    size.Height = (float)(size.Height * 2.54 / 72);
                    break;

                case UnitType.Inch:
                    size.Width = size.Width / 72;
                    size.Height = size.Height / 72;
                    break;

                case UnitType.Millimeter:
                    size.Width = (float)(size.Width * 25.4 / 72);
                    size.Height = (float)(size.Height * 25.4 / 72);
                    break;

                case UnitType.Pica:
                    size.Width = size.Width / 12;
                    size.Height = size.Height / 12;
                    break;

                default:
                    Debug.Assert(false, "Missing unit type");
                    break;
            }
            return size;
        }

        /// <summary>
        /// Returns the size of the bounding box of the specified text in point.
        /// </summary>
        public PdfSharpCore.Drawing.XSize MeasureString(string text)
        {
            return MeasureString(text, UnitType.Point);
        }

        /// <summary>
        /// Gets or sets the font used for measurement.
        /// </summary>
        public Font Font
        {
            get { return this.font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (this.font != value)
                {
                    this.font = value;
                    this.xFont = null;
                }
            }
        }
        Font font;
        XFont xFont;
        XGraphics graphics;
    }
}

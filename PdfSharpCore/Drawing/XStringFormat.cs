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

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Represents the text layout information.
    /// </summary>
    public class XStringFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XStringFormat"/> class.
        /// </summary>
        public XStringFormat()
        {
        }

        //TODO public StringFormat(StringFormat format);
        //public StringFormat(StringFormatFlags options);
        //public StringFormat(StringFormatFlags options, int language);
        //public object Clone();
        //public void Dispose();
        //private void Dispose(bool disposing);
        //protected override void Finalize();
        //public float[] GetTabStops(out float firstTabOffset);
        //public void SetDigitSubstitution(int language, StringDigitSubstitute substitute);
        //public void SetMeasurableCharacterRanges(CharacterRange[] ranges);
        //public void SetTabStops(float firstTabOffset, float[] tabStops);
        //public override string ToString();

        /// <summary>
        /// Gets or sets horizontal text alignment information.
        /// </summary>
        public XStringAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
            }
        }
        XStringAlignment _alignment;

        //public int DigitSubstitutionLanguage { get; }
        //public StringDigitSubstitute DigitSubstitutionMethod { get; }
        //public StringFormatFlags FormatFlags { get; set; }
        //public static StringFormat GenericDefault { get; }
        //public static StringFormat GenericTypographic { get; }
        //public HotkeyPrefix HotkeyPrefix { get; set; }

        /// <summary>
        /// Gets or sets the line alignment.
        /// </summary>
        public XLineAlignment LineAlignment
        {
            get { return _lineAlignment; }
            set
            {
                _lineAlignment = value;

            }
        }
        XLineAlignment _lineAlignment;
    }
}

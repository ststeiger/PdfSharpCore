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

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Defines a pixel based bitmap image.
    /// </summary>
    public sealed class XBitmapImage : XBitmapSource
    {
        // TODO: Move code from XImage to this class.

        /// <summary>
        /// Initializes a new instance of the <see cref="XBitmapImage"/> class.
        /// </summary>
        internal XBitmapImage(int width, int height)
        {
        }

        /// <summary>
        /// Creates a default 24 bit ARGB bitmap with the specified pixel size.
        /// </summary>
        public static XBitmapSource CreateBitmap(int width, int height)
        {
            // Create a default 24 bit ARGB bitmap.
            return new XBitmapImage(width, height);
        }
    }
}

#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Ben Askren
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
using System.ComponentModel;
using PdfSharpCore.Internal;
// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public sealed class XRadialGradientBrush : XBaseGradientBrush
    {
        //internal XRadialGradientBrush();

        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(XPoint center1, XPoint center2, double r1, double r2, XColor color1, XColor color2) : base(color1, color2)
        {
            _center1 = center1;
            _center2 = center2;
            _r1 = r1;
            _r2 = r2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(XPoint center, double r1, double r2, XColor color1, XColor color2) : base(color1, color2)
        {
            _center1 = center;
            _center2 = center;
            _r1 = r1;
            _r2 = r2;
        }

        internal XPoint _center1, _center2;
        internal double _r1, _r2;
    }
}
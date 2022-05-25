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
using System.ComponentModel;
using PdfSharpCore.Internal;
// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public sealed class XLinearGradientBrush : XBaseGradientBrush
    {
        //internal XLinearGradientBrush();

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XPoint point1, XPoint point2, XColor color1, XColor color2) : base(color1, color2)
        {
            _point1 = point1;
            _point2 = point2;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XRect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode) : base(color1, color2)
        {
            if (!Enum.IsDefined(typeof(XLinearGradientMode), linearGradientMode))
                throw new InvalidEnumArgumentException("linearGradientMode", (int)linearGradientMode, typeof(XLinearGradientMode));

            if (rect.Width == 0 || rect.Height == 0)
                throw new ArgumentException("Invalid rectangle.", "rect");

            _useRect = true;
            _rect = rect;
            _linearGradientMode = linearGradientMode;
        }

        // TODO: 
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);

        internal bool _useRect;
        internal XPoint _point1, _point2;
        internal XRect _rect;
        internal XLinearGradientMode _linearGradientMode;
    }
}
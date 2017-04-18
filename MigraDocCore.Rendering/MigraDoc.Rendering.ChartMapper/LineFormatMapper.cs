#region MigraDoc - Creating Documents on the Fly
//
// Authors:
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
using PdfSharpCore.Charting;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering.ChartMapper
{
    /// <summary>
    /// The LineFormatMapper class.
    /// </summary>
    public class LineFormatMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineFormatMapper"/> class.
        /// </summary>
        public LineFormatMapper()
        {
        }

        void MapObject(LineFormat lineFormat, DocumentObjectModel.Shapes.LineFormat domLineFormat)
        {
            if (domLineFormat.Color.IsEmpty)
                lineFormat.Color = XColor.Empty;
            else
            {
#if noCMYK
        lineFormat.Color = XColor.FromArgb(domLineFormat.Color.Argb);
#else
                lineFormat.Color = ColorHelper.ToXColor(domLineFormat.Color, domLineFormat.Document.UseCmykColor);
#endif
            }
            switch (domLineFormat.DashStyle)
            {
                case DocumentObjectModel.Shapes.DashStyle.Dash:
                    lineFormat.DashStyle = XDashStyle.Dash;
                    break;
                case DocumentObjectModel.Shapes.DashStyle.DashDot:
                    lineFormat.DashStyle = XDashStyle.DashDot;
                    break;
                case DocumentObjectModel.Shapes.DashStyle.DashDotDot:
                    lineFormat.DashStyle = XDashStyle.DashDotDot;
                    break;
                case DocumentObjectModel.Shapes.DashStyle.Solid:
                    lineFormat.DashStyle = XDashStyle.Solid;
                    break;
                case DocumentObjectModel.Shapes.DashStyle.SquareDot:
                    lineFormat.DashStyle = XDashStyle.Dot;
                    break;
                default:
                    lineFormat.DashStyle = XDashStyle.Solid;
                    break;
            }
            switch (domLineFormat.Style)
            {
                case DocumentObjectModel.Shapes.LineStyle.Single:
                    lineFormat.Style = LineStyle.Single;
                    break;
            }
            lineFormat.Visible = domLineFormat.Visible;
            if (domLineFormat.IsNull("Visible"))
                lineFormat.Visible = true;
            lineFormat.Width = domLineFormat.Width.Point;
        }

        internal static void Map(LineFormat lineFormat, DocumentObjectModel.Shapes.LineFormat domLineFormat)
        {
            LineFormatMapper mapper = new LineFormatMapper();
            mapper.MapObject(lineFormat, domLineFormat);
        }
    }
}

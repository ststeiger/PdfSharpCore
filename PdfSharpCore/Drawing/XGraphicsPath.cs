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
using PdfSharpCore.Internal;

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Represents a series of connected lines and curves.
    /// </summary>
    public sealed class XGraphicsPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XGraphicsPath"/> class.
        /// </summary>
        public XGraphicsPath()
        {
            _corePath = new CoreGraphicsPath();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        public XGraphicsPath Clone()
        {
            XGraphicsPath path = (XGraphicsPath)MemberwiseClone();
            _corePath = new CoreGraphicsPath(_corePath);
            return path;
        }

        // ----- AddLine ------------------------------------------------------------------------------

        /// <summary>
        /// Adds  a line segment to current figure.
        /// </summary>
        public void AddLine(XPoint pt1, XPoint pt2)
        {
            AddLine(pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void AddMove(double x1, double y1)
            => _corePath.MoveTo(x1, y1);

        /// <summary>
        /// Adds  a line segment to current figure.
        /// </summary>
        public void AddLine(double x1, double y1, double x2, double y2)
        {
            _corePath.MoveOrLineTo(x1, y1);
            _corePath.LineTo(x2, y2, false);
        }

        // ----- AddLines -----------------------------------------------------------------------------

        /// <summary>
        /// Adds a series of connected line segments to current figure.
        /// </summary>
        public void AddLines(XPoint[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count == 0)
                return;
            _corePath.MoveOrLineTo(points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx++)
                _corePath.LineTo(points[idx].X, points[idx].Y, false);
        }

        // ----- AddBezier ----------------------------------------------------------------------------

        /// <summary>
        /// Adds a cubic Bézier curve to the current figure.
        /// </summary>
        public void AddBezier(XPoint pt1, XPoint pt2, XPoint pt3, XPoint pt4)
        {
            AddBezier(pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        /// <summary>
        /// Adds a cubic Bézier curve to the current figure.
        /// </summary>
        public void AddBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            _corePath.MoveOrLineTo(x1, y1);
            _corePath.BezierTo(x2, y2, x3, y3, x4, y4, false);
        }

        // ----- AddBeziers ---------------------------------------------------------------------------

        /// <summary>
        /// Adds a sequence of connected cubic Bézier curves to the current figure.
        /// </summary>
        public void AddBeziers(XPoint[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count < 4)
                throw new ArgumentException("At least four points required for bezier curve.", "points");

            if ((count - 1) % 3 != 0)
                throw new ArgumentException("Invalid number of points for bezier curve. Number must fulfil 4+3n.",
                    "points");

            _corePath.MoveOrLineTo(points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx += 3)
            {
                _corePath.BezierTo(points[idx].X, points[idx].Y, points[idx + 1].X, points[idx + 1].Y,
                    points[idx + 2].X, points[idx + 2].Y, false);
            }
        }

        // ----- AddCurve -----------------------------------------------------------------------

        /// <summary>
        /// Adds a spline curve to the current figure.
        /// </summary>
        public void AddCurve(XPoint[] points)
        {
            AddCurve(points, 0.5);
        }

        /// <summary>
        /// Adds a spline curve to the current figure.
        /// </summary>
        public void AddCurve(XPoint[] points, double tension)
        {
            int count = points.Length;
            if (count < 2)
                throw new ArgumentException("AddCurve requires two or more points.", "points");
            _corePath.AddCurve(points, tension);
        }

        /// <summary>
        /// Adds a spline curve to the current figure.
        /// </summary>
        public void AddCurve(XPoint[] points, int offset, int numberOfSegments, double tension)
        {
            throw new NotImplementedException("AddCurve not yet implemented.");
        }

        // ----- AddArc -------------------------------------------------------------------------------

        /// <summary>
        /// Adds an elliptical arc to the current figure.
        /// </summary>
        public void AddArc(XRect rect, double startAngle, double sweepAngle)
        {
            AddArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Adds an elliptical arc to the current figure.
        /// </summary>
        public void AddArc(double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            _corePath.AddArc(x, y, width, height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Adds an elliptical arc to the current figure. The arc is specified WPF like.
        /// </summary>
        public void AddArc(XPoint point1, XPoint point2, XSize size, double rotationAngle, bool isLargeArg, XSweepDirection sweepDirection)
        {
            _corePath.AddArc(point1, point2, size, rotationAngle, isLargeArg, sweepDirection);
        }

        // ----- AddRectangle -------------------------------------------------------------------------

        /// <summary>
        /// Adds a rectangle to this path.
        /// </summary>
        public void AddRectangle(XRect rect)
        {
            _corePath.MoveTo(rect.X, rect.Y);
            _corePath.LineTo(rect.X + rect.Width, rect.Y, false);
            _corePath.LineTo(rect.X + rect.Width, rect.Y + rect.Height, false);
            _corePath.LineTo(rect.X, rect.Y + rect.Height, true);
            _corePath.CloseSubpath();
        }

        /// <summary>
        /// Adds a rectangle to this path.
        /// </summary>
        public void AddRectangle(double x, double y, double width, double height)
        {
            AddRectangle(new XRect(x, y, width, height));
        }

        // ----- AddRectangles ------------------------------------------------------------------------

        /// <summary>
        /// Adds a series of rectangles to this path.
        /// </summary>
        public void AddRectangles(XRect[] rects)
        {
            int count = rects.Length;
            for (int idx = 0; idx < count; idx++)
            {
                AddRectangle(rects[idx]);
            }
        }

        // ----- AddRoundedRectangle ------------------------------------------------------------------

        /// <summary>
        /// Adds a rectangle with rounded corners to this path.
        /// </summary>
        public void AddRoundedRectangle(double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            double arcWidth = ellipseWidth / 2;
            double arcHeight = ellipseHeight / 2;
            _corePath.MoveTo(x + width - arcWidth, y);
            _corePath.QuadrantArcTo(x + width - arcWidth, y + arcHeight, arcWidth, arcHeight, 1, true);

            _corePath.LineTo(x + width, y + height - arcHeight, false);
            _corePath.QuadrantArcTo(x + width - arcWidth, y + height - arcHeight, arcWidth, arcHeight, 4, true);

            _corePath.LineTo(x + arcWidth, y + height, false);
            _corePath.QuadrantArcTo(x + arcWidth, y + height - arcHeight, arcWidth, arcHeight, 3, true);

            _corePath.LineTo(x, y + arcHeight, false);
            _corePath.QuadrantArcTo(x + arcWidth, y + arcHeight, arcWidth, arcHeight, 2, true);

            _corePath.CloseSubpath();

        }

        // ----- AddEllipse ---------------------------------------------------------------------------

        /// <summary>
        /// Adds an ellipse to the current path.
        /// </summary>
        public void AddEllipse(XRect rect)
        {
            AddEllipse(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Adds an ellipse to the current path.
        /// </summary>
        public void AddEllipse(double x, double y, double width, double height)
        {
            double w = width / 2;
            double h = height / 2;
            double xc = x + w;
            double yc = y + h;
            _corePath.MoveTo(x + w, y);
            _corePath.QuadrantArcTo(xc, yc, w, h, 1, true);
            _corePath.QuadrantArcTo(xc, yc, w, h, 4, true);
            _corePath.QuadrantArcTo(xc, yc, w, h, 3, true);
            _corePath.QuadrantArcTo(xc, yc, w, h, 2, true);
            _corePath.CloseSubpath();
        }

        // ----- AddPolygon ---------------------------------------------------------------------------

        /// <summary>
        /// Adds a polygon to this path.
        /// </summary>
        public void AddPolygon(XPoint[] points)
        {
            int count = points.Length;
            if (count == 0)
                return;

            _corePath.MoveTo(points[0].X, points[0].Y);
            for (int idx = 0; idx < count - 1; idx++)
                _corePath.LineTo(points[idx].X, points[idx].Y, false);
            _corePath.LineTo(points[count - 1].X, points[count - 1].Y, true);
            _corePath.CloseSubpath();

        }

        // ----- AddPie -------------------------------------------------------------------------------

        /// <summary>
        /// Adds the outline of a pie shape to this path.
        /// </summary>
        public void AddPie(XRect rect, double startAngle, double sweepAngle)
        {
            AddPie(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Adds the outline of a pie shape to this path.
        /// </summary>
        public void AddPie(double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            DiagnosticsHelper.HandleNotImplemented("XGraphicsPath.AddPie");
        }

        // ----- AddClosedCurve ------------------------------------------------------------------------

        /// <summary>
        /// Adds a closed curve to this path.
        /// </summary>
        public void AddClosedCurve(XPoint[] points)
        {
            AddClosedCurve(points, 0.5);
        }

        /// <summary>
        /// Adds a closed curve to this path.
        /// </summary>
        public void AddClosedCurve(XPoint[] points, double tension)
        {
            if (points == null)
                throw new ArgumentNullException("points");
            int count = points.Length;
            if (count == 0)
                return;
            if (count < 2)
                throw new ArgumentException("Not enough points.", "points");
            DiagnosticsHelper.HandleNotImplemented("XGraphicsPath.AddClosedCurve");
        }

        // ----- AddPath ------------------------------------------------------------------------------

        /// <summary>
        /// Adds the specified path to this path.
        /// </summary>
        public void AddPath(XGraphicsPath path, bool connect)
        {
            DiagnosticsHelper.HandleNotImplemented("XGraphicsPath.AddPath");
        }

        // ----- AddString ----------------------------------------------------------------------------

        /// <summary>
        /// Adds a text string to this path.
        /// </summary>
        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, XPoint origin,
            XStringFormat format)
        {
            try
            {
                DiagnosticsHelper.HandleNotImplemented("XGraphicsPath.AddString");
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Adds a text string to this path.
        /// </summary>
        public void AddString(string s, XFontFamily family, XFontStyle style, double emSize, XRect layoutRect,
            XStringFormat format)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (family == null)
                throw new ArgumentNullException("family");

            if (format == null)
                format = XStringFormats.Default;

            if (format.LineAlignment == XLineAlignment.BaseLine && layoutRect.Height != 0)
                throw new InvalidOperationException(
                    "DrawString: With XLineAlignment.BaseLine the height of the layout rectangle must be 0.");

            if (s.Length == 0)
                return;

            XFont font = new XFont(family.Name, emSize, style);
            DiagnosticsHelper.HandleNotImplemented("XGraphicsPath.AddString");
        }

        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Closes the current figure and starts a new figure.
        /// </summary>
        public void CloseFigure()
        {
            _corePath.CloseSubpath();
        }

        /// <summary>
        /// Starts a new figure without closing the current figure.
        /// </summary>
        public void StartFigure()
        {
            // TODO: ???
        }

        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets an XFillMode that determines how the interiors of shapes are filled.
        /// </summary>
        public XFillMode FillMode
        {
            get { return _fillMode; }
            set
            {
                _fillMode = value;
                // Nothing to do.
            }
        }

        private XFillMode _fillMode;

        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Converts each curve in this XGraphicsPath into a sequence of connected line segments. 
        /// </summary>
        public void Flatten()
        {
            // Just do nothing.
        }

        /// <summary>
        /// Converts each curve in this XGraphicsPath into a sequence of connected line segments. 
        /// </summary>
        public void Flatten(XMatrix matrix)
        {
            // Just do nothing.
        }

        /// <summary>
        /// Converts each curve in this XGraphicsPath into a sequence of connected line segments. 
        /// </summary>
        public void Flatten(XMatrix matrix, double flatness)
        {
            // Just do nothing.
        }

        // --------------------------------------------------------------------------------------------

        /// <summary>
        /// Replaces this path with curves that enclose the area that is filled when this path is drawn 
        /// by the specified pen.
        /// </summary>
        public void Widen(XPen pen)
        {
            // Just do nothing.
        }

        /// <summary>
        /// Replaces this path with curves that enclose the area that is filled when this path is drawn 
        /// by the specified pen.
        /// </summary>
        public void Widen(XPen pen, XMatrix matrix)
        {
            // Just do nothing.
        }

        /// <summary>
        /// Replaces this path with curves that enclose the area that is filled when this path is drawn 
        /// by the specified pen.
        /// </summary>
        public void Widen(XPen pen, XMatrix matrix, double flatness)
        {
            // Just do nothing.
        }

        /// <summary>
        /// Grants access to internal objects of this class.
        /// </summary>
        public XGraphicsPathInternals Internals
        {
            get { return new XGraphicsPathInternals(this); }
        }

        /// <summary>
        /// Gets access to underlying Core graphics path.
        /// </summary>
        internal CoreGraphicsPath _corePath;
    }
}
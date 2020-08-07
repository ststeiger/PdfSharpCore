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
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
//using GdiLinearGradientBrush = System.Drawing.Drawing2D.LinearGradientBrush;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using WpfBrush = System.Windows.Media.Brush;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
#endif

// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharpCore.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public sealed class XRadialGradientBrush : XBaseGradientBrush
    {
        //internal XRadialGradientBrush();

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(System.Drawing.Point center1, System.Drawing.Point center2, double r1, double r2, XColor color1, XColor color2)
            : this(new XPoint(center1), new XPoint(center2), r1, r2, color1, color2)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(SysPoint center1, SysPoint center2, double r1, double r2, XColor color1, XColor color2)
            : this(new XPoint(center1), mew XPoint(center2), r1, r2, color1, color2)
        { }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(PointF center1, PointF center2, double r1, double r2, XColor color1, XColor color2)
            : this(new XPoint(center1), new XPoint(center2), r1, r2, color1, color2)
        { }
#endif

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


#if GDI
        internal override System.Drawing.Brush RealizeGdiBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            // TODO: use dirty to optimize code
            PathGradientBrush brush;
            try
            {
                Lock.EnterGdiPlus();

                    GraphicsPath gp = new GraphicsPath();

                    

                    gp.AddEllipse();

                    PathGradientBrush pgb = new PathGradientBrush(gp);

                    pgb.CenterPoint = new PointF(label1.ClientRectangle.Width / 2, 
                                                 label1.ClientRectangle.Height / 2);
                    pgb.CenterColor = Color.White;
                    pgb.SurroundingColors = new Color[] { Color.Red };


                if (_useRect)
                {
                    brush = new PathGradientBrush(_rect.ToRectangleF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor(), (LinearGradientMode)_linearGradientMode);
                }
                else
                {
                    brush = new GdiLinearGradientBrush(
                        _center.ToPointF(), _point2.ToPointF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor());
                }
                if (!_matrix.IsIdentity)
                    brush.Transform = _matrix.ToGdiMatrix();
                //brush.WrapMode = WrapMode.Clamp;
            }
            finally { Lock.ExitGdiPlus(); }
            return brush;
        }
#endif

#if WPF
        internal override WpfBrush RealizeWpfBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            System.Windows.Media.LinearGradientBrush brush;
            if (_useRect)
            {
#if !SILVERLIGHT
                brush = new System.Windows.Media.LinearGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor(), new SysPoint(0, 0), new SysPoint(1, 1));// rect.TopLeft, this.rect.BottomRight);
                //brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect.ToRectangleF(),
                //  color1.ToGdiColor(), color2.ToGdiColor(), (LinearGradientMode)linearGradientMode);
#else
                GradientStop gs1 = new GradientStop();
                gs1.Color = _color1.ToWpfColor();
                gs1.Offset = 0;

                GradientStop gs2 = new GradientStop();
                gs2.Color = _color2.ToWpfColor();
                gs2.Offset = 1;

                GradientStopCollection gsc = new GradientStopCollection();
                gsc.Add(gs1);
                gsc.Add(gs2);

                brush = new LinearGradientBrush(gsc, 0);
                brush.StartPoint = new Point(0, 0);
                brush.EndPoint = new Point(1, 1);
#endif

            }
            else
            {
#if !SILVERLIGHT
                brush = new System.Windows.Media.LinearGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor(), _center, _point2);
                //brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                //  center.ToPointF(), point2.ToPointF(),
                //  color1.ToGdiColor(), color2.ToGdiColor());
#else
                GradientStop gs1 = new GradientStop();
                gs1.Color = _color1.ToWpfColor();
                gs1.Offset = 0;

                GradientStop gs2 = new GradientStop();
                gs2.Color = _color2.ToWpfColor();
                gs2.Offset = 1;

                GradientStopCollection gsc = new GradientStopCollection();
                gsc.Add(gs1);
                gsc.Add(gs2);

                brush = new LinearGradientBrush(gsc, 0);
                brush.StartPoint = _center;
                brush.EndPoint = _point2;
#endif
            }
            if (!_matrix.IsIdentity)
            {
#if !SILVERLIGHT
                brush.Transform = new MatrixTransform(_matrix.ToWpfMatrix());
#else
                MatrixTransform transform = new MatrixTransform();
                transform.Matrix = _matrix.ToWpfMatrix();
                brush.Transform = transform;
#endif
            }
            return brush;
        }
#endif

#if UWP
        internal override ICanvasBrush RealizeCanvasBrush()
        {
            ICanvasBrush brush;

            brush = new CanvasSolidColorBrush(CanvasDevice.GetSharedDevice(), Colors.RoyalBlue);

            return brush;
        }
#endif

        internal XPoint _center1, _center2;
        internal double _r1, _r2;
    }
}
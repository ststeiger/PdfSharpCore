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
using System.Diagnostics;
using System.IO;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf.Advanced;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using PdfSharpCore.Pdf.IO.enums;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;
using PdfSharpCore.Utils;
using SixLabors.ImageSharp.PixelFormats;

namespace PdfSharpCore.Drawing
{
    [Flags]
    internal enum XImageState
    {
        UsedInDrawingContext = 0x00000001,

        StateMask = 0x0000FFFF,
    }

    /// <summary>
    /// Defines an object used to draw image files (bmp, png, jpeg, gif) and PDF forms.
    /// An abstract base class that provides functionality for the Bitmap and Metafile descended classes.
    /// </summary>
    public class XImage : IDisposable
    {
        // The hierarchy is adapted to WPF/Silverlight/WinRT
        //
        // XImage                           <-- ImageSource
        //   XForm
        //   PdfForm
        //   XBitmapSource               <-- BitmapSource
        //     XBitmapImage             <-- BitmapImage

        // ???
        //public bool Disposed
        //{
        //    get { return _disposed; }
        //    set { _disposed = value; }
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="XImage"/> class.
        /// </summary>
        protected XImage()
        { }

        // Useful stuff here: http://stackoverflow.com/questions/350027/setting-wpf-image-source-in-code
        XImage(string path)
        {
            if (ImageSource.ImageSourceImpl == null) ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
            _source = ImageSource.FromFile(path);
            Initialize();
        }

        XImage(IImageSource imageSource)
        {
            _source = imageSource;
            _path = _source.Name;
            Initialize();
        }

        XImage(Func<Stream> stream)
        {
            // Create a dummy unique path.
            _path = "*" + Guid.NewGuid().ToString("B");
            if (ImageSource.ImageSourceImpl == null) 
                ImageSource.ImageSourceImpl = new ImageSharpImageSource<Rgba32>();
            _source = ImageSource.FromStream(_path, stream);
            Initialize();
        }

        XImage(Func<byte[]> data)
        {
            // Create a dummy unique path.
            _path = "*" + Guid.NewGuid().ToString("B");
            _source = ImageSource.FromBinary(_path, data);
            Initialize();
        }

        /// <summary>
        /// Creates an image from the specified file.
        /// For non-pdf files, this requires that an instance of an implementation of <see cref="T:MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource"/> be set on the `ImageSource.ImageSourceImpl` property.
        /// For .NetCore apps, if this property is null at this point, then <see cref="T:PdfSharpCore.Utils.ImageSharpImageSource"/> with <see cref="T:SixLabors.ImageSharp.PixelFormats.Rgba32"/> Pixel Type is used
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static XImage FromFile(string path)
        {
            return FromFile(path, PdfReadAccuracy.Strict);
        }

        /// <summary>
        /// Creates an image from the specified file.
        /// For non-pdf files, this requires that an instance of an implementation of <see cref="T:MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource"/> be set on the `ImageSource.ImageSourceImpl` property.
        /// For .NetCore apps, if this property is null at this point, then <see cref="T:PdfSharpCore.Utils.ImageSharpImageSource"/> with <see cref="T:SixLabors.ImageSharp.PixelFormats.Rgba32"/> Pixel Type is used
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        /// <param name="accuracy">Moderate allows for broken references when using a PDF file.</param>
        public static XImage FromFile(string path, PdfReadAccuracy accuracy)
        {
            if (PdfReader.TestPdfFile(path) > 0)
                return new XPdfForm(path, accuracy);
            return new XImage(path);
        }

        /// <summary>
        /// Creates an image from the specified stream.<br/>
        /// For non-pdf files, this requires that an instance of an implementation of <see cref="T:MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource"/> be set on the `ImageSource.ImageSourceImpl` property.
        /// For .NetCore apps, if this property is null at this point, then <see cref="T:PdfSharpCore.Utils.ImageSharpImageSource"/> with <see cref="T:SixLabors.ImageSharp.PixelFormats.Rgba32"/> Pixel Type is used
        /// Silverlight supports PNG and JPEF only.
        /// </summary>
        /// <param name="stream">The stream containing a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static XImage FromStream(Func<Stream> stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // TODO: Check PDF stream.
            //if (PdfReader.TestPdfFile(path) > 0)
            //  return new XPdfForm(path);
            return new XImage(stream);
        }

        public static XImage FromImageSource(IImageSource imageSouce)
        {
            return new XImage(imageSouce);
        }

        /// <summary>
        /// Tests if a file exist. Supports PDF files with page number suffix.
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static bool ExistsFile(string path)
        {
            // Support for "base64:" pseudo protocol is a MigraDoc feature, currently completely implemented in MigraDoc files. TODO: Does support for "base64:" make sense for PDFsharp? Probably not as PDFsharp can handle images from streams.
            //if (path.StartsWith("base64:")) // The Image is stored in the string here, so the file exists.
            //    return true;

            if (PdfReader.TestPdfFile(path) > 0)
                return true;
            return false;
        }

        internal XImageState XImageState
        {
            get { return _xImageState; }
            set { _xImageState = value; }
        }
        XImageState _xImageState;

        internal void Initialize()
        {
            if (_source != null)
            {
                //We always get a jpeg from an image source
                _format = _source.Transparent ? XImageFormat.Png : XImageFormat.Jpeg;
            }
        }

        public MemoryStream AsJpeg()
        {
            var ms = new MemoryStream();
            _source.SaveAsJpeg(ms);
            ms.Position = 0;
            return ms;
        }

        public MemoryStream AsBitmap()
        {
            var ms = new MemoryStream();
            _source.SaveAsPdfBitmap(ms);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Under construction
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes underlying GDI+ object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                _disposed = true;
        }
        bool _disposed;

        /// <summary>
        /// Gets the width of the image in point.
        /// </summary>
        public virtual double PointWidth
        {
            get
            {
                return _source.Width * 72 / 96.0;
            }
        }

        /// <summary>
        /// Gets the height of the image in point.
        /// </summary>
        public virtual double PointHeight
        {
            get
            {
                return _source.Height * 72 / 96.0;
            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public virtual int PixelWidth
        {
            get
            {
                return _source.Width;
            }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public virtual int PixelHeight
        {
            get
            {
                return _source.Height;
            }
        }

        /// <summary>
        /// Gets the size in point of the image.
        /// </summary>
        public virtual XSize Size
        {
            get { return new XSize(PointWidth, PointHeight); }
        }

        /// <summary>
        /// Gets the horizontal resolution of the image.
        /// </summary>
        public virtual double HorizontalResolution
        {
            get
            {
                return 96;
            }
        }

        /// <summary>
        /// Gets the vertical resolution of the image.
        /// </summary>
        public virtual double VerticalResolution
        {
            get
            {
                return 96;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether image interpolation is to be performed. 
        /// </summary>
        public virtual bool Interpolate
        {
            get { return _interpolate; }
            set { _interpolate = value; }
        }
        bool _interpolate = true;

        /// <summary>
        /// Gets the format of the image.
        /// </summary>
        public XImageFormat Format
        {
            get { return _format; }
        }
        XImageFormat _format;

        internal void AssociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != null)
                throw new InvalidOperationException("XImage already associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal void DisassociateWithGraphics()
        {
            if (_associatedGraphics == null)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics.DisassociateImage();

            Debug.Assert(_associatedGraphics == null);
        }

        internal void DisassociateWithGraphics(XGraphics gfx)
        {
            if (_associatedGraphics != gfx)
                throw new InvalidOperationException("XImage not associated with XGraphics.");
            _associatedGraphics = null;
        }

        internal XGraphics AssociatedGraphics
        {
            get { return _associatedGraphics; }
            set { _associatedGraphics = value; }
        }
        XGraphics _associatedGraphics;

        /// <summary>
        /// If path starts with '*' the image is created from a stream and the path is a GUID.
        /// </summary>
        internal string _path;

        /// <summary>
        /// Cache PdfImageTable.ImageSelector to speed up finding the right PdfImage
        /// if this image is used more than once.
        /// </summary>
        internal PdfImageTable.ImageSelector _selector;
        private IImageSource _source;
    }
}

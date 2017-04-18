#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharpCore.com
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
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;

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
        /// </summary>
        /// <param name="path">The path to a BMP, PNG, GIF, JPEG, TIFF, or PDF file.</param>
        public static XImage FromFile(string path)
        {
            if (PdfReader.TestPdfFile(path) > 0)
                return new XPdfForm(path);
            return new XImage(path);
        }

        /// <summary>
        /// Creates an image from the specified stream.<br/>
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
#if !NETFX_CORE && !UWP && !PORTABLE
            return File.Exists(path);
#else
            return false;
#endif
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
                _format = XImageFormat.Jpeg;
            }
        }

#if __IOS__
        public MemoryStream AsJpeg()
        {            
            var ms = new MemoryStream();

            using (var stream = _iosImage.AsJPEG(0.7f).AsStream())
            {
                stream.CopyTo(ms);
            }

            return ms;
        }
#endif

        public MemoryStream AsJpeg()
        {
            var ms = new MemoryStream();
            _source.SaveAsJpeg(ms);
            ms.Position = 0;
            return ms;
        }

#if WPF
        /// <summary>
        /// Gets the image filename.
        /// </summary>
        /// <param name="bitmapSource">The bitmap source.</param>
        internal static string GetImageFilename(BitmapSource bitmapSource)
        {
            string filename = bitmapSource.ToString();
            filename = UrlDecodeStringFromStringInternal(filename);
            if (filename.StartsWith("file:///"))
                filename = filename.Substring(8); // Remove all 3 slashes!
            else if (filename.StartsWith("file://"))
                filename = filename.Substring(5); // Keep 2 slashes (UNC path)
            return filename;
        }

        private static string UrlDecodeStringFromStringInternal(string s/*, Encoding e*/)
        {
            int length = s.Length;
            string result = "";
            for (int i = 0; i < length; i++)
            {
                char ch = s[i];
                if (ch == '+')
                {
                    ch = ' ';
                }
                else if ((ch == '%') && (i < (length - 2)))
                {
                    if ((s[i + 1] == 'u') && (i < (length - 5)))
                    {
                        int num3 = HexToInt(s[i + 2]);
                        int num4 = HexToInt(s[i + 3]);
                        int num5 = HexToInt(s[i + 4]);
                        int num6 = HexToInt(s[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                        {
                            goto AddByte;
                        }
                        ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        result += ch;
                        continue;
                    }
                    int num7 = HexToInt(s[i + 1]);
                    int num8 = HexToInt(s[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0))
                    {
                        byte b = (byte)((num7 << 4) | num8);
                        i += 2;
                        result += (char)b;
                        continue;
                    }
                }
            AddByte:
                if ((ch & 0xff80) == 0)
                {
                    result += ch;
                }
                else
                {
                    result += ch;
                }
            }
            return result;
        }

        private static int HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
                return (h - '0');
            if (h >= 'a' && h <= 'f')
                return ((h - 'a') + 10);
            if (h >= 'A' && h <= 'F')
                return (h - 'A') + 10;
            return -1;
        }
#endif

#if WPF
        /// <summary>
        /// Tests if a file is a JPEG.
        /// </summary>
        /// <param name="filename">The filename.</param>
        internal static bool TestJpeg(string filename)
        {
            byte[] imageBits = null;
            return ReadJpegFile(filename, 16, ref imageBits);
        }

        /// <summary>
        /// Tests if a file is a JPEG.
        /// </summary>
        /// <param name="stream">The filename.</param>
        internal static bool TestJpeg(Stream stream)
        {
            byte[] imageBits = null;
            return ReadJpegFile(stream, 16, ref imageBits) == true;
        }

        /// <summary>
        /// Reads the JPEG file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="maxRead">The maximum count of bytes to be read.</param>
        /// <param name="imageBits">The bytes read from the file.</param>
        /// <returns>False, if file could not be read or is not a JPEG file.</returns>
        internal static bool ReadJpegFile(string filename, int maxRead, ref byte[] imageBits)
        {
            if (File.Exists(filename))
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
                catch
                {
                    return false;
                }

                bool? test = ReadJpegFile(fs, maxRead, ref imageBits);
                // Treat test result as definite.
                if (test == false || test == true)
                {
                    fs.Close();
                    return test.Value;
                }
                // Test result is maybe.
                // Hack: store the file in PDF if extension matches ...
                string str = filename.ToLower();
                if (str.EndsWith(".jpg") || str.EndsWith(".jpeg"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the JPEG file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="maxRead">The maximum count of bytes to be read.</param>
        /// <param name="imageBits">The bytes read from the file.</param>
        /// <returns>False, if file could not be read or is not a JPEG file.</returns>
        internal static bool? ReadJpegFile(Stream stream, int maxRead, ref byte[] imageBits)
        {
            if (!stream.CanSeek)
                return false;
            stream.Seek(0, SeekOrigin.Begin);

            if (stream.Length < 16)
            {
                return false;
            }
            int len = maxRead == -1 ? (int)stream.Length : maxRead;
            imageBits = new byte[len];
            stream.Read(imageBits, 0, len);
            if (imageBits[0] == 0xff &&
                imageBits[1] == 0xd8 &&
                imageBits[2] == 0xff &&
                imageBits[3] == 0xe0 &&
                imageBits[6] == 0x4a &&
                imageBits[7] == 0x46 &&
                imageBits[8] == 0x49 &&
                imageBits[9] == 0x46 &&
                imageBits[10] == 0x0)
            {
                return true;
            }
            // TODO: Exif: find JFIF header
            if (imageBits[0] == 0xff &&
                imageBits[1] == 0xd8 &&
                imageBits[2] == 0xff &&
                imageBits[3] == 0xe1 /*&&
                        imageBits[6] == 0x4a &&
                        imageBits[7] == 0x46 &&
                        imageBits[8] == 0x49 &&
                        imageBits[9] == 0x46 &&
                        imageBits[10] == 0x0*/)
            {
                // Hack: store the file in PDF if extension matches ...
                return null;
            }
            return false;
        }
#endif

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

#if CORE || GDI || WPF
            //if (_importedImage != null)
            {
                _importedImage = null;
            }
#endif

#if CORE_WITH_GDI || GDI
            if (_gdiImage != null)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gdiImage.Dispose();
                    _gdiImage = null;
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            _wpfImage = null;
#endif
        }
        bool _disposed;

#if CORE || GDI || WPF
        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 72 points per inch, 1000 mm per meter, 25.4 mm per inch => 72 * 1000 / 25.4.
        /// </summary>
        private const decimal FactorDPM72 = 72000 / 25.4m;

        /// <summary>
        /// The factor for conversion from DPM to PointWidth or PointHeight.
        /// 1000 mm per meter, 25.4 mm per inch => 1000 / 25.4.
        /// </summary>
        private const decimal FactorDPM = 1000 / 25.4m;
#endif

        /// <summary>
        /// Gets the width of the image in point.
        /// </summary>
        public virtual double PointWidth
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.Width * FactorDPM72 / _importedImage.Information.HorizontalDPM);
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)(_importedImage.Information.Width * 72 / _importedImage.Information.HorizontalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                double gdiWidth = _gdiImage.Width * 72 / _gdiImage.HorizontalResolution;
                double wpfWidth = _wpfImage.Width * 72.0 / 96.0;
                //Debug.Assert(gdiWidth == wpfWidth);
                Debug.Assert(DoubleUtil.AreRoughlyEqual(gdiWidth, wpfWidth, 5));
                return wpfWidth;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Width * 72 / _gdiImage.HorizontalResolution;
                //#endif
#if WPF && !GDI
#if !SILVERLIGHT
                Debug.Assert(Math.Abs(_wpfImage.PixelWidth * 72 / _wpfImage.DpiX - _wpfImage.Width * 72.0 / 96.0) < 0.001);
                return _wpfImage.Width * 72.0 / 96.0;
#else
                // AGHACK
                return _wpfImage.PixelWidth * 72 / 96.0;
#endif
#endif
#if NETFX_CORE || UWP
                //var wb = new WriteableBitmap();
                //GetImagePropertiesAsync
                return 100;
#endif
#if __IOS__
                return _iosImage.CGImage.Width * 72.0 / 96.0;
#endif
#if __ANDROID__
                return _androidImage.Width * 72 / 96.0;
#endif
#if PORTABLE
                return _source.Width * 72 / 96.0;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image in point.
        /// </summary>
        public virtual double PointHeight
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.Height * FactorDPM72 / _importedImage.Information.VerticalDPM);
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)(_importedImage.Information.Height * 72 / _importedImage.Information.VerticalDPI);
                    // Assume 72 DPI if information not available.
                    return _importedImage.Information.Width;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height * 72 / _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                double gdiHeight = _gdiImage.Height * 72 / _gdiImage.HorizontalResolution;
                double wpfHeight = _wpfImage.Height * 72.0 / 96.0;
                Debug.Assert(DoubleUtil.AreRoughlyEqual(gdiHeight, wpfHeight, 5));
                return wpfHeight;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Height * 72 / _gdiImage.HorizontalResolution;
                //#endif
#if WPF && !GDI
#if !SILVERLIGHT
                Debug.Assert(Math.Abs(_wpfImage.PixelHeight * 72 / _wpfImage.DpiY - _wpfImage.Height * 72.0 / 96.0) < 0.001);
                return _wpfImage.Height * 72.0 / 96.0;
#else
                // AGHACK
                return _wpfImage.PixelHeight * 72 / 96.0;
#endif
#endif
#if NETFX_CORE || UWP
                return _wrtImage.PixelHeight; //_gdi Image.Width * 72 / _gdiImage.HorizontalResolution;
#endif
#if __IOS__
                return _iosImage.CGImage.Height * 72.0 / 96.0;
#endif
#if __ANDROID__
                return _androidImage.Height * 72 / 96.0;
#endif
#if PORTABLE
                return _source.Height * 72 / 96.0;
#endif
            }
        }

        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public virtual int PixelWidth
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                    return (int)_importedImage.Information.Width;
#endif

#if CORE_WITH_GDI
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Width;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                int gdiWidth = _gdiImage.Width;
                int wpfWidth = _wpfImage.PixelWidth;
                Debug.Assert(gdiWidth == wpfWidth);
                return wpfWidth;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Width;
                //#endif
#if WPF && !GDI
                return _wpfImage.PixelWidth;
#endif
#if NETFX_CORE || UWP
                return _wrtImage.PixelWidth;
#endif
#if __IOS__
                return (int) _iosImage.CGImage.Width;
#endif
#if __ANDROID__
                return _androidImage.Width;
#endif
#if PORTABLE
                return _source.Width;
#endif
            }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public virtual int PixelHeight
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                    return (int)_importedImage.Information.Height;
#endif

#if CORE_WITH_GDI
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.Height;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                int gdiHeight = _gdiImage.Height;
                int wpfHeight = _wpfImage.PixelHeight;
                Debug.Assert(gdiHeight == wpfHeight);
                return wpfHeight;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.Height;
                //#endif
#if WPF && !GDI
                return _wpfImage.PixelHeight;
#endif
#if NETFX_CORE || UWP
                return _wrtImage.PixelHeight;
#endif
#if __IOS__
                return (int)_iosImage.CGImage.Height;
#endif
#if __ANDROID__
                return _androidImage.Height;
#endif
#if PORTABLE
                return _source.Height;
#endif
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
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.HorizontalDPI > 0)
                        return (double)_importedImage.Information.HorizontalDPI;
                    if (_importedImage.Information.HorizontalDPM > 0)
                        return (double)(_importedImage.Information.HorizontalDPM / FactorDPM);
                    return 72;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.HorizontalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                double gdiResolution = _gdiImage.HorizontalResolution;
                double wpfResolution = _wpfImage.PixelWidth * 96.0 / _wpfImage.Width;
                Debug.Assert(gdiResolution == wpfResolution);
                return wpfResolution;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.HorizontalResolution;
                //#endif
#if WPF && !GDI
#if !SILVERLIGHT
                return _wpfImage.DpiX; //.PixelWidth * 96.0 / _wpfImage.Width;
#else
                // AGHACK
                return 96;
#endif
#endif
#if NETFX_CORE || UWP
                return 96;
#endif
#if __IOS__ || __ANDROID__
                return 96;
#endif
#if PORTABLE
                return 96;
#endif
            }
        }

        /// <summary>
        /// Gets the vertical resolution of the image.
        /// </summary>
        public virtual double VerticalResolution
        {
            get
            {
#if CORE || GDI || WPF
                if (_importedImage != null)
                {
                    if (_importedImage.Information.VerticalDPI > 0)
                        return (double)_importedImage.Information.VerticalDPI;
                    if (_importedImage.Information.VerticalDPM > 0)
                        return (double)(_importedImage.Information.VerticalDPM / FactorDPM);
                    return 72;
                }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
                try
                {
                    Lock.EnterGdiPlus();
                    return _gdiImage.VerticalResolution;
                }
                finally { Lock.ExitGdiPlus(); }
#endif
#if GDI && WPF
                double gdiResolution = _gdiImage.VerticalResolution;
                double wpfResolution = _wpfImage.PixelHeight * 96.0 / _wpfImage.Height;
                Debug.Assert(gdiResolution == wpfResolution);
                return wpfResolution;
#endif
                //#if GDI && !WPF
                //                return _gdiImage.VerticalResolution;
                //#endif
#if WPF && !GDI
#if !SILVERLIGHT
                return _wpfImage.DpiY; //.PixelHeight * 96.0 / _wpfImage.Height;
#else
                // AGHACK
                return 96;
#endif
#endif
#if NETFX_CORE || UWP
                return 96;
#endif
#if __IOS__ || __ANDROID__
                return 96;
#endif
#if PORTABLE
                return 96;
#endif
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

#if WPF
        /// <summary>
        /// Gets a value indicating whether this image is JPEG.
        /// </summary>
        internal virtual bool IsJpeg
        {
#if !SILVERLIGHT
            //get { if (!isJpeg.HasValue) InitializeGdiHelper(); return isJpeg.HasValue ? isJpeg.Value : false; }
            get
            {
                if (!_isJpeg.HasValue)
                    InitializeJpegQuickTest();
                return _isJpeg.HasValue ? _isJpeg.Value : false;
            }
            //set { isJpeg = value; }
#else
            // AGHACK
            get { return true; }
#endif
        }
        private bool? _isJpeg;

        /// <summary>
        /// Gets a value indicating whether this image is cmyk.
        /// </summary>
        internal virtual bool IsCmyk
        {
#if !SILVERLIGHT
            get { if (!_isCmyk.HasValue) InitializeGdiHelper(); return _isCmyk.HasValue ? _isCmyk.Value : false; }
            //set { isCmyk = value; }
#else
            get { return false; } // AGHACK
#endif
        }
        private bool? _isCmyk;

#if !SILVERLIGHT
        /// <summary>
        /// Gets the JPEG memory stream (if IsJpeg returns true).
        /// </summary>
        public virtual MemoryStream Memory
        {
            get
            {
                if (!_isCmyk.HasValue) InitializeGdiHelper();
                return _memory;
            }
            //set { memory = value; }
        }
        MemoryStream _memory;

        /// <summary>
        /// Determines if an image is JPEG w/o creating an Image object.
        /// </summary>
        private void InitializeJpegQuickTest()
        {
            if (_stream != null)
                _isJpeg = TestJpeg(_stream);
            else
                _isJpeg = TestJpeg(GetImageFilename(_wpfImage));
        }

        /// <summary>
        /// Initializes the GDI helper.
        /// We use GDI+ to detect if image is JPEG.
        /// If so, we also determine if it's CMYK and we read the image bytes.
        /// </summary>
        private void InitializeGdiHelper()
        {
            if (!_isCmyk.HasValue)
            {
                try
                {
                    string imageFilename = GetImageFilename(_wpfImage);
                    // To reduce exceptions, check if file exists.
                    if (!string.IsNullOrEmpty(imageFilename) && File.Exists(imageFilename))
                    {
                        MemoryStream memory = new MemoryStream();
                        using (FileStream file = new FileStream(imageFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            memory.Write(bytes, 0, (int)file.Length);
                            memory.Seek(0, SeekOrigin.Begin);
                        }
                        InitializeJpegHelper(memory);
                    }
                    else if (_stream != null)
                    {
                        MemoryStream memory = new MemoryStream();
                        // If we have a stream, copy data from the stream.
                        if (_stream != null && _stream.CanSeek)
                        {
                            _stream.Seek(0, SeekOrigin.Begin);
                            byte[] buffer = new byte[32 * 1024]; // 32K buffer.
                            int bytesRead;
                            while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                memory.Write(buffer, 0, bytesRead);
                            }
                            InitializeJpegHelper(memory);
                        }
                    }
                }
                catch { }
            }
        }

        private void InitializeJpegHelper(MemoryStream memory)
        {
            using (System.Drawing.Image image = new System.Drawing.Bitmap(memory))
            {
                string guid = image.RawFormat.Guid.ToString("B").ToUpper();
                _isJpeg = guid == "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
                _isCmyk = (image.Flags &
                           ((int)System.Drawing.Imaging.ImageFlags.ColorSpaceCmyk | (int)System.Drawing.Imaging.ImageFlags.ColorSpaceYcck)) != 0;
                if (_isJpeg.Value)
                {
                    //_memory = new MemoryStream();
                    //image.Save(_memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                    if ((int)memory.Length != 0)
                    {
                        _memory = memory;
                    }
                    else
                    {
                        _memory = null;
                    }
                }
            }
        }
#endif
#endif

#if DEBUG_
        // TEST
        internal void CreateAllImages(string name)
        {
            if (image != null)
            {
                image.Save(name + ".bmp", ImageFormat.Bmp);
                image.Save(name + ".emf", ImageFormat.Emf);
                image.Save(name + ".exif", ImageFormat.Exif);
                image.Save(name + ".gif", ImageFormat.Gif);
                image.Save(name + ".ico", ImageFormat.Icon);
                image.Save(name + ".jpg", ImageFormat.Jpeg);
                image.Save(name + ".png", ImageFormat.Png);
                image.Save(name + ".tif", ImageFormat.Tiff);
                image.Save(name + ".wmf", ImageFormat.Wmf);
                image.Save(name + "2.bmp", ImageFormat.MemoryBmp);
            }
        }
#endif

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

#if CORE || GDI || WPF
        internal ImportedImage _importedImage;
#endif

#if CORE_WITH_GDI || GDI
        internal Image _gdiImage;
#endif
#if WPF
        internal BitmapSource _wpfImage;
#if SILVERLIGHT
        //internal byte[] _bytes;
#endif
#endif
#if NETFX_CORE || UWP
        internal BitmapSource _wrtImage;
#endif
#if __IOS__
        internal UIImage _iosImage;
#endif
#if __ANDROID__
        internal Android.Graphics.Bitmap _androidImage;
#endif

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

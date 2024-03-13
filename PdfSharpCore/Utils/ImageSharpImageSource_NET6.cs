#if NET6_0_OR_GREATER

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.IO;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Advanced;

namespace PdfSharpCore.Utils
{
    public partial class ImageSharpImageSource<TPixel> : ImageSource where TPixel : unmanaged, IPixel<TPixel>
    {

        public static IImageSource FromImageSharpImage(Image<TPixel> image, IImageFormat imgFormat, int? quality = 75)
        {
            var _path = "*" + Guid.NewGuid().ToString("B");
            return new ImageSharpImageSourceImpl<TPixel>(_path, image, (int)quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            var image = Image.Load<TPixel>(imageSource.Invoke());
            var imgFormat = image.Metadata.DecodedImageFormat;

            return new ImageSharpImageSourceImpl<TPixel>(name, image, (int)quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            var image = Image.Load<TPixel>(path);
            var imgFormat = image.Metadata.DecodedImageFormat;

            return new ImageSharpImageSourceImpl<TPixel>(path, image, (int) quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75) 
        { 
            using (var stream = imageStream.Invoke()) {
                var image = Image.Load<TPixel>(stream);
                var imgFormat = image.Metadata.DecodedImageFormat;

                return new ImageSharpImageSourceImpl<TPixel>(name, image, (int)quality, imgFormat is PngFormat);
            }
        }

        private partial class ImageSharpImageSourceImpl<TPixel2>
        {

            public void SaveAsJpeg(MemoryStream ms)
            {
                Image.SaveAsJpeg(ms, new JpegEncoder() {
                    Quality = this._quality,
                    ColorType = JpegEncodingColor.YCbCrRatio420,
                    Interleaved = true,
                });
            }

        }

    }
}

#endif
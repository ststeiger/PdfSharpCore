using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.IO;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace PdfSharpCore.Utils
{
    public class ImageSharpImageSource<TPixel> : ImageSource where TPixel : unmanaged, IPixel<TPixel>
    {

        public static IImageSource FromImageSharpImage(Image<TPixel> image, IImageFormat imgFormat, int? quality = 75)
        {
            var _path = "*" + Guid.NewGuid().ToString("B");
            return new ImageSharpImageSourceImpl<TPixel>(_path, image, (int)quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            var image = Image.Load<TPixel>(imageSource.Invoke(), out IImageFormat imgFormat);
            return new ImageSharpImageSourceImpl<TPixel>(name, image, (int)quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            var image = Image.Load<TPixel>(path, out IImageFormat imgFormat);
            return new ImageSharpImageSourceImpl<TPixel>(path, image, (int) quality, imgFormat is PngFormat);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            using (var stream = imageStream.Invoke())
            {
                var image = Image.Load<TPixel>(stream, out IImageFormat imgFormat);
                return new ImageSharpImageSourceImpl<TPixel>(name, image, (int)quality, imgFormat is PngFormat);
            }
        }

        private class ImageSharpImageSourceImpl<TPixel2> : IImageSource where TPixel2 : unmanaged, IPixel<TPixel2>
        {
            private Image<TPixel2> Image { get; }
            private readonly int _quality;

            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }
            public bool Transparent { get; internal set; }

            public ImageSharpImageSourceImpl(string name, Image<TPixel2> image, int quality, bool isTransparent)
            {
                Name = name;
                Image = image;
                _quality = quality;
                Transparent = isTransparent;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                Image.SaveAsJpeg(ms, new JpegEncoder() { Quality = this._quality });
            }

            public void Dispose()
            {
                Image.Dispose();
            }
            public void SaveAsPdfBitmap(MemoryStream ms)
            {
                BmpEncoder bmp = new BmpEncoder { BitsPerPixel = BmpBitsPerPixel.Pixel32 };
                Image.Save(ms, bmp);
            }
        }
    }
}

using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;

namespace PdfSharpCore.ImageSharp
{
    public class ImageSharpImageSource<TPixel> : ImageSource where TPixel : struct, IPixel<TPixel>
    {
        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<TPixel>(name, () =>
            {
                return Image.Load<TPixel>(imageSource.Invoke());
            }, (int)quality);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<TPixel>(path, () =>
            {
                return Image.Load<TPixel>(path);
            }, (int)quality);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl<TPixel>(name, () =>
            {
                using (var stream = imageStream.Invoke())
                {
                    return Image.Load<TPixel>(stream);
                }
            }, (int)quality);
        }

        private class ImageSharpImageSourceImpl<TPixel2> : IImageSource where TPixel2 : struct, IPixel<TPixel2>
        {

            private Image<TPixel2> _image;
            private Image<TPixel2> Image
            {
                get
                {
                    if (_image == null)
                    {
                        _image = _getImage.Invoke();
                    }
                    return _image;
                }
            }
            private Func<Image<TPixel2>> _getImage;
            private readonly int _quality;

            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }

            public ImageSharpImageSourceImpl(string name, Func<Image<TPixel2>> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                //Image.AutoOrient();
                Image.SaveAsJpeg(ms, new JpegEncoder(){Quality = this._quality});
            }

            public void Dispose()
            {
                Image.Dispose();
            }
        }
    }
}

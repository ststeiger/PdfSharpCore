using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ImageSharp;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using static MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes.ImageSource;
using ImageSharp.Formats;

namespace PdfSharpCore.ImageSharp
{
    public class ImageSharpImageSource : ImageSource
    {
        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl(name, () =>
            {
                return new Image(imageSource.Invoke());
            }, (int)quality);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl(path, () =>
            {
                return new Image(path);
            }, (int)quality);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return new ImageSharpImageSourceImpl(name, () =>
            {
                using (var stream = imageStream.Invoke())
                {
                    return new Image(stream);
                }
            }, (int)quality);
        }

        private class ImageSharpImageSourceImpl : IImageSource
        {

            private Image _image;
            private Image Image
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
            private Func<Image> _getImage;
            private readonly int _quality;

            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }

            public ImageSharpImageSourceImpl(string name, Func<Image> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                Image.AutoOrient();
                Image.SaveAsJpeg(ms, new JpegEncoderOptions() { Quality = _quality });
            }

            public void Dispose()
            {
                Image.Dispose();
            }
        }
    }
}

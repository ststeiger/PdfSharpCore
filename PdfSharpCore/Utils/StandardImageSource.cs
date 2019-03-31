using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PdfSharpCore.Utils
{
    public class StandardImageSource : ImageSource
    {
        protected override IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return new StandardImageSourceImpl(name,
                () =>
                {
                    using (var stream = new MemoryStream(imageSource.Invoke()))
                    {
                        return Image.FromStream(stream);
                    }
                }, quality ?? 0);
        }

        protected override IImageSource FromFileImpl(string path, int? quality = 75)
        {
            return new StandardImageSourceImpl(path, () => Image.FromFile(path), quality ?? 0);
        }

        protected override IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return new StandardImageSourceImpl(name, () =>
            {
                using (var stream = imageStream.Invoke())
                {
                    return Image.FromStream(stream);
                }
                
            }, quality ?? 0);
        }

        private class StandardImageSourceImpl : IImageSource
        {
            private Image _image;
            private Image Image => _image ?? (_image = _getImage.Invoke());
            private readonly Func<Image> _getImage;
            private readonly int _quality;

            public int Width => Image.Width;
            public int Height => Image.Height;
            public string Name { get; }

            public StandardImageSourceImpl(string name, Func<Image> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
            }

            public void SaveAsJpeg(MemoryStream ms)
            {
                var encoder = ImageCodecInfo.GetImageEncoders()
                    .FirstOrDefault(_ => _.FormatID == ImageFormat.Jpeg.Guid);

                Image.Save(ms, encoder, new EncoderParameters
                {
                    Param = new[]
                    {
                        new EncoderParameter(Encoder.Quality, _quality)
                    }
                });
            }

            public void Dispose()
            {
                Image.Dispose();
            }
        }
    }
}
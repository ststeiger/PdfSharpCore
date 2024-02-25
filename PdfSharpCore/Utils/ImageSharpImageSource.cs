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

        private partial class ImageSharpImageSourceImpl<TPixel2> : IImageSource where TPixel2 : unmanaged, IPixel<TPixel2>
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

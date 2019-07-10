using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.IO;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace PdfSharpCore.Utils
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
            return new ImageSharpImageSourceImpl<Rgba32>(path, () => { return Image.Load<Rgba32>(path); },
                (int) quality);
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
            private bool? _transparent;

            public bool Transparent
            {
                get
                {
                    if (_transparent == null)
                    {
                        _transparent = false;
                        //Note: This is going to result in the image being loaded into memory
                        //up front, because we need to know if it is transparent or not.
                        //Previously the image wouldn't be loaded until it was used.
                        //If this is a problem, we might want to use GetMetaData and change
                        //how ImageSources are constructed.
                        if (Image.PixelType.BitsPerPixel == 32)
                        {
                            //ImageSharp will create PngMetaData for any image type when calling
                            //GetFormatMetaData even if it wasn't PNG.
                            //To avoid generating extra metadata for non-PNG, check for 32-bit color.
                            //This will not completely prevent extra metadata from being created for
                            //images that are not PNG, but at least it avoids extra logic sometimes.
                            PngMetaData pngMeta = _image.MetaData.GetFormatMetaData(PngFormat.Instance);
                            if (pngMeta != null && (pngMeta.ColorType == PngColorType.RgbWithAlpha ||
                                                    pngMeta.ColorType == PngColorType.GrayscaleWithAlpha))
                            {
                                _transparent = true;
                            }
                        }
                    }
                    return _transparent ?? false;
                }
            }

            public ImageSharpImageSourceImpl(string name, Func<Image<TPixel2>> getImage, int quality)
            {
                Name = name;
                _getImage = getImage;
                _quality = quality;
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

using System;
using System.IO;
using PdfSharpCore.Utils;

namespace MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes
{


    public abstract class ImageSource
    {
        /// <summary>
        /// Gets or sets the image source implemention to use for reading images.
        /// </summary>
        /// <value>The image source impl.</value>
        private static ImageSource _imageSourceImpl;
        public static ImageSource ImageSourceImpl => _imageSourceImpl ?? (_imageSourceImpl = new StandardImageSource());

        public interface IImageSource
        {
            int Width { get; }
            int Height { get; }
            string Name { get; }
            void SaveAsJpeg(MemoryStream ms);
        }

        protected abstract IImageSource FromFileImpl(string path, int? quality = 75);
        protected abstract IImageSource FromBinaryImpl(string name, Func<byte[]> imageSource, int? quality = 75);
        protected abstract IImageSource FromStreamImpl(string name, Func<Stream> imageStream, int? quality = 75);


        public static IImageSource FromFile(string path, int? quality = 75)
        {
            return ImageSourceImpl.FromFileImpl(path, quality);
        }

        public static IImageSource FromBinary(string name, Func<byte[]> imageSource, int? quality = 75)
        {
            return ImageSourceImpl.FromBinaryImpl(name, imageSource, quality);
        }

        public static IImageSource FromStream(string name, Func<Stream> imageStream, int? quality = 75)
        {
            return ImageSourceImpl.FromStreamImpl(name, imageStream, quality);
        }
    }
}

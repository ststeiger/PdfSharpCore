
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes
{


    public abstract class ImageSource
    {
        public static ImageSource ImageSourceImpl { get; set; }

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

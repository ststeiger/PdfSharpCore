using PdfSharpCore.Pdf.IO;
using System;

namespace PdfSharpCore.Pdf.Filters
{
    internal static class StreamDecoder
    {
        // PdfReference, chapter 7.4.4.3

        /// <summary>
        /// Further decodes a stream of bytes that were processed by the Flate- or LZW-decoder. 
        /// </summary>
        /// <param name="data">The data to decode</param>
        /// <param name="decodeParms">Parameters for the decoder. If this is null, <paramref name="data"/> is returned unchanged</param>
        /// <returns>The decoded data as a byte-array</returns>
        /// <exception cref="PdfReaderException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static byte[] Decode(byte[] data, PdfDictionary decodeParms)
        {
            if (decodeParms == null)
                return data;

            var predictor = decodeParms.Elements.GetInteger("/Predictor");
            var colors = decodeParms.Elements.GetInteger("/Colors");
            var bpc = decodeParms.Elements.GetInteger("/BitsPerComponent");
            var columns = decodeParms.Elements.GetInteger("/Columns");

            // set up defaults according to the spec
            if (predictor < 1)
                predictor = 1;
            if (colors < 1)
                colors = 1;
            if (bpc < 1)
                bpc = 8;
            if (columns < 1)
                columns = 1;

            if (predictor == 1)     // no prediction, return data as is
                return data;

            // TIFF predictor. TODO: implement
            if (predictor == 2)
                throw new NotImplementedException("TIFF predictor is not implemented");

            // PNG predictors
            if (predictor >= 10 && predictor <= 15)
            {
                if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8 && bpc != 16)
                    throw new PdfReaderException("Invalid number of bits per component");
                var stride = (bpc * colors * columns + 7) / 8;
                var rows = data.Length / (stride + 1);
                var unfilteredData = new byte[rows * stride];
                PngFilter.Unfilter(stride, (bpc * colors + 7) / 8, data, unfilteredData);
                return unfilteredData;
            }

            throw new PdfReaderException("Invalid predictor " + predictor);
        }
    }
}

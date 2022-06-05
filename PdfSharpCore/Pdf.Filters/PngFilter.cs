using PdfSharpCore.Pdf.IO;
using System;

namespace PdfSharpCore.Pdf.Filters
{
    internal static class PngFilter
    {
        /// <summary>
        /// Implements PNG-Filtering according to the PNG-specification<br></br>
        /// see: https://datatracker.ietf.org/doc/html/rfc2083#section-6
        /// </summary>
        /// <param name="stride">The width of a scanline in bytes</param>
        /// <param name="bpp">Bytes per pixel</param>
        /// <param name="inData">The input data</param>
        /// <param name="inData">The target array where the unfiltered data is stored</param>
        /// <returns></returns>
        internal static void Unfilter(int stride, int bpp, byte[] inData, byte[] outData)
        {
            var prevRow = new byte[stride];
            var row = new byte[stride];
            var pos = 0;
            var outIndex = 0;
            while (pos < inData.Length)
            {
                Array.Copy(inData, pos + 1, row, 0, stride);
                var filterType = inData[pos];
                if (filterType > 4)
                    throw new PdfReaderException(string.Format("Unexpected Png-Predictor {0} in Xref Stream. Expected 0 to 4.", filterType));
                switch (filterType)
                {
                    case 0:         // None
                        for (var i = 0; i < row.Length; i++)
                            outData[outIndex++] = row[i];
                        break;
                    case 1:         // Sub
                        for (var i = 0; i < row.Length; i++)
                        {
                            var left = i < bpp ? 0 : outData[outIndex - bpp];
                            outData[outIndex++] = (byte)(row[i] + left);
                        }
                        break;
                    case 2:         // Up
                        for (var i = 0; i < row.Length; i++)
                            outData[outIndex++] = (byte)(row[i] + prevRow[i]);
                        break;
                    case 3:         // Average
                        for (var i = 0; i < row.Length; i++)
                        {
                            var left = i < bpp ? 0 : outData[outIndex - bpp];
                            outData[outIndex++] = (byte)(row[i] + (byte)((left + prevRow[i]) / 2));
                        }
                        break;
                    case 4:         // Paeth
                        for (var i = 0; i < row.Length; i++)
                        {
                            var left = i < bpp ? (byte)0 : outData[outIndex - bpp];
                            var above = prevRow[i];
                            var aboveLeft = i < bpp ? (byte)0 : prevRow[i - bpp];
                            outData[outIndex++] = (byte)(row[i] + PaethPredictor(left, above, aboveLeft));
                        }
                        break;
                }
                // remember current scanline
                Array.Copy(outData, outIndex - stride, prevRow, 0, stride);
                pos += stride + 1;    // each scanline is preceded by a predictor-byte
            }
        }

        // https://datatracker.ietf.org/doc/html/rfc2083#page-36
        private static byte PaethPredictor(byte a, byte b, byte c)
        {
            var p = a + b - c;
            var pa = Math.Abs(p - a);
            var pb = Math.Abs(p - b);
            var pc = Math.Abs(p - c);
            if (pa <= pb && pa <= pc)
                return a;
            else if (pb <= pc)
                return b;
            return c;
        }
    }
}

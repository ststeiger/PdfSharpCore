#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//
// Copyright (c) 2001-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
// http://www.migradoc.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using MigraDocCore.Rendering.MigraDoc.Rendering.Resources;
using System;
using System.Diagnostics;
namespace MigraDocCore.Rendering
{
    /// <summary>
    /// Formats numbers roman or with letters.
    /// </summary>
    internal class NumberFormatter
    {
        internal static string Format(int number, string format)
        {
            switch (format)
            {
                case "ROMAN":
                    return AsRoman(number, false);

                case "roman":
                    return AsRoman(number, true);

                case "ALPHABETIC":
                    return AsLetters(number, false);

                case "alphabetic":
                    return AsLetters(number, true);
            }
            return number.ToString();
        }


        static string AsRoman(int number, bool lowercase)
        {
            if (Math.Abs(number) > 32768)
            {
                Debug.WriteLine(string.Format(AppResources.NumberTooLargeForRoman, number), "warning");
                return number.ToString();
            }
            if (number == 0)
                return "0";

            string res = "";
            if (number < 0)
                res += "-";

            number = Math.Abs(number);

            string[] roman;
            if (lowercase)
                roman = new string[] { "m", "cm", "d", "cd", "c", "xc", "l", "xl", "x", "ix", "v", "iv", "i" };
            else
                roman = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            int[] numberValues = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

            for (int i = 0; i < numberValues.Length; ++i)
            {
                while (number >= numberValues[i])
                {
                    res += roman[i];
                    number -= numberValues[i];
                }
            }
            return res;
        }

        static string AsLetters(int number, bool lowercase)
        {
            if (Math.Abs(number) > 32768)
            {
                Debug.WriteLine(string.Format(AppResources.NumberTooLargeForLetters, number));
                return number.ToString();
            }

            if (number == 0)
                return "0";

            string str = "";
            if (number < 0)
                str += "-";

            number = Math.Abs(number);
            char cr;
            if (lowercase)
                cr = (char)('a' + (number - 1) % 26);
            else
                cr = (char)('A' + (number - 1) % 26);

            for (int n = 0; n <= (int)((number - 1) / 26); ++n)
                str += cr;

            return str;
        }
    }
}

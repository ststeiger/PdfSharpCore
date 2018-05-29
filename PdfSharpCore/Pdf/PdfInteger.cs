#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharpCore.com
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

using System;
using System.Diagnostics;
using System.Globalization;
using PdfSharpCore.Pdf.IO;

namespace PdfSharpCore.Pdf
{
    /// <summary>
    /// Represents a direct integer value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfInteger : PdfNumber, IConvertible, System.IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfInteger"/> class.
        /// </summary>
        public PdfInteger()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfInteger"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfInteger(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value as integer.
        /// </summary>
        public int Value
        {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get { return _value; }
        }
        readonly int _value;

        /// <summary>
        /// Returns the integer as string.
        /// </summary>
        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes the integer as string.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        #region System.IConvertible Members

        uint System.IConvertible.ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)this).ToUInt32(provider);
        }

        ulong System.IConvertible.ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)this).ToUInt64(provider);
        }

        long System.IConvertible.ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)this).ToInt64(provider);
        }

        sbyte System.IConvertible.ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)this).ToSByte(provider);
        }

        float System.IConvertible.ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)this).ToSingle(provider);
        }

        string System.IConvertible.ToString(IFormatProvider provider)
        {
            return ((IConvertible)this).ToString(provider);
        }

        object System.IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)this).ToType(conversionType, provider);
        }

        ushort System.IConvertible.ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)this).ToUInt16(provider);
        }

        decimal System.IConvertible.ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)this).ToDecimal(provider);
        }

        double System.IConvertible.ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)this).ToDouble(provider);
        }

        short System.IConvertible.ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)this).ToInt16(provider);
        }

        int System.IConvertible.ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)this).ToInt32(provider);
        }

        char System.IConvertible.ToChar(IFormatProvider provider)
        {
            return ((IConvertible)this).ToChar(provider);
        }

        DateTime System.IConvertible.ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)this).ToDateTime(provider);
        }

        System.TypeCode System.IConvertible.GetTypeCode()
        {
            return (System.TypeCode)((IConvertible)this).GetTypeCode();
        }

        bool System.IConvertible.ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)this).ToBoolean(provider);
        }

        byte System.IConvertible.ToByte(IFormatProvider provider)
        {
            return ((IConvertible)this).ToByte(provider);
        }

        #endregion

        #region IConvertible Members


        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(_value);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return _value;
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            // TODO:  Add PdfInteger.ToDateTime implementation
            return new DateTime();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return _value;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(_value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return _value;
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(_value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(_value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return _value.ToString(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(_value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(_value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return _value;
        }

        /// <summary>
        /// Returns TypeCode for 32-bit integers.
        /// </summary>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Int32;
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return _value;
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            // TODO:  Add PdfInteger.ToType implementation
            return null;
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(_value);
        }

        #endregion
    }
}
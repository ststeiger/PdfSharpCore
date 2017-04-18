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

#if SILVERLIGHT || UWP || PORTABLE
using System;

#if SILVERLIGHT
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif

public class BrowsableAttribute : Attribute
{
    public BrowsableAttribute(bool browsable)
    {
    }
}

/// <summary>
/// SerializableAttribute for compatibility with Silverlight.
/// </summary>
public class SerializableAttribute : Attribute
{ }

/// <summary>
/// ICloneable for compatibility with Silverlight.
/// </summary>
public interface ICloneable
{
    /// <summary>
    /// Creates a new object that is a copy of the current instance
    /// </summary>
    Object Clone();
}

namespace PdfSharpCore
{
    /// <summary>
    /// The exception that is thrown when a non-fatal application error occurs.
    /// </summary>
    public class ApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        public ApplicationException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        public ApplicationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
#if PORTABLE
    public enum TypeCode
    {

        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18
    }

    public interface IConvertible
    {
        TypeCode GetTypeCode();

        bool ToBoolean(IFormatProvider provider);
        byte ToByte(IFormatProvider provider);
        char ToChar(IFormatProvider provider);
        DateTime ToDateTime(IFormatProvider provider);
        decimal ToDecimal(IFormatProvider provider);
        double ToDouble(IFormatProvider provider);
        short ToInt16(IFormatProvider provider);
        int ToInt32(IFormatProvider provider);
        long ToInt64(IFormatProvider provider);
        sbyte ToSByte(IFormatProvider provider);
        float ToSingle(IFormatProvider provider);
        string ToString(IFormatProvider provider);
        object ToType(Type conversionType, IFormatProvider provider);
        ushort ToUInt16(IFormatProvider provider);
        uint ToUInt32(IFormatProvider provider);
        ulong ToUInt64(IFormatProvider provider);
    }
#endif

#if SILVERLIGHT || PORTABLE
    /// <summary>
    /// The exception that is thrown when the value of an argument is outside
    /// the allowable range of values as defined by the invoked method.
    /// </summary>
    public class ArgumentOutOfRangeException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException(string message, string message2)
            : base(message, message2)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException(string message, object value, string message2)
            : base(message, message2)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentOutOfRangeException"/> class.
        /// </summary>
        public ArgumentOutOfRangeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
#endif

    /// <summary>
    /// The exception thrown when using invalid arguments that are enumerators.
    /// </summary>
    public class InvalidEnumArgumentException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnumArgumentException"/> class.
        /// </summary>
        public InvalidEnumArgumentException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnumArgumentException"/> class.
        /// </summary>
        public InvalidEnumArgumentException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnumArgumentException"/> class.
        /// </summary>
        public InvalidEnumArgumentException(string message, string message2)
            : base(message, message2)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnumArgumentException"/> class.
        /// </summary>
        public InvalidEnumArgumentException(string message, int n, Type type)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnumArgumentException"/> class.
        /// </summary>
        public InvalidEnumArgumentException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    //public class FileNotFoundException : Exception
    //{
    //  public FileNotFoundException()
    //  { }

    //  public FileNotFoundException(string message)
    //    : base(message)
    //  { }

    //  public FileNotFoundException(string message, string path)
    //    : base(message + "/" + path)
    //  { }

    //  public FileNotFoundException(string message, Exception innerException)
    //    : base(message, innerException)
    //  { }
    //}
}
#endif

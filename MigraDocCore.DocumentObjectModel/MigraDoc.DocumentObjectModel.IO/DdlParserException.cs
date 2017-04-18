#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@PdfSharpCore.com)
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//   David Stephensen (mailto:David.Stephensen@PdfSharpCore.com)
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

using System;

namespace MigraDocCore.DocumentObjectModel.IO
{
  /// <summary>
  /// Represents an exception used by the DDL parser. This exception will always be caught inside
  /// the DDL parser.
  /// </summary>
  internal class DdlParserException : Exception // TODO DaSt/KlPo/NiSc: ApplicationException???
  {
    /// <summary>
    /// Initializes a new instance of the DdlParserException class with the specified message.
    /// </summary>
    public DdlParserException(string message)
      : base(message)
    {
      this.error = new DdlReaderError(DdlErrorLevel.Error, message, 0);
    }

    /// <summary>
    /// Initializes a new instance of the DdlParserException class with the specified message and the
    /// inner exception.
    /// </summary>
    public DdlParserException(string message, Exception innerException)
      :
      base(message, innerException)
    {
      this.error = new DdlReaderError(DdlErrorLevel.Error, message, 0);
    }

    /// <summary>
    /// Initializes a new instance of the DdlParserException class with the specified error level, name,
    /// error code and message.
    /// </summary>
    public DdlParserException(DdlErrorLevel level, string message, DomMsgID errorCode)
      :
      base(message)
    {
      this.error = new DdlReaderError(level, message, (int)errorCode);
    }

    /// <summary>
    /// Initializes a new instance of the DdlParserException class with the DdlReaderError.
    /// </summary>
    public DdlParserException(DdlReaderError error)
      : base(error.ErrorMessage)
    {
      this.error = error;
    }

    /// <summary>
    /// Gets the DdlReaderError.
    /// </summary>
    public DdlReaderError Error
    {
      get { return this.error; }
    }

    DdlReaderError error;
  }
}

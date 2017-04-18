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
  /// The tokens used by DdlScanner/DdlParser.
  /// </summary>
  enum TokenType
  {
    /// <summary>
    /// White space or comment.
    /// </summary>
    None,

    /// <summary>
    /// Same as identifiers in C#, but not case sensitive.
    /// </summary>
    Identifier,

    /// <summary>
    /// Both «true» and «\bold» are keywords, case sensitive.
    /// </summary>
    KeyWord,

    /// <summary>
    /// Sample: «42»
    /// </summary>
    IntegerLiteral,

    /// <summary>
    /// Samples: «42.0», «42.», «.42»,...
    /// </summary>
    RealLiteral,

    /// <summary>
    /// Not used.
    /// </summary>
    CharacterLiteral,

    /// <summary>
    /// Both «"text"» and «@"text with ""quotes"""».
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Samples: «.», «{», «+=»,...
    /// </summary>
    OperatorOrPunctuator,

    /// <summary>
    /// Plain text. Possible after ReadText.
    /// </summary>
    Text,
  }
}

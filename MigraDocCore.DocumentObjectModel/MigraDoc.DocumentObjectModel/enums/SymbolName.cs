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

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Represents the type of the special character.
  /// </summary>
  public enum SymbolName : uint
  {
    // \space(...)
    Blank = 0xF1000001,
    En = 0xF1000002,
    Em = 0xF1000003,
    EmQuarter = 0xF1000004,
    Em4 = EmQuarter,

    // used to serialize as \tab, \linebreak
    Tab = 0xF2000001,
    LineBreak = 0xF4000001,

    // for internal use only 
    ParaBreak = 0xF4000007,
    //MarginBreak       = 0xF4000002,

    // \symbol(...)
    Euro = 0xF8000001,
    Copyright = 0xF8000002,
    Trademark = 0xF8000003,
    RegisteredTrademark = 0xF8000004,
    Bullet = 0xF8000005,
    Not = 0xF8000006,
    EmDash = 0xF8000007,
    EnDash = 0xF8000008,
    NonBreakableBlank = 0xF8000009,
    HardBlank = NonBreakableBlank,
  }
}

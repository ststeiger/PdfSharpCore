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
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Provides functions for encoding and decoding of DDL text.
  /// </summary>
  public sealed class DdlEncoder
  {
    /// <summary>
    /// Initializes a new instance of the DdlEncoder class.
    /// </summary>
    DdlEncoder()
    {
    }

    /// <summary>
    /// Converts a string into a text phrase.
    /// </summary>
    public static string StringToText(string str)
    {
      if (str == null)
        return null;

      int length = str.Length;
      StringBuilder strb = new StringBuilder(length + (int)(length >> 2));
      for (int index = 0; index < length; ++index)
      {
        // Don't convert characters into DDL.
        char ch = str[index];
        switch (ch)
        {
          case '\\':
            strb.Append("\\\\");
            break;

          case '{':
            strb.Append("\\{");
            break;

          case '}':
            strb.Append("\\}");
            break;

          // escape comments
          case '/':
            if (index < length - 1 && str[index + 1] == '/')
            {
              strb.Append("\\//");
              ++index;
            }
            else
              strb.Append("/");
            break;

          default:
            strb.Append(ch);
            break;
        }
      }
      return strb.ToString();
    }

    /// <summary>
    /// Converts a string into a string literal (a quoted string).
    /// </summary>
    public static string StringToLiteral(string str)
    {
      int length = 0;
      if (str == null || (length = str.Length) == 0)
        return "\"\"";

      StringBuilder strb = new StringBuilder(length + (int)(length >> 2));
      strb.Append("\"");
      for (int index = 0; index < length; ++index)
      {
        char ch = str[index];
        switch (ch)
        {
          case '\\':
            strb.Append("\\\\");
            break;

          case '"':
            strb.Append("\\\"");
            break;

          default:
            strb.Append(ch);
            break;
        }
      }
      strb.Append("\"");

      return strb.ToString();
    }

    /// <summary>
    /// Scans the given string for characters which are invalid for identifiers.
    /// Strings are limited to 64 characters.
    /// </summary>
    internal static bool IsDdeIdentifier(string name)
    {
      if (name == null || name == String.Empty)
        return false;

      int len = name.Length;
      if (len > 64)
        return false;

      for (int index = 0; index < len; index++)
      {
        char ch = name[index];
        if (ch == ' ')
          return false;

        if (index == 0)
        {
          if (!Char.IsLetter(ch) && ch != '_')
            return false;
        }
        else
        {
          if (!Char.IsLetterOrDigit(ch) && ch != '_')
            return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Quotes the given name, if it contains characters which are invalid for identifiers.
    /// </summary>
    internal static string QuoteIfNameContainsBlanks(string name)
    {
      if (IsDdeIdentifier(name))
        return name;
      else
        return "\"" + name + "\"";
    }

  }
}

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
using System.Globalization;
using System.IO;
using System.Text;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.IO;
using System.Reflection;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Object to be passed to the Serialize function of a DocumentObject to convert
  /// it into DDL.
  /// </summary>
  internal class Serializer
  {
    /// <summary>
    /// A Serializer object for converting MDDOM into DDL.
    /// </summary>
    /// <param name="textWriter">A TextWriter to write DDL in.</param>
    /// <param name="indent">Indent of a new block. Default is 2.</param>
    /// <param name="initialIndent">Initial indent to start with.</param>
    internal Serializer(TextWriter textWriter, int indent, int initialIndent)
    {
      if (textWriter == null)
        throw new ArgumentNullException("textWriter");

      this.textWriter = textWriter;
      this.indent = indent;
      this.writeIndent = initialIndent;
      if (textWriter is StreamWriter)
        WriteStamp();
    }

    /// <summary>
    /// Initializes a new instance of the Serializer class with the specified TextWriter.
    /// </summary>
    internal Serializer(TextWriter textWriter) : this(textWriter, 2, 0) { }

    /// <summary>
    /// Initializes a new instance of the Serializer class with the specified TextWriter and indentation.
    /// </summary>
    internal Serializer(TextWriter textWriter, int indent) : this(textWriter, indent, 0) { }

    protected TextWriter textWriter;

    /// <summary>
    /// Gets or sets the indentation for a new indentation level.
    /// </summary>
    internal int Indent
    {
      get { return this.indent; }
      set { this.indent = value; }
    }
    protected int indent = 2;

    /// <summary>
    /// Gets or sets the initial indentation which precede every line.
    /// </summary>
    internal int InitialIndent
    {
      get { return this.writeIndent; }
      set { this.writeIndent = value; }
    }
    protected int writeIndent = 0;

    /// <summary>
    /// Increases indent of DDL code.
    /// </summary>
    void IncreaseIndent()
    {
      writeIndent += indent;
    }

    /// <summary>
    /// Decreases indent of DDL code.
    /// </summary>
    void DecreaseIndent()
    {
      writeIndent -= indent;
    }

    /// <summary>
    /// Writes the header for a DDL file containing copyright and creation time information.
    /// </summary>
    internal void WriteStamp()
    {
      if (this.fWriteStamp)
      {
        WriteComment("Created by empira MigraDoc Document Object Model");
        WriteComment(String.Format("generated file created {0:d} at {0:t}", DateTime.Now));
      }
    }

    /// <summary>
    /// Appends a string indented without line feed.
    /// </summary>
    internal void Write(string str)
    {
      string wrappedStr = DoWordWrap(str);
      if (wrappedStr.Length < str.Length && wrappedStr != "")
      {
        WriteLineToStream(wrappedStr);
        Write(str.Substring(wrappedStr.Length));
      }
      else
        WriteToStream(str);
      CommitText();
    }

    /// <summary>
    /// Writes a string indented with line feed.
    /// </summary>
    internal void WriteLine(string str)
    {
      string wrappedStr = DoWordWrap(str);
      if (wrappedStr.Length < str.Length)
      {
        WriteLineToStream(wrappedStr);
        WriteLine(str.Substring(wrappedStr.Length));
      }
      else
        WriteLineToStream(wrappedStr);
      CommitText();
    }

    /// <summary>
    /// Returns the part of the string str that fits into the line (up to 80 chars).
    /// If Wordwrap is impossible it returns the input-string str itself.
    /// </summary>
    string DoWordWrap(string str)
    {
      if (str.Length + this.writeIndent < this.lineBreakBeyond)
        return str;

      int idxCRLF = str.IndexOf("\x0D\x0A");
      if (idxCRLF > 0 && idxCRLF + this.writeIndent <= this.lineBreakBeyond)
        return str.Substring(0, idxCRLF + 1);

      int splitIndexBlank = str.Substring(0, this.lineBreakBeyond - this.writeIndent).LastIndexOf(" ");
      int splitIndexCRLF = str.Substring(0, this.lineBreakBeyond - this.writeIndent).LastIndexOf("\x0D\x0A");
      int splitIndex = System.Math.Max(splitIndexBlank, splitIndexCRLF);
      if (splitIndex == -1)
        splitIndex = System.Math.Min(str.IndexOf(" ", this.lineBreakBeyond - this.writeIndent + 1),
                                     str.IndexOf("\x0D\x0A", this.lineBreakBeyond - this.writeIndent + 1));
      return splitIndex > 0 ? str.Substring(0, splitIndex) : str;

    }

    /// <summary>
    /// Writes an empty line.
    /// </summary>
    internal void WriteLine()
    {
      WriteLine(String.Empty);
    }

    /// <summary>
    /// Write a line without committing (without marking the text as serialized).
    /// </summary>
    internal void WriteLineNoCommit(string str)
    {
      WriteLineToStream(str);
    }

    /// <summary>
    /// Write a line without committing (without marking the text as serialized).
    /// </summary>
    internal void WriteLineNoCommit()
    {
      WriteLineNoCommit(String.Empty);
    }

    /// <summary>
    /// Writes a text as comment and automatically word-wraps it.
    /// </summary>
    internal void WriteComment(string comment)
    {
      if (comment == null || comment == String.Empty)
        return;

      // if string contains CR/LF, split up recursively
      int crlf = comment.IndexOf("\x0D\x0A");
      if (crlf != -1)
      {
        WriteComment(comment.Substring(0, crlf));
        WriteComment(comment.Substring(crlf + 2));
        return;
      }
      CloseUpLine();
      int len;
      int chopBeyond = this.lineBreakBeyond - this.indent - "// ".Length;
      while ((len = comment.Length) > 0)
      {
        string wrt;
        if (len <= chopBeyond)
        {
          wrt = "// " + comment;
          comment = String.Empty;
        }
        else
        {
          int idxChop;
          if ((idxChop = comment.LastIndexOf(' ', chopBeyond)) == -1 &&
              (idxChop = comment.IndexOf(' ', chopBeyond)) == -1)
          {
            wrt = "// " + comment;
            comment = String.Empty;
          }
          else
          {
            wrt = "// " + comment.Substring(0, idxChop);
            comment = comment.Substring(idxChop + 1);
          }
        }
        WriteLineToStream(wrt);
        CommitText();
      }
    }

    /// <summary>
    /// Writes a line break if the current position is not at the beginning
    /// of a new line.
    /// </summary>
    internal void CloseUpLine()
    {
      if (this.linePos > 0)
        WriteLine();
    }

    /// <summary>
    /// Effectively writes text to the stream. The text is automatically indented and
    /// word-wrapped. A given text gets never word-wrapped to keep comments or string
    /// literals unbroken.
    /// </summary>
    void WriteToStream(string text, bool fLineBreak, bool fAutoIndent)
    {
      // if string contains CR/LF, split up recursively
      int crlf = text.IndexOf("\x0D\x0A");
      if (crlf != -1)
      {
        WriteToStream(text.Substring(0, crlf), true, fAutoIndent);
        WriteToStream(text.Substring(crlf + 2), fLineBreak, fAutoIndent);
        return;
      }

      int len = text.Length;
      if (len > 0)
      {
        if (this.linePos > 0)
        {
          // does not work
          // if (IsBlankRequired(this.lastChar, _text[0]))
          //   _text = "·" + _text;
        }
        else
        {
          if (fAutoIndent)
          {
            text = Indentation + text;
            len += this.writeIndent;
          }
        }
        this.textWriter.Write(text);
        this.linePos += len;
        // wordwrap required?
        if (this.linePos > this.lineBreakBeyond)
        {
          fLineBreak = true;
          //this.textWriter.Write("//¶");  // for debugging only
        }
        else
          this.lastChar = text[len - 1];
      }

      if (fLineBreak)
      {
        this.textWriter.WriteLine(String.Empty);  // what a line break is may depend on encoding
        this.linePos = 0;
        this.lastChar = '\x0A';
      }
    }

    /// <summary>
    /// Write the text into the stream without breaking it and adds an indentation to it.
    /// </summary>
    void WriteToStream(string text)
    {
      WriteToStream(text, false, true);
    }

    /// <summary>
    /// Write a line to the stream.
    /// </summary>
    void WriteLineToStream(string text)
    {
      WriteToStream(text, true, true);
    }

    /// <summary>
    /// Mighty function to figure out if a blank is required as separator.
    /// // Does not work without context...
    /// </summary>
    bool IsBlankRequired(char left, char right)
    {
      if (left == ' ' || right == ' ')
        return false;

      // 1st try
      bool leftLetterOrDigit = Char.IsLetterOrDigit(left);
      bool rightLetterOrDigit = Char.IsLetterOrDigit(right);

      if (leftLetterOrDigit && rightLetterOrDigit)
        return true;

      return false;
    }

    /// <summary>
    /// Start attribute part.
    /// </summary>
    internal int BeginAttributes()
    {
      int pos = this.Position;
      WriteLineNoCommit("[");
      IncreaseIndent();
      BeginBlock();
      return pos;
    }

    /// <summary>
    /// Start attribute part.
    /// </summary>
    internal int BeginAttributes(string str)
    {
      int pos = this.Position;
      WriteLineNoCommit(str);
      WriteLineNoCommit("[");
      IncreaseIndent();
      BeginBlock();
      return pos;
    }

    /// <summary>
    /// End attribute part.
    /// </summary>
    internal bool EndAttributes()
    {
      DecreaseIndent();
      WriteLineNoCommit("]");
      return EndBlock();
    }

    /// <summary>
    /// End attribute part.
    /// </summary>
    internal bool EndAttributes(int pos)
    {
      bool commit = EndAttributes();
      if (!commit)
        this.Position = pos;
      return commit;
    }

    /// <summary>
    /// Write attribute of type Unit, Color, int, float, double, bool, string or enum.
    /// </summary>
    internal void WriteSimpleAttribute(string valueName, object value)
    {
      INullableValue ival = value as INullableValue;
      if (ival != null)
        value = ival.GetValue();

      Type type = value.GetType();

      if (type == typeof(Unit))
      {
        string strUnit = value.ToString();
        if (((Unit)value).Type == UnitType.Point)
          WriteLine(valueName + " = " + strUnit);
        else
          WriteLine(valueName + " = \"" + strUnit + "\"");
      }
      else if (type == typeof(float))
      {
        WriteLine(valueName + " = " + ((float)value).ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
      else if (type == typeof(double))
      {
        WriteLine(valueName + " = " + ((double)value).ToString(System.Globalization.CultureInfo.InvariantCulture));
      }
      else if (type == typeof(bool))
      {
        WriteLine(valueName + " = " + value.ToString().ToLower());
      }
      else if (type == typeof(string))
      {
        StringBuilder sb = new StringBuilder(value.ToString());
        sb.Replace("\\", "\\\\");
        sb.Replace("\"", "\\\"");
        WriteLine(valueName + " = \"" + sb.ToString() + "\"");
      }
      else if (type == typeof(int) || type.GetTypeInfo().BaseType == typeof(System.Enum) || type == typeof(Color))
      {
        WriteLine(valueName + " = " + value.ToString());
      }
      else
      {
        string message = String.Format("Type '{0}' of value '{1}' not supported", type.ToString(), valueName);
        Debug.Assert(false, message);
      }
    }

    /// <summary>
    /// Start content part.
    /// </summary>
    internal int BeginContent()
    {
      int pos = this.Position;
      WriteLineNoCommit("{");
      IncreaseIndent();
      BeginBlock();
      return pos;
    }

    /// <summary>
    /// Start content part.
    /// </summary>
    internal int BeginContent(string str)
    {
      int pos = this.Position;
      WriteLineNoCommit(str);
      WriteLineNoCommit("{");
      IncreaseIndent();
      BeginBlock();
      return pos;
    }

    /// <summary>
    /// End content part.
    /// </summary>
    internal bool EndContent()
    {
      DecreaseIndent();
      WriteLineNoCommit("}");
      return EndBlock();
    }

    /// <summary>
    /// End content part.
    /// </summary>
    internal bool EndContent(int pos)
    {
      bool commit = EndContent();
      if (!commit)
        this.Position = pos;
      return commit;
    }

    /// <summary>
    /// Starts a new nesting block.
    /// </summary>
    internal int BeginBlock()
    {
      int pos = this.Position;
      if (stackIdx + 1 >= commitTextStack.Length)
        throw new ArgumentException("Block nesting level exhausted.");
      stackIdx += 1;
      commitTextStack[stackIdx] = false;
      return pos;
    }

    /// <summary>
    /// Ends a nesting block.
    /// </summary>
    internal bool EndBlock()
    {
      if (stackIdx <= 0)
        throw new ArgumentException("Block nesting level underflow.");
      stackIdx -= 1;
      if (commitTextStack[stackIdx + 1])
        commitTextStack[stackIdx] = commitTextStack[stackIdx + 1];
      return commitTextStack[stackIdx + 1];
    }

    /// <summary>
    /// Ends a nesting block.
    /// </summary>
    internal bool EndBlock(int pos)
    {
      bool commit = EndBlock();
      if (!commit)
        this.Position = pos;
      return commit;
    }

    /// <summary>
    /// Gets or sets the position within the underlying stream.
    /// </summary>
    int Position
    {
      get
      {
        textWriter.Flush();
        if (textWriter is StreamWriter)
          return (int)((StreamWriter)textWriter).BaseStream.Position;
        else if (textWriter is StringWriter)
          return ((StringWriter)textWriter).GetStringBuilder().Length;
        return 0;
      }
      set
      {
        textWriter.Flush();
        if (textWriter is StreamWriter)
          ((StreamWriter)textWriter).BaseStream.SetLength(value);
        else if (textWriter is StringWriter)
          ((StringWriter)textWriter).GetStringBuilder().Length = value;
      }
    }

    /// <summary>
    /// Flushes the buffers of the underlying text writer.
    /// </summary>
    internal void Flush()
    {
      textWriter.Flush();
    }

    /// <summary>
    /// Returns an indent string of blanks.
    /// </summary>
    static string Ind(int indent)
    {
      return new String(' ', indent);
    }

    /// <summary>
    /// Gets an indent string of current indent.
    /// </summary>
    string Indentation
    {
      get { return Ind(writeIndent); }
    }

    /// <summary>
    /// Marks the current block as 'committed'. That means the block contains
    /// serialized data.
    /// </summary>
    private void CommitText()
    {
      commitTextStack[stackIdx] = true;
    }
    private int stackIdx = 0;
    private bool[] commitTextStack = new bool[32];

    int linePos;
    int lineBreakBeyond = 200;
    char lastChar;
    bool fWriteStamp = false;
  }
}

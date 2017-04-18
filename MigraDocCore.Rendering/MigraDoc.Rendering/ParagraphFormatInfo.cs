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

using System;
using System.Collections;
using PdfSharpCore.Drawing;
using MigraDocCore.DocumentObjectModel;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Vertical measurements of a paragraph line.
  /// </summary>
  internal struct VerticalLineInfo
  {
    internal VerticalLineInfo(XUnit height, XUnit descent, XUnit inherentlineSpace)
    {
      this.height = height;
      this.descent = descent;
      this.inherentlineSpace = inherentlineSpace;
    }
    internal XUnit height;
    internal XUnit descent;
    internal XUnit inherentlineSpace;
  }

  /// <summary>
  /// Line info object used by the paragraph format info.
  /// </summary>
  internal struct LineInfo
  {
    internal ParagraphIterator startIter;
    internal ParagraphIterator endIter;
    internal XUnit wordsWidth;
    internal XUnit lineWidth;
    internal int blankCount;
    internal VerticalLineInfo vertical;
    internal ArrayList tabOffsets;
    internal bool reMeasureLine;
    internal DocumentObject lastTab;
  }

  /// <summary>
  /// Formatting information for a paragraph.
  /// </summary>
  internal class ParagraphFormatInfo : FormatInfo
  {
    ArrayList lineInfos = new ArrayList();

    internal ParagraphFormatInfo()
    {
    }

    internal LineInfo GetLineInfo(int lineIdx)
    {
      return (LineInfo)this.lineInfos[lineIdx];
    }

    internal LineInfo GetLastLineInfo()
    {
      return (LineInfo)this.lineInfos[this.LineCount - 1];
    }

    internal LineInfo GetFirstLineInfo()
    {
      return (LineInfo)this.lineInfos[0];
    }

    internal void AddLineInfo(LineInfo lineInfo)
    {
      this.lineInfos.Add(lineInfo);
    }

    internal int LineCount
    {
      get { return this.lineInfos.Count; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mergeInfo"></param>
    /// <returns></returns>
    internal void Append(FormatInfo mergeInfo)
    {
      ParagraphFormatInfo formatInfo = (ParagraphFormatInfo)mergeInfo;
      this.lineInfos.AddRange(formatInfo.lineInfos);
    }

    /// <summary>
    /// Indicates whether the paragraph is ending.
    /// </summary>
    /// <returns>True if the paragraph is ending.</returns>
    internal override bool IsEnding
    {
      get { return this.isEnding; }
    }
    internal bool isEnding;

    /// <summary>
    /// Indicates whether the paragraph is starting.
    /// </summary>
    /// <returns>True if the paragraph is starting.</returns>
    internal override bool IsStarting
    {
      get { return this.isStarting; }
    }
    internal bool isStarting;

    internal override bool IsComplete
    {
      get { return this.isStarting && this.isEnding; }
    }

    internal override bool IsEmpty
    {
      get { return this.lineInfos.Count == 0; }
    }

    internal override bool StartingIsComplete
    {
      get
      {
        if (this.widowControl)
          return (this.IsComplete || (this.isStarting && this.lineInfos.Count >= 2));
        else
          return this.isStarting;
      }
    }

    internal bool widowControl;

    internal override bool EndingIsComplete
    {
      get
      {
        if (this.widowControl)
          return (this.IsComplete || (this.isEnding && this.lineInfos.Count >= 2));
        else
          return this.isEnding;
      }
    }

    internal void RemoveEnding()
    {
      if (!this.IsEmpty)
      {
        if (this.widowControl && this.isEnding && this.LineCount >= 2)
          this.lineInfos.RemoveAt(this.LineCount - 2);
        if (this.LineCount > 0)
          this.lineInfos.RemoveAt(this.LineCount - 1);

        this.isEnding = false;
      }
    }

    internal string listSymbol;
    internal XFont listFont;
    internal Hashtable imageRenderInfos;
  }
}

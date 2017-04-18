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
using System.Collections.Generic;
using MigraDocCore.DocumentObjectModel;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Field information used to fill fields when rendering or formatting.
  /// </summary>
  internal class FieldInfos
  {
    internal FieldInfos(Dictionary<string, BookmarkInfo> bookmarks)
    {
      this.bookmarks = bookmarks;
    }

    internal struct BookmarkInfo
    {
      internal BookmarkInfo(int physicalPageNumber, int displayPageNumber)
      {
        this.displayPageNumber = physicalPageNumber;
        this.shownPageNumber = displayPageNumber;
      }

      internal int displayPageNumber;
      internal int shownPageNumber;
    }

    internal void AddBookmark(string name)
    {
      if (this.pyhsicalPageNr <= 0)
        return;

      if (this.bookmarks.ContainsKey(name))
        this.bookmarks.Remove(name);

      if (this.pyhsicalPageNr > 0)
        this.bookmarks.Add(name, new BookmarkInfo(this.pyhsicalPageNr, this.displayPageNr));
    }

    internal int GetShownPageNumber(string bookmarkName)
    {
      if (this.bookmarks.ContainsKey(bookmarkName))
      {
        BookmarkInfo bi = this.bookmarks[bookmarkName];
        return bi.shownPageNumber;
      }
      return -1;
    }

    internal int GetPhysicalPageNumber(string bookmarkName)
    {
      if (this.bookmarks.ContainsKey(bookmarkName))
      {
        BookmarkInfo bi = this.bookmarks[bookmarkName];
        return bi.displayPageNumber;
      }
      return -1;
    }

    Dictionary<string, BookmarkInfo> bookmarks;
    internal int displayPageNr;
    internal int pyhsicalPageNr;
    internal int section;
    internal int sectionPages;
    internal int numPages;
    internal DateTime date;
  }
}

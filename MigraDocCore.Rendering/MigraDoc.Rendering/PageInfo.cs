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
using PdfSharpCore;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Provides information necessary to render the page.
  /// </summary>
  public class PageInfo
  {
    internal PageInfo(XUnit width, XUnit height, PageOrientation orientation)
    {
      this.width = width;
      this.height = height;
      this.orientation = orientation;
    }

    /// <summary>
    /// Gets the with of the described page as specified in Document.PageSetup, i.e. the orientation
    /// is not taken into account.
    /// </summary>
    public XUnit Width
    {
      get { return this.width; }
    }
    private XUnit width;

    /// <summary>
    /// Gets the height of the described page as specified in Document.PageSetup, i.e. the orientation
    /// is not taken into account.
    /// </summary>
    public XUnit Height
    {
      get { return this.height; }
    }
    private XUnit height;

    /// <summary>
    /// Gets the orientation of the described page as specified in Document.PageSetup.
    /// The value has no influence on the properties Width or Height, i.e. if the result is PageOrientation.Landscape
    /// you must exchange the values of Width or Height to get the real page size.
    /// </summary>
    public PageOrientation Orientation
    {
      get { return this.orientation; }
    }
    private PageOrientation orientation;
  }
}

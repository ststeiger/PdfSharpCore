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

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Determines the parts of a page to be rendered.
  /// </summary>
  [Flags]
  public enum PageRenderOptions
  {
    /// <summary>
    /// renders nothing (creates an empty page)
    /// </summary>
    None = 0,
    /// <summary>
    /// renders Headers
    /// </summary>
    RenderHeader = 1,
    /// <summary>
    /// renders Footers
    /// </summary>
    RenderFooter = 2,
    /// <summary>
    /// renders Content
    /// </summary>
    RenderContent = 4,
    /// <summary>
    /// renders PDF Background pages
    /// </summary>
    RenderPdfBackground = 8,
    /// <summary>
    /// renders PDF content pages
    /// </summary>
    RenderPdfContent = 16,
    /// <summary>
    /// renders all
    /// </summary>
    All = RenderHeader | RenderFooter | RenderContent | RenderPdfBackground | RenderPdfContent,
    /// <summary>
    /// creates not even an empty page
    /// </summary>
    RemovePage = 32
  }
}

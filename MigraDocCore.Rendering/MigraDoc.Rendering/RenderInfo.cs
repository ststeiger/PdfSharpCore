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
using MigraDocCore.DocumentObjectModel;
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Abstract base class for all classes that store rendering information.
  /// </summary>
  public abstract class RenderInfo
  {
    internal abstract FormatInfo FormatInfo
    {
      get;
    }

    public LayoutInfo LayoutInfo
    {
      get { return this.layoutInfo; }
    }
    LayoutInfo layoutInfo = new LayoutInfo();

    public abstract DocumentObject DocumentObject
    {
      get;
    }

    internal virtual void RemoveEnding()
    {
      System.Diagnostics.Debug.Assert(false, "Unexpected call of RemoveEnding");
    }

    internal static XUnit GetTotalHeight(RenderInfo[] renderInfos)
    {
      if (renderInfos == null || renderInfos.Length == 0)
        return 0;

      int lastIdx = renderInfos.Length - 1;
      RenderInfo firstRenderInfo = renderInfos[0];
      RenderInfo lastRenderInfo = renderInfos[lastIdx];
      LayoutInfo firstLayoutInfo = firstRenderInfo.LayoutInfo;
      LayoutInfo lastLayoutInfo = lastRenderInfo.LayoutInfo;
      XUnit top = firstLayoutInfo.ContentArea.Y - firstLayoutInfo.MarginTop;
      XUnit bottom = lastLayoutInfo.ContentArea.Y + lastLayoutInfo.ContentArea.Height;
      bottom += lastLayoutInfo.MarginBottom;
      return bottom - top;
    }
  }
}

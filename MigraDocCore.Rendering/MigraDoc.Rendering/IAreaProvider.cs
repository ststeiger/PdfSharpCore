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
namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Represents a class that provides a series of Areas to render into.
  /// </summary>
  internal interface IAreaProvider
  {
    /// <summary>
    /// Gets the next area to render into.
    /// </summary>
    Area GetNextArea();

    /// <summary>
    /// Probes the next area to render into like GetNextArea, but doesn't change the provider state. 
    /// </summary>
    /// <returns>The area for the next rendering act.</returns>
    Area ProbeNextArea();

    FieldInfos AreaFieldInfos
    {
      get;
    }

    /// <summary>
    /// Determines whether the element requires an area break before.
    /// </summary>
    bool IsAreaBreakBefore(LayoutInfo layoutInfo);


    /// <summary>
    /// Positions the element vertically relatively to the current area.
    /// </summary>
    /// <param name="layoutInfo">The layout info of the element.</param>
    /// <returns>True, if the element was moved by the function.</returns>
    bool PositionVertically(LayoutInfo layoutInfo);

    /// <summary>
    /// Positions the element horizontally relatively to the current area.
    /// </summary>
    /// <param name="layoutInfo">The layout info of the element.</param>
    /// <returns>True, if the element was moved by the function.</returns>
    bool PositionHorizontally(LayoutInfo layoutInfo);

    /// <summary>
    /// Stores the RenderInfos of elements on the current area.
    /// </summary>
    /// <param name="renderInfos"></param>
    void StoreRenderInfos(ArrayList renderInfos);
  }
}

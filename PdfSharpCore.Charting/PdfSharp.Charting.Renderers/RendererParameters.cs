#region PDFsharp Charting - A .NET charting library based on PDFsharp
//
// Authors:
//   Niklas Schneider (mailto:Niklas.Schneider@PdfSharpCore.com)
//
// Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.PdfSharpCore.com
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
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting.Renderers
{
  /// <summary>
  /// Represents the necessary data for chart rendering.
  /// </summary>
  internal class RendererParameters
  {
    /// <summary>
    /// Initializes a new instance of the RendererParameters class.
    /// </summary>
    public RendererParameters()
    {
    }

    /// <summary>
    /// Initializes a new instance of the RendererParameters class with the specified graphics and
    /// coordinates.
    /// </summary>
    public RendererParameters(XGraphics gfx, double x, double y, double width, double height)
    {
      this.gfx = gfx;
      this.box = new XRect(x, y, width, height);
    }

    /// <summary>
    /// Initializes a new instance of the RendererParameters class with the specified graphics and
    /// rectangle.
    /// </summary>
    public RendererParameters(XGraphics gfx, XRect boundingBox)
    {
      this.gfx = gfx;
      this.box = boundingBox;
    }

    /// <summary>
    /// Gets or sets the graphics object.
    /// </summary>
    public XGraphics Graphics
    {
      get {return this.gfx;}
      set {this.gfx = value;}
    }
    XGraphics gfx;

    /// <summary>
    /// Gets or sets the item to draw.
    /// </summary>
    public object DrawingItem
    {
      get {return this.item;}
      set {this.item = value;}
    }
    object item;

    /// <summary>
    /// Gets or sets the rectangle for the drawing item.
    /// </summary>
    public XRect Box
    {
      get {return this.box;}
      set {this.box = value;}
    }
    XRect box;

    /// <summary>
    /// Gets or sets the RendererInfo.
    /// </summary>
    public RendererInfo RendererInfo
    {
      get {return this.rendererInfo;}
      set {this.rendererInfo = value;}
    }
    RendererInfo rendererInfo;
  }
}
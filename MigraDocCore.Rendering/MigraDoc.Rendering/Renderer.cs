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
using PdfSharpCore.Pdf;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Abstract base class for all renderers.
  /// </summary>
  internal abstract class Renderer
  {
    internal readonly static XUnit Tolerance = XUnit.FromPoint(0.001);
    private XUnit maxElementHeight = -1;

    /// <summary>
    /// Determines the maximum height a single element may have.
    /// </summary>
    internal XUnit MaxElementHeight
    {
      get { return this.maxElementHeight; }
      set {this.maxElementHeight = value;}
    }

    internal Renderer(XGraphics gfx, DocumentObject documentObject, FieldInfos fieldInfos)
    {
      this.documentObject = documentObject;
      this.gfx = gfx;
      this.fieldInfos = fieldInfos;
    }

    internal Renderer(XGraphics gfx, RenderInfo renderInfo, FieldInfos fieldInfos)
    {
      this.documentObject = renderInfo.DocumentObject;
      this.gfx = gfx;
      this.renderInfo = renderInfo;
      this.fieldInfos = fieldInfos;
    }
    /// <summary>
    /// In inherited classes, gets a layout info with only margin and break information set.
    /// It can be taken before the documen object is formatted.
    /// </summary>
    /// <remarks>
    /// In inherited classes, the following parts are set properly:
    /// MarginTop, MarginLeft, MarginRight, MarginBottom, 
    /// KeepTogether, KeepWithNext, PagebreakBefore, Floating,
    /// VerticalReference, HorizontalReference.
    /// </remarks>
    internal abstract LayoutInfo InitialLayoutInfo
    {
      get;
    }

    /// <summary>
    /// Renders the contents shifted to the given Coordinates.
    /// </summary>
    /// <param name="xShift">The x shift.</param>
    /// <param name="yShift">The y shift.</param>
    /// <param name="renderInfos">The render infos.</param>
    protected void RenderByInfos(XUnit xShift, XUnit yShift, RenderInfo[] renderInfos)
    {
      if (renderInfos == null)
        return;

      foreach (RenderInfo renderInfo in renderInfos)
      {
        XUnit savedX = renderInfo.LayoutInfo.ContentArea.X;
        XUnit savedY = renderInfo.LayoutInfo.ContentArea.Y;
        renderInfo.LayoutInfo.ContentArea.X += xShift;
        renderInfo.LayoutInfo.ContentArea.Y += yShift;
        Renderer renderer = Renderer.Create(this.gfx, this.documentRenderer, renderInfo, this.fieldInfos);
        renderer.Render();
        renderInfo.LayoutInfo.ContentArea.X = savedX;
        renderInfo.LayoutInfo.ContentArea.Y = savedY;
      }
    }

    protected void RenderByInfos(RenderInfo[] renderInfos)
    {
      RenderByInfos(0, 0, renderInfos);
    }


    /// <summary>
    /// Gets the render information necessary to render and position the object.
    /// </summary>
    internal RenderInfo RenderInfo
    {
      get { return this.renderInfo; }
    }
    protected RenderInfo renderInfo;

    /// <summary>
    /// Sets the field infos object.
    /// </summary>
    /// <remarks>This property is set by the AreaProvider.</remarks>
    internal FieldInfos FieldInfos
    {
      set { this.fieldInfos = value; }
    }
    protected FieldInfos fieldInfos;

    /// <summary>
    /// Renders (draws) the object to the Graphics object.
    /// </summary>
    internal abstract void Render();

    /// <summary>
    /// Formats the object by calculating distances and linebreaks and stopping when the area is filled.
    /// </summary>
    /// <param name="area">The area to render into.</param>
    /// <param name="previousFormatInfo">An information object received from a previous call of Format().
    /// Null for the first call.</param>
    internal abstract void Format(Area area, FormatInfo previousFormatInfo);

    /// <summary>
    /// Creates a fitting renderer for the given document object for formatting.
    /// </summary>
    /// <param name="gfx">The XGraphics object to do measurements on.</param>
    /// <param name="documentRenderer">The document renderer.</param>
    /// <param name="documentObject">the document object to format.</param>
    /// <param name="fieldInfos">The field infos.</param>
    /// <returns>The fitting Renderer.</returns>
    internal static Renderer Create(XGraphics gfx, DocumentRenderer documentRenderer, DocumentObject documentObject, FieldInfos fieldInfos)
    {
      Renderer renderer = null;
      if (documentObject is Paragraph)
        renderer = new ParagraphRenderer(gfx, (Paragraph)documentObject, fieldInfos);
      else if (documentObject is Table)
        renderer = new TableRenderer(gfx, (Table)documentObject, fieldInfos);
      else if (documentObject is PageBreak)
        renderer = new PageBreakRenderer(gfx, (PageBreak)documentObject, fieldInfos);
      else if (documentObject is TextFrame)
        renderer = new TextFrameRenderer(gfx, (TextFrame)documentObject, fieldInfos);
      else if (documentObject is Chart)
        renderer = new ChartRenderer(gfx, (Chart)documentObject, fieldInfos);
      else if (documentObject is Image)
        renderer = new ImageRenderer(gfx, (Image)documentObject, fieldInfos);

      if (renderer != null)
        renderer.documentRenderer = documentRenderer;

      return renderer;
    }

    /// <summary>
    /// Creates a fitting renderer for the render info to render and layout with.
    /// </summary>
    /// <param name="gfx">The XGraphics object to render on.</param>
    /// <param name="documentRenderer">The document renderer.</param>
    /// <param name="renderInfo">The RenderInfo object stored after a previous call of Format().</param>
    /// <param name="fieldInfos">The field infos.</param>
    /// <returns>The fitting Renderer.</returns>
    internal static Renderer Create(XGraphics gfx, DocumentRenderer documentRenderer, RenderInfo renderInfo, FieldInfos fieldInfos)
    {
      Renderer renderer = null;

      if (renderInfo.DocumentObject is Paragraph)
        renderer = new ParagraphRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is Table)
        renderer = new TableRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is PageBreak)
        renderer = new PageBreakRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is TextFrame)
        renderer = new TextFrameRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is Chart)
        renderer = new ChartRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is Chart)
        renderer = new ChartRenderer(gfx, renderInfo, fieldInfos);
      else if (renderInfo.DocumentObject is Image)
        renderer = new ImageRenderer(gfx, renderInfo, fieldInfos);

      if (renderer != null)
        renderer.documentRenderer = documentRenderer;

      return renderer;
    }

    #region fields

    protected DocumentObject documentObject;
    protected DocumentRenderer documentRenderer;
    protected XGraphics gfx;

    #endregion
  }
}

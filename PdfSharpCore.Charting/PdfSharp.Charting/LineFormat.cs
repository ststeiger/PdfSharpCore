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
using System.ComponentModel;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Defines the format of a line in a shape object.
  /// </summary>
  public class LineFormat : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the LineFormat class.
    /// </summary>
    public LineFormat()
    {
    }

    /// <summary>
    /// Initializes a new instance of the LineFormat class with the specified parent.
    /// </summary>
    internal LineFormat(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new LineFormat Clone()
    {
      return (LineFormat)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether the line should be visible.
    /// </summary>
    public bool Visible
    {
      get { return this.visible; }
      set { this.visible = value; }
    }
    internal bool visible;

    /// <summary>
    /// Gets or sets the width of the line in XUnit.
    /// </summary>
    public XUnit Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    internal XUnit width;

    /// <summary>
    /// Gets or sets the color of the line.
    /// </summary>
    public XColor Color
    {
      get { return this.color; }
      set { this.color = value; }
    }
    internal XColor color = XColor.Empty;

    /// <summary>
    /// Gets or sets the dash style of the line.
    /// </summary>
    public XDashStyle DashStyle
    {
      get { return this.dashStyle; }
      set { this.dashStyle = value; }
    }
    internal XDashStyle dashStyle;

    /// <summary>
    /// Gets or sets the style of the line.
    /// </summary>
    public LineStyle Style
    {
      get { return this.style; }
      set { this.style = value; }
    }
    internal LineStyle style;
    #endregion
  }
}
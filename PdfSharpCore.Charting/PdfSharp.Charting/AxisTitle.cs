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
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Represents the title of an axis.
  /// </summary>
  public class AxisTitle : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the AxisTitle class.
    /// </summary>
    public AxisTitle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the AxisTitle class with the specified parent.
    /// </summary>
    internal AxisTitle(DocumentObject parent) : base(parent) {}

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new AxisTitle Clone()
    {
      return (AxisTitle)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      AxisTitle axisTitle = (AxisTitle)base.DeepCopy();
      if (axisTitle.font != null)
      {
        axisTitle.font = axisTitle.font.Clone();
        axisTitle.font.parent = axisTitle;
      }
      return axisTitle;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the caption of the title.
    /// </summary>
    public string Caption
    {
      get {return this.caption;}
      set {this.caption = value;}
    }
    internal string caption = String.Empty;

    /// <summary>
    /// Gets the font of the title.
    /// </summary>
    public Font Font
    {
      get
      {
        if (this.font == null)
          this.font = new Font(this);

        return this.font;
      }
    }
    internal Font font;

    /// <summary>
    /// Gets or sets the orientation of the caption.
    /// </summary>
    public double Orientation
    {
      get {return this.orientation;}
      set {this.orientation = value;}
    }
    internal double orientation;

    /// <summary>
    /// Gets or sets the horizontal alignment of the caption.
    /// </summary>
    public HorizontalAlignment Alignment
    {
      get {return this.alignment;}
      set {this.alignment = value;}
    }
    internal HorizontalAlignment alignment;

    /// <summary>
    /// Gets or sets the vertical alignment of the caption.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
      get {return this.verticalAlignment;}
      set {this.verticalAlignment = value;}
    }
    internal VerticalAlignment verticalAlignment;
    #endregion
  }
}

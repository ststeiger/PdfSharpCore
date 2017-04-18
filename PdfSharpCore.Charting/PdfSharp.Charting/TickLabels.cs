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
  /// Represents the format of the label of each value on the axis.
  /// </summary>
  public class TickLabels : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the TickLabels class.
    /// </summary>
    public TickLabels()
    {
    }

    /// <summary>
    /// Initializes a new instance of the TickLabels class with the specified parent.
    /// </summary>
    internal TickLabels(DocumentObject parent) : base(parent) {}

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new TickLabels Clone()
    {
      return (TickLabels)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      TickLabels tickLabels = (TickLabels)base.DeepCopy();
      if (tickLabels.font != null)
      {
        tickLabels.font = tickLabels.font.Clone();
        tickLabels.font.parent = tickLabels;
      }
      return tickLabels;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the label's number format.
    /// </summary>
    public string Format
    {
      get {return this.format;}
      set {this.format = value;}
    }
    internal string format = String.Empty;

    /// <summary>
    /// Gets the font of the label.
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
    #endregion
  }
}

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
  /// Represents the area where the actual chart is drawn.
  /// </summary>
  public class PlotArea : ChartObject
  {
    /// <summary>
    /// Initializes a new instance of the PlotArea class.
    /// </summary>
    internal PlotArea()
    {
    }

    /// <summary>
    /// Initializes a new instance of the PlotArea class with the specified parent.
    /// </summary>
    internal PlotArea(DocumentObject parent) : base(parent) {}

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new PlotArea Clone()
    {
      return (PlotArea)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      PlotArea plotArea = (PlotArea)base.DeepCopy();
      if (plotArea.lineFormat != null)
      {
        plotArea.lineFormat = plotArea.lineFormat.Clone();
        plotArea.lineFormat.parent = plotArea;
      }
      if (plotArea.fillFormat != null)
      {
        plotArea.fillFormat = plotArea.fillFormat.Clone();
        plotArea.fillFormat.parent = plotArea;
      }
      return plotArea;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the line format of the plot area's border.
    /// </summary>
    public LineFormat LineFormat
    {
      get
      {
        if (this.lineFormat == null)
          this.lineFormat = new LineFormat(this);

        return this.lineFormat;
      }
    }
    internal LineFormat lineFormat;

    /// <summary>
    /// Gets the background filling of the plot area.
    /// </summary>
    public FillFormat FillFormat
    {
      get
      {
        if (this.fillFormat == null)
          this.fillFormat = new FillFormat(this);

        return this.fillFormat;
      }
    }
    internal FillFormat fillFormat;

    /// <summary>
    /// Gets or sets the left padding of the area.
    /// </summary>
    public XUnit LeftPadding
    {
      get {return this.leftPadding;}
      set {this.leftPadding = value;}
    }
    internal XUnit leftPadding;

    /// <summary>
    /// Gets or sets the right padding of the area.
    /// </summary>
    public XUnit RightPadding
    {
      get {return this.rightPadding;}
      set {this.rightPadding = value;}
    }
    internal XUnit rightPadding;

    /// <summary>
    /// Gets or sets the top padding of the area.
    /// </summary>
    public XUnit TopPadding
    {
      get {return this.topPadding;}
      set {this.topPadding = value;}
    }
    internal XUnit topPadding;

    /// <summary>
    /// Gets or sets the bottom padding of the area.
    /// </summary>
    public XUnit BottomPadding
    {
      get {return this.bottomPadding;}
      set {this.bottomPadding = value;}
    }
    internal XUnit bottomPadding;
    #endregion
  }
}

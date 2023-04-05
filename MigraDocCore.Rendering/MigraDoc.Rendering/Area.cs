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
using PdfSharpCore.Drawing;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Abstract base class for all areas to render in.
  /// </summary>
  public abstract class Area
  {
    internal Area()
    {
    }

    /// <summary>
    /// Gets the left boundary of the area.
    /// </summary>
    public abstract XUnit X
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the top boundary of the area.
    /// </summary>
    public abstract XUnit Y
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the largest fitting rect with the given y position and height.
    /// </summary>
    /// <param name="yPosition">Top bound of the searched rectangle.</param>
    /// <param name="height">Height of the searched rectangle.</param>
    /// <returns>
    /// The largest fitting rect with the given y position and height.
    /// Null if yPosition exceeds the area.
    /// </returns>
    internal abstract Rectangle GetFittingRect(XUnit yPosition, XUnit height);

    /// <summary>
    /// Gets or sets the height of the smallest rectangle containing the area. 
    /// </summary>
    public abstract XUnit Height
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the width of the smallest rectangle containing the area. 
    /// </summary>
    public abstract XUnit Width
    {
      get;
      set;
    }

    /// <summary>
    /// Returns the union of this area snd the given one.
    /// </summary>
    /// <param name="area">The area to unite with.</param>
    /// <returns>The union of the two areas.</returns>
    internal abstract Area Unite(Area area);

    /// <summary>
    /// Lowers the area and makes it smaller.
    /// </summary>
    /// <param name="verticalOffset">The measure of lowering.</param>
    /// <returns>The lowered Area.</returns>
    internal abstract Area Lower(XUnit verticalOffset);
  }

  internal class Rectangle : Area
  {
    /// <summary>
    /// Initializes a new rectangle object.
    /// </summary>
    /// <param name="x">Left bound of the rectangle.</param>
    /// <param name="y">Upper bound of the rectangle.</param>
    /// <param name="width">Width of the rectangle.</param>
    /// <param name="height">Height of the rectangle.</param>
    internal Rectangle(XUnit x, XUnit y, XUnit width, XUnit height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    /// <summary>
    /// Initializes a new Rectangle by copying its values.
    /// </summary>
    /// <param name="rect">The rectangle to copy.</param>
    internal Rectangle(Rectangle rect)
    {
      this.x = rect.x;
      this.y = rect.y;
      this.width = rect.width;
      this.height = rect.height;
    }

    /// <summary>
    /// Gets the largest fitting rect with the given y position and height.
    /// </summary>
    /// <param name="yPosition">Top bound of the searched rectangle.</param>
    /// <param name="height">Height of the searched rectangle.</param>
    /// <returns>The largest fitting rect with the given y position and height</returns>
    internal override Rectangle GetFittingRect(XUnit yPosition, XUnit height)
    {
      // BUG: Code removed because null is not handled in caller
#if true
      if (yPosition + height > this.y + this.height + Renderer.Tolerance)
        return null;
#endif
      return new Rectangle(this.x, yPosition, this.width, height);
    }

    /// <summary>
    /// Gets or sets the left boundary of the rectangle. 
    /// </summary>
    public override XUnit X
    {
      get { return this.x; }
      set { this.x = value; }
    }
    XUnit x;

    /// <summary>
    /// Gets or sets the top boundary of the rectangle. 
    /// </summary>
    public override XUnit Y
    {
      get { return this.y; }
      set { this.y = value; }
    }
    XUnit y;

    /// <summary>
    /// Gets or sets the top boundary of the rectangle. 
    /// </summary>
    public override XUnit Width
    {
      get { return this.width; }
      set { this.width = value; }
    }
    XUnit width;

    /// <summary>
    /// Gets or sets the height of the rectangle. 
    /// </summary>
    public override XUnit Height
    {
      get { return this.height; }
      set { this.height = value; }
    }
    XUnit height;

    /// <summary>
    /// Returns the union of the rectangle and the given area.
    /// </summary>
    /// <param name="area">The area to unite with.</param>
    /// <returns>The union of the two areas.</returns>
    internal override Area Unite(Area area)
    {
      if (area == null)
        return this;
      //This implementation is of course not correct, but it works for our purposes.
      XUnit minTop = Math.Min(this.y, area.Y);
      XUnit minLeft = Math.Min(this.x, area.X);
      XUnit maxRight = Math.Max(this.x + this.width, area.X + area.Width);
      XUnit maxBottom = Math.Max(this.y + this.height, area.Y + area.Height);
      return new Rectangle(minLeft, minTop, maxRight - minLeft, maxBottom - minTop);
    }

    internal override Area Lower(XUnit verticalOffset)
    {
      return new Rectangle(this.x, this.y + verticalOffset, this.width, this.height - verticalOffset);
    }
  }
}

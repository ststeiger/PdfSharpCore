#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@PdfSharpCore.com)
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
//   David Stephensen (mailto:David.Stephensen@PdfSharpCore.com)
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
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel.Shapes.Charts
{
  /// <summary>
  /// Represents the collection of the values in a data series.
  /// </summary>
  public class SeriesElements : DocumentObjectCollection
  {
    /// <summary>
    /// Initializes a new instance of the SeriesElements class.
    /// </summary>
    internal SeriesElements()
    {
    }

    /// <summary>
    /// Initializes a new instance of the SeriesElements class with the specified parent.
    /// </summary>
    internal SeriesElements(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new SeriesElements Clone()
    {
      return (SeriesElements)DeepCopy();
    }

    /// <summary>
    /// Adds a blank to the series.
    /// </summary>
    public void AddBlank()
    {
      base.Add((DocumentObject)null);
    }

    /// <summary>
    /// Adds a new point with a real value to the series.
    /// </summary>
    public Point Add(double value)
    {
      Point point = new Point(value);
      Add(point);
      return point;
    }

    /// <summary>
    /// Adds an array of new points with real values to the series.
    /// </summary>
    public void Add(params double[] values)
    {
      foreach (double val in values)
        this.Add(val);
    }
    #endregion

    #region Internal
    /// <summary>
    /// Converts SeriesElements into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int count = Count;
      for (int index = 0; index < count; ++index)
      {
        Point point = this[index] as Point;
        if (point == null)
          serializer.Write("null, ");
        else
          point.Serialize(serializer);
      }
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(SeriesElements));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

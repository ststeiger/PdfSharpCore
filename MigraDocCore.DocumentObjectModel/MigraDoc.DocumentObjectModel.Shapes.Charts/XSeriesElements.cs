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
  /// Represents the collection of the value in an XSeries.
  /// </summary>
  public class XSeriesElements : DocumentObjectCollection
  {
    /// <summary>
    /// Initializes a new instance of the XSeriesElements class.
    /// </summary>
    public XSeriesElements()
    {
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new XSeriesElements Clone()
    {
      return (XSeriesElements)base.DeepCopy();
    }

    /// <summary>
    /// Adds a blank to the XSeries.
    /// </summary>
    public void AddBlank()
    {
      base.Add((DocumentObject)null);
    }

    /// <summary>
    /// Adds a value to the XSeries.
    /// </summary>
    public XValue Add(string value)
    {
      XValue xValue = new XValue(value);
      Add(xValue);
      return xValue;
    }

    /// <summary>
    /// Adds an array of values to the XSeries.
    /// </summary>
    public void Add(params string[] values)
    {
      foreach (string val in values)
        this.Add(val);
    }
    #endregion

    #region Internal
    /// <summary>
    /// Converts XSeriesElements into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int count = Count;
      for (int index = 0; index < count; index++)
      {
        XValue xValue = this[index] as XValue;
        if (xValue == null)
          serializer.Write("null, ");
        else
          xValue.Serialize(serializer);
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
          meta = new Meta(typeof(XSeriesElements));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

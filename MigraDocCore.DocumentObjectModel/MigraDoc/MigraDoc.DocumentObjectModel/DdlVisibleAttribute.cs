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

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Under Construction.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  internal class DdlVisibleAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the DdlVisibleAttribute class.
    /// </summary>
    public DdlVisibleAttribute()
    {
      visible = true;
    }

    /// <summary>
    /// Initializes a new instance of the DdlVisibleAttribute class with the specified visibility.
    /// </summary>
    public DdlVisibleAttribute(bool _visible)
    {
      visible = _visible;
    }

    /// <summary>
    /// Gets or sets the visibility.
    /// </summary>
    public bool Visible
    {
      get { return visible; }
      set { visible = value; }
    }
    bool visible;

    public bool CanAddValue
    {
      get { return canAddValue; }
      set { canAddValue = value; }
    }
    bool canAddValue;

    public bool CanRemoveValue
    {
      get { return canRemoveValue; }
      set { canRemoveValue = value; }
    }
    bool canRemoveValue;
  }
}

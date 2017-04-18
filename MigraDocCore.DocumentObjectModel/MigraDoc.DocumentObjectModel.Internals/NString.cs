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

namespace MigraDocCore.DocumentObjectModel.Internals
{
  /// <summary>
  /// Represents a nullable string value.
  /// </summary>
  internal struct NString : INullableValue
  {
    /// <summary>
    /// Gets or sets the value of the instance.
    /// </summary>
    public string Value
    {
      get { return this.val != null ? this.val : ""; }
      set { this.val = value; }
    }

    /// <summary>
    /// Returns the value of the instance.
    /// </summary>
    object INullableValue.GetValue()
    {
      return this.Value;
    }

    /// <summary>
    /// Sets the value of the instance.
    /// </summary>
    void INullableValue.SetValue(object value)
    {
      this.val = (String)value;
    }

    /// <summary>
    /// Resets this instance,
    /// i.e. IsNull() will return true afterwards.
    /// </summary>
    public void SetNull()
    {
      this.val = null;
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    public bool IsNull
    {
      get { return this.val == null; }
    }

    /// <summary>
    /// Returns a value indicating whether this instance is equal to the specified object.
    /// </summary>
    public override bool Equals(object value)
    {
      if (value is NString)
        return this == (NString)value;
      return false;
    }

    public override int GetHashCode()
    {
      return this.val == null ? 0 : this.val.GetHashCode();
    }

    public static bool operator ==(NString l, NString r)
    {
      if (l.IsNull)
        return r.IsNull;
      else if (r.IsNull)
        return false;
      else
        return l.Value == r.Value;
    }

    public static bool operator !=(NString l, NString r)
    {
      return !(l == r);
    }

    public static readonly NString NullValue = new NString();

    string val;
  }
}
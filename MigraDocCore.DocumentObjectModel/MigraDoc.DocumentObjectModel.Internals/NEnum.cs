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
using System.ComponentModel;

namespace MigraDocCore.DocumentObjectModel.Internals
{
  ///// <summary>
  ///// Exists only to make code like 'NInt n = null' compile.
  ///// </summary>
  //public class Dummy
  //{
  //  Dummy() {}
  //}

  /// <summary>
  /// Represents a nullable Enum value.
  /// </summary>
  internal struct NEnum : INullableValue
  {
    public NEnum(int val, Type type)
    {
      this.type = type;
      this.val = val;
    }

    NEnum(int value)
    {
      this.type = null;
      this.val = value;
    }

    internal Type Type
    {
      get { return this.type; }
      set { this.type = value; }
    }
    Type type;

    public int Value
    {
      get { return this.val != int.MinValue ? this.val : 0; }
      set
      {
        //TODO: Klasse Character So ändern, dass symbolName und char in unterschiedlichen Feldern gespeichert wird
        //Diese Spezialbehandlung entfällt dann.
        if (this.type == typeof(SymbolName))
        {
          //          if (Enum.IsDefined(this.type, (uint)value))
          this.val = value;
          //          else
          //            throw new ArgumentOutOfRangeException("value");
        }
        else
        {
          if (Enum.IsDefined(this.type, value))
            this.val = value;
          else
            throw new ArgumentException("value");
        }
      }
    }

    object INullableValue.GetValue()
    {
      return ToObject();
    }

    void INullableValue.SetValue(object value)
    {
      this.val = (int)value;
    }

    public void SetNull()
    {
      this.val = int.MinValue;
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    public bool IsNull
    {
      get { return this.val == int.MinValue; }
    }

    public object ToObject()
    {
      if (this.val != int.MinValue)
        return Enum.ToObject(this.type, this.val);
      // BUG Have all enum 0 as valid value?
      return Enum.ToObject(this.type, 0);
    }

    //public static readonly NEnum NullValue = new NEnum(int.MinValue);

    /// <summary>
    /// Returns a value indicating whether this instance is equal to the specified object.
    /// </summary>
    public override bool Equals(object value)
    {
      if (value is NEnum)
        return this == (NEnum)value;
      return false;
    }

    public override int GetHashCode()
    {
      return this.val.GetHashCode();
    }

    public static bool operator ==(NEnum l, NEnum r)
    {
      if (l.IsNull)
        return r.IsNull;
      else if (r.IsNull)
        return false;
      else
      {
        if (l.type == r.type)
          return l.Value == r.Value;
        else
          return false;
      }
    }

    public static bool operator !=(NEnum l, NEnum r)
    {
      return !(l == r);
    }

    public static NEnum NullValue(Type fieldType)
    {
      return new NEnum(int.MinValue, fieldType);
    }

    int val;
  }
}

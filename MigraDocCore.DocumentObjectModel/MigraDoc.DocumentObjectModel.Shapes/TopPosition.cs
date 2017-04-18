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
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;

namespace MigraDocCore.DocumentObjectModel.Shapes
{
  /// <summary>
  /// Represents the top position in a shape.
  /// </summary>
  public struct TopPosition : INullableValue
  {
    /// <summary>
    /// Initializes a new instance of TopPosition from Unit.
    /// </summary>
    private TopPosition(Unit value)
    {
      this.shapePosition = ShapePosition.Undefined;
      this.position = value;
      this.notNull = !value.IsNull;
    }

    /// <summary>
    /// Initializes a new instance of TopPosition from ShapePosition.
    /// </summary>
    private TopPosition(ShapePosition value)
    {
      if (!(IsValid(value) || value == ShapePosition.Undefined))
        throw new ArgumentException(AppResources.InvalidEnumForTopPosition);

      this.shapePosition = value;
      this.position = Unit.NullValue;
      this.notNull = (value != ShapePosition.Undefined);
    }

    /// <summary>
    /// Indicates the given shapePosition is valid for TopPosition.
    /// </summary>
    private static bool IsValid(ShapePosition shapePosition)
    {
      return shapePosition == ShapePosition.Bottom ||
             shapePosition == ShapePosition.Top ||
             shapePosition == ShapePosition.Center;
    }

    /// <summary>
    /// Converts a ShapePosition to a TopPosition.
    /// </summary>
    public static implicit operator TopPosition(ShapePosition value)
    {
      return new TopPosition(value);
    }

    /// <summary>
    /// Converts a Unit to a TopPosition.
    /// </summary>
    public static implicit operator TopPosition(Unit val)
    {
      return new TopPosition(val);
    }

    /// <summary>
    /// Converts a string to a TopPosition.
    /// The string is interpreted as a Unit.
    /// </summary>
    public static implicit operator TopPosition(string value)
    {
      Unit unit = value;
      return new TopPosition(unit);
    }

    /// <summary>
    /// Converts a double to a TopPosition.
    /// The double is interpreted as a Unit in Point.
    /// </summary>
    public static implicit operator TopPosition(double value)
    {
      Unit unit = value;
      return new TopPosition(unit);
    }

    /// <summary>
    /// Converts an integer to a TopPosition. 
    /// The integer is interpreted as a Unit in Point.
    /// </summary>
    public static implicit operator TopPosition(int value)
    {
      Unit unit = value;
      return new TopPosition(unit);
    }

    /// <summary>
    /// Sets shapeposition enum and resets position.
    /// </summary>
    private void SetFromEnum(ShapePosition shapePosition)
    {
      if (!IsValid(shapePosition))
        throw new ArgumentException(AppResources.InvalidEnumForTopPosition);

      this.shapePosition = shapePosition;
      this.position = Unit.NullValue;
    }

    /// <summary>
    /// Sets the Position from a Unit.
    /// </summary>
    private void SetFromUnit(Unit unit)
    {
      this.shapePosition = ShapePosition.Undefined;
      this.position = unit;
    }

    /// <summary>
    /// Sets the Position from an object.
    /// </summary>
    void INullableValue.SetValue(object value)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      if (value is ShapePosition)
        SetFromEnum((ShapePosition)value);
      else if (value is string && Enum.IsDefined(typeof(ShapePosition), value))
        SetFromEnum((ShapePosition)Enum.Parse(typeof(ShapePosition), (string)value));
      else
        SetFromUnit(value.ToString());

      this.notNull = true;
    }

    /// <summary>
    /// Gets the Position as Unit or ShapePosition.
    /// </summary>
    object INullableValue.GetValue()
    {
      if (this.shapePosition == ShapePosition.Undefined)
        return this.position;

      return this.shapePosition;
    }

    /// <summary>
    /// Resets this instance, i.e. IsNull() will return true afterwards.
    /// </summary>
    void INullableValue.SetNull()
    {
      this = new TopPosition();
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    bool INullableValue.IsNull
    {
      get { return !this.notNull; }
    }

    /// <summary>
    /// Gets the value of the position in unit.
    /// </summary>
    public Unit Position
    {
      get { return this.position; }
    }

    /// <summary>
    /// Gets the value of the position.
    /// </summary>
    public ShapePosition ShapePosition
    {
      get { return this.shapePosition; }
    }
    internal ShapePosition shapePosition;
    internal Unit position;
    private bool notNull;

    /// <summary>
    /// Parses the specified value.
    /// </summary>
    public static TopPosition Parse(string value)
    {
      if (value == null || value.Length == 0)
        throw new ArgumentNullException("value");

      value = value.Trim();
      char ch = value[0];
      if (ch == '+' || ch == '-' || Char.IsNumber(ch))
        return Unit.Parse(value);
      else
        return (ShapePosition)Enum.Parse(typeof(ShapePosition), value, true);
    }

    #region Internal
    /// <summary>
    /// Converts TopPosition into DDL.
    /// </summary>  
    internal void Serialize(Serializer serializer)
    {
      if (this.shapePosition == ShapePosition.Undefined)
        serializer.WriteSimpleAttribute("Top", this.Position);
      else
        serializer.WriteSimpleAttribute("Top", this.ShapePosition);
    }
    #endregion

    /// <summary>
    /// Represents the unitialized TopPosition object.
    /// </summary>
    internal static readonly TopPosition NullValue = new TopPosition();
  }
}

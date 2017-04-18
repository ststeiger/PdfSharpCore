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
using System.Diagnostics;
using System.Reflection;
using MigraDocCore.DocumentObjectModel.Internals;

#if old
namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// The Color class represents an ARGB color value.
  /// </summary>
  public struct Color : INullableValue
  {
    /// <summary>
    /// Initializes a new instance of the Color class.
    /// </summary>
    public Color(uint argb)
    {
      this.argb = argb;
    }

    /// <summary>
    /// Initializes a new instance of the Color class.
    /// </summary>
    public Color(byte r, byte g, byte b)
    {
      this.argb = 0xFF000000 | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
    }

    /// <summary>
    /// Initializes a new instance of the Color class.
    /// </summary>
    public Color(byte a, byte r, byte g, byte b)
    {
      this.argb = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | (uint)b;
    }

    /// <summary>
    /// Determines whether this color is empty.
    /// </summary>
    public bool IsEmpty
    {
      get { return this == Color.Empty; }
    }

    /// <summary>
    /// Returns the value.
    /// </summary>
    object INullableValue.GetValue()
    {
      return this;
    }

    /// <summary>
    /// Sets the given value.
    /// </summary>
    void INullableValue.SetValue(object value)
    {
      if (value is uint)
        this.argb = (uint)value;
      else
        this = Color.Parse(value.ToString());
    }

    /// <summary>
    /// Resets this instance, i.e. IsNull() will return true afterwards.
    /// </summary>
    void INullableValue.SetNull()
    {
      this = Color.Empty;
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    bool INullableValue.IsNull
    {
      get { return this == Color.Empty; }
    }

    /// <summary>
    /// Determines whether this instance is null (not set).
    /// </summary>
    internal bool IsNull
    {
      get { return this == Color.Empty; }
    }

    /// <summary>
    /// Gets or sets the ARGB value.
    /// </summary>
    public uint Argb
    {
      get { return argb; }
      set { argb = value; }
    }
    uint argb;

    /// <summary>
    /// Gets or sets the RGB value.
    /// </summary>
    public uint RGB
    {
      get { return argb; }
      set { argb = value; }
    }

    /// <summary>
    /// Calls base class Equals.
    /// </summary>
    public override bool Equals(Object obj)
    {
      if (obj is Color)
        return this.argb == ((Color)obj).argb;
      return false;
    }

    /// <summary>
    /// Gets the ARGB value that this Color instance represents.
    /// </summary>
    public override int GetHashCode()
    {
      return (int)this.argb;
    }

    /// <summary>
    /// Compares two color objects. True if both argb values are equal, false otherwise.
    /// </summary>
    public static bool operator ==(Color clr1, Color clr2)
    {
      return clr1.argb == clr2.argb;
    }

    /// <summary>
    /// Compares two color objects. True if both argb values are not equal, false otherwise.
    /// </summary>
    public static bool operator !=(Color clr1, Color clr2)
    {
      return clr1.argb != clr2.argb;
    }

    /// <summary>
    /// Parses the string and returns a color object.
    /// Throws ArgumentException if color is invalid.
    /// </summary>
    /// <param name="color">integer, hex or color name.</param>
    public static Color Parse(string color)
    {
      if (color == null)
        throw new ArgumentNullException("color");
      if (color == "")
        throw new ArgumentException("color");

      try
      {
        uint clr = 0;
        // Must use Enum.Parse because Enum.IsDefined is case sensitive
        try
        {
          object obj = Enum.Parse(typeof(ColorName), color, true);
          clr = (uint)obj;
          return new Color(clr);
        }
        catch
        {
          //ignore exception cause it's not a ColorName.
        }

        System.Globalization.NumberStyles numberStyle = System.Globalization.NumberStyles.Integer;
        string number = color.ToLower();
        if (number.StartsWith("0x"))
        {
          numberStyle = System.Globalization.NumberStyles.HexNumber;
          number = color.Substring(2);
        }
        clr = uint.Parse(number, numberStyle);
        return new Color(clr);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(DomSR.InvalidColorString(color), ex);
      }
    }

    /// <summary>
    /// Gets the alpha (transparency) part of the Color.
    /// </summary>
    public uint A
    {
      get { return (this.argb & 0xFF000000) >> 24; }
    }

    /// <summary>
    /// Gets the red part of the Color.
    /// </summary>
    public uint R
    {
      get { return (this.argb & 0xFF0000) >> 16; }
    }

    /// <summary>
    /// Gets the green part of the Color.
    /// </summary>
    public uint G
    {
      get { return (this.argb & 0x00FF00) >> 8; }
    }

    /// <summary>
    /// Gets the blue part of the Color.
    /// </summary>
    public uint B
    {
      get { return this.argb & 0x0000FF; }
    }

    /// <summary>
    /// Gets a non transparent color brightened in terms of transparency if any is given(A &lt; 255),
    /// otherwise this instance itself.
    /// </summary>
    public Color GetMixedTransparencyColor()
    {
      int alpha = (int)A;
      if (alpha == 0xFF)
        return this;

      int red = (int)R;
      int green = (int)G;
      int blue = (int)B;

      double whiteFactor = 1 - alpha / 255.0;

      red = (int)(red + (255 - red) * whiteFactor);
      green = (int)(green + (255 - green) * whiteFactor);
      blue = (int)(blue + (255 - blue) * whiteFactor);
      return new Color((uint)(0xFF << 24 | (red << 16) | (green << 8) | blue));
    }

    /// <summary>
    /// Writes the Color object in its hexadecimal value.
    /// </summary>
    public override string ToString()
    {
      if (stdColors == null)
      {
        Array colorNames = Enum.GetNames(typeof(ColorName));
        Array colorValues = Enum.GetValues(typeof(ColorName));
        int count = colorNames.GetLength(0);
        stdColors = new Hashtable(count);
        for (int index = 0; index < count; index++)
        {
          string c = (string)colorNames.GetValue(index);
          uint d = (uint)colorValues.GetValue(index);
          // Some color are double named...
          // Aqua == Cyan
          // Fuchsia == Magenta
          if (!stdColors.ContainsKey(d))
            stdColors.Add(d, c);
        }
      }
      if (stdColors.ContainsKey(argb))
        return (string)stdColors[argb];
      else
      {
        if ((this.argb & 0xFF000000) == 0xFF000000)
          return "RGB(" +
            ((this.argb & 0xFF0000) >> 16).ToString() + "," +
            ((this.argb & 0x00FF00) >> 8).ToString() + "," +
            (this.argb & 0x0000FF).ToString() + ")";
        else
          return "0x" + argb.ToString("X");
      }
    }
    static Hashtable stdColors;

    /// <summary>
    /// Creates a color from an existing color with a new alpha (transparency) value.
    /// </summary>
    public static Color FromColor(byte a, Color clr)
    {
      return new Color(a, (byte)clr.R, (byte)clr.G, (byte)clr.B);
    }

    /// <summary>
    /// Represents a null color.
    /// </summary>
    public static readonly Color Empty = new Color(0);
  }
}
#endif
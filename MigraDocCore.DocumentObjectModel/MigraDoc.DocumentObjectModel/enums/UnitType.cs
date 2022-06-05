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

namespace MigraDocCore.DocumentObjectModel
{

  /// <summary>
  /// Specifies the measure of an Unit object.
  /// </summary>
  public enum UnitType
  {
    /// <summary>
    /// Measure is in points. A point represents 1/72 of an inch. 
    /// </summary>
    Point = 0,  // Default for new Unit() is Point

    /// <summary>
    /// Measure is in centimeter. 
    /// </summary>
    Centimeter = 1,

    //[Obsolete("Use Centimeter")]
    //CM = 1,

    /// <summary>
    /// Measure is in inch. 
    /// </summary>
    Inch = 2,

    /// <summary>
    /// Measure is in millimeter. 
    /// </summary>
    Millimeter = 3,

    //[Obsolete("Use Millimeter")]
    //MM = 3,

    /// <summary>
    /// Measure is in picas. A pica represents 12 points, i.e. 6 pica are one inch.
    /// </summary>
    Pica = 4,
  }
}

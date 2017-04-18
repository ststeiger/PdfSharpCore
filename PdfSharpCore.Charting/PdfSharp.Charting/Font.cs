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
using System.Diagnostics;
using System.Reflection;
using System.Globalization;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Font represents the formatting of characters in a paragraph.
  /// </summary>
  public sealed class Font : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the Font class that can be used as a template.
    /// </summary>
    public Font()
    {}

    /// <summary>
    /// Initializes a new instance of the Font class with the specified parent.
    /// </summary>
    internal Font(DocumentObject parent) : base(parent)
    {}

    /// <summary>
    /// Initializes a new instance of the Font class with the specified name and size.
    /// </summary>
    public Font(string name, XUnit size) : this()
    {
      this.name = name;
      this.size = size;
    }

    #region Methods
    /// <summary>
    /// Creates a copy of the Font.
    /// </summary>
    public new Font Clone()
    {
      return (Font)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the name of the font.
    /// </summary>
    public string Name 
    {
      get {return this.name;}
      set {this.name = value;}
    }
    internal string name = String.Empty;

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    public XUnit Size
    {
      get {return this.size;}
      set {this.size = value;}
    }
    internal XUnit size;

    /// <summary>
    /// Gets or sets the bold property.
    /// </summary>
    public bool Bold
    {
      get {return this.bold;}
      set {this.bold = value;}
    }
    internal bool bold;

    /// <summary>
    /// Gets or sets the italic property.
    /// </summary>
    public bool Italic
    {
      get {return this.italic;}
      set {this.italic = value;}
    }
    internal bool italic;
    
    /// <summary>
    /// Gets or sets the underline property.
    /// </summary>
    public Underline Underline
    {
      get {return this.underline;}
      set {this.underline = value;}
    }
    internal Underline underline;

    /// <summary>
    /// Gets or sets the color property.
    /// </summary>
    public XColor Color
    {
      get {return this.color;}
      set {this.color = value;}
    }
    internal XColor color = XColor.Empty;

    /// <summary>
    /// Gets or sets the superscript property.
    /// </summary>
    public bool Superscript
    {
      get {return this.superscript;}
      set 
      {
        this.superscript = value;
        this.subscript = false;
      }    
    }
    internal bool superscript;

    /// <summary>
    /// Gets or sets the subscript property.
    /// </summary>
    public bool Subscript
    {
      get {return this.subscript;}
      set 
      {
        this.subscript = value;
        this.superscript = false;
      }
    }
    internal bool subscript;
    #endregion
  }
}

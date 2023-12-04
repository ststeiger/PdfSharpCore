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
using System.Diagnostics;
using System.Reflection;
using MigraDocCore.DocumentObjectModel.Internals;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Represents text in a paragraph.
  /// </summary>
  public class Text : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the Text class.
    /// </summary>
    public Text()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Text class with the specified parent.
    /// </summary>
    internal Text(DocumentObject parent) : base(parent) { }

    /// <summary>
    /// Initializes a new instance of the Text class with a string as paragraph content.
    /// </summary>
    public Text(string content)
      : this()
    {
      //is this constructor needed? or just the default constructor?
      this.Content = content;
    }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Text Clone()
    {
      return (Text)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    public string Content
    {
      get { return this.content.Value; }
      set { this.content.Value = value; }
    }
    [DV]
    internal NString content = NString.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts Text into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      string text = DdlEncoder.StringToText(content.Value);
      // To make DDL more readable write soft hypens as keywords.
      text = text.Replace(new string((char)173, 1), "\\-");
      serializer.Write(text);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Text));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

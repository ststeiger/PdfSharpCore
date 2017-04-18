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
  /// A ListInfo is the representation of a series of paragraphs as a list.
  /// </summary>
  public class ListInfo : DocumentObject
  {
    /// <summary>
    /// Initializes a new instance of the ListInfo class.
    /// </summary>
    public ListInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ListInfo class with the specified parent.
    /// </summary>
    internal ListInfo(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new ListInfo Clone()
    {
      return (ListInfo)DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the type of the list.
    /// </summary>
    public ListType ListType
    {
      get { return (ListType)this.listType.Value; }
      set { this.listType.Value = (int)value; }
    }
    [DV(Type = typeof(ListType))]
    internal NEnum listType = NEnum.NullValue(typeof(ListType));

    /// <summary>
    /// Gets or sets the left indent of the list symbol.
    /// </summary>
    public Unit NumberPosition
    {
      get { return this.numberPosition; }
      set { this.numberPosition = value; }
    }
    [DV]
    internal Unit numberPosition = Unit.NullValue;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the previous list numbering should be continued.
    /// </summary>
    public bool ContinuePreviousList
    {
      get { return this.continuePreviousList.Value; }
      set { this.continuePreviousList.Value = value; }
    }
    [DV]
    internal NBool continuePreviousList = NBool.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts ListInfo into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      if (!this.listType.IsNull)
        serializer.WriteSimpleAttribute("ListInfo.ListType", this.ListType);
      if (!this.numberPosition.IsNull)
        serializer.WriteSimpleAttribute("ListInfo.NumberPosition", this.NumberPosition);
      if (!this.continuePreviousList.IsNull)
        serializer.WriteSimpleAttribute("ListInfo.ContinuePreviousList", this.ContinuePreviousList);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(ListInfo));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

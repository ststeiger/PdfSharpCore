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

namespace MigraDocCore.DocumentObjectModel.Tables
{
  /// <summary>
  /// Represents the collection of all cells of a row.
  /// </summary>
  public class Cells : DocumentObjectCollection
  {
    /// <summary>
    /// Initializes a new instance of the Cells class.
    /// </summary>
    public Cells()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Cells class with the specified parent.
    /// </summary>
    internal Cells(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Cells Clone()
    {
      return (Cells)base.DeepCopy();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the table the cells collection belongs to.
    /// </summary>
    public Table Table
    {
      get
      {
        if (this.table == null)
        {
          Row rw = this.Parent as Row;
          if (rw != null)
            this.table = rw.Table;
        }
        return this.table;
      }
    }
    Table table;

    /// <summary>
    /// Gets the row the cells collection belongs to.
    /// </summary>
    public Row Row
    {
      get
      {
        if (this.row == null)
          this.row = this.Parent as Row;

        return this.row;
      }
    }
    Row row;

    /// <summary>
    /// Gets a cell by its index. The first cell has the index 0.
    /// </summary>
    public new Cell this[int index]
    {
      get
      {
        if (index < 0 || (this.Table != null && index >= this.Table.Columns.Count))
          throw new ArgumentOutOfRangeException("index");

        Resize(index);
        return base[index] as Cell;
      }
    }
    #endregion

    /// <summary>
    /// Resizes this cells' list if necessary.
    /// </summary>
    private void Resize(int index)
    {
      for (int currentIndex = this.Count; currentIndex <= index; currentIndex++)
        Add(new Cell());
    }

    #region Internal
    /// <summary>
    /// Converts Cells into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      int cells = Count;
      for (int cell = 0; cell < cells; cell++)
        this[cell].Serialize(serializer);
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Cells));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

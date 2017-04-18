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
using System.Collections;
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;

namespace MigraDocCore.DocumentObjectModel.Tables
{
  /// <summary>
  /// Represents a table in a document.
  /// </summary>
  public class Table : DocumentObject, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the Table class.
    /// </summary>
    public Table()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Table class with the specified parent.
    /// </summary>
    internal Table(DocumentObject parent) : base(parent) { }

    #region Methods
    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new Table Clone()
    {
      return (Table)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      Table table = (Table)base.DeepCopy();
      if (table.columns != null)
      {
        table.columns = table.columns.Clone();
        table.columns.parent = table;
      }
      if (table.rows != null)
      {
        table.rows = table.rows.Clone();
        table.rows.parent = table;
      }
      if (table.format != null)
      {
        table.format = table.format.Clone();
        table.format.parent = table;
      }
      if (table.borders != null)
      {
        table.borders = table.borders.Clone();
        table.borders.parent = table;
      }
      if (table.shading != null)
      {
        table.shading = table.shading.Clone();
        table.shading.parent = table;
      }
      return table;
    }

    /// <summary>
    /// Adds a new column to the table. Allowed only before any row was added.
    /// </summary>
    public Column AddColumn()
    {
      return this.Columns.AddColumn();
    }

    /// <summary>
    /// Adds a new column of the specified width to the table. Allowed only before any row was added.
    /// </summary>
    public Column AddColumn(Unit width)
    {
      Column clm = this.Columns.AddColumn();
      clm.Width = width;
      return clm;
    }

    /// <summary>
    /// Adds a new row to the table. Allowed only if at least one column was added.
    /// </summary>
    public Row AddRow()
    {
      return this.rows.AddRow();
    }

    /// <summary>
    /// Returns true if no cell exists in the table.
    /// </summary>
    public bool IsEmpty
    {
      get { return this.Rows.Count == 0 || this.Columns.Count == 0; }
    }

    /// <summary>
    /// Sets a shading of the specified Color in the specified Tablerange.
    /// </summary>
    public void SetShading(int clm, int row, int clms, int rows, Color clr)
    {
      int rowsCount = this.rows.Count;
      int clmsCount = this.columns.Count;

      if (row < 0 || row >= rowsCount)
        throw new ArgumentOutOfRangeException("row", row, "Invalid row index.");

      if (clm < 0 || clm >= clmsCount)
        throw new ArgumentOutOfRangeException("clm", clm, "Invalid column index.");

      if (rows <= 0 || row + rows > rowsCount)
        throw new ArgumentOutOfRangeException("rows", rows, "Invalid row count.");

      if (clms <= 0 || clm + clms > clmsCount)
        throw new ArgumentOutOfRangeException("clms", clms, "Invalid column count.");

      int maxRow = row + rows - 1;
      int maxClm = clm + clms - 1;
      for (int r = row; r <= maxRow; r++)
      {
        Row currentRow = this.rows[r];
        for (int c = clm; c <= maxClm; c++)
          currentRow[c].Shading.Color = clr;
      }
    }

    /// <summary>
    /// Sets the borders surrounding the specified range of the table.
    /// </summary>
    public void SetEdge(int clm, int row, int clms, int rows,
      Edge edge, BorderStyle style, Unit width, Color clr)
    {
      Border border;
      int maxRow = row + rows - 1;
      int maxClm = clm + clms - 1;
      for (int r = row; r <= maxRow; r++)
      {
        Row currentRow = this.rows[r];
        for (int c = clm; c <= maxClm; c++)
        {
          Cell currentCell = currentRow[c];
          if ((edge & Edge.Top) == Edge.Top && r == row)
          {
            border = currentCell.Borders.Top;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.Left) == Edge.Left && c == clm)
          {
            border = currentCell.Borders.Left;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.Bottom) == Edge.Bottom && r == maxRow)
          {
            border = currentCell.Borders.Bottom;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.Right) == Edge.Right && c == maxClm)
          {
            border = currentCell.Borders.Right;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.Horizontal) == Edge.Horizontal && r < maxRow)
          {
            border = currentCell.Borders.Bottom;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.Vertical) == Edge.Vertical && c < maxClm)
          {
            border = currentCell.Borders.Right;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.DiagonalDown) == Edge.DiagonalDown)
          {
            border = currentCell.Borders.DiagonalDown;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
          if ((edge & Edge.DiagonalUp) == Edge.DiagonalUp)
          {
            border = currentCell.Borders.DiagonalUp;
            border.Style = style;
            border.Width = width;
            if (clr != Color.Empty)
              border.Color = clr;
          }
        }
      }
    }

    /// <summary>
    /// Sets the borders surrounding the specified range of the table.
    /// </summary>
    public void SetEdge(int clm, int row, int clms, int rows, Edge edge, BorderStyle style, Unit width)
    {
      SetEdge(clm, row, clms, rows, edge, style, width, Color.Empty);
    }

    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the Columns collection of the table.
    /// </summary>
    public Columns Columns
    {
      get
      {
        if (this.columns == null)
          this.columns = new Columns(this);

        return this.columns;
      }
      set
      {
        SetParent(value);
        this.columns = value;
      }
    }
    [DV]
    internal Columns columns;

    /// <summary>
    /// Gets the Rows collection of the table.
    /// </summary>
    public Rows Rows
    {
      get
      {
        if (this.rows == null)
          this.rows = new Rows(this);

        return this.rows;
      }
      set
      {
        SetParent(value);
        this.rows = value;
      }
    }
    [DV]
    internal Rows rows;

    /// <summary>
    /// Sets or gets the default style name for all rows and columns of the table.
    /// </summary>
    public string Style
    {
      get { return this.style.Value; }
      set { this.style.Value = value; }
    }
    [DV]
    internal NString style = NString.NullValue;

    /// <summary>
    /// Gets the default ParagraphFormat for all rows and columns of the table.
    /// </summary>
    public ParagraphFormat Format
    {
      get
      {
        if (this.format == null)
          this.format = new ParagraphFormat(this);

        return this.format;
      }
      set
      {
        SetParent(value);
        this.format = value;
      }
    }
    [DV]
    internal ParagraphFormat format;

    /// <summary>
    /// Gets or sets the default top padding for all cells of the table.
    /// </summary>
    public Unit TopPadding
    {
      get { return this.topPadding; }
      set { this.topPadding = value; }
    }
    [DV]
    internal Unit topPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the default bottom padding for all cells of the table.
    /// </summary>
    public Unit BottomPadding
    {
      get { return this.bottomPadding; }
      set { this.bottomPadding = value; }
    }
    [DV]
    internal Unit bottomPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the default left padding for all cells of the table.
    /// </summary>
    public Unit LeftPadding
    {
      get { return this.leftPadding; }
      set { this.leftPadding = value; }
    }
    [DV]
    internal Unit leftPadding = Unit.NullValue;

    /// <summary>
    /// Gets or sets the default right padding for all cells of the table.
    /// </summary>
    public Unit RightPadding
    {
      get { return this.rightPadding; }
      set { this.rightPadding = value; }
    }
    [DV]
    internal Unit rightPadding = Unit.NullValue;

    /// <summary>
    /// Gets the default Borders object for all cells of the column.
    /// </summary>
    public Borders Borders
    {
      get
      {
        if (this.borders == null)
          this.borders = new Borders(this);

        return this.borders;
      }
      set
      {
        SetParent(value);
        this.borders = value;
      }
    }
    [DV]
    internal Borders borders;

    /// <summary>
    /// Gets the default Shading object for all cells of the column.
    /// </summary>
    public Shading Shading
    {
      get
      {
        if (this.shading == null)
          this.shading = new Shading(this);

        return this.shading;
      }
      set
      {
        SetParent(value);
        this.shading = value;
      }
    }
    [DV]
    internal Shading shading;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// to keep all the table rows on the same page.
    /// </summary>
    public bool KeepTogether
    {
      get { return this.keepTogether.Value; }
      set { this.keepTogether.Value = value; }
    }
    [DV]
    internal NBool keepTogether = NBool.NullValue;

    /// <summary>
    /// Gets or sets a comment associated with this object.
    /// </summary>
    public string Comment
    {
      get { return this.comment.Value; }
      set { this.comment.Value = value; }
    }
    [DV]
    internal NString comment = NString.NullValue;
    #endregion

    #region Internal
    /// <summary>
    /// Converts Table into DDL.
    /// </summary>
    internal override void Serialize(Serializer serializer)
    {
      serializer.WriteComment(this.comment.Value);

      serializer.WriteLine("\\table");

      int pos = serializer.BeginAttributes();

      if (this.style.Value != String.Empty)
        serializer.WriteSimpleAttribute("Style", this.Style);

      if (!this.IsNull("Format"))
        this.format.Serialize(serializer, "Format", null);

      if (!this.topPadding.IsNull)
        serializer.WriteSimpleAttribute("TopPadding", this.TopPadding);

      if (!this.leftPadding.IsNull)
        serializer.WriteSimpleAttribute("LeftPadding", this.LeftPadding);

      if (!this.rightPadding.IsNull)
        serializer.WriteSimpleAttribute("RightPadding", this.RightPadding);

      if (!this.bottomPadding.IsNull)
        serializer.WriteSimpleAttribute("BottomPadding", this.BottomPadding);

      if (!this.IsNull("Borders"))
        this.borders.Serialize(serializer, null);

      if (!this.IsNull("Shading"))
        this.shading.Serialize(serializer);

      serializer.EndAttributes(pos);

      serializer.BeginContent();
      this.Columns.Serialize(serializer);
      this.Rows.Serialize(serializer);
      serializer.EndContent();
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitTable(this);

      ((IVisitable)this.columns).AcceptVisitor(visitor, visitChildren);
      ((IVisitable)this.rows).AcceptVisitor(visitor, visitChildren);
    }

    /// <summary>
    /// Gets the cell with the given row and column indices.
    /// </summary>
    public Cell this[int rwIdx, int clmIdx]
    {
      get { return this.Rows[rwIdx].Cells[clmIdx]; }
    }

    /// <summary>
    /// Returns the meta object of this instance.
    /// </summary>
    internal override Meta Meta
    {
      get
      {
        if (meta == null)
          meta = new Meta(typeof(Table));
        return meta;
      }
    }
    static Meta meta;
    #endregion
  }
}

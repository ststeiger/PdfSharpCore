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
using MigraDocCore.DocumentObjectModel.IO;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using MigraDocCore.DocumentObjectModel.Tables;

namespace MigraDocCore.DocumentObjectModel.Visitors
{
  /// <summary>
  /// Represents the base visitor for the DocumentObject.
  /// </summary>
  public abstract class DocumentObjectVisitor
  {

    public abstract void Visit(DocumentObject documentObject);

    //Chart
    internal virtual void VisitChart(Chart chart) { }
    internal virtual void VisitTextArea(TextArea textArea) { }
    internal virtual void VisitLegend(Legend legend) { }

    //Document
    internal virtual void VisitDocument(Document document) { }
    internal virtual void VisitDocumentElements(DocumentElements elements) { }
    internal virtual void VisitDocumentObjectCollection(DocumentObjectCollection elements) { }

    //Fields

    //Format
    internal virtual void VisitFont(Font font) { }
    internal virtual void VisitParagraphFormat(ParagraphFormat paragraphFormat) { }
    internal virtual void VisitShading(Shading shading) { }
    internal virtual void VisitStyle(Style style) { }
    internal virtual void VisitStyles(Styles styles) { }

    //Paragraph
    internal virtual void VisitFootnote(Footnote footnote) { }
    internal virtual void VisitHyperlink(Hyperlink hyperlink) { }
    internal virtual void VisitFormattedText(FormattedText formattedText) { }
    internal virtual void VisitParagraph(Paragraph paragraph) { }

    //Section
    internal virtual void VisitHeaderFooter(HeaderFooter headerFooter) { }
    internal virtual void VisitHeadersFooters(HeadersFooters headersFooters) { }
    internal virtual void VisitSection(Section section) { }
    internal virtual void VisitSections(Sections sections) { }

    //Shape
    internal virtual void VisitImage(Image image) { }
    internal virtual void VisitTextFrame(TextFrame textFrame) { }

    //Table
    internal virtual void VisitCell(Cell cell) { }
    internal virtual void VisitColumns(Columns columns) { }
    internal virtual void VisitRow(Row row) { }
    internal virtual void VisitRows(Rows rows) { }
    internal virtual void VisitTable(Table table) { }
  }
}

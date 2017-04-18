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
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.IO;

namespace MigraDocCore.DocumentObjectModel.Visitors
{
  /// <summary>
  /// Flattens a document for PDF rendering.
  /// </summary>
  public class PdfFlattenVisitor : VisitorBase
  {
    /// <summary>
    /// Initializes a new instance of the PdfFlattenVisitor class.
    /// </summary>
    public PdfFlattenVisitor()
    {
      //this.docObject = documentObject;
    }

    internal override void VisitDocumentElements(DocumentElements elements)
    {
      SortedList splitParaList = new SortedList();

      for (int idx = 0; idx < elements.Count; ++idx)
      {
        Paragraph paragraph = elements[idx] as Paragraph;
        if (paragraph != null)
        {
          Paragraph[] paragraphs = paragraph.SplitOnParaBreak();
          if (paragraphs != null)
            splitParaList.Add(idx, paragraphs);
        }
      }

      int insertedObjects = 0;
      for (int idx = 0; idx < splitParaList.Count; ++idx)
      {
        int insertPosition = (int)splitParaList.GetKey(idx);
        Paragraph[] paragraphs = (Paragraph[])splitParaList.GetByIndex(idx);
        foreach (Paragraph paragraph in paragraphs)
        {
          elements.InsertObject(insertPosition + insertedObjects, paragraph);
          ++insertedObjects;
        }
        elements.RemoveObjectAt(insertPosition + insertedObjects);
        --insertedObjects;
      }
    }

    internal override void VisitDocumentObjectCollection(DocumentObjectCollection elements)
    {
      ArrayList textIndices = new ArrayList();
      if (elements is ParagraphElements)
      {
        for (int idx = 0; idx < elements.Count; ++idx)
        {
          if (elements[idx] is Text)
            textIndices.Add(idx);
        }
      }

      int[] indices = (int[])textIndices.ToArray(typeof(int));
      if (indices != null)
      {
        int insertedObjects = 0;
        foreach (int idx in indices)
        {
          Text text = (Text)elements[idx + insertedObjects];
          string currentString = "";
          foreach (char ch in text.Content)
          {
            switch (ch)
            {
              case ' ':
              case '\r':
              case '\n':
              case '\t':
                if (currentString != "")
                {
                  elements.InsertObject(idx + insertedObjects, new Text(currentString));
                  ++insertedObjects;
                  currentString = "";
                }
                elements.InsertObject(idx + insertedObjects, new Text(" "));
                ++insertedObjects;
                break;

              case '-': //minus
                elements.InsertObject(idx + insertedObjects, new Text(currentString + ch));
                ++insertedObjects;
                currentString = "";
                break;

              case '­': //soft hyphen
                if (currentString != "")
                {
                  elements.InsertObject(idx + insertedObjects, new Text(currentString));
                  ++insertedObjects;
                  currentString = "";
                }
                elements.InsertObject(idx + insertedObjects, new Text("­"));
                ++insertedObjects;
                currentString = "";
                break;

              default:
                currentString += ch;
                break;
            }
          }
          if (currentString != "")
          {
            elements.InsertObject(idx + insertedObjects, new Text(currentString));
            ++insertedObjects;
          }
          elements.RemoveObjectAt(idx + insertedObjects);
          --insertedObjects;
        }
      }
    }

    internal override void VisitFormattedText(FormattedText formattedText)
    {
      Document document = formattedText.Document;
      ParagraphFormat format = null;

      Style style = document.styles[formattedText.style.Value];
      if (style != null)
        format = style.paragraphFormat;
      else if (formattedText.style.Value != "")
        format = document.styles["InvalidStyleName"].paragraphFormat;

      if (format != null)
      {
        if (formattedText.font == null)
          formattedText.Font = format.font.Clone();
        else if (format.font != null)
          FlattenFont(formattedText.font, format.font);
      }

      Font parentFont = GetParentFont(formattedText);

      if (formattedText.font == null)
        formattedText.Font = parentFont.Clone();
      else if (parentFont != null)
        FlattenFont(formattedText.font, parentFont);
    }

    internal override void VisitHyperlink(Hyperlink hyperlink)
    {
      Font styleFont = hyperlink.Document.Styles["Hyperlink"].Font;
      if (hyperlink.font == null)
        hyperlink.Font = styleFont.Clone();
      else
        FlattenFont(hyperlink.font, styleFont);

      FlattenFont(hyperlink.font, GetParentFont(hyperlink));
    }

    protected Font GetParentFont(DocumentObject obj)
    {
      DocumentObject parentElements = DocumentRelations.GetParent(obj);
      DocumentObject parentObject = DocumentRelations.GetParent(parentElements);
      Font parentFont = null;
      if (parentObject is Paragraph)
      {
        ParagraphFormat format = ((Paragraph)parentObject).Format;
        parentFont = format.font;
      }
      else //Hyperlink or FormattedText
      {
        parentFont = parentObject.GetValue("Font") as Font;
      }
      return parentFont;
    }
  }
}

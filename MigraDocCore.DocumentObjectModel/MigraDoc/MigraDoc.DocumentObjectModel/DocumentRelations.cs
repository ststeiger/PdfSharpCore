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

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Provides relational information between document objects.
  /// </summary>
  public class DocumentRelations
  {
    /// <summary>
    /// Determines whether the specified documentObject has a
    /// parent of the given type somewhere within the document hierarchy.
    /// </summary>
    /// <param name="documentObject">The document object to check.</param>
    /// <param name="type">The parent type to search for.</param>
    public static bool HasParentOfType(DocumentObject documentObject, Type type)
    {
      if (documentObject == null)
        throw new ArgumentNullException("documentObject");

      if (type == null)
        throw new ArgumentNullException("type");

      return GetParentOfType(documentObject, type) != null;
    }

    /// <summary>
    /// Gets the direct parent of the given document object.
    /// </summary>
    /// <param name="documentObject">The document object the parent is searched for.</param>
    public static DocumentObject GetParent(DocumentObject documentObject)
    {
      if (documentObject == null)
        throw new ArgumentNullException("documentObject");

      return documentObject.Parent;
    }

    /// <summary>
    /// Gets a parent of the document object with the given type somewhere within the document hierarchy.
    /// Returns null if none exists.
    /// </summary>
    /// <param name="documentObject">The document object the parent is searched for.</param>
    /// <param name="type">The parent type to search for.</param>
    public static DocumentObject GetParentOfType(DocumentObject documentObject, Type type)
    {
      if (documentObject == null)
        throw new ArgumentNullException("documentObject");

      if (type == null)
        throw new ArgumentNullException("type");

      if (documentObject.parent != null)
      {
        if (documentObject.parent.GetType() == type)
          return documentObject.parent;
        else
          return GetParentOfType(documentObject.parent, type);
      }
      return null;
    }
  }
}

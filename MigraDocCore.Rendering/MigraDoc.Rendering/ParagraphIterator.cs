#region MigraDoc - Creating Documents on the Fly
//
// Authors:
//   Klaus Potzesny (mailto:Klaus.Potzesny@PdfSharpCore.com)
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
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Internals;
using System.Collections;
using System.Collections.Specialized;

namespace MigraDocCore.Rendering
{
  /// <summary>
  /// Iterates sequentially through the elements of a paragraph.
  /// </summary>
  internal class ParagraphIterator
  {
    /// <summary>
    /// Initializes a paragraph iterator pointing on the given paragraph elements object.
    /// Paragraph iterators received from this paragraph iterator relate to this root node.
    /// </summary>
    /// <param name="rootNode">The root node for the paragraph iterator.</param>
    internal ParagraphIterator(ParagraphElements rootNode)
    {
      this.rootNode = rootNode;
      this.current = rootNode;
      this.positionIndices = new ArrayList();
    }

    /// <summary>
    /// Initializes a paragraph iterator given the root node, its position in the object tree and the current object
    /// </summary>
    /// <param name="rootNode">The node the position indices relate to.</param>
    /// <param name="current">The element the iterator shall point to.</param>
    /// <param name="indices">The position of the paragraph iterator in terms of element indices.</param>
    private ParagraphIterator(ParagraphElements rootNode, DocumentObject current, ArrayList indices)
    {
      this.rootNode = rootNode;
      this.positionIndices = indices;
      this.current = current;
    }

    /// <summary>
    /// Determines whether this iterator is the first leaf of the root node.
    /// </summary>
    internal bool IsFirstLeaf
    {
      get
      {
        if (!(this.current is DocumentElements))
        {
          ParagraphIterator prevIter = this.GetPreviousLeaf();
          return prevIter == null;
        }
        return false;
      }
    }

    /// <summary>
    /// Determines whether this iterator is the last leaf of the document object tree.
    /// </summary>
    internal bool IsLastLeaf
    {
      get
      {
        if (!(this.current is DocumentElements))
        {
          ParagraphIterator nextIter = this.GetNextLeaf();
          return nextIter == null;
        }
        return false;
      }
    }

    /// <summary>
    /// Gets the document object this instance ponits to.
    /// </summary>
    internal DocumentObject Current
    {
      get{return current;}
    }

    /// <summary>
    /// Gets the last leaf of the document object tree.
    /// </summary>
    /// <returns>The paragraph iterator pointing to the last leaf in the document object tree.</returns>
    internal ParagraphIterator GetLastLeaf()
    {
      if (this.rootNode.Count == 0)
        return null;
      return SeekLastLeaf();
    }


    /// <summary>
    /// Gets the first leaf of the element tree.
    /// </summary>
    /// <returns>The paragraph iterator pointing to the first leaf in the element tree.</returns>
    internal ParagraphIterator GetFirstLeaf()
    {
      if (this.rootNode.Count == 0)
        return null;
      return SeekFirstLeaf();
    }

    /// <summary>
    /// Returns the next iterator in the tree pointing to a leaf.
    /// </summary>
    /// <remarks>This function is intended to receive the renderable objects of a paragraph.
    /// Thus, empty ParagraphElement objects (which are collections) don't count as leafs.</remarks>
    internal ParagraphIterator GetNextLeaf()
    {
      //Move up to appropriate parent element
      ParagraphIterator parIterator = GetParentIterator();
      if (parIterator == null)
        return null;

      int elementIndex = this.LastIndex;
      ParagraphElements parEls = (ParagraphElements)parIterator.current;
      while (elementIndex == parEls.Count - 1)
      {
        elementIndex = parIterator.LastIndex;
        parIterator = parIterator.GetParentIterator();
        if (parIterator == null)
          break;

        parEls = (ParagraphElements)parIterator.current;
      }
      if (parIterator == null)
        return null;
      int newIndex = elementIndex + 1;
      if (newIndex >= parEls.Count)
         return null;

      ArrayList indices = (ArrayList)parIterator.positionIndices.Clone();
      indices.Add(newIndex);
      DocumentObject obj = GetNodeObject(parEls[newIndex]);
      ParagraphIterator iterator = new ParagraphIterator(this.rootNode, obj, indices);
      return iterator.SeekFirstLeaf();
    }

    /// <summary>
    /// Gets the object a paragraph iterator shall point to.
    /// Only ParagraphElements and renderable objects are allowed.
    /// </summary>
    /// <param name="obj">The object to select the node object for.</param>
    /// <returns>The object a paragraph iterator shall point to.</returns>
    private DocumentObject GetNodeObject(DocumentObject obj)
    {
      if (obj is FormattedText)
        return ((FormattedText)obj).Elements;
      else if (obj is Hyperlink)
        return ((Hyperlink)obj).Elements;
      else return obj;
    }

    /// <summary>
    /// Returns the previous iterator to a leaf in the document object tree pointing.
    /// </summary>
    /// <returns>The previous leaf, null if none exists.</returns>
    internal ParagraphIterator GetPreviousLeaf()
    {
      //Move up to appropriate parent element
      ParagraphIterator parIterator = GetParentIterator();
      if (parIterator == null)
        return null;

      int elementIndex = this.LastIndex;
      ParagraphElements parEls = (ParagraphElements)parIterator.current;
      while (elementIndex == 0)
      {
        elementIndex = parIterator.LastIndex;
        parIterator = parIterator.GetParentIterator();
        if (parIterator == null)
          break;

        parEls = (ParagraphElements)parIterator.current;
      }
      if (parIterator == null)
        return null;

      int newIndex = elementIndex - 1;
      if (newIndex < 0)
        return null;

      ArrayList indices = (ArrayList)parIterator.positionIndices.Clone();
      indices.Add(newIndex);

      DocumentObject obj = GetNodeObject(parEls[newIndex]);
      ParagraphIterator iterator = new ParagraphIterator(this.rootNode, obj, indices);
      return iterator.SeekLastLeaf();
    }

    private ParagraphIterator SeekLastLeaf()
    {
      DocumentObject obj = this.Current;
      if (!(obj is ParagraphElements))
        return this;

      ArrayList indices = (ArrayList)this.positionIndices.Clone();

      while (obj is ParagraphElements)
      {
        ParagraphElements parEls = (ParagraphElements)obj;
        if (((ParagraphElements)obj).Count == 0)
          return new ParagraphIterator(this.rootNode, obj, indices);

        int idx = ((ParagraphElements)obj).Count - 1;
        indices.Add(idx);
        obj = GetNodeObject(parEls[idx]);
      }
      return new ParagraphIterator(this.rootNode, obj, indices);
    }

    /// <summary>
    /// Gets the leftmost leaf within the hierarchy.
    /// </summary>
    /// <returns>The searched leaf.</returns>
    ParagraphIterator SeekFirstLeaf()
    {
      DocumentObject obj = this.Current;
      if (!(obj is ParagraphElements))
        return this;
      ArrayList indices = (ArrayList)this.positionIndices.Clone();

      while (obj is ParagraphElements)
      {
        ParagraphElements parEls = (ParagraphElements)obj;
        if (parEls.Count == 0)
          return new ParagraphIterator(this.rootNode, obj, indices);

        indices.Add(0);
        obj = GetNodeObject(parEls[0]);
      }
      return new ParagraphIterator(this.rootNode, obj, indices);
    }

    private ParagraphIterator GetParentIterator()
    {
      if (this.positionIndices.Count == 0)
        return null;

      ArrayList indices = (ArrayList)this.positionIndices.Clone();
      indices.RemoveAt(indices.Count - 1);
      DocumentObject parent = DocumentRelations.GetParentOfType(this.current, typeof(ParagraphElements));
      return new ParagraphIterator(this.rootNode, parent, indices);
    }

    private int LastIndex
    {
      get
      {
        if (this.positionIndices.Count == 0)
          return -1;
        else
          return (int)this.positionIndices[this.positionIndices.Count - 1];
      }
    }

    private readonly ParagraphElements rootNode;
    private readonly ArrayList positionIndices;
    private readonly DocumentObject current;
  }
}

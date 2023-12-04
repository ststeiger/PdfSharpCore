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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using MigraDocCore.DocumentObjectModel.Visitors;

namespace MigraDocCore.DocumentObjectModel
{
  /// <summary>
  /// Base class of all collections of the MigraDoc Document Object Model.
  /// </summary>
  public abstract class DocumentObjectCollection : DocumentObject, IList, IVisitable
  {
    /// <summary>
    /// Initializes a new instance of the DocumentObjectCollection class.
    /// </summary>
    internal DocumentObjectCollection()
    {
      this.elements = new ArrayList();
    }

    /// <summary>
    /// Initializes a new instance of the DocumentObjectCollection class with the specified parent.
    /// </summary>
    internal DocumentObjectCollection(DocumentObject parent)
      : base(parent)
    {
      this.elements = new ArrayList();
    }

    /// <summary>
    /// Gets the first value in the Collection, if there is any, otherwise null.
    /// </summary>
    public DocumentObject First
    {
      get
      {
        if (Count > 0)
          return this[0];
        else
          return null;
      }
    }

    /// <summary>
    /// Creates a deep copy of this object.
    /// </summary>
    public new DocumentObjectCollection Clone()
    {
      return (DocumentObjectCollection)DeepCopy();
    }

    /// <summary>
    /// Implements the deep copy of the object.
    /// </summary>
    protected override object DeepCopy()
    {
      DocumentObjectCollection coll = (DocumentObjectCollection)base.DeepCopy();

      int count = Count;
      coll.elements = new ArrayList(count);
      for (int index = 0; index < count; ++index)
      {
        DocumentObject doc = this[index];
        if (doc != null)
        {
          doc = doc.Clone() as DocumentObject;
          doc.parent = coll;
        }
        coll.elements.Add(doc);
      }
      return coll;
    }

    /// <summary>
    /// Copies the entire collection to a compatible one-dimensional System.Array,
    /// starting at the specified index of the target array.
    /// </summary>
    public void CopyTo(Array array, int index)
    {
      this.elements.CopyTo(array, index);
    }

    /// <summary>
    /// Gets a value indicating whether the Collection is read-only.
    /// </summary>
    bool IList.IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether the Collection has a fixed size.
    /// </summary>
    bool IList.IsFixedSize
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether access to the Collection is synchronized.
    /// </summary>
    bool ICollection.IsSynchronized
    {
      get { return false; }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the collection.
    /// </summary>
    object ICollection.SyncRoot
    {
      get { return null; }
    }

    /// <summary>
    /// Gets the number of elements actually contained in the collection.
    /// </summary>
    public int Count
    {
      get { return this.elements.Count; }
    }

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public void Clear()
    {
      ((IList)this).Clear();
    }

    /// <summary>
    /// Inserts an object at the specified index.
    /// </summary>
    public virtual void InsertObject(int index, DocumentObject val)
    {
      SetParent(val);
      ((IList)this).Insert(index, val);
      // Call ResetCachedValues for all objects moved by the Insert operation.
      int count = ((IList)this).Count;
      for (int idx = index + 1; idx < count; ++idx)
      {
        DocumentObject obj = (DocumentObject)((IList)this)[idx];
        obj.ResetCachedValues();
      }
    }

    /// <summary>
    /// Determines the index of a specific item in the collection.
    /// </summary>
    public int IndexOf(DocumentObject val)
    {
      return ((IList)this).IndexOf(val);
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    public virtual DocumentObject this[int index]
    {
      get { return this.elements[index] as DocumentObject; }
      set
      {
        SetParent(value);
        this.elements[index] = value;
      }
    }

    /// <summary>
    /// Gets the last element or null, if no such element exists.
    /// </summary>
    public DocumentObject LastObject
    {
      get
      {
        int count = this.elements.Count;
        if (count > 0)
          return (DocumentObject)this.elements[count - 1];
        return null;
      }
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    public void RemoveObjectAt(int index)
    {
      ((IList)this).RemoveAt(index);
      // Call ResetCachedValues for all objects moved by the RemoveAt operation.
      int count = ((IList)this).Count;
      for (int idx = index; idx < count; ++idx)
      {
        DocumentObject obj = (DocumentObject)((IList)this)[idx];
        obj.ResetCachedValues();
      }
    }

    /// <summary>
    /// Inserts the object into the collection and sets it's parent.
    /// </summary>
    public virtual void Add(DocumentObject value)
    {
      SetParent(value);
      this.elements.Add(value);
    }

    /// <summary>
    /// Determines whether this instance is null.
    /// </summary>
    public override bool IsNull()
    {
      if (!Meta.IsNull(this))
        return false;
      if (this.elements == null)
        return true;
      foreach (DocumentObject docObject in elements)
      {
        if (docObject != null && !docObject.IsNull())
          return false;
      }
      return true;
    }

    /// <summary>
    /// Allows the visitor object to visit the document object and it's child objects.
    /// </summary>
    void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
    {
      visitor.VisitDocumentObjectCollection(this);

      foreach (DocumentObject docobj in this)
      {
        IVisitable visitable = docobj as IVisitable;
        if (visitable != null)
          visitable.AcceptVisitor(visitor, visitChildren);
      }
    }

    /// <summary>
    /// Returns an enumerator that can iterate through this collection.
    /// </summary>
    public IEnumerator GetEnumerator()
    {
      return this.elements.GetEnumerator();
    }

    ArrayList elements;

    #region IList Members
    /// <summary>
    /// Gets or sets the element at the specified index. 
    /// </summary>
    object IList.this[int index]
    {
      get { return this.elements[index]; }
      set { this.elements[index] = value; }
    }

    /// <summary>
    /// Removes the item at the specified index from the Collection.
    /// </summary>
    void IList.RemoveAt(int index)
    {
      this.elements.RemoveAt(index);
    }

    /// <summary>
    /// Inserts an object at the specified index.
    /// </summary>
    void IList.Insert(int index, object value)
    {
      this.elements.Insert(index, value);
    }

    /// <summary>
    /// Removes the first occurrence of the specific object.
    /// </summary>
    void IList.Remove(object value)
    {
      this.elements.Remove(value);
    }

    /// <summary>
    /// Determines whether an element exists.
    /// </summary>
    bool IList.Contains(object value)
    {
      return this.elements.Contains(value);
    }

    /// <summary>
    /// Determines the index of a specific item in the Collection.
    /// </summary>
    int IList.IndexOf(object value)
    {
      return this.elements.IndexOf(value);
    }

    /// <summary>
    /// Adds an item to the Collection.
    /// </summary>
    int IList.Add(object value)
    {
      return this.elements.Add(value);
    }

    /// <summary>
    /// Removes all items from the Collection.
    /// </summary>
    void IList.Clear()
    {
      this.elements.Clear();
    }
    #endregion
  }
}

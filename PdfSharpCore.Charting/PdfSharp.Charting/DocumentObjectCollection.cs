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
using System.Collections;
using System.Globalization;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Charting
{
  /// <summary>
  /// Base class of all collections.
  /// </summary>
  public abstract class DocumentObjectCollection : DocumentObject, IList
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
    internal DocumentObjectCollection(DocumentObject parent) : base(parent)
    {
      this.elements = new ArrayList();
    }

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    public virtual DocumentObject this[int index]
    {
      get {return this.elements[index] as DocumentObject;}
      // TODO: überprüfen ob das erlaubt sein soll
      set {this.elements[index] = value;}
    }

    #region Methods
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
        coll.elements.Add(this[index].Clone());
      return coll;
    }

    /// <summary>
    /// Copies the ArrayList or a portion of it to a one-dimensional array.
    /// </summary>
    public void CopyTo(Array array, int index)
    {
      this.elements.CopyTo(array, index);
    }

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public void Clear()
    {
      this.elements.Clear();
    }

    /// <summary>
    /// Inserts an element into the collection at the specified position.
    /// </summary>
    public virtual void InsertObject(int index, DocumentObject val)
    {
      this.elements.Insert(index, val);
    }

    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence.
    /// </summary>
    public int IndexOf(DocumentObject val)
    {
      return this.elements.IndexOf(val);
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    public void RemoveObjectAt(int index)
    {
      elements.RemoveAt(index);
    }

    /// <summary>
    /// Adds the specified document object to the collection.
    /// </summary>
    public virtual void Add(DocumentObject value)
    {
      if (value != null)
        value.parent = this;
      this.elements.Add(value);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the number of elements actually contained in the collection.
    /// </summary>
    public int Count
    {
      get {return this.elements.Count;}
    }

    /// <summary>
    /// Gets the first value in the collection, if there is any, otherwise null.
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
    #endregion

    #region IList
    bool IList.IsReadOnly
    {
      get {return false;}
    }

    bool IList.IsFixedSize
    {
      get {return false;}
    }

    object IList.this[int index]
    {
      get {return this.elements[index];}
      set {this.elements[index] = value;}
    }

    void IList.RemoveAt(int index)
    {
      throw new NotImplementedException("IList.RemoveAt");
      // TODO:  Add DocumentObjectCollection.RemoveAt implementation
    }

    void IList.Insert(int index, object value)
    {
      throw new NotImplementedException("IList.Insert");
      // TODO:  Add DocumentObjectCollection.Insert implementation
    }

    void IList.Remove(object value)
    {
      throw new NotImplementedException("IList.Remove");
      // TODO:  Add DocumentObjectCollection.Remove implementation
    }

    bool IList.Contains(object value)
    {
      throw new NotImplementedException("IList.Contains");
      // TODO:  Add DocumentObjectCollection.Contains implementation
      //return false;
    }

    int System.Collections.IList.IndexOf(object value)
    {
      throw new NotImplementedException("IList.IndexOf");
      // TODO:  Add DocumentObjectCollection.System.Collections.IList.IndexOf implementation
      //return 0;
    }

    int IList.Add(object value)
    {
      throw new NotImplementedException("IList.Add");
      // TODO:  Add DocumentObjectCollection.Add implementation
      //return 0;
    }
    #endregion

    #region ICollection
    bool ICollection.IsSynchronized
    {
      get {return false;}
    }

    object ICollection.SyncRoot
    {
      get {return null;}
    }
    #endregion

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return this.elements.GetEnumerator();
    }

    ArrayList elements;
  }
}

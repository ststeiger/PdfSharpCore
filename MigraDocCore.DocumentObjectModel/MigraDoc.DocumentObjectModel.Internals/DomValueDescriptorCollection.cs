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

namespace MigraDocCore.DocumentObjectModel.Internals
{
  /// <summary>
  /// A collection that manages ValueDescriptors.
  /// </summary>
  public class ValueDescriptorCollection : IEnumerable
  {
    /// <summary>
    /// Gets the count of ValueDescriptors.
    /// </summary>
    public int Count
    {
      get { return this.arrayList.Count; }
    }

    /// <summary>
    /// Adds the specified ValueDescriptor.
    /// </summary>
    public int Add(ValueDescriptor vd)
    {
      this.hashTable.Add(vd.ValueName, vd);
      return this.arrayList.Add(vd);
    }

    /// <summary>
    /// Gets the <see cref="MigraDoc.DocumentObjectModel.Internals.ValueDescriptor"/> at the specified index.
    /// </summary>
    /// <value></value>
    public ValueDescriptor this[int index]
    {
      get { return this.arrayList[index] as ValueDescriptor; }
    }

    /// <summary>
    /// Gets the <see cref="MigraDoc.DocumentObjectModel.Internals.ValueDescriptor"/> with the specified name.
    /// </summary>
    /// <value></value>
    public ValueDescriptor this[string name]
    {
      get { return this.hashTable[name] as ValueDescriptor; }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return this.arrayList.GetEnumerator();
    }

    ArrayList arrayList = new ArrayList();
    Hashtable hashTable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
  }
}

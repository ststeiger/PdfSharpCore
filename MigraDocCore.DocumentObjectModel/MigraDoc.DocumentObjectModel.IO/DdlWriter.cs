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
using System.IO;
using System.Text;
using MigraDocCore.DocumentObjectModel;

namespace MigraDocCore.DocumentObjectModel.IO
{
    /// <summary>
    /// Represents the MigraDoc DDL writer.
    /// </summary>
    public class DdlWriter : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified Stream.
        /// </summary>
        public DdlWriter(Stream stream)
        {
            this.writer = new StreamWriter(stream);
            this.serializer = new Serializer(this.writer);
        }

        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified filename.
        /// </summary>
        public DdlWriter(string filename)
        {
            this.writer = new StreamWriter(File.Open(filename, FileMode.Create), Encoding.UTF8, 1024, false);
            this.serializer = new Serializer(this.writer);
        }

        /// <summary>
        /// Initializes a new instance of the DdlWriter class with the specified TextWriter.
        /// </summary>
        public DdlWriter(TextWriter writer)
        {
            this.serializer = new Serializer(writer);
        }

        /// <summary>
        /// Flushes the underlying TextWriter.
        /// </summary>
        public void Flush()
        {
            this.serializer.Flush();
        }

        /// <summary>
        /// Gets or sets the indentation for the DDL file.
        /// </summary>
        public int Indent
        {
            get { return serializer.Indent; }
            set { serializer.Indent = value; }
        }

        /// <summary>
        /// Gets or sets the initial indentation for the DDL file.
        /// </summary>
        public int InitialIndent
        {
            get { return serializer.InitialIndent; }
            set { serializer.InitialIndent = value; }
        }

        /// <summary>
        /// Writes the specified DocumentObject to DDL.
        /// </summary>
        public void WriteDocument(DocumentObject documentObject)
        {
            documentObject.Serialize(this.serializer);
            this.serializer.Flush();
        }

        /// <summary>
        /// Writes the specified DocumentObjectCollection to DDL.
        /// </summary>
        public void WriteDocument(DocumentObjectCollection documentObjectContainer)
        {
            documentObjectContainer.Serialize(this.serializer);
            this.serializer.Flush();
        }

        /// <summary>
        /// Writes a DocumentObject type object to string.
        /// </summary>
        public static string WriteToString(DocumentObject docObject)
        {
            return WriteToString(docObject, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to string. Indent a new block by indent characters.
        /// </summary>
        public static string WriteToString(DocumentObject docObject, int indent)
        {
            return WriteToString(docObject, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to string. Indent a new block by indent + initialIndent characters.
        /// </summary>
        public static string WriteToString(DocumentObject docObject, int indent, int initialIndent)
        {
            StringBuilder strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                using (var wrt = new DdlWriter(writer)
                {
                    Indent = indent,
                    InitialIndent = initialIndent
                })
                    wrt.WriteDocument(docObject);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer)
        {
            return WriteToString(docObjectContainer, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string. Indent a new block by _indent characters.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent)
        {
            return WriteToString(docObjectContainer, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to string. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static string WriteToString(DocumentObjectCollection docObjectContainer, int indent, int initialIndent)
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                using (var wrt = new DdlWriter(writer)
                {
                    Indent = indent,
                    InitialIndent = initialIndent
                })
                {
                    wrt.WriteDocument(docObjectContainer);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Writes a document object to a DDL file.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename)
        {
            WriteToFile(docObject, filename, 2, 0);
        }

        /// <summary>
        /// Writes a document object to a DDL file. Indent a new block by the specified number of characters.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename, int indent)
        {
            WriteToFile(docObject, filename, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObject type object to a DDL file. Indent a new block by indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObject docObject, string filename, int indent, int initialIndent)
        {
            using (var wrt = new DdlWriter(filename)
            {
                Indent = indent,
                InitialIndent = initialIndent
            })
            {
                wrt.WriteDocument(docObject);
            }
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename)
        {
            WriteToFile(docObjectContainer, filename, 2, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent)
        {
            WriteToFile(docObjectContainer, filename, indent, 0);
        }

        /// <summary>
        /// Writes a DocumentObjectCollection type object to a DDL file. Indent a new block by
        /// indent + initialIndent characters.
        /// </summary>
        public static void WriteToFile(DocumentObjectCollection docObjectContainer, string filename, int indent, int initialIndent)
        {
            using (var wrt = new DdlWriter(filename)
            {
                Indent = indent,
                InitialIndent = initialIndent
            })
            {
                wrt.WriteDocument(docObjectContainer);
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.serializer != null)
                this.serializer = null;

            if (this.writer != null)
            {
                this.writer.Dispose();
                this.writer = null;
            }
        }

        StreamWriter writer;
        Serializer serializer;
    }
}

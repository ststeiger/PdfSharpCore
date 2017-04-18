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
using System.Reflection;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Visitors;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;
using PdfSharpCore.Fonts;

namespace MigraDocCore.DocumentObjectModel
{
    /// <summary>
    /// Represents the collection of all styles.
    /// </summary>
    public class Styles : DocumentObjectCollection, IVisitable
    {
        /// <summary>
        /// Initializes a new instance of the Styles class.
        /// </summary>
        public Styles()
        {
            SetupStyles();
        }

        /// <summary>
        /// Initializes a new instance of the Styles class with the specified parent.
        /// </summary>
        internal Styles(DocumentObject parent)
          : base(parent)
        {
            SetupStyles();
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Styles Clone()
        {
            return (Styles)base.DeepCopy();
        }

        /// <summary>
        /// Gets a style by its name.
        /// </summary>
        public Style this[string styleName]
        {
            get
            {
                int count = Count;
                // index starts from 1; DefaultParagraphFont cannot be modified.
                for (int index = 1; index < count; ++index)
                {
                    Style style = (Style)this[index];
                    if (String.Compare(style.Name, styleName, true) == 0)
                        return style;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a style by index. 
        /// </summary>
        internal new Style this[int index]
        {
            get { return (Style)base[index]; }
        }

        /// <summary>
        /// Gets the index of a style by name.
        /// </summary>
        /// <param name="styleName">Name of the style looking for.</param>
        /// <returns>Index or -1 if not exists.</returns>
        public int GetIndex(string styleName)
        {
            if (styleName == null)
                throw new ArgumentNullException("styleName");

            int count = Count;
            for (int index = 0; index < count; ++index)
            {
                Style style = (Style)this[index];
                if (String.Compare(style.Name, styleName, true) == 0)
                    return index;
            }
            return -1;
        }

        /// <summary>
        /// Adds a new style to the styles collection.
        /// </summary>
        /// <param name="name">Name of the style.</param>
        /// <param name="baseStyleName">Name of the base style.</param>
        public Style AddStyle(string name, string baseStyleName)
        {
            if (name == null || baseStyleName == null)
                throw new ArgumentNullException(name == null ? "name" : "baseStyleName");
            if (name == "" || baseStyleName == "")
                throw new ArgumentException(name == "" ? "name" : "baseStyleName");

            Style style = new Style();
            style.name.Value = name;
            style.baseStyle.Value = baseStyleName;
            this.Add(style);
            return style;
        }

        /// <summary>
        /// Adds a DocumentObject to the styles collection.
        /// </summary>
        public override void Add(DocumentObject value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Style style = value as Style;
            if (style == null)
                throw new InvalidOperationException(AppResources.StyleExpected);

            bool isRootStyle = style.IsRootStyle;

            if (style.BaseStyle == "" && !isRootStyle)
                throw new ArgumentException(DomSR.UndefinedBaseStyle(style.BaseStyle));

            Style baseStyle = null;
            int styleIndex = GetIndex(style.BaseStyle);

            if (styleIndex != -1)
                baseStyle = this[styleIndex] as Style;
            else if (!isRootStyle)
                throw new ArgumentException(DomSR.UndefinedBaseStyle(style.BaseStyle));

            if (baseStyle != null)
                style.styleType.Value = (int)baseStyle.Type;

            int index = GetIndex(style.Name);

            if (index >= 0)
            {
                style = style.Clone();
                style.parent = this;
                ((IList)this)[index] = style;
            }
            else
                base.Add(value);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default paragraph style.
        /// </summary>
        public Style Normal
        {
            get { return this[Style.DefaultParagraphName]; }
        }

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

        /// <summary>
        /// Initialize the built in styles.
        /// </summary>
        internal void SetupStyles()
        {
            Style style;

            // First standard style
            style = new Style(Style.DefaultParagraphFontName, null)
            {
                readOnly = true
            };
            style.styleType.Value = (int)StyleType.Character;
            style.buildIn.Value = true;
            this.Add(style);

            // Normal 'Standard' (Paragraph Style)
            style = new Style(Style.DefaultParagraphName, null);
            style.styleType.Value = (int)StyleType.Paragraph;
            style.buildIn.Value = true;
            style.Font.Name = GlobalFontSettings.FontResolver.DefaultFontName;
            style.Font.Size = 10;
            style.Font.Bold = false;
            style.Font.Italic = false;
            style.Font.Underline = Underline.None;
            style.Font.Color = Colors.Black;
            style.Font.Subscript = false;
            style.Font.Superscript = false;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            style.ParagraphFormat.FirstLineIndent = 0;
            style.ParagraphFormat.LeftIndent = 0;
            style.ParagraphFormat.RightIndent = 0;
            style.ParagraphFormat.KeepTogether = false;
            style.ParagraphFormat.KeepWithNext = false;
            style.ParagraphFormat.SpaceBefore = 0;
            style.ParagraphFormat.SpaceAfter = 0;
            style.ParagraphFormat.LineSpacing = 10;
            style.ParagraphFormat.LineSpacingRule = LineSpacingRule.Single;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.BodyText;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.WidowControl = true;
            this.Add(style);

            // Heading1 'Überschrift 1' (Paragraph Style)
            style = new Style("Heading1", "Normal");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level1;
            this.Add(style);

            // Heading2 'Überschrift 2' (Paragraph Style)
            style = new Style("Heading2", "Heading1");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level2;
            this.Add(style);

            // Heading3 'Überschrift 3' (Paragraph Style)
            style = new Style("Heading3", "Heading2");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level3;
            this.Add(style);

            // Heading4 'Überschrift 4' (Paragraph Style)
            style = new Style("Heading4", "Heading3");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level4;
            this.Add(style);

            // Heading5 'Überschrift 5' (Paragraph Style)
            style = new Style("Heading5", "Heading4");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level5;
            this.Add(style);

            // Heading6 'Überschrift 6' (Paragraph Style)
            style = new Style("Heading6", "Heading5");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level6;
            this.Add(style);

            // Heading7 'Überschrift 7' (Paragraph Style)
            style = new Style("Heading7", "Heading6");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level7;
            this.Add(style);

            // Heading8 'Überschrift 8' (Paragraph Style)
            style = new Style("Heading8", "Heading7");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level8;
            this.Add(style);

            // Heading9 'Überschrift 9' (Paragraph Style)
            style = new Style("Heading9", "Heading8");
            style.buildIn.Value = true;
            style.ParagraphFormat.OutlineLevel = OutlineLevel.Level9;
            this.Add(style);

            // List 'Liste' (Paragraph Style)
            style = new Style("List", "Normal");
            style.buildIn.Value = true;
            this.Add(style);

            // Footnote 'Fußnote' (Paragraph Style)
            style = new Style("Footnote", "Normal");
            style.buildIn.Value = true;
            this.Add(style);

            // Header 'Kopfzeile' (Paragraph Style)
            style = new Style("Header", "Normal");
            style.buildIn.Value = true;
            this.Add(style);

            // -33: Footer 'Fußzeile' (Paragraph Style)
            style = new Style("Footer", "Normal");
            style.buildIn.Value = true;
            this.Add(style);

            // Hyperlink 'Hyperlink' (Character Style)
            style = new Style("Hyperlink", "DefaultParagraphFont");
            style.buildIn.Value = true;
            this.Add(style);

            // InvalidStyleName 'Ungültiger Formatvorlagenname' (Paragraph Style)
            style = new Style("InvalidStyleName", "Normal");
            style.buildIn.Value = true;
            style.Font.Bold = true;
            style.Font.Underline = Underline.Dash;
            style.Font.Color = new Color(0xFF00FF00);
            this.Add(style);
        }

        #region Internal
        /// <summary>
        /// Converts Styles into DDL.
        /// </summary>
        internal override void Serialize(Serializer serializer)
        {
            serializer.WriteComment(this.comment.Value);
            int pos = serializer.BeginContent("\\styles");

            // A style can only be added to Styles if its base style exists. Therefore the
            // styles collection is consistent at any one time by definition. But because it 
            // is possible  to change the base style of a style, the sequence of the styles 
            // in the styles collection can be in an order that a style comes before its base
            // style. The styles in an DDL file must be ordered such that each style appears
            // after its base style. We cannot simple reorder the styles collection, because
            // the predefined styles are expected at a fixed position.
            // The solution is to reorder the styles during serialization.
            int count = Count;
            bool[] fSerialized = new bool[count];  // already serialized
            fSerialized[0] = true;                       // consider DefaultParagraphFont as serialized
            bool[] fSerializePending = new bool[count];  // currently serializing
            bool newLine = false;  // gets true if at least one style was written
                                   //Start from 1 and do not serialize DefaultParagraphFont
            for (int index = 1; index < count; index++)
            {
                if (!fSerialized[index])
                {
                    Style style = this[index];
                    SerializeStyle(serializer, index, ref fSerialized, ref fSerializePending, ref newLine);
                }
            }
            serializer.EndContent(pos);
        }

        /// <summary>
        /// Serialize a style, but serialize its base style first (if that was not yet done).
        /// </summary>
        void SerializeStyle(Serializer serializer, int index, ref bool[] fSerialized, ref bool[] fSerializePending,
          ref bool newLine)
        {
            Style style = this[index];

            // It is not possible to modify the default paragraph font
            if (style.Name == Style.DefaultParagraphFontName)
                return;

            // Circular dependencies cannot occur if changing the base style is implemented
            // correctly. But before we proof that, we check it here.
            if (fSerializePending[index])
            {
                string message = String.Format("Circular dependency detected according to style '{0}'.", style.Name);
                throw new Exception(message);
            }

            // Only style 'Normal' has no base style
            if (style.BaseStyle != "")
            {
                int idxBaseStyle = GetIndex(style.BaseStyle);
                if (idxBaseStyle != -1)
                {
                    if (!fSerialized[idxBaseStyle])
                    {
                        fSerializePending[index] = true;
                        SerializeStyle(serializer, idxBaseStyle, ref fSerialized, ref fSerializePending, ref newLine);
                        fSerializePending[index] = false;
                    }
                }
            }
            int pos2 = serializer.BeginBlock();
            if (newLine)
                serializer.WriteLineNoCommit();
            style.Serialize(serializer);
            if (serializer.EndBlock(pos2))
                newLine = true;
            fSerialized[index] = true;
        }

        /// <summary>
        /// Allows the visitor object to visit the document object and it's child objects.
        /// </summary>
        void IVisitable.AcceptVisitor(DocumentObjectVisitor visitor, bool visitChildren)
        {
            visitor.VisitStyles(this);

            Hashtable visitedStyles = new Hashtable();
            foreach (Style style in this)
                VisitStyle(visitedStyles, style, visitor, visitChildren);
        }

        /// <summary>
        /// Ensures that base styles are visited first.
        /// </summary>
        void VisitStyle(Hashtable visitedStyles, Style style, DocumentObjectVisitor visitor, bool visitChildren)
        {
            if (!visitedStyles.Contains(style))
            {
                Style baseStyle = style.GetBaseStyle();
                if (baseStyle != null && !visitedStyles.Contains(baseStyle)) //baseStyle != ""
                    VisitStyle(visitedStyles, baseStyle, visitor, visitChildren);
                ((IVisitable)style).AcceptVisitor(visitor, visitChildren);
                visitedStyles.Add(style, null);
            }
        }

        internal static readonly Styles BuildInStyles = new Styles();

        /// <summary>
        /// Returns the meta object of this instance.
        /// </summary>
        internal override Meta Meta
        {
            get
            {
                if (meta == null)
                    meta = new Meta(typeof(Styles));
                return meta;
            }
        }
        static Meta meta;
        #endregion
    }
}

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
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Reflection;
using MigraDocCore.DocumentObjectModel.Internals;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes.Charts;
using MigraDocCore.DocumentObjectModel.Fields;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Resources;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;

namespace MigraDocCore.DocumentObjectModel.IO
{
    /// <summary>
    /// A simple hand-coded parser for MigraDoc DDL.
    /// </summary>
    internal class DdlParser
    {
        /// <summary>
        /// Initializes a new instance of the DdlParser class.
        /// </summary>
        internal DdlParser(string ddl, DdlReaderErrors errors)
          : this("", ddl, errors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DdlParser class.
        /// </summary>
        internal DdlParser(string fileName, string ddl, DdlReaderErrors errors)
        {
            if (errors != null)
                this.errors = errors;
            else
                this.errors = new DdlReaderErrors();

            scanner = new DdlScanner(fileName, ddl, errors);
        }

        /// <summary>
        /// Parses the keyword «\document».
        /// </summary>
        internal Document ParseDocument(Document document)
        {
            if (document == null)
                document = new Document();

            MoveToCode();
            AssertSymbol(Symbol.Document);
            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(document);

            AssertSymbol(Symbol.BraceLeft);

            // Styles come first
            ReadCode();
            if (Symbol == Symbol.Styles)
                ParseStyles(document.Styles);

            // A document with no sections is valid and has zero pages.
            while (Symbol == Symbol.Section)
                ParseSection(document.Sections);

            AssertSymbol(Symbol.BraceRight);
            ReadCode();
            AssertCondition(Symbol == Symbol.Eof, DomMsgID.EndOfFileExpected);

            return document;
        }

        /// <summary>
        /// Parses one of the keywords «\document», «\styles», «\section», «\table», «\textframe», «\chart»
        /// and «\paragraph» and returns the corresponding DocumentObject or DocumentObjectCollection.
        /// </summary>
        internal DocumentObject ParseDocumentObject()
        {
            DocumentObject obj = null;

            MoveToCode();
            switch (Symbol)
            {
                case Symbol.Document:
                    obj = ParseDocument(null);
                    break;

                case Symbol.Styles:
                    obj = ParseStyles(new Styles());
                    break;

                case Symbol.Section:
                    obj = ParseSection(new Sections());
                    break;

                case Symbol.Table:
                    obj = new Table();
                    ParseTable(null, (Table)obj);
                    break;

                case Symbol.TextFrame:
                    DocumentElements elems = new DocumentElements();
                    ParseTextFrame(elems);
                    obj = elems[0];
                    break;

                case Symbol.Chart:
                    throw new NotImplementedException();

                case Symbol.Paragraph:
                    obj = new DocumentElements();
                    ParseParagraph((DocumentElements)obj);
                    break;

                default:
                    ThrowParserException(DomMsgID.UnexpectedSymbol);
                    break;
            }
            ReadCode();
            AssertCondition(Symbol == Symbol.Eof, DomMsgID.EndOfFileExpected);

            return obj;
        }

        /// <summary>
        /// Parses the keyword «\styles».
        /// </summary>
        private Styles ParseStyles(Styles styles)
        {
            MoveToCode();
            AssertSymbol(Symbol.Styles);

            ReadCode();  // read '{'
            AssertSymbol(Symbol.BraceLeft);

            ReadCode();  // read first style name
                         // An empty \styles block is valid.
            while (Symbol == Symbol.Identifier || Symbol == Symbol.StringLiteral)
                ParseStyleDefinition(styles);

            AssertSymbol(Symbol.BraceRight);
            ReadCode();  // read beyond '}'

            return styles;
        }

        /// <summary>
        /// Parses a style definition block within the keyword «\styles».
        /// </summary>
        private Style ParseStyleDefinition(Styles styles)
        {
            //   StyleName [: BaseStyleName]
            //   {
            //     ...
            //   }
            Style style = null;
            try
            {
                string styleName = scanner.Token;
                string baseStyleName = null;

                if (Symbol != Symbol.Identifier && Symbol != Symbol.StringLiteral)
                    ThrowParserException(DomMsgID.StyleNameExpected, styleName);

                ReadCode();

                if (Symbol == Symbol.Colon)
                {
                    ReadCode();
                    if (Symbol != Symbol.Identifier && Symbol != Symbol.StringLiteral)
                        this.ThrowParserException(DomMsgID.StyleNameExpected, styleName);

                    // If baseStyle is not valid, choose InvalidStyleName by default.
                    baseStyleName = scanner.Token;
                    if (styles.GetIndex(baseStyleName) == -1)
                    {
                        ReportParserInfo(DdlErrorLevel.Warning, DomMsgID.UseOfUndefinedBaseStyle, baseStyleName);
                        baseStyleName = "InvalidStyleName";
                    }

                    ReadCode();
                }

                // Get or create style.
                style = styles[styleName];
                if (style != null)
                {
                    // Reset base style.
                    if (baseStyleName != null)
                        style.BaseStyle = baseStyleName;
                }
                else
                {
                    // Style does not exist and no base style is given, choose InvalidStyleName by default.
                    if (baseStyleName == null || baseStyleName == "")
                    {
                        baseStyleName = "InvalidStyleName";
                        ReportParserInfo(DdlErrorLevel.Warning, DomMsgID.UseOfUndefinedStyle, styleName);
                    }

                    style = styles.AddStyle(styleName, baseStyleName);
                }

                // Parse definition (if any).

                if (Symbol == Symbol.BraceLeft)
                {
                    ParseAttributeBlock(style);
                }
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
            return style;
        }

        /// <summary>
        /// Determines if the current symbol is a header or footer.
        /// </summary>
        private bool IsHeaderFooter()
        {
            Symbol sym = Symbol;
            return (sym == Symbol.Header || sym == Symbol.Footer ||
              sym == Symbol.PrimaryHeader || sym == Symbol.PrimaryFooter ||
              sym == Symbol.EvenPageHeader || sym == Symbol.EvenPageFooter ||
              sym == Symbol.FirstPageHeader || sym == Symbol.FirstPageFooter);
        }

        /// <summary>
        /// Parses the keyword «\section».
        /// </summary>
        private Section ParseSection(Sections sections)
        {
            Debug.Assert(sections != null);

            MoveToCode();
            AssertSymbol(Symbol.Section);

            Section section = null;
            try
            {
                section = sections.AddSection();

                ReadCode(); // read '[' or '{'
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(section);

                AssertSymbol(Symbol.BraceLeft);

                // Consider the case that the keyword «\paragraph» can be omitted.
                if (IsParagraphContent())
                {
                    Paragraph paragraph = section.Elements.AddParagraph();
                    ParseParagraphContent(section.Elements, paragraph);
                }
                else
                {
                    ReadCode(); // read beyond '{'

                    // 1st parse headers and footers
                    while (IsHeaderFooter())
                        ParseHeaderFooter(section);

                    // 2nd parse all other stuff
                    ParseDocumentElements(section.Elements, Symbol.Section);
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
            return section;
        }

        /// <summary>
        /// Parses the keywords «\header».
        /// </summary>
        private void ParseHeaderFooter(Section section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            HeaderFooter headerFooter = null;
            try
            {
                Symbol hdrFtrSym = Symbol;
                bool isHeader = hdrFtrSym == Symbol.Header ||
                  hdrFtrSym == Symbol.PrimaryHeader ||
                  hdrFtrSym == Symbol.FirstPageHeader ||
                  hdrFtrSym == Symbol.EvenPageHeader;

                // Recall that the styles "Header" resp. "Footer" are used as default if
                // no other style was given. But this belongs to the rendering process,
                // not to the DDL parser. Therefore no code here belongs to that.
                headerFooter = new HeaderFooter();
                ReadCode(); // read '[' or '{'
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(headerFooter);

                AssertSymbol(Symbol.BraceLeft);
                if (IsParagraphContent())
                {
                    Paragraph paragraph = headerFooter.Elements.AddParagraph();
                    ParseParagraphContent(headerFooter.Elements, paragraph);
                }
                else
                {
                    ReadCode(); // parse '{'
                    ParseDocumentElements(headerFooter.Elements, Symbol.HeaderOrFooter);
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // parse beyond '{'

                HeadersFooters headersFooters = isHeader ? section.Headers : section.Footers;
                if (hdrFtrSym == Symbol.Header || hdrFtrSym == Symbol.Footer)
                {
                    headersFooters.Primary = headerFooter.Clone();
                    headersFooters.EvenPage = headerFooter.Clone();
                    headersFooters.FirstPage = headerFooter.Clone();
                }
                else
                {
                    switch (hdrFtrSym)
                    {
                        case Symbol.PrimaryHeader:
                        case Symbol.PrimaryFooter:
                            headersFooters.Primary = headerFooter;
                            break;

                        case Symbol.EvenPageHeader:
                        case Symbol.EvenPageFooter:
                            headersFooters.EvenPage = headerFooter;
                            break;

                        case Symbol.FirstPageHeader:
                        case Symbol.FirstPageFooter:
                            headersFooters.FirstPage = headerFooter;
                            break;
                    }
                }
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Determines whether the next text is paragraph content or document element.
        /// </summary>
        private bool IsParagraphContent()
        {
            if (MoveToParagraphContent())
            {
                if (scanner.Char == Chars.BackSlash)
                {
                    Symbol symbol = scanner.PeekKeyword();
                    switch (symbol)
                    {
                        case Symbol.Bold:
                        case Symbol.Italic:
                        case Symbol.Underline:
                        case Symbol.Field:
                        case Symbol.Font:
                        case Symbol.FontColor:
                        case Symbol.FontSize:
                        case Symbol.Footnote:
                        case Symbol.Hyperlink:
                        case Symbol.Symbol:
                        case Symbol.Chr:
                        case Symbol.Tab:
                        case Symbol.LineBreak:
                        case Symbol.Space:
                        case Symbol.SoftHyphen:
                            return true;
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Parses the document elements of a «\paragraph», «\cell» or comparable.
        /// </summary>
        private DocumentElements ParseDocumentElements(DocumentElements elements, Symbol context)
        {
            //
            // This is clear:
            //   \section { Hallo World! }
            // All section content will be treated as paragraph content.
            //
            // but this is ambiguous:
            //   \section { \image(...) }
            // It could be an image inside a paragraph or at the section level.
            // In this case it will be treated as an image on section level.
            //
            // If this is not your intention it must be like this:
            //   \section { \paragraph { \image(...) } }
            //

            while (TokenType == TokenType.KeyWord)
            {
                switch (Symbol)
                {
                    case Symbol.Paragraph:
                        ParseParagraph(elements);
                        break;

                    case Symbol.PageBreak:
                        ParsePageBreak(elements);
                        break;

                    case Symbol.Table:
                        ParseTable(elements, null);
                        break;

                    case Symbol.TextFrame:
                        ParseTextFrame(elements);
                        break;

                    case Symbol.Image:
                        ParseImage(elements.AddImage(ImageSource.FromFile("")), false);
                        break;

                    case Symbol.Chart:
                        ParseChart(elements);
                        break;

                    case Symbol.Barcode:
                        ParseBarcode(elements);
                        break;

                    default:
                        ThrowParserException(DomMsgID.UnexpectedSymbol, scanner.Token);
                        break;
                }
            }
            return elements;
        }

        /// <summary>
        /// Parses the keyword «\paragraph».
        /// </summary>
        private void ParseParagraph(DocumentElements elements)
        {
            MoveToCode();
            AssertSymbol(Symbol.Paragraph);

            Paragraph paragraph = elements.AddParagraph();
            try
            {
                ReadCode(); // read '[' or '{'
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(paragraph);

                // Empty paragraphs without braces are valid.
                if (Symbol == Symbol.BraceLeft)
                {
                    ParseParagraphContent(elements, paragraph);
                    AssertSymbol(Symbol.BraceRight);
                    ReadCode(); // read beyond '}'
                }
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the inner text of a paragraph, i.e. stops on BraceRight and treats empty
        /// line as paragraph separator.
        /// </summary>
        private void ParseParagraphContent(DocumentElements elements, Paragraph paragraph)
        {
            Paragraph para = paragraph;
            if (para == null)
                para = elements.AddParagraph();

            while (para != null)
            {
                ParseFormattedText(para.Elements, 0);
                if (Symbol != Symbol.BraceRight && Symbol != Symbol.Eof)
                {
                    para = elements.AddParagraph();
                }
                else
                    para = null;
            }
        }

        /// <summary>
        /// Removes the last blank from the text. Used before a tab, a linebreak or a space will be
        /// added to the text.
        /// </summary>
        private void RemoveTrailingBlank(ParagraphElements elements)
        {
            DocumentObject dom = elements.LastObject;
            if (dom is Text)
            {
                Text text = (Text)dom;
                if (text.Content.EndsWith(" "))
                    text.Content = text.Content.Remove(text.Content.Length - 1, 1);
            }
        }

        /// <summary>
        /// Parses the inner text of a paragraph. Parsing ends if '}' is reached or an empty
        /// line occurs on nesting level 0.
        /// </summary>
        private void ParseFormattedText(ParagraphElements elements, int nestingLevel)
        {
            MoveToParagraphContent();

            bool loop = true;
            bool rootLevel = nestingLevel == 0;
            ReadText(rootLevel);
            while (loop)
            {
                switch (Symbol)
                {
                    case Symbol.Eof:
                        ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                        break;

                    case Symbol.EmptyLine:
                        elements.AddCharacter(SymbolName.ParaBreak);
                        ReadText(rootLevel);
                        break;

                    case Symbol.BraceRight:
                        loop = false;
                        break;

                    case Symbol.Comment:
                        // Ignore comments.
                        ReadText(rootLevel);
                        break;

                    case Symbol.Text:
                        elements.AddText(Token);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Tab:
                        RemoveTrailingBlank(elements);
                        elements.AddTab();
                        scanner.MoveToNonWhiteSpaceOrEol();
                        ReadText(rootLevel);
                        break;

                    case Symbol.LineBreak:
                        RemoveTrailingBlank(elements);
                        elements.AddLineBreak();
                        scanner.MoveToNonWhiteSpaceOrEol();
                        ReadText(rootLevel);
                        break;

                    case Symbol.Bold:
                        ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Bold), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Italic:
                        ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Italic), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Underline:
                        ParseBoldItalicEtc(elements.AddFormattedText(TextFormat.Underline), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Font:
                        ParseFont(elements.AddFormattedText(), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.FontSize:
                        ParseFontSize(elements.AddFormattedText(), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.FontColor:
                        ParseFontColor(elements.AddFormattedText(), nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Image:
                        ParseImage(elements.AddImage(ImageSource.FromFile("")), true);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Field:
                        ParseField(elements, nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Footnote:
                        ParseFootnote(elements, nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Hyperlink:
                        ParseHyperlink(elements, nestingLevel + 1);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Space:
                        RemoveTrailingBlank(elements);
                        ParseSpace(elements, nestingLevel + 1);
                        scanner.MoveToNonWhiteSpaceOrEol();
                        ReadText(rootLevel);
                        break;

                    case Symbol.Symbol:
                        ParseSymbol(elements);
                        ReadText(rootLevel);
                        break;

                    case Symbol.Chr:
                        ParseChr(elements);
                        ReadText(rootLevel);
                        break;

                    default:
                        ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the keywords «\bold», «\italic», and «\underline».
        /// </summary>
        private void ParseBoldItalicEtc(FormattedText formattedText, int nestingLevel)
        {
            ReadCode();
            AssertSymbol(Symbol.BraceLeft);
            ParseFormattedText(formattedText.Elements, nestingLevel);
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses the keyword «\font».
        /// </summary>
        private void ParseFont(FormattedText formattedText, int nestingLevel)
        {
            AssertSymbol(Symbol.Font);
            ReadCode();

            if (Symbol == Symbol.ParenLeft)
            {
                formattedText.Style = ParseElementName();
                ReadCode();
            }

            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(formattedText);

            AssertSymbol(Symbol.BraceLeft);
            ParseFormattedText(formattedText.Elements, nestingLevel);
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses code like «("name")».
        /// </summary>
        private string ParseElementName()
        {
            AssertSymbol(Symbol.ParenLeft);
            ReadCode();
            if (Symbol != Symbol.StringLiteral)
                ThrowParserException(DomMsgID.StringExpected, Token);

            string name = Token;
            ReadCode();
            AssertSymbol(Symbol.ParenRight);

            return name;
        }

        /// <summary>
        /// Parses the keyword «\fontsize».
        /// </summary>
        private void ParseFontSize(FormattedText formattedText, int nestingLevel)
        {
            AssertSymbol(Symbol.FontSize);
            ReadCode();

            AssertSymbol(Symbol.ParenLeft);
            ReadCode();
            //NYI: Check token for correct Unit format
            formattedText.Font.Size = Token;
            ReadCode();
            AssertSymbol(Symbol.ParenRight);
            ReadCode();

            AssertSymbol(Symbol.BraceLeft);
            ParseFormattedText(formattedText.Elements, nestingLevel);
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses the keyword «\fontcolor».
        /// </summary>
        private void ParseFontColor(FormattedText formattedText, int nestingLevel)
        {
            AssertSymbol(Symbol.FontColor);
            ReadCode();  // read '('

            AssertSymbol(Symbol.ParenLeft);
            ReadCode();  // read color token
            Color color = ParseColor();
            formattedText.Font.Color = color;
            AssertSymbol(Symbol.ParenRight);
            ReadCode();
            AssertSymbol(Symbol.BraceLeft);
            ParseFormattedText(formattedText.Elements, nestingLevel);
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses the keyword «\symbol» resp. «\(».
        /// </summary>
        private void ParseSymbol(ParagraphElements elements)
        {
            AssertSymbol(Symbol.Symbol);

            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            char ch = (char)0;
            SymbolName symtype = 0;
            int count = 1;

            ReadCode();  // read name
            if (TokenType == TokenType.Identifier)
            {
                try
                {
                    if (Enum.IsDefined(typeof(SymbolName), Token))
                    {
                        AssertCondition(IsSymbolType(Token), DomMsgID.InvalidSymbolType, Token);
                        symtype = (SymbolName)Enum.Parse(typeof(SymbolName), Token, true);
                    }
                }
                catch (Exception ex)
                {
                    ThrowParserException(ex, DomMsgID.InvalidEnum, Token);
                }
            }
            else
            {
                ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
            }

            ReadCode();  // read integer or identifier
            if (Symbol == Symbol.Comma)
            {
                ReadCode();  // read integer
                if (TokenType == TokenType.IntegerLiteral)
                    count = scanner.GetTokenValueAsInt();
                ReadCode();
            }

            AssertSymbol(Symbol.ParenRight);

            Character character;
            if (symtype != 0)
                character = elements.AddCharacter(symtype, count);
            else
                character = elements.AddCharacter(ch, count);
        }

        /// <summary>
        /// Parses the keyword «\chr».
        /// </summary>
        private void ParseChr(ParagraphElements elements)
        {
            AssertSymbol(Symbol.Chr);

            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            char ch = (char)0;
            SymbolName symtype = 0;
            int count = 1;

            ReadCode();  // read integer
            if (TokenType == TokenType.IntegerLiteral)
            {
                int val = this.scanner.GetTokenValueAsInt();
                if (val >= 1 && val < 256)
                    ch = (char)val;
                else
                    ThrowParserException(DomMsgID.OutOfRange, "1 - 255");
            }
            else
            {
                ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
            }

            ReadCode();  // read integer or identifier
            if (Symbol == Symbol.Comma)
            {
                ReadCode();  // read integer
                if (TokenType == TokenType.IntegerLiteral)
                    count = scanner.GetTokenValueAsInt();
                ReadCode();
            }

            AssertSymbol(Symbol.ParenRight);

            Character character;
            if (symtype != 0)
                character = elements.AddCharacter(symtype, count);
            else
                character = elements.AddCharacter(ch, count);
        }

        /// <summary>
        /// Parses the keyword «\field».
        /// </summary>
        private void ParseField(ParagraphElements elements, int nestingLevel)
        {
            AssertSymbol(Symbol.Field);

            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            ReadCode();  // read identifier
            AssertSymbol(Symbol.Identifier);
            string fieldType = Token.ToLower();

            ReadCode();  // read ')'
            AssertSymbol(Symbol.ParenRight);

            DocumentObject field = null;
            switch (fieldType)
            {
                case "date":
                    field = elements.AddDateField();
                    break;

                case "page":
                    field = elements.AddPageField();
                    break;

                case "numpages":
                    field = elements.AddNumPagesField();
                    break;

                case "info":
                    field = elements.AddInfoField((InfoFieldType)0);
                    break;

                case "sectionpages":
                    field = elements.AddSectionPagesField();
                    break;

                case "section":
                    field = elements.AddSectionField();
                    break;

                case "bookmark":
                    field = elements.AddBookmark("");
                    break;

                case "pageref":
                    field = elements.AddPageRefField("");
                    break;
            }
            AssertCondition(field != null, DomMsgID.InvalidFieldType, Token);

            if (scanner.PeekSymbol() == Symbol.BracketLeft)
            {
                ReadCode();  // read '['
                ParseAttributes(field, false);
            }
        }

        /// <summary>
        /// Parses the keyword «\footnote».
        /// </summary>
        private void ParseFootnote(ParagraphElements elements, int nestingLevel)
        {
            AssertSymbol(Symbol.Footnote);
            ReadCode();

            Footnote footnote = elements.AddFootnote();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(footnote);

            AssertSymbol(Symbol.BraceLeft);

            // The keyword «\paragraph» is typically ommitted.
            if (IsParagraphContent())
            {
                Paragraph paragraph = footnote.Elements.AddParagraph();
                ParseParagraphContent(footnote.Elements, paragraph);
            }
            else
            {
                ReadCode(); // read beyond '{'
                ParseDocumentElements(footnote.Elements, Symbol.Footnote);
            }
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses the keyword «\hyperlink».
        /// </summary>
        private void ParseHyperlink(ParagraphElements elements, int nestingLevel)
        {
            AssertSymbol(Symbol.Hyperlink);
            ReadCode();

            Hyperlink hyperlink = elements.AddHyperlink("");
            //NYI: Without name and type the hyperlink is senseless, so attributes need to be checked
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(hyperlink);

            AssertSymbol(Symbol.BraceLeft);
            ParseFormattedText(hyperlink.Elements, nestingLevel);
            AssertSymbol(Symbol.BraceRight);
        }

        /// <summary>
        /// Parses the keyword «\space».
        /// </summary>
        private void ParseSpace(ParagraphElements elements, int nestingLevel)
        {
            // Samples
            // \space
            // \space(5)
            // \space(em)
            // \space(em,5)
            AssertSymbol(Symbol.Space);

            Character space = elements.AddSpace(1);

            // «\space» can stand alone
            if (scanner.PeekSymbol() == Symbol.ParenLeft)
            {
                ReadCode(); // read '('
                AssertSymbol(Symbol.ParenLeft);

                ReadCode(); // read beyond '('
                if (Symbol == Symbol.Identifier)
                {
                    string type = Token;
                    if (!IsSpaceType(type))
                        ThrowParserException(DomMsgID.InvalidEnum, type);

                    space.SymbolName = (SymbolName)Enum.Parse(typeof(SymbolName), type, true);

                    ReadCode(); // read ',' or ')'
                    if (Symbol == Symbol.Comma)
                    {
                        ReadCode();  // read integer
                        AssertSymbol(Symbol.IntegerLiteral);
                        space.Count = scanner.GetTokenValueAsInt();
                        ReadCode(); // read ')'
                    }
                }
                else if (Symbol == Symbol.IntegerLiteral)
                {
                    space.Count = scanner.GetTokenValueAsInt();
                    ReadCode();
                }
                AssertSymbol(Symbol.ParenRight);
            }
        }

        /// <summary>
        /// Parses a page break in a document elements container.
        /// </summary>
        private void ParsePageBreak(DocumentElements elements)
        {
            AssertSymbol(Symbol.PageBreak);
            elements.AddPageBreak();
            ReadCode();
        }

        /// <summary>
        /// Parses the keyword «\table».
        /// </summary>
        private void ParseTable(DocumentElements elements, Table table)
        {
            Table tbl = table;
            try
            {
                if (tbl == null)
                    tbl = elements.AddTable();

                MoveToCode();
                AssertSymbol(Symbol.Table);

                ReadCode();
                if (scanner.Symbol == Symbol.BracketLeft)
                    ParseAttributes(tbl);

                AssertSymbol(Symbol.BraceLeft);
                ReadCode();

                // Table must start with «\columns»...
                AssertSymbol(Symbol.Columns);
                ParseColumns(tbl);

                // ...followed by «\rows».
                AssertSymbol(Symbol.Rows);
                ParseRows(tbl);

                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\columns».
        /// </summary>
        private void ParseColumns(Table table)
        {
            Debug.Assert(table != null);
            Debug.Assert(Symbol == Symbol.Columns);

            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(table.Columns);

            AssertSymbol(Symbol.BraceLeft);
            ReadCode();

            bool loop = true;
            while (loop)
            {
                switch (Symbol)
                {
                    case Symbol.Eof:
                        ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                        break;

                    case Symbol.BraceRight:
                        loop = false;
                        ReadCode();
                        break;

                    case Symbol.Column:
                        ParseColumn(table.AddColumn());
                        break;

                    default:
                        AssertSymbol(Symbol.Column);
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the keyword «\column».
        /// </summary>
        private void ParseColumn(Column column)
        {
            Debug.Assert(column != null);
            Debug.Assert(Symbol == Symbol.Column);

            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(column);

            // Read empty content
            if (Symbol == Symbol.BraceLeft)
            {
                ReadCode();
                AssertSymbol(Symbol.BraceRight);
                ReadCode();
            }
        }

        /// <summary>
        /// Parses the keyword «\rows».
        /// </summary>
        private void ParseRows(Table table)
        {
            Debug.Assert(table != null);
            Debug.Assert(Symbol == Symbol.Rows);

            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(table.Rows);

            AssertSymbol(Symbol.BraceLeft);
            ReadCode();

            bool loop = true;
            while (loop)
            {
                switch (Symbol)
                {
                    case Symbol.Eof:
                        ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                        break;

                    case Symbol.BraceRight:
                        ReadCode(); // read '}'
                        loop = false;
                        break;

                    case Symbol.Row:
                        ParseRow(table.AddRow());
                        break;

                    default:
                        AssertSymbol(Symbol.Row);
                        break;
                }
            }
        }

        /// <summary>
        /// Parses the keyword «\row».
        /// </summary>
        private void ParseRow(Row row)
        {
            Debug.Assert(row != null);
            Debug.Assert(Symbol == Symbol.Row);

            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(row);

            if (Symbol == Symbol.BraceLeft)
            {
                ReadCode();

                bool loop = true;
                int idx = 0;
                int cells = row.Cells.Count;
                while (loop)
                {
                    switch (Symbol)
                    {
                        case Symbol.Eof:
                            ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                            break;

                        case Symbol.BraceRight:
                            loop = false;
                            ReadCode();
                            break;

                        case Symbol.Cell:
                            ParseCell(row[idx]);
                            idx++;
                            break;

                        default:
                            ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Parses the keyword «\cell».
        /// </summary>
        private void ParseCell(Cell cell)
        {
            Debug.Assert(cell != null);
            Debug.Assert(Symbol == Symbol.Cell);

            ReadCode();
            if (Symbol == Symbol.BracketLeft)
                ParseAttributes(cell);

            // Empty cells without braces are valid.
            if (Symbol == Symbol.BraceLeft)
            {
                if (IsParagraphContent())
                {
                    ParseParagraphContent(cell.Elements, null);
                }
                else
                {
                    ReadCode();
                    if (Symbol != Symbol.BraceRight)
                        ParseDocumentElements(cell.Elements, Symbol.Cell);
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read '}'
            }
        }

        /// <summary>
        /// Parses the keyword «\image».
        /// </summary>
        private void ParseImage(Image image, bool paragraphContent)
        {
            // Future syntax by example
            //   \image("Name")
            //   \image("Name")[...]
            //   \image{base64...}       //NYI
            //   \image[...]{base64...}  //NYI
            Debug.Assert(image != null);

            try
            {
                MoveToCode();
                AssertSymbol(Symbol.Image);
                ReadCode();

                if (scanner.Symbol == Symbol.ParenLeft)
                    image.Source = ImageSource.FromFile(ParseElementName());

                if (scanner.PeekSymbol() == Symbol.BracketLeft)
                {
                    ReadCode();
                    ParseAttributes(image, !paragraphContent);
                }
                else if (!paragraphContent)
                    ReadCode(); // We are a part of a section, cell etc.; read beyond ')'.
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\textframe».
        /// </summary>
        private void ParseTextFrame(DocumentElements elements)
        {
            Debug.Assert(elements != null);

            TextFrame textFrame = elements.AddTextFrame();
            try
            {
                ReadCode();
                if (scanner.Symbol == Symbol.BracketLeft)
                    ParseAttributes(textFrame);

                AssertSymbol(Symbol.BraceLeft);
                if (IsParagraphContent())
                {
                    ParseParagraphContent(textFrame.Elements, null);
                }
                else
                {
                    ReadCode(); // read '{'
                    ParseDocumentElements(textFrame.Elements, Symbol.TextFrame);
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
        }

        private void ParseBarcode(DocumentElements elements)
        {
            // Syntax:
            // 1.  \barcode(Code)
            // 2.  \barcode(Code)[...]
            // 3.  \barcode(Code, Type)
            // 4.  \barcode(Code, Type)[...]

            try
            {
                ReadCode();
                AssertSymbol(Symbol.ParenLeft, DomMsgID.MissingParenLeft, GetSymbolText(Symbol.Barcode));
                ReadCode();
                AssertSymbol(Symbol.StringLiteral, DomMsgID.UnexpectedSymbol);

                Barcode barcode = elements.AddBarcode();
                barcode.SetValue("Code", Token);
                ReadCode();
                if (Symbol == Symbol.Comma)
                {
                    ReadCode();
                    AssertSymbol(Symbol.Identifier, DomMsgID.IdentifierExpected, Token);
                    BarcodeType barcodeType = (BarcodeType)Enum.Parse(typeof(BarcodeType), Token, true);
                    barcode.SetValue("type", barcodeType);
                    ReadCode();
                }
                AssertSymbol(Symbol.ParenRight, DomMsgID.MissingParenRight, GetSymbolText(Symbol.Barcode));

                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(barcode);
                //barcode->ConsistencyCheck(mInfoHandler->Infos());
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\chart».
        /// </summary>
        private void ParseChart(DocumentElements elements)
        {
            // Syntax:
            // 1.  \chartarea(Type){...}
            // 2.  \chartarea(Type)[...]{...}
            //
            // Usage of header-, bottom-, footer-, left- and rightarea are similar.

            ChartType chartType = 0;
            Chart chart = null;
            try
            {
                ReadCode(); // read '('
                AssertSymbol(Symbol.ParenLeft, DomMsgID.MissingParenLeft, GetSymbolText(Symbol.Chart));

                ReadCode(); // ChartType name
                AssertSymbol(Symbol.Identifier, DomMsgID.IdentifierExpected, Token);
                string chartTypeName = Token;

                ReadCode(); // read ')'
                AssertSymbol(Symbol.ParenRight, DomMsgID.MissingParenRight, GetSymbolText(Symbol.Chart));

                try
                {
                    chartType = (ChartType)Enum.Parse(typeof(ChartType), chartTypeName, true);
                }
                catch (Exception ex)
                {
                    ThrowParserException(ex, DomMsgID.UnknownChartType, chartTypeName);
                }

                chart = elements.AddChart(chartType);

                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(chart);

                AssertSymbol(Symbol.BraceLeft, DomMsgID.MissingBraceLeft, GetSymbolText(Symbol.Chart));

                ReadCode(); // read beyond '{'

                bool fContinue = true;
                while (fContinue)
                {
                    switch (Symbol)
                    {
                        case Symbol.Eof:
                            ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                            break;

                        case Symbol.BraceRight:
                            fContinue = false;
                            break;

                        case Symbol.PlotArea:
                            ParseArea(chart.PlotArea);
                            break;

                        case Symbol.HeaderArea:
                            ParseArea(chart.HeaderArea);
                            break;

                        case Symbol.FooterArea:
                            ParseArea(chart.FooterArea);
                            break;

                        case Symbol.TopArea:
                            ParseArea(chart.TopArea);
                            break;

                        case Symbol.BottomArea:
                            ParseArea(chart.BottomArea);
                            break;

                        case Symbol.LeftArea:
                            ParseArea(chart.LeftArea);
                            break;

                        case Symbol.RightArea:
                            ParseArea(chart.RightArea);
                            break;

                        case Symbol.XAxis:
                            ParseAxes(chart.XAxis, Symbol);
                            break;

                        case Symbol.YAxis:
                            ParseAxes(chart.YAxis, Symbol);
                            break;

                        case Symbol.ZAxis:
                            ParseAxes(chart.ZAxis, Symbol);
                            break;

                        case Symbol.Series:
                            ParseSeries(chart.SeriesCollection.AddSeries());
                            break;

                        case Symbol.XValues:
                            ParseSeries(chart.XValues.AddXSeries());
                            break;

                        default:
                            ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
                            break;
                    }
                }
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\plotarea» inside a chart.
        /// </summary>
        private void ParseArea(PlotArea area)
        {
            // Syntax:
            // 1.  \plotarea{...}
            // 2.  \plotarea[...]{...} //???

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                {
                    ParseAttributes(area, false);
                    ReadCode();
                }

                if (Symbol != Symbol.BraceLeft)
                    return;

                bool fContinue = true;
                while (fContinue)
                {
                    ReadCode();
                    switch (Symbol)
                    {
                        case Symbol.BraceRight:
                            fContinue = false;
                            break;

                        default:
                            // Alles ignorieren? Warnung ausgeben?
                            break;
                    }
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keywords «\headerarea», «\toparea», «\bottomarea», «\footerarea»,
        /// «\leftarea» or «\rightarea» inside a chart.
        /// </summary>
        private void ParseArea(TextArea area)
        {
            // Syntax:
            // 1.  \toparea{...}
            // 2.  \toparea[...]{...}
            //
            // Usage of header-, bottom-, footer-, left- and rightarea are similar.

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                {
                    ParseAttributes(area, false);
                    ReadCode();
                }

                if (Symbol != Symbol.BraceLeft)
                    return;

                if (IsParagraphContent())
                    ParseParagraphContent(area.Elements, null);
                else
                {
                    ReadCode(); // read beyond '{'
                    bool fContinue = true;
                    while (fContinue)
                    {
                        switch (Symbol)
                        {
                            case Symbol.BraceRight:
                                fContinue = false;
                                break;

                            case Symbol.Legend:
                                ParseLegend(area.AddLegend());
                                break;

                            case Symbol.Paragraph:
                                ParseParagraph(area.Elements);
                                break;

                            case Symbol.Table:
                                ParseTable(null, area.AddTable());
                                break;

                            case Symbol.TextFrame:
                                ParseTextFrame(area.Elements);
                                break;

                            case Symbol.Image:
                                Image image = new Image();
                                ParseImage(image, false);
                                area.Elements.Add(image);
                                break;

                            default:
                                ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
                                break;
                        }
                    }
                }
                AssertSymbol(Symbol.BraceRight);
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keywords «\xaxis», «\yaxis» or «\zaxis» inside a chart.
        /// </summary>
        private void ParseAxes(Axis axis, Symbol symbolAxis)
        {
            // Syntax:
            // 1.  \xaxis[...]
            // 2.  \xaxis[...]{...} //???
            //
            // Usage of yaxis and zaxis are similar.

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                {
                    ParseAttributes(axis, false);
                    ReadCode();
                }

                if (Symbol != Symbol.BraceLeft)
                    return;

                while (Symbol != Symbol.BraceRight)
                    ReadCode();

                AssertSymbol(Symbol.BraceRight, DomMsgID.MissingBraceRight, GetSymbolText(symbolAxis));
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\series» inside a chart.
        /// </summary>
        private void ParseSeries(Series series)
        {
            // Syntax:
            // 1.  \series{...}
            // 2.  \series[...]{...}

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(series);

                AssertSymbol(Symbol.BraceLeft, DomMsgID.MissingBraceLeft, GetSymbolText(Symbol.Series));
                ReadCode(); // read beyond '{'

                bool fContinue = true;
                bool fFoundComma = true;
                while (fContinue)
                {
                    switch (Symbol)
                    {
                        case Symbol.Eof:
                            ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                            break;

                        case Symbol.BraceRight:
                            fContinue = false;
                            break;

                        case Symbol.Comma:
                            fFoundComma = true;
                            ReadCode();
                            break;

                        case Symbol.Point:
                            AssertCondition(fFoundComma, DomMsgID.MissingComma);
                            ParsePoint(series.Add(0.0));
                            fFoundComma = false;
                            break;

                        case Symbol.Null:
                            AssertCondition(fFoundComma, DomMsgID.MissingComma);
                            series.AddBlank();
                            fFoundComma = false;
                            ReadCode();
                            break;

                        default:
                            AssertCondition(fFoundComma, DomMsgID.MissingComma);
                            series.Add(this.scanner.GetTokenValueAsReal());
                            fFoundComma = false;
                            ReadCode();
                            break;
                    }
                }
                AssertSymbol(Symbol.BraceRight, DomMsgID.MissingBraceRight, GetSymbolText(Symbol.Series));
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\xvalues» inside a chart.
        /// </summary>
        private void ParseSeries(XSeries series)
        {
            // Syntax:
            // 1.  \xvalues{...}

            try
            {
                ReadCode();
                AssertSymbol(Symbol.BraceLeft, DomMsgID.MissingBraceLeft, GetSymbolText(Symbol.Series));

                bool fFoundComma = true;
                bool fContinue = true;
                while (fContinue)
                {
                    ReadCode();
                    switch (Symbol)
                    {
                        case Symbol.Eof:
                            ThrowParserException(DomMsgID.UnexpectedEndOfFile);
                            break;

                        case Symbol.BraceRight:
                            fContinue = false;
                            break;

                        case Symbol.Comma:
                            fFoundComma = true;
                            break;

                        case Symbol.Null:
                            AssertCondition(fFoundComma, DomMsgID.MissingComma);
                            series.AddBlank();
                            fFoundComma = false;
                            break;

                        case Symbol.StringLiteral:
                        case Symbol.IntegerLiteral:
                        case Symbol.RealLiteral:
                        case Symbol.HexIntegerLiteral:
                            AssertCondition(fFoundComma, DomMsgID.MissingComma);
                            series.Add(Token);
                            fFoundComma = false;
                            break;

                        default:
                            ThrowParserException(DomMsgID.UnexpectedSymbol, Token);
                            break;
                    }
                }
                AssertSymbol(Symbol.BraceRight, DomMsgID.MissingBraceRight, GetSymbolText(Symbol.Series));
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\point» inside a series.
        /// </summary>
        private void ParsePoint(Point point)
        {
            // Syntax:
            // 1.  \point{...}
            // 2.  \point[...]{...}

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                    ParseAttributes(point);

                AssertSymbol(Symbol.BraceLeft, DomMsgID.MissingBraceLeft, GetSymbolText(Symbol.Point));
                ReadCode(); // read beyond '{'
                point.Value = this.scanner.GetTokenValueAsReal();

                ReadCode(); // read '}'
                AssertSymbol(Symbol.BraceRight, DomMsgID.MissingBraceRight, GetSymbolText(Symbol.Point));
                ReadCode(); // read beyond '}'
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses the keyword «\legend» inside a textarea.
        /// </summary>
        private void ParseLegend(Legend legend)
        {
            // Syntax:
            // 1.  \legend
            // 2.  \legend[...]
            // 3.  \legend[...]{...}

            try
            {
                ReadCode();
                if (Symbol == Symbol.BracketLeft)
                {
                    ParseAttributes(legend, false);
                    ReadCode();
                }

                // Empty legends are allowed.
                if (Symbol != Symbol.BraceLeft)
                    return;

                AdjustToNextBlock(); // consume/ignore all content
            }
            catch (DdlParserException pe)
            {
                ReportParserException(pe);
                AdjustToNextBlock();
            }
        }

        /// <summary>
        /// Parses an attribute declaration block enclosed in brackets «[…]». If readNextSymbol is
        /// set to true, the closing bracket will be read.
        /// </summary>
        private void ParseAttributes(DocumentObject element, bool readNextSymbol)
        {
            AssertSymbol(Symbol.BracketLeft);
            ReadCode();  // read beyond '['

            while (Symbol == Symbol.Identifier)
                ParseAttributeStatement(element);

            AssertSymbol(Symbol.BracketRight);

            // Do not read ']' when parsing in paragraph content.
            if (readNextSymbol)
                ReadCode();  // read beyond ']'
        }

        /// <summary>
        /// Parses an attribute declaration block enclosed in brackets «[…]».
        /// </summary>
        private void ParseAttributes(DocumentObject element)
        {
            ParseAttributes(element, true);
        }

        /// <summary>
        /// Parses a single statement in an attribute declaration block.
        /// </summary>
        private void ParseAttributeStatement(DocumentObject dom)
        {
            // Syntax is easy
            //   identifier: xxxxx
            // or 
            //   sequence of identifiers: xxx.yyy.zzz
            //
            // followed by: «=», «+=», «-=», or «{»
            //
            // Parser of rhs depends on the type of the l-value.

            object val = null;
            string valueName = "";
            try
            {
                valueName = scanner.Token;

                DocumentObject doc = dom;
                ReadCode();

                // Resolve path, if it exists.
                while (Symbol == Symbol.Dot)
                {
#if DEBUG
                    if (valueName == "TabStops")
                        valueName.GetType();
#endif

                    val = doc.GetValue(valueName);
                    if (val == null)
                    {
                        DocumentObject documentObject = (DocumentObject)doc;
                        val = documentObject.CreateValue(valueName);
                        doc.SetValue(valueName, val);
                    }
                    AssertCondition(val != null, DomMsgID.InvalidValueName, valueName);
                    doc = val as DocumentObject;
                    AssertCondition(doc != null, DomMsgID.SymbolIsNotAnObject, valueName);

                    ReadCode();
                    AssertCondition(Symbol == Symbol.Identifier, DomMsgID.InvalidValueName, scanner.Token);
                    valueName = scanner.Token;
                    AssertCondition(valueName[0] != '_', DomMsgID.NoAccess, scanner.Token);

#if DEBUG
                    if (valueName == "TabStops")
                        valueName.GetType();
#endif

                    ReadCode();
                }

                switch (Symbol)
                {
                    case Symbol.Assign:
                        //DomValueDescriptor is needed from assignment routine.
                        ValueDescriptor pvd = doc.Meta[valueName];
                        AssertCondition(pvd != null, DomMsgID.InvalidValueName, valueName);
                        ParseAssign(doc, pvd);
                        break;

                    case Symbol.PlusAssign:
                    case Symbol.MinusAssign:
                        // Hard-coded for TabStops only...
                        if (!(doc is ParagraphFormat))
                            ThrowParserException(DomMsgID.SymbolNotAllowed, scanner.Token);
                        if (String.Compare(valueName, "TabStops", true) != 0)
                            ThrowParserException(DomMsgID.InvalidValueForOperation, valueName, scanner.Token);

                        ParagraphFormat paragraphFormat = (ParagraphFormat)doc;
                        TabStops tabStops = paragraphFormat.TabStops;

                        if (true) // HACK in ParseAttributeStatement
                        {
                            bool fAddItem = Symbol == Symbol.PlusAssign;
                            TabStop tabStop = new TabStop();

                            ReadCode();

                            if (Symbol == Symbol.BraceLeft)
                            {
                                ParseAttributeBlock(tabStop);
                            }
                            else if (Symbol == Symbol.StringLiteral || Symbol == Symbol.RealLiteral || Symbol == Symbol.IntegerLiteral)
                            {
                                // Special hack for tab stops...
                                Unit unit = Token;
                                tabStop.SetValue("Position", unit);

                                ReadCode();
                            }
                            else
                                ThrowParserException(DomMsgID.UnexpectedSymbol, Token);

                            if (fAddItem)
                                tabStops.AddTabStop(tabStop);
                            else
                                tabStops.RemoveTabStop(tabStop.Position);
                        }
                        break;

                    case Symbol.BraceLeft:
                        val = doc.GetValue(valueName);
                        AssertCondition(val != null, DomMsgID.InvalidValueName, valueName);

                        if (val is DocumentObject)
                            ParseAttributeBlock((DocumentObject)val);
                        else
                            ThrowParserException(DomMsgID.SymbolIsNotAnObject, valueName);
                        break;

                    default:
                        ThrowParserException(DomMsgID.SymbolNotAllowed, scanner.Token);
                        return;
                }
            }
            catch (DdlParserException ex)
            {
                ReportParserException(ex);
                AdjustToNextBlock();
            }
            catch (ArgumentException e)
            {
                ReportParserException(e, DomMsgID.InvalidAssignment, valueName);
            }
        }

        /// <summary>
        /// Parses an attribute declaration block enclosed in braces «{…}».
        /// </summary>
        private void ParseAttributeBlock(DocumentObject element)
        {
            // Technically the same as ParseAttributes

            AssertSymbol(Symbol.BraceLeft);
            ReadCode();  // move beyond '{'

            while (Symbol == Symbol.Identifier)
                ParseAttributeStatement(element);

            AssertSymbol(Symbol.BraceRight);
            ReadCode();  // move beyond '}'
        }

        /// <summary>
        /// Parses an assign statement in an attribute declaration block.
        /// </summary>
        private void ParseAssign(DocumentObject dom, ValueDescriptor vd)
        {
            if (dom == null)
                throw new ArgumentNullException("dom");
            if (vd == null)
                throw new ArgumentNullException("vd");

            if (Symbol == Symbol.Assign)
                ReadCode();

            Type valType = vd.ValueType;
            try
            {
                if (valType == typeof(string))
                    ParseStringAssignment(dom, vd);
                else if (valType == typeof(int))
                    ParseIntegerAssignment(dom, vd);
                else if (valType == typeof(Unit))
                    ParseUnitAssignment(dom, vd);
                else if (valType == typeof(double) || valType == typeof(float))
                    ParseRealAssignment(dom, vd);
                else if (valType == typeof(bool))
                    ParseBoolAssignment(dom, vd);
                else if (typeof(Enum).GetTypeInfo().IsAssignableFrom(valType.GetTypeInfo()))
                    ParseEnumAssignment(dom, vd);
                else if (valType == typeof(Color))
                    ParseColorAssignment(dom, vd);
                else if (typeof(ValueType).IsAssignableFrom(valType))
                {
                    ParseValueTypeAssignment(dom, vd);
                }
                else if (typeof(DocumentObject).IsAssignableFrom(valType))
                {
                    ParseDocumentObjectAssignment(dom, vd);
                }
                else
                {
                    AdjustToNextStatement();
                    ThrowParserException(DomMsgID.InvalidType, vd.ValueType.Name, vd.ValueName);
                }
            }
            catch (Exception ex)
            {
                ReportParserException(ex, DomMsgID.InvalidAssignment, vd.ValueName);
            }
        }

        /// <summary>
        /// Parses the assignment to a boolean l-value.
        /// </summary>
        private void ParseBoolAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertCondition(Symbol == Symbol.True || Symbol == Symbol.False, DomMsgID.BoolExpected,
              scanner.Token);

            dom.SetValue(vd.ValueName, Symbol == Symbol.True);
            ReadCode();
        }

        /// <summary>
        /// Parses the assignment to an integer l-value.
        /// </summary>
        private void ParseIntegerAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral || Symbol == Symbol.StringLiteral,
              DomMsgID.IntegerExpected, Token);

            int n = Int32.Parse(scanner.Token, CultureInfo.InvariantCulture);
            dom.SetValue(vd.ValueName, n);

            ReadCode();
        }

        /// <summary>
        /// Parses the assignment to a floating point l-value.
        /// </summary>
        private void ParseRealAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertCondition(Symbol == Symbol.RealLiteral || Symbol == Symbol.IntegerLiteral || Symbol == Symbol.StringLiteral,
              DomMsgID.RealExpected, scanner.Token);

            double r = double.Parse(scanner.Token, CultureInfo.InvariantCulture);
            dom.SetValue(vd.ValueName, r);

            ReadCode();
        }

        /// <summary>
        /// Parses the assignment to a Unit l-value.
        /// </summary>
        private void ParseUnitAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertCondition(Symbol == Symbol.RealLiteral || Symbol == Symbol.IntegerLiteral || Symbol == Symbol.StringLiteral,
              DomMsgID.RealExpected, scanner.Token);

            Unit unit = Token;
            dom.SetValue(vd.ValueName, unit);
            ReadCode();
        }

        /// <summary>
        /// Parses the assignment to a string l-value.
        /// </summary>
        private void ParseStringAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertCondition(Symbol == Symbol.StringLiteral, DomMsgID.StringExpected, scanner.Token);

            vd.SetValue(dom, Token);  //dom.SetValue(vd.ValueName, scanner.Token);

            ReadCode();  // read next token
        }

        /// <summary>
        /// Parses the assignment to an enum l-value.
        /// </summary>
        private void ParseEnumAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            AssertSymbol(Symbol.Identifier, DomMsgID.IdentifierExpected, scanner.Token);

            try
            {
                object val = Enum.Parse(vd.ValueType, Token, true);
                dom.SetValue(vd.ValueName, val);
            }
            catch (Exception ex)
            {
                ThrowParserException(ex, DomMsgID.InvalidEnum, scanner.Token, vd.ValueName);
            }

            ReadCode();  // read next token
        }

        /// <summary>
        /// Parses the assignment to a struct (i.e. LeftPosition) l-value.
        /// </summary>
        private void ParseValueTypeAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            object val = vd.GetValue(dom, GV.ReadWrite);
            try
            {
                INullableValue ival = (INullableValue)val;
                ival.SetValue(Token);
                dom.SetValue(vd.ValueName, val);
                ReadCode();
            }
            catch (Exception ex)
            {
                ReportParserException(ex, DomMsgID.InvalidAssignment, vd.ValueName);
            }
        }

        /// <summary>
        /// Parses the assignment to a DocumentObject l-value.
        /// </summary>
        private void ParseDocumentObjectAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            // Create value if it does not exist
            object val = vd.GetValue(dom, GV.ReadWrite);
            DocumentObject docObj = (DocumentObject)val;

            try
            {
                if (Symbol == Symbol.Null)
                {
                    string name = vd.ValueName;
                    Type type = vd.ValueType;
                    if (typeof(Border) == type)
                        ((Border)val).Clear();
                    else if (typeof(Borders) == type)
                        ((Borders)val).ClearAll();
                    else if (typeof(Shading) == type)
                        ((Shading)val).Clear();
                    else if (typeof(TabStops) == type)
                    {
                        TabStops tabStops = (TabStops)vd.GetValue(dom, GV.ReadWrite);
                        tabStops.ClearAll();
                    }
                    else
                        ThrowParserException(DomMsgID.NullAssignmentNotSupported, vd.ValueName);

                    ReadCode();
                }
                else
                {
                    throw new Exception("Case: TopPosition");
                    //dom.SetValue(vd.ValueName, docObj);
                }
            }
            catch (Exception ex)
            {
                ReportParserException(ex, DomMsgID.InvalidAssignment, vd.ValueName);
            }
        }

        /// <summary>
        /// Parses the assignment to a Value l-value.
        /// </summary>
        private void ParseValueAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            try
            {
                // What ever it is, send it to SetValue.
                dom.SetValue(vd.ValueName, Token);
            }
            catch (Exception ex)
            {
                ThrowParserException(ex, DomMsgID.InvalidEnum, scanner.Token, vd.ValueName);
            }

            ReadCode();  // read next token
        }

        /// <summary>
        /// Parses the assignment to a Color l-value.
        /// </summary>
        private void ParseColorAssignment(DocumentObject dom, ValueDescriptor vd)
        {
            object val = vd.GetValue(dom, GV.ReadWrite);
            Color color = ParseColor();
            dom.SetValue(vd.ValueName, color);
        }

        /// <summary>
        /// Parses a color. It can be «green», «123456», «0xFFABCDEF», 
        /// «RGB(r, g, b)», «CMYK(c, m, y, k)», «CMYK(a, c, m, y, k)», «GRAY(g)», or «"MyColor"».
        /// </summary>
        private Color ParseColor()
        {
            MoveToCode();
            Color color = Color.Empty;
            if (Symbol == Symbol.Identifier)
            {
                switch (Token)
                {
                    case "RGB":
                        color = ParseRGB();
                        break;

                    case "CMYK":
                        color = ParseCMYK();
                        break;

                    case "HSB":
                        throw new NotImplementedException("ParseColor(HSB)");

                    case "Lab":
                        throw new NotImplementedException("ParseColor(Lab)");

                    case "GRAY":
                        color = ParseGray();
                        break;

                    default: // Must be color enum
                        try
                        {
                            color = Color.Parse(Token);
                            ReadCode();  // read token
                        }
                        catch (Exception ex)
                        {
                            ThrowParserException(ex, DomMsgID.InvalidColor, scanner.Token);
                        }
                        break;
                }
            }
            else if (Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral)
            {
                color = new Color(scanner.GetTokenValueAsUInt());
                ReadCode(); // read beyond literal
            }
            else if (Symbol == Symbol.StringLiteral)
            {
                throw new NotImplementedException("ParseColor(color-name)");
            }
            else
                ThrowParserException(DomMsgID.StringExpected, scanner.Token);
            return color;
        }

        /// <summary>
        /// Parses «RGB(r, g, b)».
        /// </summary>
        private Color ParseRGB()
        {
            uint r, g, b;
            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            ReadCode();  // read red value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral,
              DomMsgID.IntegerExpected, this.scanner.Token);
            r = this.scanner.GetTokenValueAsUInt();
            AssertCondition(r >= 0 && r <= 255, DomMsgID.InvalidRange, "0 - 255");

            ReadCode();  // read ','
            AssertSymbol(Symbol.Comma);

            ReadCode();  // read green value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral,
              DomMsgID.IntegerExpected, this.scanner.Token);
            g = this.scanner.GetTokenValueAsUInt();
            AssertCondition(g >= 0 && g <= 255, DomMsgID.InvalidRange, "0 - 255");

            ReadCode();  // read ','
            AssertSymbol(Symbol.Comma);

            ReadCode();  // read blue value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral,
              DomMsgID.IntegerExpected, this.scanner.Token);
            b = this.scanner.GetTokenValueAsUInt();
            AssertCondition(b >= 0 && b <= 255, DomMsgID.InvalidRange, "0 - 255");

            ReadCode();  // read ')'
            AssertSymbol(Symbol.ParenRight);

            ReadCode();  // read next token

            return new Color((uint)(0xFF000000 | (r << 16) | (g << 8) | b));
        }

        /// <summary>
        /// Parses «CMYK(c, m, y, k)» or «CMYK(a, c, m, y, k)».
        /// </summary>
        private Color ParseCMYK()
        {
            double v1, v2, v3, v4, v5 = 0;
            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            ReadCode();  // read v1 value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.RealLiteral,
              DomMsgID.NumberExpected, this.scanner.Token);
            v1 = this.scanner.GetTokenValueAsReal();
            AssertCondition(v1 >= 0.0f && v1 <= 100.0f, DomMsgID.InvalidRange, "0.0 - 100.0");

            ReadCode();  // read ','
            AssertSymbol(Symbol.Comma);

            ReadCode();  // read v2 value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.RealLiteral,
              DomMsgID.NumberExpected, this.scanner.Token);
            v2 = this.scanner.GetTokenValueAsReal();
            AssertCondition(v2 >= 0.0f && v2 <= 100.0f, DomMsgID.InvalidRange, "0.0 - 100.0");

            ReadCode();  // read ','
            AssertSymbol(Symbol.Comma);

            ReadCode();  // read v3 value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.RealLiteral,
              DomMsgID.NumberExpected, this.scanner.Token);
            v3 = this.scanner.GetTokenValueAsReal();
            AssertCondition(v3 >= 0.0f && v3 <= 100.0f, DomMsgID.InvalidRange, "0.0 - 100.0");

            ReadCode();  // read ','
            AssertSymbol(Symbol.Comma);

            ReadCode();  // read v4 value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.RealLiteral,
              DomMsgID.NumberExpected, this.scanner.Token);
            v4 = this.scanner.GetTokenValueAsReal();
            AssertCondition(v4 >= 0.0f && v4 <= 100.0, DomMsgID.InvalidRange, "0.0 - 100.0");

            ReadCode();  // read ')' or ','
            bool hasAlpha = false;
            if (Symbol == Symbol.Comma)
            {
                hasAlpha = true;
                ReadCode();  // read v5 value
                AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.RealLiteral,
                  DomMsgID.NumberExpected, this.scanner.Token);
                v5 = this.scanner.GetTokenValueAsReal();
                AssertCondition(v5 >= 0.0f && v5 <= 100.0, DomMsgID.InvalidRange, "0.0 - 100.0");

                ReadCode();  // read ')'
            }
            AssertSymbol(Symbol.ParenRight);

            ReadCode();  // read next token

            double a, c, m, y, k;
            if (hasAlpha)
            {
                a = v1; c = v2; m = v3; y = v4; k = v5;
            }
            else
            {
                a = 100.0; c = v1; m = v2; y = v3; k = v4;
            }
            return Color.FromCmyk(a, c, m, y, k);
        }

        /// <summary>
        /// Parses «GRAY(g)».
        /// </summary>
        private Color ParseGray()
        {
            double gray;
            ReadCode();  // read '('
            AssertSymbol(Symbol.ParenLeft);

            ReadCode();  // read gray value
            AssertCondition(Symbol == Symbol.IntegerLiteral || Symbol == Symbol.HexIntegerLiteral,
              DomMsgID.IntegerExpected, this.scanner.Token);
            gray = this.scanner.GetTokenValueAsReal();
            AssertCondition(gray >= 0.0f && gray <= 100.0f, DomMsgID.InvalidRange, "0.0 - 100.0");

            ReadCode();  // read ')'
            AssertSymbol(Symbol.ParenRight);

            ReadCode();  // read next token

            uint g = (uint)((1 - gray / 100.0) * 255 + 0.5);
            return new Color(0xff000000 + (g << 16) + (g << 8) + g);
        }

        /// <summary>
        /// Determines the name/text of the given symbol.
        /// </summary>
        private string GetSymbolText(Symbol docSym)
        {
            return KeyWords.NameFromSymbol(docSym);
        }

        /// <summary>
        /// Returns whether the specified type is a valid SpaceType.
        /// </summary>
        private bool IsSpaceType(string type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type == "")
                throw new ArgumentException("type");

            if (Enum.IsDefined(typeof(SymbolName), type))
            {
                SymbolName symbolName = (SymbolName)Enum.Parse(typeof(SymbolName), type); // symbols are case sensitive
                switch (symbolName)
                {
                    case SymbolName.Blank:
                    case SymbolName.Em:
                    //case SymbolName.Em4: // same as SymbolName.EmQuarter
                    case SymbolName.EmQuarter:
                    case SymbolName.En:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether the specified type is a valid enum for \symbol.
        /// </summary>
        private bool IsSymbolType(string type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type == "")
                throw new ArgumentException("type");

            if (Enum.IsDefined(typeof(SymbolName), type))
            {
                SymbolName symbolName = (SymbolName)Enum.Parse(typeof(SymbolName), type); // symbols are case sensitive
                switch (symbolName)
                {
                    case SymbolName.Euro:
                    case SymbolName.Copyright:
                    case SymbolName.Trademark:
                    case SymbolName.RegisteredTrademark:
                    case SymbolName.Bullet:
                    case SymbolName.Not:
                    case SymbolName.EmDash:
                    case SymbolName.EnDash:
                    case SymbolName.NonBreakableBlank:
                        //case SymbolName.HardBlank: //same as SymbolName.NonBreakableBlank:
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// If cond is evaluated to false, a DdlParserException with the specified error will be thrown.
        /// </summary>
        private void AssertCondition(bool cond, DomMsgID error, params object[] args)
        {
            if (!cond)
                ThrowParserException(error, args);
        }

        /// <summary>
        /// If current symbol is not equal symbol a DdlParserException will be thrown.
        /// </summary>
        private void AssertSymbol(Symbol symbol)
        {
            if (Symbol != symbol)
                ThrowParserException(DomMsgID.SymbolExpected, KeyWords.NameFromSymbol(symbol), Token);
        }

        /// <summary>
        /// If current symbol is not equal symbol a DdlParserException with the specified message id
        /// will be thrown.
        /// </summary>
        private void AssertSymbol(Symbol symbol, DomMsgID err)
        {
            if (Symbol != symbol)
                ThrowParserException(err, KeyWords.NameFromSymbol(symbol), Token);
        }

        /// <summary>
        /// If current symbol is not equal symbol a DdlParserException with the specified message id
        /// will be thrown.
        /// </summary>
        private void AssertSymbol(Symbol symbol, DomMsgID err, params object[] parms)
        {
            if (Symbol != symbol)
                ThrowParserException(err, KeyWords.NameFromSymbol(symbol), parms);
        }

        /// <summary>
        /// Creates an ErrorInfo based on the given errorlevel, error and parms and adds it to the ErrorManager2.
        /// </summary>
        private void ReportParserInfo(DdlErrorLevel level, DomMsgID errorCode, params string[] parms)
        {
            string message = DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(level, message, (int)errorCode,
              this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);

            this.errors.AddError(error);
        }

        /// <summary>
        /// Creates an ErrorInfo based on the given error and parms and adds it to the ErrorManager2.
        /// </summary>
        private void ReportParserException(DomMsgID error, params string[] parms)
        {
            ReportParserException(null, error, parms);
        }

        /// <summary>
        /// Adds the ErrorInfo from the ErrorInfoException2 to the ErrorManager2.
        /// </summary>
        private void ReportParserException(DdlParserException ex)
        {
            this.errors.AddError(ex.Error);
        }

        /// <summary>
        /// Creates an ErrorInfo based on the given inner exception, error and parms and adds it to the ErrorManager2.
        /// </summary>
        private void ReportParserException(Exception innerException, DomMsgID errorCode, params string[] parms)
        {
            string message = "";
            if (innerException != null)
                message = ": " + innerException.ToString();

            message += DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(DdlErrorLevel.Error, message, (int)errorCode,
              this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);

            this.errors.AddError(error);
        }

        /// <summary>
        /// Creates an ErrorInfo based on the DomMsgID and the specified parameters.
        /// Throws a DdlParserException with that ErrorInfo.
        /// </summary>
        private void ThrowParserException(DomMsgID errorCode, params object[] parms)
        {
            string message = DomSR.FormatMessage(errorCode, parms);
            DdlReaderError error = new DdlReaderError(DdlErrorLevel.Error, message, (int)errorCode,
              this.scanner.DocumentFileName, this.scanner.CurrentLine, this.scanner.CurrentLinePos);

            throw new DdlParserException(error);
        }

        /// <summary>
        /// Determines the error message based on the DomMsgID and the parameters.
        /// Throws a DdlParserException with that error message and the Exception as the inner exception.
        /// </summary>
        private void ThrowParserException(Exception innerException, DomMsgID errorCode, params object[] parms)
        {
            string message = DomSR.FormatMessage(errorCode, parms);
            throw new DdlParserException(message, innerException);
        }

        /// <summary>
        /// Used for exception handling. Sets the DDL stream to the next valid position behind
        /// the current block.
        /// </summary>
        private void AdjustToNextBlock()
        {
            bool skipClosingBraceOrBracket = (Symbol == Symbol.BraceLeft || Symbol == Symbol.BracketLeft);
            ReadCode();

            bool finish = false;
            while (!finish)
            {
                switch (Symbol)
                {
                    case Symbol.BraceLeft:
                    case Symbol.BracketLeft:
                        AdjustToNextBlock();
                        break;

                    case Symbol.BraceRight:
                    case Symbol.BracketRight:
                        if (skipClosingBraceOrBracket)
                            ReadCode();
                        finish = true;
                        break;

                    default:
                        AdjustToNextStatement();
                        break;
                }
            }
        }

        /// <summary>
        /// Used for exception handling. Sets the DDL stream to the next valid position behind
        /// the current statement.
        /// </summary>
        private void AdjustToNextStatement()
        {
            bool finish = false;
            while (!finish)
            {
                switch (Symbol)
                {
                    case Symbol.Assign:
                        //read one more symbol
                        ReadCode();
                        break;

                    default:
                        ReadCode();
                        finish = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Shortcut for scanner.ReadCode().
        /// Reads the next DDL token. Comments are ignored.
        /// </summary>
        private Symbol ReadCode()
        {
            return scanner.ReadCode();
        }

        /// <summary>
        /// Shortcut for scanner.ReadText().
        /// Reads either text or \keyword from current position.
        /// </summary>
        private Symbol ReadText(bool rootLevel)
        {
            return scanner.ReadText(rootLevel);
        }

        /// <summary>
        /// Shortcut for scanner.MoveToCode().
        /// Moves to the next DDL token if Symbol is not set to a valid position.
        /// </summary>
        private Symbol MoveToCode()
        {
            return scanner.MoveToCode();
        }

        /// <summary>
        /// Shortcut for scanner.MoveToParagraphContent().
        /// Moves to the first character the content of a paragraph starts with. Empty lines
        /// and comments are skipped. Returns true if such a character exists, and false if the
        /// paragraph ends without content.
        /// </summary>
        internal bool MoveToParagraphContent()
        {
            return scanner.MoveToParagraphContent();
        }

        /// <summary>
        /// Shortcut for scanner.MoveToNextParagraphContentLine().
        /// Moves to the first character of the content of a paragraph beyond an EOL. 
        /// Returns true if such a character exists and belongs to the current paragraph.
        /// Returns false if a new line (at root level) or '}' occurs. If a new line caused
        /// the end of the paragraph, the DDL cursor is moved to the next valid content
        /// character or '}' respectively.
        /// </summary>
        internal bool MoveToNextParagraphContentLine(bool rootLevel)
        {
            return scanner.MoveToNextParagraphContentLine(rootLevel);
        }

        /// <summary>
        /// Gets the current symbol from the scanner.
        /// </summary>
        private Symbol Symbol
        {
            get { return scanner.Symbol; }
        }

        /// <summary>
        /// Gets the current token from the scanner.
        /// </summary>
        private string Token
        {
            get { return scanner.Token; }
        }

        /// <summary>
        /// Gets the current token type from the scanner.
        /// </summary>
        private TokenType TokenType
        {
            get { return scanner.TokenType; }
        }

        private DdlScanner scanner;
        private DdlReaderErrors errors;
    }
}
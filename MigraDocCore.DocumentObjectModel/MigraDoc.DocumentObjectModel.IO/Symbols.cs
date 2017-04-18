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
using System.Diagnostics;
using System.Collections;

namespace MigraDocCore.DocumentObjectModel.IO
{
  internal class KeyWords
  {
    static KeyWords()
    {
      enumToName.Add(Symbol.True, "true");
      enumToName.Add(Symbol.False, "false");
      enumToName.Add(Symbol.Null, "null");

      enumToName.Add(Symbol.Styles, @"\styles");
      enumToName.Add(Symbol.Document, @"\document");
      enumToName.Add(Symbol.Section, @"\section");
      enumToName.Add(Symbol.Paragraph, @"\paragraph");
      enumToName.Add(Symbol.Header, @"\header");
      enumToName.Add(Symbol.Footer, @"\footer");
      enumToName.Add(Symbol.PrimaryHeader, @"\primaryheader");
      enumToName.Add(Symbol.PrimaryFooter, @"\primaryfooter");
      enumToName.Add(Symbol.FirstPageHeader, @"\firstpageheader");
      enumToName.Add(Symbol.FirstPageFooter, @"\firstpagefooter");
      enumToName.Add(Symbol.EvenPageHeader, @"\evenpageheader");
      enumToName.Add(Symbol.EvenPageFooter, @"\evenpagefooter");
      enumToName.Add(Symbol.Table, @"\table");
      enumToName.Add(Symbol.Columns, @"\columns");
      enumToName.Add(Symbol.Column, @"\column");
      enumToName.Add(Symbol.Rows, @"\rows");
      enumToName.Add(Symbol.Row, @"\row");
      enumToName.Add(Symbol.Cell, @"\cell");
      enumToName.Add(Symbol.Image, @"\image");
      enumToName.Add(Symbol.TextFrame, @"\textframe");
      enumToName.Add(Symbol.PageBreak, @"\pagebreak");
      enumToName.Add(Symbol.Barcode, @"\barcode");
      enumToName.Add(Symbol.Chart, @"\chart");
      enumToName.Add(Symbol.HeaderArea, @"\headerarea");
      enumToName.Add(Symbol.FooterArea, @"\footerarea");
      enumToName.Add(Symbol.TopArea, @"\toparea");
      enumToName.Add(Symbol.BottomArea, @"\bottomarea");
      enumToName.Add(Symbol.LeftArea, @"\leftarea");
      enumToName.Add(Symbol.RightArea, @"\rightarea");
      enumToName.Add(Symbol.PlotArea, @"\plotarea");
      enumToName.Add(Symbol.Legend, @"\legend");
      enumToName.Add(Symbol.XAxis, @"\xaxis");
      enumToName.Add(Symbol.YAxis, @"\yaxis");
      enumToName.Add(Symbol.ZAxis, @"\zaxis");
      enumToName.Add(Symbol.Series, @"\series");
      enumToName.Add(Symbol.XValues, @"\xvalues");
      enumToName.Add(Symbol.Point, @"\point");

      enumToName.Add(Symbol.Bold, @"\bold");
      enumToName.Add(Symbol.Italic, @"\italic");
      enumToName.Add(Symbol.Underline, @"\underline");
      enumToName.Add(Symbol.FontSize, @"\fontsize");
      enumToName.Add(Symbol.FontColor, @"\fontcolor");
      enumToName.Add(Symbol.Font, @"\font");
      //
      enumToName.Add(Symbol.Field, @"\field");
      enumToName.Add(Symbol.Symbol, @"\symbol");
      enumToName.Add(Symbol.Chr, @"\chr");
      //
      enumToName.Add(Symbol.Footnote, @"\footnote");
      enumToName.Add(Symbol.Hyperlink, @"\hyperlink");
      //
      enumToName.Add(Symbol.SoftHyphen, @"\-");
      enumToName.Add(Symbol.Tab, @"\tab");
      enumToName.Add(Symbol.LineBreak, @"\linebreak");
      enumToName.Add(Symbol.Space, @"\space");
      enumToName.Add(Symbol.NoSpace, @"\nospace");

      //
      //
      enumToName.Add(Symbol.BraceLeft, "{");
      enumToName.Add(Symbol.BraceRight, "}");
      enumToName.Add(Symbol.BracketLeft, "[");
      enumToName.Add(Symbol.BracketRight, "]");
      enumToName.Add(Symbol.ParenLeft, "(");
      enumToName.Add(Symbol.ParenRight, ")");
      enumToName.Add(Symbol.Colon, ":");
      enumToName.Add(Symbol.Semicolon, ";");  //??? id DDL?
      enumToName.Add(Symbol.Dot, ".");
      enumToName.Add(Symbol.Comma, ",");
      enumToName.Add(Symbol.Percent, "%");  //??? id DDL?
      enumToName.Add(Symbol.Dollar, "$");  //??? id DDL?
      //enumToName.Add(Symbol.At,                "@");
      enumToName.Add(Symbol.Hash, "#");  //??? id DDL?
      //enumToName.Add(Symbol.Question,          "?");  //??? id DDL?
      //enumToName.Add(Symbol.Bar,               "|");  //??? id DDL?
      enumToName.Add(Symbol.Assign, "=");
      enumToName.Add(Symbol.Slash, "/");  //??? id DDL?
      enumToName.Add(Symbol.BackSlash, "\\");
      enumToName.Add(Symbol.Plus, "+");  //??? id DDL?
      enumToName.Add(Symbol.PlusAssign, "+=");
      enumToName.Add(Symbol.Minus, "-");  //??? id DDL?
      enumToName.Add(Symbol.MinusAssign, "-=");
      enumToName.Add(Symbol.Blank, " ");

      //---------------------------------------------------------------
      //---------------------------------------------------------------
      //---------------------------------------------------------------

      nameToEnum.Add("true", Symbol.True);
      nameToEnum.Add("false", Symbol.False);
      nameToEnum.Add("null", Symbol.Null);
      //
      nameToEnum.Add(@"\styles", Symbol.Styles);
      nameToEnum.Add(@"\document", Symbol.Document);
      nameToEnum.Add(@"\section", Symbol.Section);
      nameToEnum.Add(@"\paragraph", Symbol.Paragraph);
      nameToEnum.Add(@"\header", Symbol.Header);
      nameToEnum.Add(@"\footer", Symbol.Footer);
      nameToEnum.Add(@"\primaryheader", Symbol.PrimaryHeader);
      nameToEnum.Add(@"\primaryfooter", Symbol.PrimaryFooter);
      nameToEnum.Add(@"\firstpageheader", Symbol.FirstPageHeader);
      nameToEnum.Add(@"\firstpagefooter", Symbol.FirstPageFooter);
      nameToEnum.Add(@"\evenpageheader", Symbol.EvenPageHeader);
      nameToEnum.Add(@"\evenpagefooter", Symbol.EvenPageFooter);
      nameToEnum.Add(@"\table", Symbol.Table);
      nameToEnum.Add(@"\columns", Symbol.Columns);
      nameToEnum.Add(@"\column", Symbol.Column);
      nameToEnum.Add(@"\rows", Symbol.Rows);
      nameToEnum.Add(@"\row", Symbol.Row);
      nameToEnum.Add(@"\cell", Symbol.Cell);
      nameToEnum.Add(@"\image", Symbol.Image);
      nameToEnum.Add(@"\textframe", Symbol.TextFrame);
      nameToEnum.Add(@"\pagebreak", Symbol.PageBreak);
      nameToEnum.Add(@"\barcode", Symbol.Barcode);
      nameToEnum.Add(@"\chart", Symbol.Chart);
      nameToEnum.Add(@"\headerarea", Symbol.HeaderArea);
      nameToEnum.Add(@"\footerarea", Symbol.FooterArea);
      nameToEnum.Add(@"\toparea", Symbol.TopArea);
      nameToEnum.Add(@"\bottomarea", Symbol.BottomArea);
      nameToEnum.Add(@"\leftarea", Symbol.LeftArea);
      nameToEnum.Add(@"\rightarea", Symbol.RightArea);
      nameToEnum.Add(@"\plotarea", Symbol.PlotArea);
      nameToEnum.Add(@"\legend", Symbol.Legend);
      nameToEnum.Add(@"\xaxis", Symbol.XAxis);
      nameToEnum.Add(@"\yaxis", Symbol.YAxis);
      nameToEnum.Add(@"\zaxis", Symbol.ZAxis);
      nameToEnum.Add(@"\series", Symbol.Series);
      nameToEnum.Add(@"\xvalues", Symbol.XValues);
      nameToEnum.Add(@"\point", Symbol.Point);
      nameToEnum.Add(@"\bold", Symbol.Bold);
      nameToEnum.Add(@"\italic", Symbol.Italic);
      nameToEnum.Add(@"\underline", Symbol.Underline);
      nameToEnum.Add(@"\fontsize", Symbol.FontSize);
      nameToEnum.Add(@"\fontcolor", Symbol.FontColor);
      nameToEnum.Add(@"\font", Symbol.Font);
      //
      nameToEnum.Add(@"\field", Symbol.Field);
      nameToEnum.Add(@"\symbol", Symbol.Symbol);
      nameToEnum.Add(@"\chr", Symbol.Chr);
      //
      nameToEnum.Add(@"\footnote", Symbol.Footnote);
      nameToEnum.Add(@"\hyperlink", Symbol.Hyperlink);
      //
      nameToEnum.Add(@"\-", Symbol.SoftHyphen); //??? \( ist auch was spezielles
      nameToEnum.Add(@"\tab", Symbol.Tab);
      nameToEnum.Add(@"\linebreak", Symbol.LineBreak);
      nameToEnum.Add(@"\space", Symbol.Space);
      nameToEnum.Add(@"\nospace", Symbol.NoSpace);
    }

    /// <summary>
    /// Returns Symbol value from name, or Symbol.None if no such Symbol exists.
    /// </summary>
    internal static Symbol SymbolFromName(string name)
    {
      Symbol docsym;
      object obj = nameToEnum[name];
      if (obj == null)
      {
        // Check for case sensitive keywords. Allow first character upper case only.
        if (string.Compare(name, "True", false) == 0)
          docsym = Symbol.True;
        else if (string.Compare(name, "False", false) == 0)
          docsym = Symbol.False;
        else if (string.Compare(name, "Null", false) == 0)
          docsym = Symbol.Null;
        else
          docsym = Symbol.None;
      }
      else
        docsym = (Symbol)obj;
      return docsym;
    }

    /// <summary>
    /// Returns string from Symbol value.
    /// </summary>
    internal static string NameFromSymbol(Symbol symbol)
    {
      object obj = enumToName[symbol];
      Debug.Assert(obj != null);
      string name = obj != null ? (string)obj : null;
      return name;
    }

    protected static Hashtable enumToName = new Hashtable();
    protected static Hashtable nameToEnum = new Hashtable();
  }
}

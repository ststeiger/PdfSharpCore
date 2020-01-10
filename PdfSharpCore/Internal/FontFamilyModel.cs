using System.Collections.Generic;
using PdfSharpCore.Drawing;

namespace PdfSharpCore.Internal
{
    public class FontFamilyModel
    {
        public string Name { get; set; }

        public Dictionary<XFontStyle, string> FontFiles = new Dictionary<XFontStyle, string>();

        public bool IsStyleAvailable(XFontStyle fontStyle)
        {
            return FontFiles.ContainsKey(fontStyle);
        }
    }
}

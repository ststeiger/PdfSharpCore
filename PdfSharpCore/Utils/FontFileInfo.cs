using PdfSharpCore.Drawing;
using SixLabors.Fonts;

namespace PdfSharpCore.Utils
{
    internal struct FontFileInfo
    {
        private FontFileInfo(string path, FontDescription fontDescription)
        {
            Path = path;
            FontDescription = fontDescription;
        }

        public string Path { get; }
        public FontDescription FontDescription { get; }
        public string FamilyName => FontDescription.FontFamilyInvariantCulture;

        public XFontStyle GuessFontStyle()
        {
            switch (FontDescription.Style)
            {
                case FontStyle.Bold:
                    return XFontStyle.Bold;
                case FontStyle.Italic:
                    return XFontStyle.Italic;
                case FontStyle.BoldItalic:
                    return XFontStyle.BoldItalic;
                default:
                    return XFontStyle.Regular;
            }
        }
        public static FontFileInfo Load(string path)
        {
            var fontDescription = FontDescription.LoadDescription(path);
            return new FontFileInfo(path, fontDescription);
        }
    }
}
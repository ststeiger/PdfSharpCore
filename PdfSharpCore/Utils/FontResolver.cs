
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Internal;
using SixLabors.Fonts;

namespace PdfSharpCore.Utils
{
    public class FontResolver
        : IFontResolver
    {
        public string DefaultFontName => "Arial";

        private static readonly Dictionary<string, FontFamilyModel> InstalledFonts = new Dictionary<string, FontFamilyModel>();

        private static readonly string[] SSupportedFonts;

        private static readonly string[] SSupportedFontNames;

        public FontResolver()
        {
        }

        static FontResolver()
        {
#if NETSTANDARD1_3
            string[] convertToNames(string[] paths)
            {
                var result = new string[paths.Length];
                for (int i = 0; i < paths.Length; i++)
                {
                    result[i] = Path.GetFileName(paths[i]).ToLower();
                }

                return result;
            }
#else
            string[] convertToNames(string[] paths) => Array.ConvertAll(paths, p => Path.GetFileName(p).ToLower());
#endif

            bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            if (isOSX)
            {
                string fontDir = "/Library/Fonts/";
                SSupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
                SSupportedFontNames = convertToNames(SSupportedFonts);
                SetupFontsFiles(SSupportedFonts);
                return;
            }

            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
            if (isLinux)
            {
                SSupportedFonts = LinuxSystemFontResolver.Resolve();
                SetupFontsFiles(SSupportedFonts);
                SSupportedFontNames = convertToNames(SSupportedFonts);
                return;
            }

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (isWindows)
            {
                string fontDir = System.Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Fonts");
                SSupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
                SetupFontsFiles(SSupportedFonts);
                SSupportedFontNames = convertToNames(SSupportedFonts);
                return;
            }

            throw new System.NotImplementedException("FontResolver not implemented for this platform (PdfSharpCore.Utils.FontResolver.cs).");
        }


        private readonly struct FontFileInfo
        {
            private FontFileInfo(string path, FontDescription fontDescription)
            {
                this.Path = path;
                this.FontDescription = fontDescription;
            }

            public string Path { get; }

            public FontDescription FontDescription { get; }

            public string FamilyName => this.FontDescription.FontFamilyInvariantCulture;


            public XFontStyle GuessFontStyle()
            {
                switch (this.FontDescription.Style)
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
                FontDescription fontDescription = FontDescription.LoadDescription(path);
                return new FontFileInfo(path, fontDescription);
            }
        }

        public static void SetupFontsFiles(string[] sSupportedFonts)
        {
            List<FontFileInfo> tempFontInfoList = new List<FontFileInfo>(sSupportedFonts.Length);
            foreach (string fontPathFile in sSupportedFonts)
            {
                try
                {
                    FontFileInfo fontInfo = FontFileInfo.Load(fontPathFile);
                    Debug.WriteLine(fontPathFile);
                    tempFontInfoList.Add(fontInfo);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine(e);
                }
            }

            // Deserialize all font families
            foreach (IGrouping<string, FontFileInfo> familyGroup in tempFontInfoList.GroupBy(info => info.FamilyName))
                try
                {
                    string familyName = familyGroup.Key;
                    FontFamilyModel family = DeserializeFontFamily(familyName, familyGroup);
                    InstalledFonts.Add(familyName.ToLower(), family);
                }
                catch (System.Exception e)
                {
                    System.Console.Error.WriteLine(e);
                }
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static FontFamilyModel DeserializeFontFamily(string fontFamilyName, IEnumerable<FontFileInfo> fontList)
        {
            FontFamilyModel font = new FontFamilyModel { Name = fontFamilyName };

            // there is only one font
            if (fontList.Count() == 1)
                font.FontFiles.Add(XFontStyle.Regular, fontList.First().Path);
            else
            {
                foreach (FontFileInfo info in fontList)
                {
                    XFontStyle style = info.GuessFontStyle();
                    if (!font.FontFiles.ContainsKey(style))
                        font.FontFiles.Add(style, info.Path);
                }
            }

            return font;
        }

        public byte[] GetFont(string faceFileName)
        {
            string ttfPathFile = "";
            try
            {
                var name = Path.GetFileName(faceFileName).ToLower();
                int index = Array.IndexOf(SSupportedFontNames, name);
                if (index > -1)
                {
                    ttfPathFile = SSupportedFonts[index];
                    return File.ReadAllBytes(ttfPathFile);
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }

            throw new System.Exception("No Font File Found - " + faceFileName + " - " + ttfPathFile);
        }

        public bool NullIfFontNotFound { get; set; } = false;

        public virtual FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (InstalledFonts.Count == 0)
                throw new System.IO.FileNotFoundException("No Fonts installed on this device!");

            if (InstalledFonts.TryGetValue(familyName.ToLower(), out FontFamilyModel family))
            {
                if (isBold && isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.BoldItalic, out string boldItalicFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(boldItalicFile));
                }
                else if (isBold)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Bold, out string boldFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(boldFile));
                }
                else if (isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Italic, out string italicFile))
                        return new FontResolverInfo(System.IO.Path.GetFileName(italicFile));
                }

                if (family.FontFiles.TryGetValue(XFontStyle.Regular, out string regularFile))
                    return new FontResolverInfo(System.IO.Path.GetFileName(regularFile));

                return new FontResolverInfo(System.IO.Path.GetFileName(family.FontFiles.First().Value));
            }

            if (NullIfFontNotFound)
                return null;

            string ttfFile = InstalledFonts.First().Value.FontFiles.First().Value;
            return new FontResolverInfo(System.IO.Path.GetFileName(ttfFile));
        }
    }
}

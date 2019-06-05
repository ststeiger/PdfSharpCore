using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharpCore.Fonts;

namespace PdfSharpCore.Utils
{
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Calibri";

        private static string[] s_SupportedFonts;

        private readonly Dictionary<string, string> fontfamilyTofontfaceMap;
        private static Dictionary<string, int> fontfaceTofontfileMap;

        public FontResolver()
        {
            fontfamilyTofontfaceMap = new Dictionary<string, string>();
        }

        public static void SetupFontsFiles()
        {
            int numFonts = s_SupportedFonts.Length;
            fontfaceTofontfileMap = new Dictionary<string, int>();

            for (int i = 0; i < numFonts; ++i)
                fontfaceTofontfileMap.Add(Path.GetFileName(s_SupportedFonts[i]), i);
        }

        static FontResolver()
        {
            bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            if (isOSX)
            {
                string fontDir = "/Library/Fonts/";
                s_SupportedFonts = Directory.GetFiles(fontDir, "*.ttf", SearchOption.AllDirectories);
                SetupFontsFiles();
                return;
            }
            
            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
            if (isLinux)
            {
                string fontDir = "/usr/share/fonts/truetype/";
                s_SupportedFonts = Directory.GetFiles(fontDir, "*.ttf", SearchOption.AllDirectories);
                SetupFontsFiles();
                return;
            }
            
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (isWindows)
            {
                string fontDir = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Fonts");
                s_SupportedFonts = Directory.GetFiles(fontDir, "*.ttf", SearchOption.AllDirectories);
                SetupFontsFiles();
                return;
            }
            
            throw new NotImplementedException("FontResolver not implemented for this platform (PdfSharpCore.Utils.FontResolver.cs).");
        }

        public byte[] GetFont(string faceName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                string ttfFile;

                if (fontfaceTofontfileMap.TryGetValue(faceName, out int idx))
                    ttfFile = s_SupportedFonts[idx];
                else if (s_SupportedFonts.Length > 0)
                    ttfFile = s_SupportedFonts[0];
                else
                    throw new System.Exception("No Font Files Found");

                using (System.IO.FileStream ttf = System.IO.File.OpenRead(ttfFile))
                {
                    ttf.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string ttfFile = fontfaceTofontfileMap.Keys.First(),
                tf, fontkey;

            familyName = familyName.ToLower();

            fontkey = familyName;
            if (isBold) fontkey += "b";
            if (isItalic) fontkey += "i";

            if (!fontfamilyTofontfaceMap.TryGetValue(fontkey, out ttfFile))
            {
                foreach (string fontfile in fontfaceTofontfileMap.Keys)
                {
                    tf = Path.GetFileNameWithoutExtension(fontfile).ToLower().TrimEnd('-', '_');

                    if (tf.Contains(familyName))
                    {
                        ttfFile = fontfile;

                        if (isBold && isItalic)
                        {
                            if ((tf.Contains("bold") && tf.Contains("italic")) || (tf.EndsWith("bi", StringComparison.Ordinal) || tf.EndsWith("ib", StringComparison.Ordinal)))
                            {
                                ttfFile = fontfile;
                                break;
                            }
                        }
                        else if (isBold)
                        {
                            if (tf.Contains("bold") || tf.EndsWith("b", StringComparison.Ordinal) || tf.EndsWith("bd", StringComparison.Ordinal))
                            {
                                ttfFile = fontfile;
                                break;
                            }
                        }
                        else if (isItalic)
                        {
                            if (tf.Contains("italic") || tf.EndsWith("i", StringComparison.Ordinal) || tf.EndsWith("ib", StringComparison.Ordinal))
                            {
                                ttfFile = fontfile;
                                break;
                            }
                        }
                        else
                        {
                            //We found a match on this font and did not want bold or italic.
                            //This is not guaranteed to always be correct, but the first matching key is usually the normal variant.
                            break;
                        }
                    }
                }

                fontfamilyTofontfaceMap.Add(fontkey, ttfFile);
            }

            if (ttfFile == null) ttfFile = fontfaceTofontfileMap.Keys.First();
            return new FontResolverInfo(ttfFile);
        }
    }
}

using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Internal;
using SixLabors.Fonts;

namespace PdfSharpCore.Utils
{
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        private static readonly Dictionary<string, FontFamilyModel> InstalledFonts = new Dictionary<string, FontFamilyModel>();

        private static readonly string[] SSupportedFonts;

        public FontResolver()
        {
        }

        static FontResolver()
        {
            string fontDir;

            bool isOSX = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            if (isOSX)
            {
                fontDir = "/Library/Fonts/";
                SSupportedFonts = Directory.GetFiles(fontDir, "*.ttf", SearchOption.AllDirectories);
                SetupFontsFiles(SSupportedFonts);
                return;
            }

            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
            if (isLinux)
            {
                SSupportedFonts = ResolveLinuxFontFiles();
                SetupFontsFiles(SSupportedFonts);
                return;
            }

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (isWindows)
            {
                fontDir = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Fonts");
                SSupportedFonts = Directory.GetFiles(fontDir, "*.ttf", SearchOption.AllDirectories);
                SetupFontsFiles(SSupportedFonts);
                return;
            }

            throw new NotImplementedException("FontResolver not implemented for this platform (PdfSharpCore.Utils.FontResolver.cs).");
        }

        static string[] ResolveLinuxFontFiles()
        {
            var fontList = new List<string>();
            var confRegex = new Regex("<dir>(?<dir>.*)</dir>", RegexOptions.Compiled);
            var ttfRegex = new Regex(@"\.ttf", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            using (var reader = new StreamReader(File.OpenRead("/etc/fonts/fonts.conf")))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = confRegex.Match(line);
                    if (!match.Success) continue;

                    var path = match.Groups["dir"].Value.Replace("~", Environment.GetEnvironmentVariable("HOME"));
                    if (!Directory.Exists(path)) continue;

                    foreach (var enumerateDirectory in Directory.EnumerateDirectories(path))
                    {
                        var pathFont = Path.Combine(path, enumerateDirectory);

                        foreach (var strDir in Directory.EnumerateDirectories(pathFont))
                        {
                            fontList.AddRange(Directory.EnumerateFiles(strDir)
                                .Where(x => ttfRegex.IsMatch(x)));
                        }
                    }
                }
            }

            return fontList.ToArray();
        }

        public static void SetupFontsFiles(string[] sSupportedFonts)
        {
            // First group all fonts to font families
            Dictionary<string, List<string>> tmpFontFamiliesTtfFilesDict = new Dictionary<string, List<string>>();
            foreach (var fontPathFile in sSupportedFonts)
            {
                try
                {
                    FontDescription fontDescription = FontDescription.LoadDescription(fontPathFile);
                    string fontFamilyName = fontDescription.FontFamily(CultureInfo.InvariantCulture);
                    Debug.WriteLine(fontPathFile);

                    if (tmpFontFamiliesTtfFilesDict.TryGetValue(fontFamilyName, out List<string> familyTtfFiles))
                        familyTtfFiles.Add(fontPathFile);
                    else
                        tmpFontFamiliesTtfFilesDict.Add(fontFamilyName, new List<string> { fontPathFile });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // Deserialize all font families
            foreach (var fontFamily in tmpFontFamiliesTtfFilesDict)
                try
                {
                    InstalledFonts.Add(fontFamily.Key.ToLower(), DeserializeFontFamily(fontFamily));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static FontFamilyModel DeserializeFontFamily(KeyValuePair<string, List<string>> fontFamily)
        {
            var font = new FontFamilyModel { Name = fontFamily.Key };

            var fontList = fontFamily.Value;

            // there is only one font
            if (fontFamily.Value.Count == 1)
            {
                font.FontFiles.Add(XFontStyle.Regular, fontFamily.Value[0]);
                return font;
            }

            // if element filenames have diff. lengths -> shortest name is regular
            // skip this check if Regular font has 'regular' sufix, because 'fontName-bold' shorter that 'fontName-regular' 
            if (fontList.Any(e => e.Length != fontList[0].Length)
                && fontList.All(fontFileName => !Path.GetFileNameWithoutExtension(fontFileName)?.ToLower().Contains("regular") ?? true))
            {
                var orderedList = fontList.OrderBy(o => o.Length);
                font.FontFiles.Add(XFontStyle.Regular, orderedList.First());

                foreach (var elem in orderedList.Skip(1))
                {
                    var pair = DeserializeFontName(elem);
                    if (!font.FontFiles.ContainsKey(pair.Key))
                        font.FontFiles.Add(pair.Key, pair.Value);
                }

                return font;
            }

            // else
            foreach (var elem in fontList)
            {
                var pair = DeserializeFontName(elem);
                if (!font.FontFiles.ContainsKey(pair.Key))
                    font.FontFiles.Add(pair.Key, pair.Value);
            }
            return font;
        }

        private static KeyValuePair<XFontStyle, string> DeserializeFontName(string fontFileName)
        {
            var tf = Path.GetFileNameWithoutExtension(fontFileName)?.ToLower().TrimEnd('-', '_');
            if (tf == null)
                return new KeyValuePair<XFontStyle, string>(XFontStyle.Regular, null);

            // bold italic
            if (tf.Contains("bold") && tf.Contains("italic") ||
                tf.EndsWith("bi", StringComparison.Ordinal) ||
                tf.EndsWith("ib", StringComparison.Ordinal) ||
                tf.EndsWith("z", StringComparison.Ordinal))
                return new KeyValuePair<XFontStyle, string>(XFontStyle.BoldItalic, fontFileName);
            // bold
            if (tf.Contains("bold") || tf.EndsWith("b", StringComparison.Ordinal) ||
                tf.EndsWith("bd", StringComparison.Ordinal))
                return new KeyValuePair<XFontStyle, string>(XFontStyle.Bold, fontFileName);
            // italic
            if (tf.Contains("italic") || tf.EndsWith("i", StringComparison.Ordinal) ||
                tf.EndsWith("ib", StringComparison.Ordinal))
                return new KeyValuePair<XFontStyle, string>(XFontStyle.Italic, fontFileName);

            //We found a match on this font and did not want bold or italic.
            //This is not guaranteed to always be correct, but the first matching key is usually the normal variant.
            return new KeyValuePair<XFontStyle, string>(XFontStyle.Regular, fontFileName);
        }


        public byte[] GetFont(string faceFileName)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                string ttfPathFile = "";
                try
                {
                    ttfPathFile = SSupportedFonts.ToList().First(x => x.ToLower().Contains(Path.GetFileName(faceFileName).ToLower()));
                    using (var ttf = File.OpenRead(ttfPathFile))
                    {
                        ttf.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw new Exception("No Font File Found - " + faceFileName + " - " + ttfPathFile);
                }
            }
        }

        public bool NullIfFontNotFound { get; set; } = false;

        public virtual FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (InstalledFonts.Count == 0)
                throw new FileNotFoundException("No Fonts installed on this device!");

            if (InstalledFonts.TryGetValue(familyName.ToLower(), out var family))
            {
                if (isBold && isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.BoldItalic, out string boldItalicFile))
                        return new FontResolverInfo(Path.GetFileName(boldItalicFile));
                }
                else if (isBold)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Bold, out string boldFile))
                        return new FontResolverInfo(Path.GetFileName(boldFile));
                }
                else if (isItalic)
                {
                    if (family.FontFiles.TryGetValue(XFontStyle.Italic, out string italicFile))
                        return new FontResolverInfo(Path.GetFileName(italicFile));
                }

                if (family.FontFiles.TryGetValue(XFontStyle.Regular, out string regularFile))
                    return new FontResolverInfo(Path.GetFileName(regularFile));

                return new FontResolverInfo(Path.GetFileName(family.FontFiles.First().Value));
            }

            if (NullIfFontNotFound)
                return null;

            string ttfFile = InstalledFonts.First().Value.FontFiles.First().Value;
            return new FontResolverInfo(Path.GetFileName(ttfFile));
        }
    }
}

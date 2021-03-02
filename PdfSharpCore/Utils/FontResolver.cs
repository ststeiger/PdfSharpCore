using System.Text.RegularExpressions;
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
                var fontDescription = FontDescription.LoadDescription(path);
                return new FontFileInfo(path, fontDescription);
            }
        }


        public static void SetupFontsFiles(string[] sSupportedFonts)
        {
            var tempFontInfoList = new List<FontFileInfo>();
            foreach (var fontPathFile in sSupportedFonts)
            {
                try
                {
                    var fontInfo = FontFileInfo.Load(fontPathFile);
                    Debug.WriteLine(fontPathFile);
                    tempFontInfoList.Add(fontInfo);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }

            // Deserialize all font families
            foreach (var familyGroup in tempFontInfoList.GroupBy(info => info.FamilyName))
                try
                {
                    var familyName = familyGroup.Key;
                    var family = DeserializeFontFamily(familyName, familyGroup);
                    InstalledFonts.Add(familyName.ToLower(), family);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static FontFamilyModel DeserializeFontFamily(string fontFamilyName, IEnumerable<FontFileInfo> fontList)
        {
            var font = new FontFamilyModel { Name = fontFamilyName };

            // there is only one font
            if (fontList.Count() == 1)
                font.FontFiles.Add(XFontStyle.Regular, fontList.First().Path);
            else
            {
                foreach (var info in fontList)
                {
                    var style = info.GuessFontStyle();
                    if (!font.FontFiles.ContainsKey(style))
                        font.FontFiles.Add(style, info.Path);
                }
            }

            return font;
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

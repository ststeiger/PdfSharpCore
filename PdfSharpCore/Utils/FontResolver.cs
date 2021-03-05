
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using PdfSharpCore.Internal;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;

using SixLabors.Fonts;


namespace PdfSharpCore.Utils
{


    public class FontResolver 
        : IFontResolver
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
                SSupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
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
                fontDir = System.Environment.ExpandEnvironmentVariables(@"%SystemRoot%\Fonts");
                SSupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
                SetupFontsFiles(SSupportedFonts);
                return;
            }

            throw new System.NotImplementedException("FontResolver not implemented for this platform (PdfSharpCore.Utils.FontResolver.cs).");
        }


        private static string[] ResolveLinuxFontFiles()
        {
            List<string> fontList = new List<string>();
            Regex confRegex = new Regex("<dir>(?<dir>.*)</dir>", RegexOptions.Compiled);
            HashSet<string> hs = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            try
            {
                using (System.IO.TextReader reader = new System.IO.StreamReader(
                    System.IO.File.OpenRead("/etc/fonts/fonts.conf")))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match match = confRegex.Match(line);
                        if (!match.Success)
                            continue;

                        string path = match.Groups["dir"].Value.Trim();
                        if (path.StartsWith("~"))
                        {
                            path = System.Environment.GetEnvironmentVariable("HOME") + path.Substring(1);
                        }

                        if (!hs.Contains(path))
                            hs.Add(path);

                        AddFontsToFontList(path, fontList);
                    } // Whend 

                } // End Using reader 
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.WriteLine(ex.StackTrace);
            }

            string shareFonts = "/usr/share/fonts";
            if (!hs.Contains(shareFonts))
                AddFontsToFontList(shareFonts, fontList);

            string localshareFonts = "/usr/local/share/fonts";
            if (!hs.Contains(localshareFonts))
                AddFontsToFontList(localshareFonts, fontList);

            string homeFonts = System.Environment.GetEnvironmentVariable("HOME") + "/.fonts";
            if (!hs.Contains(homeFonts))
                AddFontsToFontList(homeFonts, fontList);

            return fontList.ToArray();
        } // End Function ResolveLinuxFontFiles 


        private static void AddFontsToFontList(string path, List<string> fontList)
        {
            if (!System.IO.Directory.Exists(path))
                return;

            foreach (string enumerateDirectory in System.IO.Directory.EnumerateDirectories(
                path,
                "*.*",
                System.IO.SearchOption.AllDirectories))
            {
                string pathFont = System.IO.Path.Combine(path, enumerateDirectory);

                foreach (string strDir in System.IO.Directory.EnumerateDirectories(
                    pathFont,
                    "*.*",
                    System.IO.SearchOption.AllDirectories
                    ))
                {
                    fontList.AddRange(System.IO.Directory
                        .EnumerateFiles(strDir, "*.*", System.IO.SearchOption.AllDirectories)
                        .Where(x => x.EndsWith(".ttf", System.StringComparison.OrdinalIgnoreCase)
                        )
                    );
                } // Next strDir 

            } // Next enumerateDirectory 

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
            List<FontFileInfo> tempFontInfoList = new List<FontFileInfo>();
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
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                string ttfPathFile = "";
                try
                {
                    ttfPathFile = SSupportedFonts.ToList().First(x => x.ToLower().Contains(
                        System.IO.Path.GetFileName(faceFileName).ToLower())
                    );

                    using (System.IO.Stream ttf = System.IO.File.OpenRead(ttfPathFile))
                    {
                        ttf.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e);
                    throw new System.Exception("No Font File Found - " + faceFileName + " - " + ttfPathFile);
                }
            }
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

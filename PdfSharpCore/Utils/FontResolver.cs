using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PdfSharpCore.Utils
{
    public class FontResolver : IFontResolver
    {
        private static readonly List<FontFamilyModel> installedFonts = new List<FontFamilyModel>();
        private static readonly string[] installedFontFilePaths;

        static FontResolver()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                installedFontFilePaths = LoadWindowsFonts();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                installedFontFilePaths = LoadOSXFonts();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                installedFontFilePaths = LinuxSystemFontResolver.Resolve();
            }
            else
            {
                throw new NotImplementedException($"FontResolver not implemented for this platform (PdfSharpCore.Utils.FontResolver.cs).");
            }

            SetupFontFiles();
        }

        public string DefaultFontName => "Arial";
        public bool NullIfFontNotFound { get; set; } = false;

        public virtual byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                string ttfPathFile = "";
                try
                {
                    ttfPathFile = installedFontFilePaths
                        .ToList()
                        .First(x => x.ToLower().Contains(Path.GetFileName(faceName).ToLower()));

                    if (File.Exists(ttfPathFile))
                    {
                        using (var fileStream = File.OpenRead(ttfPathFile))
                        {
                            fileStream.CopyTo(ms);
                            ms.Position = 0;
                            return ms.ToArray();
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(ttfPathFile);
                    }
                }
                catch (Exception)
                {
                    throw new Exception($"Font file with name {faceName} and path {ttfPathFile} not found.");
                }
            }
        }
        public virtual FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (installedFonts.Count == 0)
            {
                throw new FileNotFoundException("No Fonts installed on this device!");
            }

            var fontFamily = installedFonts.Find(x => x.Name.ToLower().Equals(familyName.ToLower()));
            if (fontFamily != null)
            {
                if (isBold && isItalic)
                {
                    if (fontFamily.FontFiles.TryGetValue(XFontStyle.BoldItalic, out string boldItalicFile))
                    {
                        return new FontResolverInfo(Path.GetFileName(boldItalicFile));
                    }
                }
                else if (isBold)
                {
                    if (fontFamily.FontFiles.TryGetValue(XFontStyle.Bold, out string boldFile))
                    {
                        return new FontResolverInfo(Path.GetFileName(boldFile));
                    }
                }
                else if (isItalic)
                {
                    if (fontFamily.FontFiles.TryGetValue(XFontStyle.Italic, out string italicFile))
                    {
                        return new FontResolverInfo(Path.GetFileName(italicFile));
                    }
                }

                if (fontFamily.FontFiles.TryGetValue(XFontStyle.Regular, out string regularFile))
                {
                    return new FontResolverInfo(Path.GetFileName(regularFile));
                }

                return new FontResolverInfo(Path.GetFileName(fontFamily.FontFiles.First().Value));
            }

            if (NullIfFontNotFound)
            {
                return null;
            }

            var firstFontPath = installedFonts.First().FontFiles.First().Value;
            return new FontResolverInfo(Path.GetFileName(firstFontPath));
        }

        private static string[] LoadOSXFonts()
        {
            var fontDirectory = "/Library/Fonts/";
            return Directory.GetFiles(fontDirectory, "*.ttf", SearchOption.AllDirectories);
        }
        private static string[] LoadWindowsFonts()
        {
            var fontPaths = new List<string>();

            var systemFontPath = @"%SystemRoot%\Fonts";
            var localAppDataFontPath = @"%LOCALAPPDATA%\Microsoft\Windows\Fonts";

            var systemFontDirectory = Environment.ExpandEnvironmentVariables(systemFontPath);
            var systemFontPaths = Directory.GetFiles(systemFontDirectory, "*.ttf", SearchOption.AllDirectories);
            fontPaths.AddRange(systemFontPaths);

            var appdataFontDirectory = Environment.ExpandEnvironmentVariables(localAppDataFontPath);
            if (Directory.Exists(appdataFontDirectory))
            {
                var appdataFontPaths = Directory.GetFiles(appdataFontDirectory, "*.ttf", SearchOption.AllDirectories);
                fontPaths.AddRange(appdataFontPaths);
            }

            return fontPaths.ToArray();
        }
        private static void SetupFontFiles()
        {
            var fontFiles = new List<FontFileInfo>();
            foreach (var fontFilePath in installedFontFilePaths)
            {
                try
                {
                    var fontFileInfo = FontFileInfo.Load(fontFilePath);
                    fontFiles.Add(fontFileInfo);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }

            foreach (var familyGroup in fontFiles.GroupBy(info => info.FamilyName))
            {
                try
                {
                    var familyName = familyGroup.Key;
                    FontFamilyModel family = DeserializeFontFamily(familyName, familyGroup);
                    installedFonts.Add(family);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private static FontFamilyModel DeserializeFontFamily(string familyName, IEnumerable<FontFileInfo> fontFiles)
        {
            var fontFamilyModel = new FontFamilyModel
            {
                Name = familyName
            };

            if (fontFiles.Count() == 1)
            {
                fontFamilyModel.FontFiles.Add(XFontStyle.Regular, fontFiles.First().Path);
            }
            else
            {
                foreach (var info in fontFiles)
                {
                    var style = info.GuessFontStyle();
                    if (!fontFamilyModel.FontFiles.ContainsKey(style))
                    {
                        fontFamilyModel.FontFiles.Add(style, info.Path);
                    }
                }
            }

            return fontFamilyModel;
        }
    }
}

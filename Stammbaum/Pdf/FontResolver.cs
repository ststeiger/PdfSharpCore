
namespace Stammbaum
{

    class FontResolver : PdfSharpCore.Fonts.IFontResolver
    {
        public string DefaultFontName => "Tinos";

        public static string[] s_SupportedFonts;
        public static string[] s_SupportedFontsFiles;


        public static void SetupFontsFiles()
        {
            int numFonts = s_SupportedFonts.Length;
            s_SupportedFontsFiles = new string[numFonts];
            for (int i = 0; i < numFonts; ++i)
            {
                s_SupportedFontsFiles[i] = System.IO.Path.GetFileName(s_SupportedFonts[i]);
            } // Next i 

        } // End Sub SetupFontsFiles 


        static FontResolver()
        {
            bool isLinux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
            if (isLinux)
            {
                string fontDir = "/usr/share/fonts/truetype/";
                s_SupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
                SetupFontsFiles();
                return;
            }

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (isWindows)
            {
                string fontDir = @"C:\Windows\Fonts";
                s_SupportedFonts= System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
                SetupFontsFiles();
                return;
            }

            //bool isMacOs = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            //if (isMacOs)
            //{
            //    string fontDir = "/usr/share/fonts/truetype/";
            //    s_SupportedFonts = System.IO.Directory.GetFiles(fontDir, "*.ttf", System.IO.SearchOption.AllDirectories);
            //    SetupFontsFiles();
            //    return;
            //}

            throw new System.NotImplementedException("FontResolver not implemented for this platform.");
        } // End Static Constructor 


        public byte[] GetFont(string faceName)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                string ttfFile = s_SupportedFonts[0];

                using (System.IO.FileStream ttf = System.IO.File.OpenRead(ttfFile))
                {
                    ttf.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }

            }
        }

        public PdfSharpCore.Fonts.FontResolverInfo
            ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            string ttfFile = s_SupportedFontsFiles[0];
            return new PdfSharpCore.Fonts.FontResolverInfo(ttfFile);

            if (familyName.Equals("Tinos", System.StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new PdfSharpCore.Fonts.FontResolverInfo("Tinos-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new PdfSharpCore.Fonts.FontResolverInfo("Tinos-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new PdfSharpCore.Fonts.FontResolverInfo("Tinos-Italic.ttf");
                }
                else
                {
                    return new PdfSharpCore.Fonts.FontResolverInfo("Tinos-Regular.ttf");
                }
            }
            return null;
        }
    }


}

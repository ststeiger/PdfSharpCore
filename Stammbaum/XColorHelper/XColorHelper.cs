
using System;


namespace Stammbaum
{


    public class XColorHelper
    {


        // StaticConvertFromString
        // https://github.com/mono/mono/blob/master/mcs/class/System.Drawing/System.Drawing/ColorConverter.cs
        public static PdfSharpCore.Drawing.XColor FromHtml(string htmlColor)
        {
            if (string.IsNullOrEmpty(htmlColor))
            {
                // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.Color.Empty);
                return System.Drawing.Color.Empty;
            }

            if (htmlColor.StartsWith("#"))
            {
                htmlColor = htmlColor.Substring(1);

                if (htmlColor.Length == 3)
                {
                    string R = htmlColor.Substring(0, 1);
                    string G = htmlColor.Substring(1, 1);
                    string B = htmlColor.Substring(2, 1);
                    htmlColor = System.Convert.ToString(System.Convert.ToString(System.Convert.ToString(System.Convert.ToString(R + R) + G) + G) + B) + B;
                }
                // (htmlColor.Length == 3)

                if (htmlColor.Length == 6)
                {
                    string sR = htmlColor.Substring(0, 2);
                    string sG = htmlColor.Substring(2, 2);
                    string sB = htmlColor.Substring(4, 2);

                    byte R = System.Convert.ToByte(sR, 16);
                    byte G = System.Convert.ToByte(sG, 16);
                    byte B = System.Convert.ToByte(sB, 16);

                    PdfSharpCore.Drawing.XColor xc = new PdfSharpCore.Drawing.XColor();
                    xc.R = R;
                    xc.G = G;
                    xc.B = B;
                    xc.A = 1.0;
                    return xc;
                }
                // (htmlColor.Length == 6)

                throw new ArgumentException("Invalid HTML color.");
            }
            // (htmlColor.StartsWith("#"))

            switch (htmlColor.ToLowerInvariant())
            {
                case "buttonface":
                case "threedface":
                    //return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.Control);
                    return System.Drawing.SystemColors.Control;
                case "buttonhighlight":
                case "threedlightshadow":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ControlLightLight);
                    return System.Drawing.SystemColors.ControlLightLight;
                case "buttonshadow":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ControlDark);
                    return System.Drawing.SystemColors.ControlDark;
                case "captiontext":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ActiveCaptionText);
                    return System.Drawing.SystemColors.ActiveCaptionText;
                case "threeddarkshadow":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ControlDarkDark);
                    return System.Drawing.SystemColors.ControlDarkDark;
                case "threedhighlight":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ControlLight);
                    return System.Drawing.SystemColors.ControlLight;
                case "background":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.Desktop);
                    return System.Drawing.SystemColors.Desktop;
                case "buttontext":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.ControlText);
                    return System.Drawing.SystemColors.ControlText;
                case "infobackground":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.SystemColors.Info);
                    return System.Drawing.SystemColors.Info;
                // special case for Color.LightGray versus html's LightGrey (#340917)
                case "lightgrey":
                    // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.Color.LightGray);
                    return System.Drawing.Color.LightGray;
            } // End Switch (htmlColor.ToLowerInvariant())

            
            PdfSharpCore.Drawing.XKnownColor kk = default(PdfSharpCore.Drawing.XKnownColor);
            if (TryEnumParse<PdfSharpCore.Drawing.XKnownColor>(htmlColor, true, ref kk))
            {
                return PdfSharpCore.Drawing.XColor.FromKnownColor(kk);
            }

            // return PdfSharpCore.Drawing.XColor.FromArgb(System.Drawing.Color.Empty);
            return System.Drawing.Color.Empty;
        } // FromHtml


        // .NET 2.0 BackPort
        public static bool TryEnumParse<T>(string Value, bool CaseSens, ref T kk)
        {
            try
            {
                kk = (T)Enum.Parse(typeof(T), Value, true);
                return true;
            }
            catch
            {
            }

            kk = default(T);
            return false;
        } // TryEnumParse


    } // End Class XColorHelper


} // End Namespace Stammbaum

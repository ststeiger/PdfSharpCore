#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharpCore.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

#if __IOS__
using ObjCRuntime;
using UIKit;
using CoreGraphics;
#endif

#if CORE || GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiFontFamily = System.Drawing.FontFamily;
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
#if WPF
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfTypeface = System.Windows.Media.Typeface;
using WpfGlyphTypeface = System.Windows.Media.GlyphTypeface;
using WpfStyleSimulations = System.Windows.Media.StyleSimulations;
#endif
using PdfSharpCore.Drawing;

#pragma warning disable 1591
// ReSharper disable RedundantNameQualifier

namespace PdfSharpCore.Fonts
{
    /// <summary>
    /// Default platform specific font resolving.
    /// </summary>
    public static class PlatformFontResolver
    {
        /// <summary>
        /// Resolves the typeface by generating a font resolver info.
        /// </summary>
        /// <param name="familyName">Name of the font family.</param>
        /// <param name="isBold">Indicates whether a bold font is requested.</param>
        /// <param name="isItalic">Indicates whether an italic font is requested.</param>
        public static FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            FontResolvingOptions fontResolvingOptions = new FontResolvingOptions(FontHelper.CreateStyle(isBold, isItalic));
            return ResolveTypeface(familyName, fontResolvingOptions, XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions));
        }

        /// <summary>
        /// Internal implementation.
        /// </summary>
        internal static FontResolverInfo ResolveTypeface(string familyName, FontResolvingOptions fontResolvingOptions, string typefaceKey)
        {
            // Internally we often have the typeface key already.
            if (string.IsNullOrEmpty(typefaceKey))
                typefaceKey = XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions);

            // The user may call ResolveTypeface anytime from anywhere, so check cache in FontFactory in the first place.
            FontResolverInfo fontResolverInfo;
            if (FontFactory.TryGetFontResolverInfoByTypefaceKey(typefaceKey, out fontResolverInfo))
                return fontResolverInfo;

            // Let the platform create the requested font source and save both PlattformResolverInfo
            // and XFontSource in FontFactory cache.
            // It is possible that we already have the correct font source. E.g. we already have the regular typeface in cache
            // and looking now for the italic typeface, but no such font exists. In this case we get the regular font source
            // and cache again it with the italic typeface key. Furthermore in glyph typeface style simulation for italic is set.
#if (CORE || GDI) && !WPF
            GdiFont gdiFont;
            XFontSource fontSource = CreateFontSource(familyName, fontResolvingOptions, out gdiFont, typefaceKey);
#endif
#if WPF && !SILVERLIGHT
            WpfFontFamily wpfFontFamily;
            WpfTypeface wpfTypeface;
            WpfGlyphTypeface wpfGlyphTypeface;
            XFontSource fontSource = CreateFontSource(familyName, fontResolvingOptions, out wpfFontFamily, out wpfTypeface, out wpfGlyphTypeface, typefaceKey);
#endif
#if SILVERLIGHT
            //GlyphTypeface wpfGlyphTypeface;
            XFontSource fontSource = null;//CreateFontSource(familyName, isBold, isItalic, out wpfGlyphTypeface, typefaceKey);
#endif
#if NETFX_CORE || UWP
            //GlyphTypeface wpfGlyphTypeface;
            XFontSource fontSource = null;//CreateFontSource(familyName, isBold, isItalic, out wpfGlyphTypeface, typefaceKey);
#endif
#if __IOS__
            XFontSource fontSource = CreateFontSource(familyName, fontResolvingOptions, typefaceKey);
#endif
#if __ANDROID__
            XFontSource fontSource = CreateFontSource(familyName, fontResolvingOptions, typefaceKey);
#endif
#if PORTABLE
            XFontSource fontSource = null;
#endif
            // If no such font exists return null. PDFsharp will fail.
            if (fontSource == null)
                return null;

            //#if (CORE || GDI) && !WPF
            //            // TODO: Support style simulation for GDI+ platform fonts.
            //            fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, false, false, gdiFont);
            //#endif
            if (fontResolvingOptions.OverrideStyleSimulations)
            {
#if (CORE || GDI) && !WPF
                // TODO: Support style simulation for GDI+ platform fonts.
                fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, fontResolvingOptions.MustSimulateBold, fontResolvingOptions.MustSimulateItalic, gdiFont);
#endif
#if WPF && !SILVERLIGHT
                fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, fontResolvingOptions.MustSimulateBold, fontResolvingOptions.MustSimulateItalic,
                    wpfFontFamily, wpfTypeface, wpfGlyphTypeface);
#endif
            }
            else
            {
#if (CORE || GDI) && !WPF
                bool mustSimulateBold = gdiFont.Bold && !fontSource.Fontface.os2.IsBold;
                bool mustSimulateItalic = gdiFont.Italic && !fontSource.Fontface.os2.IsItalic;
                fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, mustSimulateBold, mustSimulateItalic, gdiFont);
#endif
#if WPF && !SILVERLIGHT
                // WPF knows what styles have to be simulated.
                bool mustSimulateBold = (wpfGlyphTypeface.StyleSimulations & WpfStyleSimulations.BoldSimulation) == WpfStyleSimulations.BoldSimulation;
                bool mustSimulateItalic = (wpfGlyphTypeface.StyleSimulations & WpfStyleSimulations.ItalicSimulation) == WpfStyleSimulations.ItalicSimulation;

                // Weird behavior of WPF is fixed here in case we request a bold italic typeface.
                // If only italic is available, bold is simulated based on italic.
                // If only bold is available, italic is simulated based on bold.
                // But if both bold and italic is available, italic face is used and bold is simulated.
                // The latter case is reversed here, i.e. bold face is used and italic is simulated.
                if (fontResolvingOptions.IsBoldItalic && mustSimulateBold && !mustSimulateItalic)
                {
                    // Try to get the bold typeface.
                    string typefaceKeyBold = XGlyphTypeface.ComputeKey(familyName, true, false);
                    FontResolverInfo infoBold = ResolveTypeface(familyName,
                        new FontResolvingOptions(FontHelper.CreateStyle(true, false)), typefaceKeyBold);
                    // Use it if it does not base on simulateion.
                    if (infoBold != null && infoBold.StyleSimulations == XStyleSimulations.None)
                    {
                        // Use existing bold typeface and simualte italic.
                        fontResolverInfo = new PlatformFontResolverInfo(typefaceKeyBold, false, true,
                            wpfFontFamily, wpfTypeface, wpfGlyphTypeface);
                    }
                    else
                    {
                        // Simulate both.
                        fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, true, true,
                            wpfFontFamily, wpfTypeface, wpfGlyphTypeface);
                    }
                }
                else
                {
                    fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, mustSimulateBold, mustSimulateItalic,
                        wpfFontFamily, wpfTypeface, wpfGlyphTypeface);
                }
#endif
            }

#if SILVERLIGHT
            fontResolverInfo = null; //new PlattformResolverInfo(typefaceKey, false, false, wpfGlyphTypeface);
#endif

#if __IOS__
            fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, false, false);
#endif

#if __ANDROID__
            fontResolverInfo = new PlatformFontResolverInfo(typefaceKey, false, false);
#endif
            FontFactory.CacheFontResolverInfo(typefaceKey, fontResolverInfo);

            // Register font data under the platform specific face name.
            // Already done in CreateFontSource.
            // FontFactory.CacheNewFontSource(typefaceKey, fontSource);

            return fontResolverInfo;
        }

#if (CORE_WITH_GDI || GDI) && !WPF
        /// <summary>
        /// Create a GDI+ font and use its handle to retrieve font data using native calls.
        /// </summary>
        internal static XFontSource CreateFontSource(string familyName, FontResolvingOptions fontResolvingOptions, out GdiFont font, string typefaceKey)
        {
            if (string.IsNullOrEmpty(typefaceKey))
                typefaceKey = XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions);
#if true_
            if (familyName == "Cambria")
                Debug-Break.Break();
#endif
            GdiFontStyle gdiStyle = (GdiFontStyle)(fontResolvingOptions.FontStyle & XFontStyle.BoldItalic);

            // Create a 10 point GDI+ font as an exemplar.
            XFontSource fontSource;
            font = FontHelper.CreateFont(familyName, 10, gdiStyle, out fontSource);

            if (fontSource != null)
            {
                Debug.Assert(font != null);
                // Case: Font was created by a GDI+ private font collection.
#if true
#if DEBUG
                XFontSource existingFontSource;
                Debug.Assert(FontFactory.TryGetFontSourceByTypefaceKey(typefaceKey, out existingFontSource) &&
                    ReferenceEquals(fontSource, existingFontSource));
#endif
#else
    // Win32 API cannot get font data from fonts created by private font collection,
    // because this is handled internally in GDI+.
    // Therefore the font source was created when the private font is added to the private font collection.
                if (!FontFactory.TryGetFontSourceByTypefaceKey(typefaceKey, out fontSource))
                {
                    // Simplify styles.
                    // (The code is written for clarity - do not rearrange for optimization)
                    if (font.Bold && font.Italic)
                    {
                        if (FontFactory.TryGetFontSourceByTypefaceKey(XGlyphTypeface.ComputeKey(font.Name, true, false), out fontSource))
                        {
                            // Use bold font.
                            FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
                        }
                        else if (FontFactory.TryGetFontSourceByTypefaceKey(XGlyphTypeface.ComputeKey(font.Name, false, true), out fontSource))
                        {
                            // Use italic font.
                            FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
                        }
                        else if (FontFactory.TryGetFontSourceByTypefaceKey(XGlyphTypeface.ComputeKey(font.Name, false, false), out fontSource))
                        {
                            // Use regular font.
                            FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
                        }
                    }
                    else if (font.Bold || font.Italic)
                    {
                        // Use regular font.
                        if (FontFactory.TryGetFontSourceByTypefaceKey(XGlyphTypeface.ComputeKey(font.Name, false, false), out fontSource))
                        {
                            FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
                        }
                    }
                    else
                    {
                        if (FontFactory.TryGetFontSourceByTypefaceKey(XGlyphTypeface.ComputeKey(font.Name, false, false), out fontSource))
                        {
                            // Should never come here...
                            FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
                        }
                    }
                }
#endif
            }
            else
            {
                // Get or create the font source and cache it unter the specified typeface key.
                fontSource = XFontSource.GetOrCreateFromGdi(typefaceKey, font);
            }
            return fontSource;
        }
#endif

#if WPF && !SILVERLIGHT
        /// <summary>
        /// Create a WPF GlyphTypeface and retrieve font data from it.
        /// </summary>
        internal static XFontSource CreateFontSource(string familyName, FontResolvingOptions fontResolvingOptions,
            out WpfFontFamily wpfFontFamily, out WpfTypeface wpfTypeface, out WpfGlyphTypeface wpfGlyphTypeface, string typefaceKey)
        {
            if (string.IsNullOrEmpty(typefaceKey))
                typefaceKey = XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions);
            XFontStyle style = fontResolvingOptions.FontStyle;

#if DEBUG
            if (StringComparer.OrdinalIgnoreCase.Compare(familyName, "Segoe UI Semilight") == 0
                && (style & XFontStyle.BoldItalic) == XFontStyle.Italic)
                familyName.GetType();
#endif

            // Use WPF technique to create font data.
            wpfTypeface = XPrivateFontCollection.TryCreateTypeface(familyName, style, out wpfFontFamily);
#if DEBUG__
            if (wpfTypeface != null)
            {
                WpfGlyphTypeface glyphTypeface;
                ICollection<WpfTypeface> list = wpfFontFamily.GetTypefaces();
                foreach (WpfTypeface tf in list)
                {
                    if (!tf.TryGetGlyphTypeface(out glyphTypeface))
                        Debug-Break.Break();
                }

                //if (!WpfTypeface.TryGetGlyphTypeface(out glyphTypeface))
                //    throw new InvalidOperationException(PSSR.CannotGetGlyphTypeface(familyName));
            }
#endif
            if (wpfFontFamily == null)
                wpfFontFamily = new WpfFontFamily(familyName);

            if (wpfTypeface == null)
                wpfTypeface = FontHelper.CreateTypeface(wpfFontFamily, style);

            // Let WPF choose the right glyph typeface.
            if (!wpfTypeface.TryGetGlyphTypeface(out wpfGlyphTypeface))
                throw new InvalidOperationException(PSSR.CannotGetGlyphTypeface(familyName));

            // Get or create the font source and cache it unter the specified typeface key.
            XFontSource fontSource = XFontSource.GetOrCreateFromWpf(typefaceKey, wpfGlyphTypeface);
            return fontSource;
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Silverlight has no access to the bytes of its fonts and threrefore return null.
        /// </summary>
        internal static XFontSource CreateFontSource(string familyName, bool isBold, bool isItalic)
        {
            // PDFsharp does not provide a default font because this would blow up the assembly
            // unneccessarily if the font is not needed. Provide your own font resolver to generate
            // PDF files containing text.
            return null;
        }
#endif

#if NETFX_CORE
        internal static XFontSource CreateFontSource(string familyName, bool isBold, bool isItalic, string typefaceKey)
        {
            throw new NotImplementedException();
        }
#endif

#if __IOS__

        [StructLayout(LayoutKind.Sequential)]
        public struct FontHeader
        {
            public Int32 fVersion;
            public UInt16 fNumTables;
            public UInt16 fSearchRange;
            public UInt16 fEntrySelector;
            public UInt16 fRangeShift;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TableEntry
        {
            public UInt32 fTag;
            public UInt32 fCheckSum;
            public UInt32 fOffset;
            public UInt32 fLength;
        };

        [DllImport(Constants.CoreGraphicsLibrary)]
        extern static IntPtr CGFontCopyTableTags(IntPtr font);

        [DllImport(Constants.CoreGraphicsLibrary)]
        extern static IntPtr CGFontCopyTableForTag(IntPtr font, int tag);

        [DllImport(Constants.CoreFoundationLibrary)]
        extern static nint CFArrayGetCount(IntPtr theArray);

        [DllImport(Constants.CoreFoundationLibrary)]
        extern static IntPtr CFArrayGetValueAtIndex(IntPtr theArray, nint idx);

        [DllImport(Constants.CoreFoundationLibrary)]
        extern static nint CFDataGetLength(IntPtr theData);

        [DllImport(Constants.CoreFoundationLibrary)]
        internal extern static IntPtr CFRelease(IntPtr handle);

        [DllImport(Constants.CoreFoundationLibrary)]
        extern static IntPtr CFDataGetBytePtr(IntPtr theData);

        public static ushort SwapBytes(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static uint SwapBytes(uint x)
        {
            return ((x & 0x000000ff) << 24) +
                   ((x & 0x0000ff00) << 8) +
                   ((x & 0x00ff0000) >> 8) +
                   ((x & 0xff000000) >> 24);
        }

        private unsafe static UInt32 CalcTableCheckSum(UInt32* table, nint numberOfBytesInTable)
        {
            UInt32 sum = 0;
            nint nLongs = (numberOfBytesInTable + 3) / 4;
            while (nLongs-- > 0)
            {
                sum += SwapBytes(*table++);
            }
            return sum;
        }

        internal unsafe static XFontSource CreateFontSource(string familyName, FontResolvingOptions fontResolvingOptions, string typefaceKey)
        {
            if (string.IsNullOrEmpty(typefaceKey))
                typefaceKey = XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions);

            var descriptor = UIFont.FromName(familyName, 20.0f).FontDescriptor;

            UIFontDescriptorSymbolicTraits traits = 0;

            if (fontResolvingOptions.IsItalic)
            {
                traits |= UIFontDescriptorSymbolicTraits.Italic;
            }

            if (fontResolvingOptions.IsBold)
            {
                traits |= UIFontDescriptorSymbolicTraits.Bold;
            }

            var uifont = UIFont.FromDescriptor(descriptor.CreateWithTraits(traits), 20.0f);
            var cgFont = CGFont.CreateWithFontName(uifont.Name);

            IntPtr tags = CGFontCopyTableTags(cgFont.Handle);
            nint tableCount = CFArrayGetCount(tags);
            nint totalSize = sizeof(FontHeader) + sizeof(TableEntry) * tableCount;

            for (int index = 0; index < tableCount; ++index)
            {
                nint tableSize = 0;

                int aTag = (int)CFArrayGetValueAtIndex(tags, index);

                IntPtr tableDataRef = CGFontCopyTableForTag(cgFont.Handle, aTag);

                if (tableDataRef != IntPtr.Zero)
                {
                    tableSize = CFDataGetLength(tableDataRef);
                    CFRelease(tableDataRef);
                }

                totalSize += (tableSize + 3) & ~3;
            }

            var data = new byte[totalSize];

            fixed (byte* dataStart = data)
            {
                byte* dataPtr = dataStart;

                UInt16 entrySelector = 0;
                UInt16 searchRange = 1;

                while (searchRange < tableCount >> 1)
                {
                    entrySelector++;
                    searchRange <<= 1;
                }
                searchRange <<= 4;

                UInt16 rangeShift = (UInt16)((tableCount << 4) - searchRange);

                FontHeader* offsetTable = (FontHeader*)dataPtr;
                offsetTable->fVersion = (Int32)SwapBytes((UInt16)1);
                offsetTable->fNumTables = SwapBytes((UInt16)tableCount);
                offsetTable->fSearchRange = SwapBytes((UInt16)searchRange);
                offsetTable->fEntrySelector = SwapBytes((UInt16)entrySelector);
                offsetTable->fRangeShift = SwapBytes((UInt16)rangeShift);
                dataPtr += sizeof(FontHeader);

                TableEntry* entry = (TableEntry*) dataPtr;
                dataPtr += sizeof(TableEntry) * tableCount;

                for (int index = 0; index < tableCount; ++index)
                {
                    int aTag = (int)CFArrayGetValueAtIndex(tags, index);

                    IntPtr tableDataRef = CGFontCopyTableForTag(cgFont.Handle, aTag);

                    nint tableSize = 0;

                    if (tableDataRef != IntPtr.Zero)
                    {
                        tableSize = CFDataGetLength(tableDataRef);
                        Buffer.MemoryCopy((byte*)CFDataGetBytePtr(tableDataRef), dataPtr, tableSize, tableSize);
                        entry->fTag = SwapBytes((UInt32)aTag);
                        entry->fCheckSum = SwapBytes(CalcTableCheckSum((UInt32*)dataPtr, tableSize));
                        UInt32 offset = (UInt32)(dataPtr - dataStart);
                        entry->fOffset = SwapBytes((UInt32)offset);
                        entry->fLength = SwapBytes((UInt32)tableSize);
                        CFRelease(tableDataRef);
                    }

                    dataPtr += (tableSize + 3) & ~3;
                    ++entry;
                }
            }

            var fontSource = XFontSource.GetOrCreateFrom(typefaceKey, data);

            return fontSource;
        }
#endif

#if __ANDROID__

        public struct AndroidFontInfo
        {
            public string FamilyName;
            public bool Italic;
            public bool Bold;
        }

        private static Dictionary<AndroidFontInfo, string> fonts = null;

        internal static XFontSource CreateFontSource(string familyName, FontResolvingOptions fontResolvingOptions, string typefaceKey)
        {
            if (string.IsNullOrEmpty(typefaceKey))
                typefaceKey = XGlyphTypeface.ComputeKey(familyName, fontResolvingOptions);

            if (fonts == null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("/system/etc/fonts.xml");

                fonts = new Dictionary<AndroidFontInfo, string>();

                var familyset = xmlDoc.DocumentElement;
                foreach (XmlNode family in familyset.ChildNodes)
                {
                    if (family.Attributes != null && family.Attributes["name"] != null)
                    {
                        foreach (XmlNode font in family.ChildNodes)
                        {
                            if (font.Attributes["style"] != null && font.Attributes["weight"] != null)
                            {
                                var id = new AndroidFontInfo
                                {
                                    FamilyName = family.Attributes["name"].Value,
                                    Italic = font.Attributes["style"].Value == "italic",
                                    Bold = int.Parse(font.Attributes["weight"].Value) > 500,
                                };

                                fonts[id] = font.ChildNodes[0].Value;
                            }
                        }
                    }
                }
            }
            
            XFontSource fontSource = null;

            string path;

            var searchId = new AndroidFontInfo
            {
                FamilyName = familyName,
                Italic = fontResolvingOptions.IsItalic,
                Bold = fontResolvingOptions.IsBold,
            };

            if (fonts.TryGetValue(searchId, out path))
            {
                fontSource = XFontSource.GetOrCreateFrom(typefaceKey, File.ReadAllBytes(Path.Combine("/system/fonts/", path)));
            }            

            return fontSource;
        }
#endif
    }
}

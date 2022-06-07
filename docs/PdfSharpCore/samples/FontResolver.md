# Font Resolver

This sample shows how to use fonts that are included with your application. This allows you to use fonts that are not installed on the computer.

For tasks running on web servers, private fonts may be the only available fonts.

Note: The FontResolver is a global object and applies to all consumers of the PdfSharpCore library. It is also used when the MigraDocCore library creates PDF files.


## The IFontResolver Interface

In your application you create a class that implements the IFontResolver interface (it is in the PdfSharpCore.Fonts namespace).

There are only two methods you have to implement. The first method returns a FontResolverInfo for every supported font.

```cs
public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
```

The other method is called using the FaceName from the FontResolverInfo you previously returned. At this stage, return the font data as a byte array.

```cs
public byte[] GetFont(string faceName)
```

Now you only need one more step: register your font resolver using the global font resolver property. Here SegoeWpFontResolver is the class that implements IFontResolver.

```cs
// Register font resolver before start using PdfSharpCore.
GlobalFontSettings.FontResolver = new SegoeWpFontResolver();
```

## Additional Information

The font resolver set using GlobalFontSettings.FontResolver will be used by PdfSharpCore. Since MigraDocCore uses PdfSharpCore to create PDF files, the font resolver will also be used when generating PDF files from MigraDocCore.

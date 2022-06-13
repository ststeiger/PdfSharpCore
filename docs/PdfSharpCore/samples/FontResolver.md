# Font Resolver

This sample shows how to use fonts that are included with your application. This allows you to use fonts that are not installed on the computer.


## Default font resolver

PdfSharpCore comes with a [default font resolver](../../../PdfSharpCore/Utils/FontResolver.cs).
This resolver uses the fonts from the operating system.
The font directories depend on the used operating system.

**Windows**
1. `%SystemRoot%\Fonts`
1. `%LOCALAPPDATA%\Microsoft\Windows\Fonts`

**Linux**
1. `/usr/share/fonts`
1. `/usr/local/share/fonts`
1. `~/.fonts`

**iOS**
1. `/Library/Fonts/`


## Custom font resolver

When running on web services or servers, the operating system might **not** have the fonts installed you need or you can **not** install the font you need.
In this scenario you must provide the fonts yourself and therefore implement your own font resolver.

### IFontResolver interface

In your application you create a class that implements the `IFontResolver` interface.

There are only two methods you have to implement. The first method returns a `FontResolverInfo` for every supported font.

```cs
public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
```

The other method is called using the `FaceName` from the FontResolverInfo you previously returned. At this stage, return the font data as a byte array.

```cs
public byte[] GetFont(string faceName)
```

Now you only need one more step: register your font resolver using the global font resolver property.
Here MyFontResolver is the class that implements `IFontResolver`.

```cs
GlobalFontSettings.FontResolver = new MyFontResolver();
```

Note: The `FontResolver` is a global object and applies to all consumers of the PdfSharpCore library. It is also used when the MigraDocCore library creates PDF files.

### Code

This implementation is obviously not complete.
But it should be enough for everyone to implement their own.
For more details have a look at the [default font resolver](../../../PdfSharpCore/Utils/FontResolver.cs).

```cs
using System;
using System.IO;
using PdfSharpCore.Utils;

public class MyFontResolver : IFontResolver
{
    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("OpenSans", StringComparison.CurrentCultureIgnoreCase))
        {
            if (isBold && isItalic)
            {
                return new FontResolverInfo("OpenSans-BoldItalic.ttf");
            }
            else if (isBold)
            {
                return new FontResolverInfo("OpenSans-Bold.ttf");
            }
            else if (isItalic)
            {
                return new FontResolverInfo("OpenSans-Italic.ttf");
            }
            else
            {
                return new FontResolverInfo("OpenSans-Regular.ttf");
            }
        }
        return null;
    }
    
    public byte[] GetFont(string faceName)
    {
        var faceNamePath = Path.Join("my path", faceName);
        using(var ms = new MemoryStream())
        {
            try
            {
                using(var fs = File.OpenRead(faceNamePath))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"No Font File Found - " + faceNamePath);
            }
        }
    }
}
```

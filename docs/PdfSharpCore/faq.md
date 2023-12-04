# PdfSharpCore > FAQ

FAQ for [PdfSharpCore](index.md):


## What is PdfSharpCore

PdfSharpCore is a .NET library for creating and modifying Adobe PDF documents programmatically. It is written in C# and can be used from any .NET language.


## Is PdfSharpCore based on or does it require other libraries or tools?

PdfSharpCore is newly designed and built from scratch in C#. Neither Adobe's PDF Library nor Acrobat are required.


## What is the license of PdfSharpCore?

PdfSharpCore is Open Source. You can copy, modify and integrate the source code of PdfSharpCore in your application without restrictions at all.

See also: PdfSharpCore [license](../../LICENCE.md)


## Can PdfSharpCore show PDF files? Print PDF files? Create images from PDF files?

PdfSharpCore comes with a preview control designed to visualize drawing operations of the XGraphics object, but it cannot render PDF files.

Further the DrawImage function can be used to draw so called form XObjects in PDF pages. If you try to render such an object in the preview, only the bounding box is drawn to show that it cannot be rendered.

The PdfSharpCore [samples](samples/index.md) show how to invoke Adobe Reader or Acrobat to view or print PDF files and how to invoke GhostScript to create images from PDF pages.


## Can I use PostScript fonts with PdfSharpCore?

PdfSharpCore cannot work with PostScript fonts. Only TrueType fonts and OpenType fonts with TrueType outlines can be used with PdfSharpCore. Read more...


## Can PdfSharpCore run on Web Servers under Medium Trust?

You can run applications on web servers without full trust provided you only use fonts that are serviced by your own FontResolver. See the PdfSharpCore sample: [Font Resolver](samples/FontResolver.md) for further information.


## Does PdfSharpCore support for Arabic, Hebrew, CJK (Chinese, Japanese, Korean)?

Not yet. Right-to-left languages are not yet supported. Only simple languages like English or German are supported, with an easy one-to-one relationship between characters and glyphs.

"Not supported" needs some explanation.

It seems that Hebrew works if you reverse the strings and set all paragraphs to left-aligned.

Japanese characters will be displayed, but left to right and not top to bottom. We cannot read Japanese and cannot verify they are shown correctly. Make sure you select a font that contains Japanese characters.

Arabic characters have different shapes (glyphs), depending on their position (beginning, end, middle, isolated). PdfSharpCore does not support the selection of the correct glyphs. Arabic text may work if you reverse the string and if you make sure to select the correct Unicode characters for beginning, end, middle, or isolated display. Make sure you select a font that contains Arabic characters.


## Which PDF versions are supported by PdfSharpCore?

With PdfSharpCore you can create files with PDF versions from 1.2 (Adobe Acrobat Reader 3.0) through 1.7 (Adobe Reader 8.0).
PdfSharpCore fully supports PDF 1.4 (Adobe Reader 5.0) including the transparency features introduced with this version.
Some features of PDF 1.5 (Adobe Reader 6.0) are not yet implemented. Therefore PdfSharpCore cannot yet open all files marked for PDF 1.5 or higher. Since not all compression features of PDF 1.5 are implemented, with some files the file size may increase when they are processed with PdfSharpCore.


## Does PdfSharpCore support PDF/A?

Not yet.


## Does PdfSharpCore support AcroForms?

There is limited support for AcroForms included.


## Can I use PdfSharpCore to convert HTML or RTF to PDF?

No, not "out of the box", and we do not plan to write such a converter in the near future.

Yes, PdfSharpCore with some extra code can do it. But we do not supply that extra code.
On NuGet and other sources you can find a third party library "HTML Renderer for PDF using PdfSharpCore" that converts HTML to PDF. And there may be other libraries for the same or similar purposes, too. Maybe they work for you, maybe they get you started.


## Can I use PdfSharpCore to convert PDF to Word, RTF, HTML?

No, and we do not plan to write such a converter in the near future.


## Can I use PDF files created with SQL Server 2008 Reporting Services?

There is an issue with the PDFs created by SQL Server 2008 Reporting Services. We are working on it.
As a workaround, create reports with SQL Server 2005 Reporting Services. Workaround for SQL Server 2008 Reporting Services: For the DeviceSettings parameter for the Render method on the ReportExecutionService object, pass this value:
`theDeviceSettings = "<DeviceInfo><HumanReadablePDF>True</HumanReadablePDF></DeviceInfo>";`.
This disables PDF file compression for SSRS 2008. Then, PdfSharpCore is able to handle the resulting uncompressed PDF file. (Note: SSRS 2005 ignores this setting so it can be passed to both SSRS versions.)


## Can I use PdfSharpCore to extract text from PDF?

This can be done at a low level. You can get at the characters in the order they are drawn - and most applications draw them from top-left to bottom-right. There are no high-level functions that return words, paragraphs, or whole pages.


## Can PdfSharpCore simulate Bold or Italics?

Not yet.


## How many DPI are there in a PDF file? How can I set the DPI?

PDF is a vector format, so there are no DPI. Raster images used in a PDF file do have DPI, but DPI is determined by the usage.
Consider an image with 300 DPI. This image can be embedded once in the PDF file, but can be drawn several times. There could be a thumbnail on page 1, a full size reproduction on page 2, and a double size reproduction on page 3. Thus the image is drawn with 600 DPI on page 1, 300 DPI on page 2, and 150 DPI on page 3. But when you watch the PDF file in Adobe Reader with a Zoom factor of 1000%, the DPI value will be much lower than that.
PDF is vector. There is no DPI. PdfSharpCore uses Points as the unit for coordinates. There are 72 Points per Inch. For ease of use, units can be converted from Inch, Centimeter, Millimeter and other units.

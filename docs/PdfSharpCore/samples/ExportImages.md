# Export Images

This sample shows how to export JPEG images from a PDF file.

Note: This snippet shows how to export JPEG images from a PDF file. PdfSharpCore cannot convert PDF pages to JPEG files. This sample does not handle non-JPEG images. It does not (yet) handle JPEG images that have been flate-encoded.

There are several different formats for non-JPEG images in PDF. Those are not supported by this simple sample and require several hours of coding, but this is left as an exercise to the reader.

PdfSharpCore cannot render PDF pages - not to printers, not to bitmaps, not to JPEG files.


## Code

Here is the source code that does the work:

```cs
const string filename = "../../../../../PDFs/SomeLayout.pdf";
PdfDocument document = PdfReader.Open(filename);
 
int imageCount = 0;
// Iterate pages
foreach (PdfPage page in document.Pages)
{
    // Get resources dictionary
    PdfDictionary resources = page.Elements.GetDictionary("/Resources");
    if (resources != null)
    {
        // Get external objects dictionary
        PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
        if (xObjects != null)
        {
            ICollection<pdfitem> items = xObjects.Elements.Values;
            // Iterate references to external objects
            foreach (PdfItem item in items)
            {
                PdfReference reference = item as PdfReference;
                if (reference != null)
                {
                    PdfDictionary xObject = reference.Value as PdfDictionary;
                    // Is external object an image?
                    if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                    {
                        ExportImage(xObject, ref imageCount);
                    }
                }
            }
        }
    }
}
```

The image exporter:

```cs
static void ExportImage(PdfDictionary image, ref int count)
{
    string filter = image.Elements.GetName("/Filter");
    switch (filter)
    {
        case "/DCTDecode":
        ExportJpegImage(image, ref count);
        break;
        
        case "/FlateDecode":
        ExportAsPngImage(image, ref count);
        break;
    }
}
```

Here's the routine that exports JPEG images:

```cs
static void ExportJpegImage(PdfDictionary image, ref int count)
{
    // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
    byte[] stream = image.Stream.Value;
    FileStream fs = new FileStream(String.Format("Image{0}.jpeg", count++), FileMode.Create, FileAccess.Write);
    BinaryWriter bw = new BinaryWriter(fs);
    bw.Write(stream);
    bw.Close();
}
```

Other image formats are not yet implemented, here is the stub:

```cs
static void ExportAsPngImage(PdfDictionary image, ref int count)
{
    int width = image.Elements.GetInteger(PdfImage.Keys.Width);
    int height = image.Elements.GetInteger(PdfImage.Keys.Height);
    int bitsPerComponent = image.Elements.GetInteger(PdfImage.Keys.BitsPerComponent);
    
    // TODO: You can put the code here that converts vom PDF internal image format to a Windows bitmap
    // and use GDI+ to save it in PNG format.
    // It is the work of a day or two for the most important formats. Take a look at the file
    // PdfSharpCore.Pdf.Advanced/PdfImage.cs to see how we create the PDF image formats.
    // We don't need that feature at the moment and therefore will not implement it.
    // If you write the code for exporting images I would be pleased to publish it in a future release
    // of PdfSharpCore.
}
```

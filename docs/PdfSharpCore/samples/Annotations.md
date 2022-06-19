# Annotations

This sample shows how to create PDF annotations.

PdfSharpCore supports the creation of the following annotations:
* [Text annotations](#text-annotations)
* [Text annotations opened](#text-annotations-opened)
* [Rubber stamp annotations](#rubber-stamp-annotations)


## Text annotations

```cs
// Create a PDF text annotation
PdfTextAnnotation textAnnot = new PdfTextAnnotation();
textAnnot.Title = "This is the title";
textAnnot.Subject = "This is the subject";
textAnnot.Contents = "This is the contents of the annotation.\rThis is the 2nd line.";
textAnnot.Icon = PdfTextAnnotationIcon.Note;
 
gfx.DrawString("The first text annotation", font, XBrushes.Black, 30, 50, XStringFormats.Default);
 
// Convert rectangle from world space to page space. This is necessary because the annotation is
// placed relative to the bottom left corner of the page with units measured in point.
XRect rect = gfx.Transformer.WorldToDefaultPage(new XRect(new XPoint(30, 60), new XSize(30, 30)));
textAnnot.Rectangle = new PdfRectangle(rect);
 
// Add the annotation to the page
page.Annotations.Add(textAnnot);
```


## Text annotations opened
```cs
// Create another PDF text annotation which is open and transparent
textAnnot = new PdfTextAnnotation();
textAnnot.Title = "Annotation 2 (title)";
textAnnot.Subject = "Annotation 2 (subject)";
textAnnot.Contents = "This is the contents of the 2nd annotation.";
textAnnot.Icon = PdfTextAnnotationIcon.Help;
textAnnot.Color = XColors.LimeGreen;
textAnnot.Opacity = 0.5;
textAnnot.Open = true;
 
gfx.DrawString("The second text annotation (opened)", font, XBrushes.Black, 30, 140, XStringFormats.Default);
 
rect = gfx.Transformer.WorldToDefaultPage(new XRect(new XPoint(30, 150), new XSize(30, 30)));
textAnnot.Rectangle = new PdfRectangle(rect);
 
// Add the 2nd annotation to the page
page.Annotations.Add(textAnnot);
```


## Rubber stamp annotations

```cs
// Create a so called rubber stamp annotation. I'm not sure if it is useful, but at least
// it looks impressive...
PdfRubberStampAnnotation rsAnnot = new PdfRubberStampAnnotation();
rsAnnot.Icon = PdfRubberStampAnnotationIcon.TopSecret;
rsAnnot.Flags = PdfAnnotationFlags.ReadOnly;
 
rect = gfx.Transformer.WorldToDefaultPage(new XRect(new XPoint(100, 400), new XSize(350, 150)));
rsAnnot.Rectangle = new PdfRectangle(rect);
 
// Add the rubber stamp annotation to the page
page.Annotations.Add(rsAnnot);
```

PDF supports some more pretty types of annotations like PdfLineAnnotation, PdfSquareAnnotation, PdfCircleAnnotation, PdfMarkupAnnotation (with the subtypes PdfHighlightAnnotation, PdfUnderlineAnnotation, PdfStrikeOutAnnotation, and PdfSquigglyAnnotation), PdfSoundAnnotation, or PdfMovieAnnotation.
If you need one of them, feel encouraged to implement it. It is quite easy.

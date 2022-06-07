# Welcome to PdfSharpCore & MigraDocCore!

The libraries in this project are published Open Source and under the [MIT license](https://en.wikipedia.org/wiki/MIT_License) and are free to use.


## PdfSharpCore

PdfSharpCore is the Open Source .NET library that easily creates and processes PDF documents on the fly.
The same drawing routines can be used to create PDF documents, draw on the screen, or send output to any printer.

* [PdfSharpCore](PdfSharpCore/index.md)


## MigraDocCore

MigraDocCore is the Open Source .NET library that easily creates documents based on an object model with paragraphs, tables, styles, etc. and renders them into PDF by using the PdfSharpCore library.

* [MigraDocCore](MigraDocCore/index.md)


## Use PdfSharpCore or MigraDocCore?

Use PdfSharpCore if you want to create PDF files only, but be able to control every pixel and every line that is drawn.
Use MigraDocCore if you need documents as PDF files and if you want to enjoy the comfort of a word processor.


## Mixing PdfSharpCore and MigraDocCore

If MigraDocCore does almost anything you need, then you can use it to create PDF files and post-process them with PdfSharpCore to add some extra features.

Or use PdfSharpCore to create the document but use MigraDocCore to create individual pages.
This could be the best choice if your application uses lots of graphics, but also needs some layout text.

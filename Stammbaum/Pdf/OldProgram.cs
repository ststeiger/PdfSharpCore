using SixLabors.ImageSharp;

namespace Stammbaum
{
    class OldProgram
    {
        static void OldSimpleMain(string[] args)
        {
            PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new FontResolver();

            MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes
                .ImageSource.ImageSourceImpl = new PdfSharpCore.ImageSharp.ImageSharpImageSource<Rgba32>();


            using (PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument())
            {
                document.Info.Title = "Family Tree";
                document.Info.Author = "FamilyTree Ltd. - Stefan Steiger";
                document.Info.Subject = "Family Tree";
                document.Info.Keywords = "Family Tree, Genealogical Tree, Genealogy, Bloodline, Pedigree";


                document.ViewerPreferences.Direction = PdfSharpCore.Pdf.PdfReadingDirection.LeftToRight;

                PdfSharpCore.Pdf.PdfPage page = document.AddPage();

                // page.Width = PdfSettings.PaperFormatSettings.Width
                // page.Height = PdfSettings.PaperFormatSettings.Height

                page.Width = 500;
                page.Height = 1000;

                page.Orientation = PdfSharpCore.PageOrientation.Landscape;


                double mid = page.Width / 2;
                double textBoxWidth = 200;
                double halfTextBoxWidth = textBoxWidth / 2;


                using (PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page))
                {
                    gfx.MUH = PdfSharpCore.Pdf.PdfFontEncoding.Unicode;

                    PdfSharpCore.Drawing.Layout.XTextFormatter tf = new PdfSharpCore.Drawing.Layout.XTextFormatter(gfx);
                    tf.Alignment = PdfSharpCore.Drawing.Layout.XParagraphAlignment.Left;

                    PdfSharpCore.Drawing.Layout.XTextFormatterEx2 etf = new PdfSharpCore.Drawing.Layout.XTextFormatterEx2(gfx);


                    string fn = @"C:\Users\username\Pictures\4_Warning_Signs_Of_Instability_In_Russia1.png";
                    // fn = @"C:\Users\username\Pictures\62e867ba-9bb7-40d0-b01f-57f7cc951929.jpg";
                    // DrawImg(fn, gfx, 10, 10);



                    double dblWidth = 1.0;
                    string strHtmlColor = "#FF00FF";
                    PdfSharpCore.Drawing.XColor LineColor = XColorHelper.FromHtml(strHtmlColor);
                    PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(LineColor, dblWidth);


                    double dblX1 = 100;
                    double dblY1 = 100;

                    double dblX2 = 200;
                    double dblY2 = 200;
                    gfx.DrawLine(pen, dblX1, dblY1, dblX2, dblY2);



                    double rectX = 100;
                    double rectY = 100;
                    double rectWidth = 200;
                    double rectHeight = 100;

                    PdfSharpCore.Drawing.XRect rect = new PdfSharpCore.Drawing.XRect(rectX, rectY
                        , rectWidth, rectHeight
                    );

                    PdfSharpCore.Drawing.XFont font = new PdfSharpCore.Drawing.XFont("Arial"
                        , 12.0, PdfSharpCore.Drawing.XFontStyle.Bold
                    );

                    int lastFittingChar = 0;
                    double neededHeight = 0.0;


                    string text = @"I bi dr Gummiboum
U schtah eifach so chli da
So wie jedä Gummiboum 
'sch im Fau aues, woni cha

U i nime d Tage so wie si sii
U si chömed u gö wider verbii
U aues wird anders oder blibt wies isch gsii

Ja i nime d Tage so we si sii
U si chömed u gö wider verbii
Aues wird anders oder blibt wies isch gsii

Ja aues wird anders oder blibt wies isch gsii

I bi dr Gummiboum
U verstoube fängs ä chli
Oh Gummiboum

I bin ä geile huere Gummiboum
öppert mues ne schliesslich sii";

                    etf.PrepareDrawString(text, font, rect, out lastFittingChar, out neededHeight);
                    System.Console.WriteLine(lastFittingChar);
                    System.Console.WriteLine(neededHeight);

                    string foo = text.Substring(0, lastFittingChar);
                    System.Console.WriteLine(foo);


                    tf.DrawString(text
                                , font
                                , PdfSharpCore.Drawing.XBrushes.Black
                                , rect
                                , PdfSharpCore.Drawing.XStringFormats.TopLeft
                    );

                    //gfx.DrawRectangle(PdfSharpCore.Drawing.XBrushes.HotPink, rect);
                    gfx.DrawRectangle(pen, rect);

                } // End Using gfx 


                byte[] baPdfDocument;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    document.Save(ms, false);
                    ms.Flush();

                    // baPdfDocument = new byte[ms.Length];
                    // ms.Seek(0, System.IO.SeekOrigin.Begin);
                    // ms.Read(baPdfDocument, 0, (int)ms.Length);

                    baPdfDocument = ms.ToArray();
                } // End Using ms 


                System.IO.File.WriteAllBytes("FamilyTree.pdf", baPdfDocument);
                //document.Save(filename);
            } // End Using document 

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main 


        public static void DrawImg(System.IO.Stream imageSource, PdfSharpCore.Drawing.XGraphics gfx
            , double x, double y)
        {
            using (PdfSharpCore.Drawing.XImage image =
                  PdfSharpCore.Drawing.XImage.FromStream(
                      delegate ()
                      {
                          return imageSource;
                      }
                 )
            )
            {
                gfx.DrawImage(image, x, y);
            }
        } // End Function DrawImg


        public static void DrawImg(byte[] imgData, PdfSharpCore.Drawing.XGraphics gfx
            , double x, double y)
        {
            using (System.IO.Stream stream = new System.IO.MemoryStream(imgData))
            {
                DrawImg(stream, gfx, x, y);
            } // End Using stream 

        } // End Function DrawImg


        public static void DrawImg(string fileName, PdfSharpCore.Drawing.XGraphics gfx
            , double x, double y)
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(fileName))
            {
                DrawImg(fs, gfx, x, y);
            } // End Using fs 

        } // End Function DrawImg


    } // End Class 


} // End Namespace 

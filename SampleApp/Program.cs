
namespace SampleApp 
{


    public class Program
    {
        

        private static string GetOutFilePath(string name)
        {
            string OutputDirName = @".";
            return System.IO.Path.Combine(OutputDirName, name);
        }


        private static void SaveDocument(PdfSharpCore.Pdf.PdfDocument document, string name)
        {
            string outFilePath = GetOutFilePath(name);
            string? dir = System.IO.Path.GetDirectoryName(outFilePath);
            if (dir != null && !System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            document.Save(outFilePath);
        }


        public static void Main(string[] args)
        {
            System.Console.WriteLine("Starting...");

            const string outName = "test1.pdf";

            PdfSharpCore.Pdf.PdfDocument? document = new PdfSharpCore.Pdf.PdfDocument();

            PdfSharpCore.Pdf.PdfPage? pageNewRenderer = document.AddPage();

            PdfSharpCore.Drawing.XGraphics? renderer = PdfSharpCore.Drawing.XGraphics.FromPdfPage(pageNewRenderer);

            renderer.DrawString(
                "Testy Test Test"
                , new PdfSharpCore.Drawing.XFont("Arial", 12)
                , PdfSharpCore.Drawing.XBrushes.Black
                , new PdfSharpCore.Drawing.XPoint(12, 12)
            );
            
            PdfSharpCore.Drawing.Layout.XTextFormatter? formatter = new PdfSharpCore.Drawing.Layout.XTextFormatter(renderer);

            var font = new PdfSharpCore.Drawing.XFont("Arial", 12);
            var brush = PdfSharpCore.Drawing.XBrushes.Black;

            formatter.AllowVerticalOverflow = true;
            var originalLayout = new PdfSharpCore.Drawing.XRect(0, 30, 120, 120);
            var text = "More and more text boxes to show alignment capabilities"; // " with addipional gline";
            var anotherText =
                "Text to determine the size of the box I would like to place the text I'm goint to test";
            var rect = formatter.GetLayout(
                anotherText,
                font,
                brush,
                originalLayout);
            rect.Location = new PdfSharpCore.Drawing.XPoint(50, 50);
            formatter.AllowVerticalOverflow = false;
            
            // Draw the string
            formatter.DrawString(
                text,
                font,
                brush,
                rect,
                PdfSharpCore.Drawing.XStringFormats.BottomRight
            );
            
            // Draw the box to check that the text fits and aligns correctly
            var transparentBrush = new PdfSharpCore.Drawing.XSolidBrush(PdfSharpCore.Drawing.XColor.FromArgb(20, brush.Color.R, brush.Color.G, brush.Color.B));
            renderer.DrawRectangle(transparentBrush, rect);
            
            SaveDocument(document, outName);

            System.Console.WriteLine("Done!");
        } // End Sub Main 


    } // End Class Program 


} // End Namespace SampleApp 

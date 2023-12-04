
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

            SaveDocument(document, outName);

            System.Console.WriteLine("Done!");
        } // End Sub Main 


    } // End Class Program 


} // End Namespace SampleApp 

// See https://aka.ms/new-console-template for more information
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

string GetOutFilePath(string name)
{
    var OutputDirName = @"C:\Users\Arbeit\Documents\";
    return Path.Combine(OutputDirName, name);
}

void SaveDocument(PdfDocument document, string name)
{
    var outFilePath = GetOutFilePath(name);
    var dir = Path.GetDirectoryName(outFilePath);
    if (dir is not null && !Directory.Exists(dir))
    {
        Directory.CreateDirectory(dir);
    }

    document.Save(outFilePath);
}

Console.WriteLine("Hello, World!");

const string outName = "test1.pdf";

var document = new PdfDocument();

var pageNewRenderer = document.AddPage();

var renderer = XGraphics.FromPdfPage(pageNewRenderer);

renderer.DrawString("Testy Test Test", new XFont("Arial", 12), XBrushes.Black, new XPoint(12, 12));

SaveDocument(document, outName);
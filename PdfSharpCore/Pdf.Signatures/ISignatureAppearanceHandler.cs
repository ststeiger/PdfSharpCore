using PdfSharpCore.Drawing;

namespace PdfSharpCore.Pdf.Signatures
{
    public interface ISignatureAppearanceHandler
    {
        void DrawAppearance(XGraphics gfx, XRect rect);
    }
}

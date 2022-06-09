using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfSharpCore.Pdf.Signatures
{
    public interface ISigner
    {
        byte[] GetSignedCms(Stream stream);

        string GetName();
        
    }
}

using MigraDocCore.Rendering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Xunit;
using MigraDocCore.DocumentObjectModel;

namespace PdfSharpCore.Test
{
    public class MigradocTurkishTest
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        [Fact]
        public void RenderDocument_TurkishCulture_NoCrashing()
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
            originalUICulture = Thread.CurrentThread.CurrentUICulture;
            var cultureInfo = CultureInfo.GetCultureInfo("tr-TR");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            try
            {
                Document doc = new Document();
                PdfDocumentRenderer printer = new PdfDocumentRenderer() { Document = doc };
                printer.RenderDocument();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCulture;
                Thread.CurrentThread.CurrentUICulture = originalUICulture;
                CultureInfo.CurrentCulture.ClearCachedData();
                CultureInfo.CurrentUICulture.ClearCachedData();
            }
        }
    }
}

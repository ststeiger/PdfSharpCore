
using System.Linq;
using Stammbaum.DataStructures;
using SixLabors.ImageSharp;

namespace Stammbaum
{
    class Program
    {
        public static TreeInfo GetAncestors()
        {
            TreeInfo ti = new TreeInfo();
            
            CoreDb.DalConfig config = new CoreDb.DalConfig(
                typeof(System.Data.SqlClient.SqlClientFactory)
                , delegate (CoreDb.DalConfig conf)
                {
                    System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();

                    csb.DataSource = "127.0.0.1";

                    // Set during installation
                    // csb.DataSource = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
                    // https://stackoverflow.com/questions/804700/how-to-find-fqdn-of-local-machine-in-c-net

                    // https://stackoverflow.com/questions/1233217/difference-between-systeminformation-computername-environment-machinename-and
                    // Environment.MachineName : NetBIOS name of local computer read from registry

                    // TCP-Based network-name
                    if("COR".Equals(System.Environment.UserDomainName, System.StringComparison.InvariantCultureIgnoreCase))
                        csb.DataSource = System.Net.Dns.GetHostName() + @"\SQLEXPRESS";
                    else
                        csb.DataSource = System.Net.Dns.GetHostName();

                    csb.InitialCatalog = "Ahnen";
                    csb.IntegratedSecurity = true;
                    if (!csb.IntegratedSecurity)
                    {
                        csb.UserID = "GenealogicalResearchWebServices";
                        csb.Password = "icanhazPassword2017";
                    }

                    csb.PersistSecurityInfo = false;
                    csb.PacketSize = 4096;
                    csb.Pooling = true;
                    return csb.ConnectionString;
                }
            );


            CoreDb.ReadDAL DAL = new CoreDb.ReadDAL(config);

            using (System.Data.Common.DbCommand cmd = DAL.CreateCommandFromFile(typeof(Program), "Ahnen.sql"))
            {
                ti.AllData = DAL.GetList<PersonData>(cmd);
            }

            System.Console.WriteLine(ti.AllData);

            var lsAncestorGenerations = (
                from ancestorList in ti.AllData
                orderby ancestorList.generation ascending, ancestorList.Id ascending
                group ancestorList by new { ancestorList.generation } into g
                select new
                {
                    Generation = g.Key.generation,
                    PresenceCount = g.Count(x => x.Id > -1)
                }
            ).ToList();


            ti.MaxGeneration = lsAncestorGenerations.Max(x => x.Generation); ;
            ti.MaxPresence = lsAncestorGenerations.Max(x => x.PresenceCount);

            System.Console.WriteLine(ti.MaxGeneration);
            System.Console.WriteLine(ti.MaxPresence);



            for (int i = 0; i < ti.MaxGeneration; ++i)
            {
                ti.ls.Add(new System.Collections.Generic.List<PersonData>());
                ti.ls[i].AddRange(
                     (
                        from ancestorList in ti.AllData
                        where ancestorList.generation == i
                        orderby
                          ancestorList.generation ascending
                        , ancestorList.Child ascending
                        , ancestorList.Id ascending
                        , ancestorList.gender descending 

                        select ancestorList
                    ).ToList()
                );

            }

            System.Console.WriteLine(ti);
            return ti;
        }


        static void TestPdfReader()
        {
            string fn = @"C:\Program Files\Microsoft\R Client\R_SERVER\doc\manual\fullrefman.pdf";

            int version = PdfSharpCore.Pdf.IO.PdfReader.TestPdfFile(fn);
            System.Console.WriteLine(version);

            using (PdfSharpCore.Pdf.PdfDocument pr = PdfSharpCore.Pdf.IO.PdfReader.Open(fn))
            {
                System.Console.WriteLine(pr.Pages.Count);
            }

        }
        
        





        public static void CropPdf()
        {
            double baseY = 0;
            
            PdfSharpCore.Pdf.PdfDocument outputDocument =
                new PdfSharpCore.Pdf.PdfDocument();
            
            PdfSharpCore.Pdf.PdfPage page = outputDocument.AddPage();
            
            //page.Height = PdfSharpCore.Drawing.XUnit.FromMillimeter(baseY + 10).Point;
            double height = PdfSharpCore.Drawing. XUnit.FromMillimeter(baseY + 10).Point;
            page.CropBox = new PdfSharpCore.Pdf.PdfRectangle(
                new PdfSharpCore.Drawing.XPoint(0, page.Height - height),
                new PdfSharpCore.Drawing.XSize(page.Width, height));
        }


        public static void ReadPdf()
        {
            // Get a fresh copy of the sample PDF file
            string filename = "FamilyTree.pdf";
            
            // Create the output document
            PdfSharpCore.Pdf.PdfDocument outputDocument =
                new PdfSharpCore.Pdf.PdfDocument();
            
            // Show single pages
            // (Note: one page contains two pages from the source document)
            outputDocument.PageLayout = PdfSharpCore.Pdf.PdfPageLayout.SinglePage;

            /*
            PdfSharpCore.Drawing.XFont font =
                new PdfSharpCore.Drawing.XFont("Verdana", 8, PdfSharpCore.Drawing.XFontStyle.Bold);
            PdfSharpCore.Drawing.XStringFormat format = new PdfSharpCore.Drawing.XStringFormat();
            format.Alignment = PdfSharpCore.Drawing.XStringAlignment.Center;
            format.LineAlignment = PdfSharpCore.Drawing.XLineAlignment.Far;
            */
            PdfSharpCore.Drawing.XGraphics gfx;
            PdfSharpCore.Drawing.XRect box;

            // Open the external document as XPdfForm object
            PdfSharpCore.Drawing.XPdfForm form = 
                PdfSharpCore.Drawing.XPdfForm.FromFile(filename);

            for (int idx = 0; idx < form.PageCount; idx += 2)
            {
                // Add a new page to the output document
                PdfSharpCore.Pdf.PdfPage page = outputDocument.AddPage();
                page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                double width = page.Width;
                double height = page.Height;

                int rotate = page.Elements.GetInteger("/Rotate");

                gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);

                // Set page number (which is one-based)
                form.PageNumber = idx + 1;

                box = new PdfSharpCore.Drawing.XRect(0, 0, width / 2, height);
                // Draw the page identified by the page number like an image
                gfx.DrawImage(form, box);

                // Write document file name and page number on each page
                box.Inflate(0, -10);
               /*
                gfx.DrawString(string.Format("- {1} -", filename, idx + 1),
                    font, PdfSharpCore.Drawing.XBrushes.Red, box, format);
                */
                if (idx + 1 < form.PageCount)
                {
                    // Set page number (which is one-based)
                    form.PageNumber = idx + 2;

                    box = new PdfSharpCore.Drawing.XRect(width / 2, 0, width / 2, height);
                    // Draw the page identified by the page number like an image
                    gfx.DrawImage(form, box);

                    // Write document file name and page number on each page
                    box.Inflate(0, -10);
                    /*
                    gfx.DrawString(string.Format("- {1} -", filename, idx + 2),
                        font, PdfSharpCore.Drawing.XBrushes.Red, box, format);
                        */
                }
            }

            // Save the document...
            filename = "TwoPagesOnOne_tempfile.pdf";
            outputDocument.Save(filename);
            // ...and start a viewer.
            System.Diagnostics.Process.Start(filename);
        }
        

        
        static void Main(string[] args)
        {
            // ReadPdf();
            
            // TfsRemover.RemoveTFS();
            TreeInfo ti = GetAncestors();

            int maxNumPeople = (int)System.Math.Pow(2.0, (double) ti.MaxGeneration);
            System.Console.WriteLine(maxNumPeople);

            System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int
                , DataPoint>> dict
                = new System.Collections.Generic.Dictionary<int
                , System.Collections.Generic.Dictionary<int, DataPoint>>();

            PdfSharpCore.Fonts.GlobalFontSettings.FontResolver = new FontResolver();

            MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes
                .ImageSource.ImageSourceImpl = new PdfSharpCore.ImageSharp.ImageSharpImageSource<Rgba32>();


            using (PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument())
            {
                document.Info.Title = "Family Tree";
                document.Info.Author = "FamilyTree Ltd. - Stefan Steiger";
                document.Info.Subject = "Family Tree";
                document.Info.Keywords = "Family Tree, Genealogical Tree, Genealogy, Bloodline, Pedigree";


                PdfSharpCore.Pdf.Security.PdfSecuritySettings securitySettings = document.SecuritySettings;
                
                // Setting one of the passwords automatically sets the security level to
                // PdfDocumentSecurityLevel.Encrypted128Bit.
                securitySettings.UserPassword = "user";
                securitySettings.OwnerPassword = "owner";

                // Don't use 40 bit encryption unless needed for compatibility
                //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

                // Restrict some rights.
                securitySettings.PermitAccessibilityExtractContent = false;
                securitySettings.PermitAnnotations = false;
                securitySettings.PermitAssembleDocument = false;
                securitySettings.PermitExtractContent = false;
                securitySettings.PermitFormsFill = true;
                securitySettings.PermitFullQualityPrint = false;
                securitySettings.PermitModifyDocument = true;
                securitySettings.PermitPrint = false;


                document.ViewerPreferences.Direction = PdfSharpCore.Pdf.PdfReadingDirection.LeftToRight;

                PdfSharpCore.Pdf.PdfPage page = document.AddPage();

                // page.Width = PdfSettings.PaperFormatSettings.Width
                // page.Height = PdfSettings.PaperFormatSettings.Height

                const double GOLDEN_RATIO = 1.61803398875;

                // https://en.wikipedia.org/wiki/Golden_ratio
                double marginLeft = 125;
                double marginTop = marginLeft;
                double textBoxWidth = 200;
                double textBoxHeight = textBoxWidth/ GOLDEN_RATIO;

                double textBoxVdistance = textBoxHeight / (GOLDEN_RATIO / (GOLDEN_RATIO * GOLDEN_RATIO));
                double textBoxLargeHdistance = textBoxWidth / (GOLDEN_RATIO*GOLDEN_RATIO);
                double textBoxSmallHdistance = textBoxLargeHdistance / (GOLDEN_RATIO * GOLDEN_RATIO);



                
                int numGenerationsToList = 5;
                int maxGenerationIndex = numGenerationsToList - 1;
                int numItems = (int)System.Math.Pow(2, maxGenerationIndex);


                page.Orientation = PdfSharpCore.PageOrientation.Landscape;

                page.Width = marginLeft * 2
                    + numItems * textBoxWidth
                    + (numItems / 2) * textBoxSmallHdistance
                    + (numItems / 2 - 1) * textBoxLargeHdistance
                ;

                page.Height = marginTop * 2
                    + numGenerationsToList * textBoxHeight
                    + (numGenerationsToList - 1) * textBoxVdistance
                ;






                double dblLineWidth = 1.0;
                string strHtmlColor = "#FF00FF";
                PdfSharpCore.Drawing.XColor lineColor = XColorHelper.FromHtml(strHtmlColor);
                PdfSharpCore.Drawing.XPen pen = new PdfSharpCore.Drawing.XPen(lineColor, dblLineWidth);

                PdfSharpCore.Drawing.XFont font = new PdfSharpCore.Drawing.XFont("Arial"
                        , 12.0, PdfSharpCore.Drawing.XFontStyle.Bold
                );


                using (PdfSharpCore.Drawing.XGraphics gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page))
                {
                    gfx.MUH = PdfSharpCore.Pdf.PdfFontEncoding.Unicode;

                    PdfSharpCore.Drawing.Layout.XTextFormatter tf = new PdfSharpCore.Drawing.Layout.XTextFormatter(gfx);
                    tf.Alignment = PdfSharpCore.Drawing.Layout.XParagraphAlignment.Left;

                    PdfSharpCore.Drawing.Layout.XTextFormatterEx2 etf = new PdfSharpCore.Drawing.Layout.XTextFormatterEx2(gfx);


                    for (int generationNumber = maxGenerationIndex; generationNumber > -1; --generationNumber)
                    {
                        dict[generationNumber] = new System.Collections.Generic.Dictionary<int, DataPoint>();

                        int num = (int)System.Math.Pow(2.0, generationNumber);

                        for (int i = 0; i < num; ++i)
                        {
                            if (generationNumber != maxGenerationIndex)
                            {
                                
                                var dp1 = dict[generationNumber + 1][i * 2];
                                var dp2 = dict[generationNumber + 1][i * 2 + 1];

                                var rect1 = dp1.rect;
                                var rect2 = dp2.rect;


                                double xNew = (rect1.TopLeft.X + rect2.TopRight.X) / 2.0;
                                double yNew = marginTop
                                + generationNumber * textBoxHeight
                                + generationNumber * textBoxVdistance;

                                gfx.DrawLine(pen, xNew, yNew + rect1.Height, rect1.X + rect1.Width/2.0, rect1.Y);
                                gfx.DrawLine(pen, xNew, yNew + rect1.Height, rect2.X + rect2.Width/2.0, rect2.Y);

                                xNew = xNew - rect1.Width / 2.0;

                                dict[generationNumber][i] = new DataPoint()
                                {
                                    Person = (
                                                 from itemList in ti.ls[generationNumber]
                                                 where itemList.Id == dp1.Person.Child
                                                 select itemList
                                             ).FirstOrDefault(),
                                    rect = new DataStructures.Rectangle(xNew, yNew, rect1.Width, rect1.Height)
                                };

                            }
                            else
                            {
                                System.Console.WriteLine($"i: {i}");
                                int numSmallSpaces = (i + 1) / 2;
                                System.Console.WriteLine($"numSmallSpaces: {numSmallSpaces}");
                                int numPairs = i / 2;
                                System.Console.WriteLine($"numPairs: {numPairs}");


                                double rectX = marginLeft
                                    + i * textBoxWidth
                                    + numSmallSpaces * textBoxSmallHdistance
                                    + numPairs * textBoxLargeHdistance
                                ;

                                double rectY = marginTop
                                    + generationNumber * textBoxHeight
                                    + generationNumber * textBoxVdistance
                                ;


                                dict[generationNumber][i] = new DataPoint()
                                {
                                    Person = ti.ls[generationNumber][i],
                                    rect = new DataStructures.Rectangle(rectX, rectY, textBoxWidth, textBoxHeight)
                                };

                            }

                            gfx.DrawRectangle(pen, dict[generationNumber][i].rect.ToXRect());


                            string text = $@"Generation {generationNumber} Person {i}";
                            text = dict[generationNumber][i].Person.composite_name;


                            tf.DrawString(text
                                        , font
                                        , PdfSharpCore.Drawing.XBrushes.Black
                                        , dict[generationNumber][i].rect.ToXRect()
                                        , PdfSharpCore.Drawing.XStringFormats.TopLeft
                            );

                        } // Next i 

                    } // Next generationNumber 


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


    } // End Class 


} // End Namespace 

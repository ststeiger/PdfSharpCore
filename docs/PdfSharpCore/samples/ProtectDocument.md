# Protect Document

This sample shows how to protect a document with a password.


## Code

This is the whole source code needed to create the PDF file:

```cs
// Get a fresh copy of the sample PDF file
const string filenameSource = "HelloWorld.pdf";
const string filenameDest = "HelloWorld_tempfile.pdf";
File.Copy(Path.Combine("../../../../../PDFs/", filenameSource),
Path.Combine(Directory.GetCurrentDirectory(), filenameDest), true);
 
// Open an existing document. Providing an unrequired password is ignored.
PdfDocument document = PdfReader.Open(filenameDest, "some text");
 
PdfSecuritySettings securitySettings = document.SecuritySettings;
 
// Setting one of the passwords automatically sets the security level to
// PdfDocumentSecurityLevel.Encrypted128Bit.
securitySettings.UserPassword  = "user";
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
 
// Save the document...
document.Save(filenameDest);
```

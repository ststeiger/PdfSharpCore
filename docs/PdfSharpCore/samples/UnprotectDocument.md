# Unprotect Document

This sample shows how to unprotect a document (if you know the password).

Note: that we will not explain nor give any tips how to crack a protected document with PdfSharpCore.

## Code

This code shows how to unprotect a document to allow modification:

```cs
// Get a fresh copy of the sample PDF file.
// The passwords are 'user' and 'owner' in this sample.
const string filenameSource = "HelloWorld (protected).pdf";
const string filenameDest = "HelloWorld_tempfile.pdf";
File.Copy(Path.Combine("../../../../../PDFs/", filenameSource),
Path.Combine(Directory.GetCurrentDirectory(), filenameDest), true);
 
PdfDocument document;
 
// Opening a document will fail with an invalid password.
try
{
    document = PdfReader.Open(filenameDest, "invalid password");
}
catch (Exception ex)
{
    Debug.WriteLine(ex.Message);
}
 
// You can specify a delegate, which is called if the document needs a
// password. If you want to modify the document, you must provide the
// owner password.
document = PdfReader.Open(filenameDest, PdfDocumentOpenMode.Modify, PasswordProvider);
 
// Open the document with the user password.
document = PdfReader.Open(filenameDest, "user", PdfDocumentOpenMode.ReadOnly);
 
// Use the property HasOwnerPermissions to decide whether the used password
// was the user or the owner password. In both cases PdfSharpCore provides full
// access to the PDF document. It is up to the programmer who uses PdfSharpCore
// to honor the access rights. PdfSharpCore doesn't try to protect the document
// because this make little sense for an open source library.
bool hasOwnerAccess = document.SecuritySettings.HasOwnerPermissions;
 
// Open the document with the owner password.
document = PdfReader.Open(filenameDest, "owner");
hasOwnerAccess = document.SecuritySettings.HasOwnerPermissions;
 
// A document opened with the owner password is completely unprotected
// and can be modified.
XGraphics gfx = XGraphics.FromPdfPage(document.Pages[0]);
gfx.DrawString("Some text...",
new XFont("Times New Roman", 12), XBrushes.Firebrick, 50, 100);
 
// The modified document is saved without any protection applied.
PdfDocumentSecurityLevel level = document.SecuritySettings.DocumentSecurityLevel;
 
// If you want to save it protected, you must set the DocumentSecurityLevel
// or apply new passwords.
// In the current implementation the old passwords are not automatically
// reused. See 'ProtectDocument' sample for further information.
 
// Save the document...
document.Save(filenameDest);
```

Here's the source code for the password provider:

```cs
/// <summary>
/// The 'get the password' call back function.
/// </summary>
static void PasswordProvider(PdfPasswordProviderArgs args)
{
    // Show a dialog here in a real application
    args.Password = "owner";
}
```  

using PdfSharpCore.Pdf.IO;

namespace PdfSharpCore.Pdf.Security
{
    internal class EncryptorFactory
    {
        public static void Create(PdfDocument doc, PdfDictionary dict, out IEncryptor stringEncryptor, out IEncryptor streamEncryptor)
        {
            stringEncryptor = streamEncryptor = null;

            var filter = dict.Elements.GetName(PdfSecurityHandler.Keys.Filter);
            var v = dict.Elements.GetInteger(PdfSecurityHandler.Keys.V);
            var maxSupportedVersion = 5;
            if (filter != "/Standard" || !(v >= 1 && v <= maxSupportedVersion))
                throw new PdfReaderException(PSSR.UnknownEncryption);
            foreach (var keyName in new []{"/StrF", "/StmF"})
            {
                IEncryptor encryptor = null;
                if (v >= 4)
                {
                    var cf = dict.Elements.GetDictionary(PdfSecurityHandler.Keys.CF);
                    if (cf != null)
                    {
                        var filterName = dict.Elements.GetName(keyName);
                        if (!string.IsNullOrEmpty(filterName))
                        {
                            /*
                             * Pdf Reference 1.7 Chapter 7.6.5 (Crypt Filters)
                             * 
                             * None:  The application shall not decrypt data but shall direct the input stream to the security handler for decryption.
                             * V2:    The application shall ask the security handler for the encryption key and shall implicitly decrypt data with 
                             *        "Algorithm 1: Encryption of data using the RC4 or AES algorithms", using the RC4 algorithm.
                             * AESV2: (PDF 1.6)The application shall ask the security handler for the encryption key and shall implicitly decrypt data with 
                             *        "Algorithm 1: Encryption of data using the RC4 or AES algorithms", using the AES algorithm in Cipher Block 
                             *        Chaining (CBC) mode with a 16-byte block size and an initialization vector that shall be randomly generated and 
                             *        placed as the first 16 bytes in the stream or string.
                             * AESV3: (PDF 1.7, ExtensionLevel 3) The application asks the security handler for the encryption key and implicitly decrypts data with 
                             *        Algorithm 3.1a, using the AES-256 algorithm in Cipher Block Chaining (CBC) with padding mode with a 16-byte block size and an 
                             *        initialization vector that is randomly generated and placed as the first 16 bytes in the stream or string. 
                             *        The key size (Length) shall be 256 bits.
                             */
                            var filterDict = cf.Elements.GetDictionary(filterName);
                            if (filterDict != null)
                            {
                                var cfm = filterDict.Elements.GetName(PdfSecurityHandler.Keys.CFM);
                                if (!string.IsNullOrEmpty(cfm) && cfm.StartsWith("/AESV")) // AESV2(PDF 1.6), AESV3(PDF 1.7, ExtensionLevel 3)
                                    encryptor = new AESEncryptor();
                            }
                        }
                    }
                }
                // default to RC4 encryption
                if (encryptor == null)
                    encryptor = new RC4Encryptor();
                encryptor.Initialize(doc, dict);
                if (keyName == "/StrF")
                    stringEncryptor = encryptor;
                else
                    streamEncryptor = encryptor;
            }
        }
    }
}

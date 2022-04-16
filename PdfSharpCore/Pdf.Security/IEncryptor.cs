namespace PdfSharpCore.Pdf.Security
{
    internal interface IEncryptor
    {
        bool PasswordValid { get; }

        bool HaveOwnerPermission { get; }

        void Initialize(PdfDocument document, PdfDictionary encryptionDict);

        void InitEncryptionKey(string password);

        bool ValidatePassword(string password);

        void SetEncryptionKey(byte[] key);

        void CreateHashKey(PdfObjectID objectId);

        byte[] Encrypt(byte[] bytes);
    }
}
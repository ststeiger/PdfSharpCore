using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PdfSharpCore.Pdf.Security
{
    internal class AESEncryptor : RC4Encryptor
    {
        public override void InitEncryptionKey(string password)
        {
            if (rValue == 5)
            {
                InitVersion5(password);
                return;
            }
            if (rValue == 6)
            {
                // http://esec-lab.sogeti.com/post/The-undocumented-password-validation-algorithm-of-Adobe-Reader-X
                InitVersion6(password);
                return;
            }
            base.InitEncryptionKey(password);
        }

        /// <summary>
        /// Pdf Reference 1.7 Extension Level 3, Chapter 3.5.2, Algorithm 3.2a
        /// </summary>
        /// <param name="password"></param>
        protected void InitVersion5(string password)
        {
            var pwdBytes = Encoding.UTF8.GetBytes(password);
            if (pwdBytes.Length > 127)
                pwdBytes = pwdBytes.Take(127).ToArray();
            // split O and U into their components
            var oHash = new byte[32];
            var oValidation = new byte[8];
            var oSalt = new byte[8];
            var uHash = new byte[32];
            var uValidation = new byte[8];
            var uSalt = new byte[8];

            Array.Copy(ownerValue, oHash, 32);
            Array.Copy(ownerValue, 32, oValidation, 0, 8);
            Array.Copy(ownerValue, 40, oSalt, 0, 8);
            Array.Copy(userValue, uHash, 32);
            Array.Copy(userValue, 32, uValidation, 0, 8);
            Array.Copy(userValue, 40, uSalt, 0, 8);

            computedOwnerValue = new byte[32];
            computedUserValue = new byte[32];

            var oKeyBytes = new byte[pwdBytes.Length + 8 + 48];
            Array.Copy(pwdBytes, oKeyBytes, pwdBytes.Length);
            Array.Copy(oValidation, 0, oKeyBytes, pwdBytes.Length, 8);
            Array.Copy(userValue, 0, oKeyBytes, pwdBytes.Length + 8, 48);

            HaveOwnerPermission = PasswordMatchR5(oKeyBytes, ownerValue);
            if (HaveOwnerPermission)
            {
                PasswordValid = true;
                Array.Copy(ownerValue, computedOwnerValue, 32);
                CreateEncryptionKeyR5(oeValue, pwdBytes, oSalt, userValue);
            }
            else
            {
                oKeyBytes = new byte[pwdBytes.Length + 8];
                Array.Copy(pwdBytes, oKeyBytes, pwdBytes.Length);
                Array.Copy(uValidation, 0, oKeyBytes, pwdBytes.Length, 8);

                // if the result matches the first 32 bytes of userValue, we have the user password
                PasswordValid = PasswordMatchR5(oKeyBytes, userValue);
                if (PasswordValid)
                {
                    Array.Copy(userValue, computedUserValue, 32);
                    CreateEncryptionKeyR5(ueValue, pwdBytes, uSalt, null);
                }
            }
        }

        private void CreateEncryptionKeyR5(byte[] encryptedValue, byte[] password, byte[] salt, byte[] uservalue)
        {
            var sha = SHA256.Create();
            var aes256Cbc = Aes.Create();
            aes256Cbc.KeySize = 256;
            aes256Cbc.Mode = CipherMode.CBC;
            aes256Cbc.Padding = PaddingMode.None;
            var bufLen = password.Length + salt.Length + (uservalue != null ? 48 : 0);
            var buf = new byte[bufLen];
            Array.Copy(password, buf, password.Length);
            Array.Copy(salt, 0, buf, password.Length, salt.Length);
            if (uservalue != null)
                Array.Copy(uservalue, 0, buf, password.Length + salt.Length, 48);
            var shaKey = sha.ComputeHash(buf);
            using (var decryptor = aes256Cbc.CreateDecryptor(shaKey, new byte[16]))
            {
                using (var ms = new MemoryStream(encryptedValue))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        encryptionKey = new byte[32];
                        cs.Read(encryptionKey, 0, 32);
                    }
                }
            }
            aes256Cbc.Clear();
        }

        private void InitVersion6(string password)
        {
            // split O and U into their components
            var oSalt = new byte[8];
            var uSalt = new byte[8];
            var uKeySalt = new byte[8];
            var oKeySalt = new byte[8];
            var oKey = new byte[48];
            Array.Copy(ownerValue, 32, oSalt, 0, 8);
            Array.Copy(userValue, 32, uSalt, 0, 8);
            Array.Copy(userValue, 40, uKeySalt, 0, 8);
            Array.Copy(ownerValue, 40, oKeySalt, 0, 8);
            Array.Copy(userValue, oKey, 48);

            computedUserValue = new byte[32];
            computedOwnerValue = new byte[32];
            ValidateVersion6(password, uSalt, null, computedUserValue);
            ValidateVersion6(password, oSalt, oKey, computedOwnerValue);

            byte[] keyToDecrypt = null;
            byte[] salt = null;
            byte[] hashKey = null;
            if (CompareArrays(computedOwnerValue, ownerValue, 32))
            {
                keyToDecrypt = oeValue;
                salt = oKeySalt;
                hashKey = oKey;
                PasswordValid = true;
                HaveOwnerPermission = true;
            }
            else if (CompareArrays(computedUserValue, userValue, 32))
            {
                keyToDecrypt = ueValue;
                salt = uKeySalt;
                PasswordValid = true;
            }

            if (keyToDecrypt != null)
            {
                encryptionKey = new byte[32];
                var hash = new byte[32];
                var iv = new byte[16];
                ValidateVersion6(password, salt, hashKey, hash);
                using (var aes256 = Aes.Create())
                {
                    aes256.KeySize = 256;
                    aes256.Mode = CipherMode.CBC;
                    aes256.Padding = PaddingMode.None;
                    using (var decryptor = aes256.CreateDecryptor(hash, iv))
                    {
                        decryptor.TransformBlock(keyToDecrypt, 0, 32, encryptionKey, 0);
                    }
                }
            }
        }

        private static void ValidateVersion6(string password, byte[] salt, byte[] ownerKey, byte[] hash)
        {
            var data = new byte[(128 + 64 + 48) * 64];
            var block = new byte[64];
            var blockSize = 32;
            var dataLen = 0;
            int i, j, sum;

            using (var aes128 = Aes.Create())
            {
                aes128.BlockSize = 16 * 8;
                aes128.Mode = CipherMode.CBC;
                var pwdBytes = Encoding.UTF8.GetBytes(password);
                var iv = new byte[16];
                var aesKey = new byte[16];

                /* Step 1: calculate initial data block */
                using (var sha256 = SHA256.Create())
                {
                    sha256.TransformBlock(pwdBytes, 0, pwdBytes.Length, pwdBytes, 0);
                    sha256.TransformBlock(salt, 0, salt.Length, salt, 0);
                    if (ownerKey != null)
                        sha256.TransformBlock(ownerKey, 0, ownerKey.Length, ownerKey, 0);
                    sha256.TransformFinalBlock(salt, 0, 0);
                    Array.Copy(sha256.Hash, block, sha256.HashSize / 8);
                }
                for (i = 0; i < 64 || i < data[dataLen * 64 - 1] + 32; i++)
                {
                    /* Step 2: repeat password and data block 64 times */
                    Array.Copy(pwdBytes, data, pwdBytes.Length);
                    Array.Copy(block, 0, data, pwdBytes.Length, blockSize);
                    if (ownerKey != null)
                        Array.Copy(ownerKey, 0, data, pwdBytes.Length + blockSize, 48);
                    dataLen = pwdBytes.Length + blockSize + (ownerKey != null ? 48 : 0);
                    for (j = 1; j < 64; j++)
                        Array.Copy(data, 0, data, j * dataLen, dataLen);

                    /* Step 3: encrypt data using data block as key and iv */
                    Array.Copy(block, 16, iv, 0, 16);
                    Array.Copy(block, 0, aesKey, 0, 16);
                    using (var aesEnc = aes128.CreateEncryptor(aesKey, iv))
                    {
                        aesEnc.TransformBlock(data, 0, dataLen * 64, data, 0);

                        /* Step 4: determine SHA-2 hash size for this round */
                        for (j = 0, sum = 0; j < 16; j++)
                            sum += data[j];

                        /* Step 5: calculate data block for next round */
                        blockSize = 32 + sum % 3 * 16;
                        HashAlgorithm hashAlg = null;
                        switch (blockSize)
                        {
                            case 32:
                                hashAlg = SHA256.Create();
                                break;
                            case 48:
                                hashAlg = SHA384.Create();
                                break;
                            case 64:
                                hashAlg = SHA512.Create();
                                break;
                        }
                        hashAlg.TransformBlock(data, 0, dataLen * 64, data, 0);
                        hashAlg.TransformFinalBlock(data, 0, 0);
                        Array.Copy(hashAlg.Hash, block, hashAlg.HashSize / 8);
                        hashAlg.Dispose();
                    }
                }
            }
            Array.Copy(block, hash, 32);
        }

        private static bool PasswordMatchR5(byte[] key, byte[] comparand)
        {
            var sha = SHA256.Create();
            var hash = sha.ComputeHash(key);
            for (var i = 0; i < 32; i++)
            {
                if (hash[i] != comparand[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Pdf Reference 1.7, Chapter 7.6.2, Algorithm #1
        /// </summary>
        /// <param name="id"></param>
        public override void CreateHashKey(PdfObjectID id)
        {
            if (rValue >= 5)
            {
                if (key == null || key.Length != encryptionKey.Length)
                    key = new byte[encryptionKey.Length];
                Array.Copy(encryptionKey, key, encryptionKey.Length);
                return;
            }
            var objectId = new byte[5];
            md5.Initialize();
            // Split the object number and generation
            objectId[0] = (byte)id.ObjectNumber;
            objectId[1] = (byte)(id.ObjectNumber >> 8);
            objectId[2] = (byte)(id.ObjectNumber >> 16);
            objectId[3] = (byte)id.GenerationNumber;
            objectId[4] = (byte)(id.GenerationNumber >> 8);
            var salt = new byte[] { 0x73, 0x41, 0x6C, 0x54 };
            var k = new byte[encryptionKey.Length + 9];
            Array.Copy(encryptionKey, k, encryptionKey.Length);
            Array.Copy(objectId, 0, k, encryptionKey.Length, objectId.Length);
            Array.Copy(salt, 0, k, encryptionKey.Length + objectId.Length, salt.Length);
            key = md5.ComputeHash(k);
            md5.Initialize();
            keySize = encryptionKey.Length + 5;
            if (keySize > 16)
                keySize = 16;
        }

        /// <summary>
        /// Decrypts a block of data
        /// </summary>
        /// <param name="bytes">Bytes to decrypt</param>
        /// <returns></returns>
        public override byte[] Encrypt(byte[] bytes)
        {
            // first 16 bytes should be an initialization vector for the encryption
            if (bytes.Length <= 16)
                return bytes;

            var iv = new byte[16];
            Array.Copy(bytes, iv, 16);
            // Pdf Reference 1.7, Section 7.6.2 :
            // "Strings and streams encrypted with AES shall use a padding scheme that is described in Internet RFC 2898, PKCS #5"
            var output = new byte[bytes.Length - 16];
            int dataLength;
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var decryptor = aes.CreateDecryptor(key, iv);
                try
                {
                    var offset = decryptor.TransformBlock(bytes, 16, bytes.Length - 16, output, 0);
                    var suffix = decryptor.TransformFinalBlock(bytes, 0, 0);
                    Array.Copy(suffix, 0, output, offset, suffix.Length);
                    dataLength = offset + suffix.Length;
                }
                catch
                {
                    // return unmodified
                    // (encountered documents that were "partly" encrypted, i.e. everything was encrypted except object-streams)
                    return bytes;
                }
            }
            return output.Take(dataLength).ToArray();
        }
    }
}

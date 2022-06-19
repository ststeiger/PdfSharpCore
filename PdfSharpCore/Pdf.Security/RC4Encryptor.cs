using PdfSharpCore.Pdf.Internal;
using System;

namespace PdfSharpCore.Pdf.Security
{
    class RC4Encryptor : EncryptorBase, IEncryptor
    {
        /// <summary>
        /// Bytes used for RC4 encryption.
        /// </summary>
        readonly byte[] state = new byte[256];

        /// <summary>
        /// Creates the encryption Key.
        /// Based on algorithm #2 (3.2 in Extension Level 3) in the Pdf 1.7 reference (7.6.3.3)
        /// </summary>
        public virtual void InitEncryptionKey(string password)
        {
            var userPad = PadPassword(password);
            md5.Initialize();
            md5.TransformBlock(userPad, 0, userPad.Length, userPad, 0);
            md5.TransformBlock(ownerValue, 0, ownerValue.Length, ownerValue, 0);
            var permission = new byte[4];
            permission[0] = (byte)pValue;
            permission[1] = (byte)(pValue >> 8);
            permission[2] = (byte)(pValue >> 16);
            permission[3] = (byte)(pValue >> 24);
            md5.TransformBlock(permission, 0, 4, permission, 0);
            md5.TransformBlock(documentId, 0, documentId.Length, documentId, 0);
            if (rValue >= 4 && !encryptMetadata)
            {
                var ff = new byte[] { 0xff, 0xff, 0xff, 0xff };
                md5.TransformBlock(ff, 0, ff.Length, ff, 0);
            }
            md5.TransformFinalBlock(permission, 0, 0);
            var hash = md5.Hash;
            if (rValue >= 3)
            {
                for (var i = 0; i < 50; i++)
                {
                    md5.Initialize();
                    hash = md5.ComputeHash(hash, 0, keyLength);
                }
            }
            encryptionKey = new byte[keyLength];
            Array.Copy(hash, encryptionKey, keyLength);
        }

        public bool ValidatePassword(string password)
        {
            // AESEncryptor sets these
            if (HaveOwnerPermission || PasswordValid)
                return true;

            ValidateOwnerPassword(password);
            if (!PasswordValid)
                ValidateUserPassword(password);
            return PasswordValid;

        }

        private void ValidateUserPassword(string password)
        {
            CreateUserKey(password);
            PasswordValid = CompareArrays(computedUserValue, userValue, 16);
        }

        private void ValidateOwnerPassword(string password)
        {
            var pwdPad = PadPassword(password);
            md5.Initialize();
            var pwdKey = md5.ComputeHash(pwdPad);
            if (rValue >= 3)
            {
                for (var i = 0; i < 50; i++)
                {
                    pwdKey = md5.ComputeHash(pwdKey, 0, keyLength);
                }
            }
            var n = rValue <= 2 ? 5 : keyLength;
            var rc4Input = new byte[n];
            Array.Copy(pwdKey, rc4Input, n);

            var ov = new byte[ownerValue.Length];
            Array.Copy(ownerValue, ov, ov.Length);
            if (rValue < 3)
            {
                PrepareRC4Key(rc4Input, 0, n);
                EncryptRC4(ov);
            }
            else
            {
                var xor = new byte[n];
                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < n; j++)
                        xor[j] = (byte)(rc4Input[j] ^ (19 - i));
                    PrepareRC4Key(xor, 0, n);
                    EncryptRC4(ov);
                }
            }
            var userPass = PdfEncoders.RawEncoding.GetString(ov);
            ValidateUserPassword(userPass);
            if (PasswordValid)
                HaveOwnerPermission = true;
        }

        /// <summary>
        /// Pdf Reference 1.7, Chapter 7.6.3.4, Algorithm #3
        /// </summary>
        public void CreateOwnerKey(string password)
        {
            var pwdPad = PadPassword(password);
            md5.Initialize();
            var pwdKey = md5.ComputeHash(pwdPad);
            if (rValue >= 3)
            {
                for (var i = 0; i < 50; i++)
                    pwdKey = md5.ComputeHash(pwdKey);
            }
            var n = rValue <= 2 ? 5 : keyLength;
            var rc4Input = new byte[n];
            Array.Copy(pwdKey, rc4Input, n);
            if (rValue >= 3)
            {
                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < rc4Input.Length; j++)
                        rc4Input[j] = (byte)(rc4Input[j] ^ i);
                    PrepareRC4Key(rc4Input);
                    EncryptRC4(pwdPad);
                }
            }
            computedOwnerValue = new byte[n];
            Array.Copy(pwdPad, computedOwnerValue, n);
        }

        /// <summary>
        /// Pdf Reference 1.7, Chapter 7.6.3.4, Algorithm #4 and #5
        /// </summary>
        public void CreateUserKey(string password)
        {
            InitEncryptionKey(password);
            if (rValue == 2)
            {
                var data = new byte[passwordPadding.Length];
                Array.Copy(passwordPadding, data, data.Length);
                PrepareRC4Key(encryptionKey);
                EncryptRC4(data);
                computedUserValue = new byte[data.Length];
                Array.Copy(data, computedUserValue, data.Length);
            }
            else
            {
                computedUserValue = new byte[32];
                md5.Initialize();
                md5.TransformBlock(passwordPadding, 0, passwordPadding.Length, passwordPadding, 0);
                md5.TransformFinalBlock(documentId, 0, documentId.Length);
                var mkey = md5.Hash;
                Array.Copy(mkey, computedUserValue, mkey.Length);
                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < mkey.Length; j++)
                        mkey[j] = (byte)(encryptionKey[j] ^ i);
                    PrepareRC4Key(mkey);
                    EncryptRC4(computedUserValue, 0, 16);
                }
                for (var i = 16; i < 32; i++)
                    computedUserValue[i] = 0;
            }
        }

        /// <summary>
        /// Pdf Reference 1.7, Chapter 7.6.2, Algorithm #1
        /// </summary>
        /// <param name="id"></param>
        public virtual void CreateHashKey(PdfObjectID id)
        {
            var objectId = new byte[5];
            md5.Initialize();
            // Split the object number and generation
            objectId[0] = (byte)id.ObjectNumber;
            objectId[1] = (byte)(id.ObjectNumber >> 8);
            objectId[2] = (byte)(id.ObjectNumber >> 16);
            objectId[3] = (byte)id.GenerationNumber;
            objectId[4] = (byte)(id.GenerationNumber >> 8);
            md5.TransformBlock(encryptionKey, 0, encryptionKey.Length, encryptionKey, 0);   // ?? incomplete
            md5.TransformFinalBlock(objectId, 0, objectId.Length);
            key = md5.Hash;
            md5.Initialize();
            keySize = encryptionKey.Length + 5;
            if (keySize > 16)
                keySize = 16;
        }

        public virtual byte[] Encrypt(byte[] bytes)
        {
            PrepareRC4Key(key);
            EncryptRC4(bytes);
            return bytes;
        }

        /// <summary>
        /// Prepare the encryption key.
        /// </summary>
        protected void PrepareRC4Key(byte[] key)
        {
            PrepareRC4Key(key, 0, keySize);
        }

        /// <summary>
        /// Prepare the encryption key.
        /// </summary>
        protected void PrepareRC4Key(byte[] key, int offset, int length)
        {
            int idx1 = 0;
            int idx2 = 0;
            for (int idx = 0; idx < 256; idx++)
                this.state[idx] = (byte)idx;
            byte tmp;
            for (int idx = 0; idx < 256; idx++)
            {
                idx2 = (key[idx1 + offset] + this.state[idx] + idx2) & 255;
                tmp = this.state[idx];
                this.state[idx] = this.state[idx2];
                this.state[idx2] = tmp;
                idx1 = (idx1 + 1) % length;
            }
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        protected void EncryptRC4(byte[] data)
        {
            EncryptRC4(data, 0, data.Length, data);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        protected void EncryptRC4(byte[] data, int offset, int length)
        {
            EncryptRC4(data, offset, length, data);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        protected void EncryptRC4(byte[] inputData, byte[] outputData)
        {
            EncryptRC4(inputData, 0, inputData.Length, outputData);
        }

        /// <summary>
        /// Encrypts the data.
        /// </summary>
        protected void EncryptRC4(byte[] inputData, int offset, int length, byte[] outputData)
        {
            length += offset;
            int x = 0, y = 0;
            byte b;
            for (int idx = offset; idx < length; idx++)
            {
                x = (x + 1) & 255;
                y = (this.state[x] + y) & 255;
                b = this.state[x];
                this.state[x] = this.state[y];
                this.state[y] = b;
                outputData[idx] = (byte)(inputData[idx] ^ state[(this.state[x] + this.state[y]) & 255]);
            }
        }

    }
}

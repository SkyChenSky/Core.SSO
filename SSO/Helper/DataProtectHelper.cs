using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SSO.Helper
{
    public static class AESHelper
    {
        private static readonly byte[] SaltBytes = { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

        public static byte[] Encrypt(byte[] bytesToBeEncrypted, string key)
        {
            return Excute(bytesToBeEncrypted, key, OpType.Encrypt);
        }

        public static byte[] Decrypt(byte[] bytesToBeDecrypted, string key)
        {
            return Excute(bytesToBeDecrypted, key, OpType.Decrypt);
        }

        private static byte[] Excute(byte[] inputBytes, string key, OpType opType)
        {
            byte[] outputBytes;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    SetAesOption(aes, passwordBytes);

                    using (var cs = new CryptoStream(ms, CreateCryptoTransform(opType, aes), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                    }

                    outputBytes = ms.ToArray();
                }
            }

            return outputBytes;
        }

        private static void SetAesOption(RijndaelManaged aes, byte[] passwordBytes)
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;

            var keyByte = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, 1000);
            aes.Key = keyByte.GetBytes(32);
            aes.IV = keyByte.GetBytes(16);

            aes.Mode = CipherMode.CBC;
        }

        private static ICryptoTransform CreateCryptoTransform(OpType opType, RijndaelManaged rijndaelManaged)
        {
            switch (opType)
            {
                case OpType.Decrypt:
                    return rijndaelManaged.CreateDecryptor();
                case OpType.Encrypt:
                    return rijndaelManaged.CreateEncryptor();
                default: throw new Exception("enkonw type");
            }
        }

    }

    internal enum OpType
    {
        Decrypt,
        Encrypt
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SSO.Web.Core.Helper
{
    public static class AESHelper
    {
        public static byte[] Encrypt(byte[] bytesToBeEncrypted, string key)
        {
            byte[] encryptedBytes;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var saltBytes = new byte[] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var keyByt = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = keyByt.GetBytes(32);
                    aes.IV = keyByt.GetBytes(16);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] Decrypt(byte[] bytesToBeDecrypted, string key)
        {
            byte[] decryptedBytes;

            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var saltBytes = new byte[9] { 13, 34, 27, 67, 189, 255, 104, 219, 122 };

            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var keyByte = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = keyByte.GetBytes(32);
                    aes.IV = keyByte.GetBytes(16);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}

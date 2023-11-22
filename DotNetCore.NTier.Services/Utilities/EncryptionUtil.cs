using System.Security.Cryptography;
using System.Text;

namespace DotNetCore.NTier.Services
{
    public class EncryptionUtil
    {
        private static readonly string _keyStr = "RandomGenUTF16Code";
        private static readonly string _ivStr = "RandomGenUTF16Code";

        public static string EncryptString(string text)
        {
            var key = Encoding.UTF8.GetBytes(_keyStr);
            var iv = Encoding.UTF8.GetBytes(_ivStr);

            using var aesAlg = Aes.Create();
            using var encryptor = aesAlg.CreateEncryptor(key, iv);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            var decryptedContent = msEncrypt.ToArray();

            var result = new byte[iv.Length + decryptedContent.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(_keyStr);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key, iv);
            string result;
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                result = srDecrypt.ReadToEnd();
            }

            return result;
        }

        public static string HashString(string text)
        {
            using SHA512 mySHA512 = SHA512.Create();
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hashValue = mySHA512.ComputeHash(data);

            return ByteArrayToString(hashValue);
        }

        public static string HashBase64String(string text)
        {
            using SHA512 mySHA512 = SHA512.Create();
            byte[] data = Encoding.UTF8.GetBytes(text);
            byte[] hashValue = mySHA512.ComputeHash(data);

            return Convert.ToBase64String(hashValue);
        }

        private static string ByteArrayToString(byte[] array)
        {
            var result = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                result.Append($"{array[i]:X2}");
                //if ((i % 4) == 3) result.Append(" ");
            }

            return result.ToString();
        }
    }
}

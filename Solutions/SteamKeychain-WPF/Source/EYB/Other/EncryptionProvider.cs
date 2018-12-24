using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EYB
{
    public interface IEncryptionProvider
    {
        string Encrypt(string toEncrypt);
        string Decrypt(string toDecrypt);
    }

    public sealed class EncryptionProvider : IEncryptionProvider
    {
        private byte[] _key = GetBytes("2DhQmf0H3VaCyazz");
        private byte[] _iv = GetBytes("TcuN9bmOWkjBWlsE");

        /// <summary>
        /// Both the key and the iv must be non null, and measure either 8, 16 or 32 bytes
        /// </summary>
        public EncryptionProvider(string key, string iv)
        {
            _key = GetBytes(key);
            _iv = GetBytes(iv);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string Encrypt(string toEncrypt)
        {
            // Encode data string to be stored in memory.
            var originalStrAsBytes = Encoding.ASCII.GetBytes(toEncrypt);
            byte[] originalBytes = { };

            using (var rijndael = new RijndaelManaged())
            using (var memStream = new MemoryStream(originalStrAsBytes.Length))
            using (ICryptoTransform rdTransform = rijndael.CreateEncryptor((byte[])_key.Clone(), (byte[])_iv.Clone()))
            using (var cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Write))
            {
                // Write encrypted data to the MemoryStream.
                cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
                cryptoStream.FlushFinalBlock();
                originalBytes = memStream.ToArray();
            }

            // Convert encrypted string.
            return Convert.ToBase64String(originalBytes);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string Decrypt(string toDecrypt)
        {
            // Unconvert encrypted string.
            var encryptedStrAsBytes = Convert.FromBase64String(toDecrypt);
            var initialText = new byte[encryptedStrAsBytes.Length];

            using (var rijndael = new RijndaelManaged())
            using (var memStream = new MemoryStream(encryptedStrAsBytes))
            using (ICryptoTransform rdTransform = rijndael.CreateDecryptor((byte[])_key.Clone(), (byte[])_iv.Clone()))
            using (var cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Read))
            {
                // Read in decrypted string as a byte[].
                cryptoStream.Read(initialText, 0, initialText.Length);
            }

            return Encoding.ASCII.GetString(initialText);
        }

        private static byte[] GetBytes(string value)
        {
            if (string.IsNullOrEmpty(value) || (value.Length != 8 && value.Length != 16 && value.Length != 32))
            {
                throw new ArgumentException("Both the key and the iv must be non null, and measure either 8, 16 or 32 bytes");
            }

            return Encoding.UTF8.GetBytes(value);
        }
    }
}
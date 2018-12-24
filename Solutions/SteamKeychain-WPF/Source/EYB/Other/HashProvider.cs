using System;
using System.Security.Cryptography;
using System.Text;

namespace EYB
{
    public static class HashProvider
    {
        public static string Md5(string value, int iterations = 1)
        {
            using (var provider = MD5.Create())
            {
                return GenericHash(provider, value, iterations);
            }
        }

        public static string Sha256(string value, int iterations = 1)
        {
            using (var provider = new SHA256Managed())
            {
                return GenericHash(provider, value, iterations);
            }
        }

        public static string ToBase64(string value, int iterations = 1)
        {
            var result = value;
            byte[] bytes;

            while (iterations-- > 0)
            {
                bytes = Encoding.UTF8.GetBytes(result);
                result = Convert.ToBase64String(bytes);
            }

            return result;
        }

        public static string FromBase64(string value, int iterations = 1)
        {
            var result = value;
            byte[] bytes;

            while (iterations-- > 0)
            {
                bytes = Convert.FromBase64String(result);
                result = Encoding.UTF8.GetString(bytes);
            }

            return result;
        }

        private static string GenericHash(HashAlgorithm provider, string value, int iterations)
        {
            var newValue = value;
            var builder = new StringBuilder();

            for (var i = 0; i < iterations; i++)
            {
                foreach (var b in provider.ComputeHash(Encoding.UTF8.GetBytes(newValue)))
                {
                    builder.Append(b.ToString("x2"));
                }

                newValue = builder.ToString();
                builder.Length = 0; // Clear()
            }

            return newValue;
        }
    }
}
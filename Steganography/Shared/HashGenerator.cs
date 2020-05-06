using System.Security.Cryptography;
using System.Text;

namespace Steganography.Shared
{
    class HashGenerator
    {
        /// <summary>
        /// Generate SHA256 hash for given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateUniqueValue256(string input)
        {
            SHA256 shaHash = SHA256.Create();
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
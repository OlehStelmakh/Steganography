using System;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Steganography.Models;
using Steganography.Shared;

namespace Steganography
{
    public class RijndaelAlgorithm
    {

        public RijndaelAlgorithm(ImageInfo imageInfo)
        {
            GenereteKeyAndIV(imageInfo);
        }

        private static byte[] Key = new byte[32];
        private static byte[] IV = new byte[16];

        public byte[] Encrypt(string textForEncrypt)
        {
            byte[] encrypted = EncryptStringToBytes(textForEncrypt, Key, IV);
            return encrypted;
        }

        public string Decrypt(byte[] textForDecrypt)
        {
            string decrypted = DecryptStringFromBytes(textForDecrypt, Key, IV);
            return decrypted;
        }

        public static void GenereteKeyAndIV(ImageInfo imageInfo)
        {
            //key 
            Pixel pixel1 = new Pixel(1,1, imageInfo.Pixels[1,1]);
            Pixel pixel2 = new Pixel(imageInfo.Height - 1, imageInfo.Width-1, imageInfo.Pixels[imageInfo.Height - 1, imageInfo.Width - 1]);
            string input = pixel1.Color.Name + pixel2.Color.Name;
            string hashKey = HashGenerator.GenerateUniqueValue256(input);
            Array.Copy(Encoding.ASCII.GetBytes(hashKey), Key, 32);

            Pixel pixel3 = new Pixel(1, imageInfo.Width - 1, imageInfo.Pixels[1, imageInfo.Width - 1]);
            Pixel pixel4 = new Pixel(imageInfo.Height - 1, 1, imageInfo.Pixels[imageInfo.Height - 1, 1]);
            input = pixel3.Color.Name + pixel4.Color.Name;
            hashKey = HashGenerator.GenerateUniqueValue256(input);
            Array.Copy(Encoding.ASCII.GetBytes(hashKey), IV, 16);
            Console.WriteLine();
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {

            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
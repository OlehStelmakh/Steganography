using Steganography.Models;
using Steganography.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Encrypt
{
    class ImageEncoder
    {
        private readonly ImageInfo _imageInformation;
        public ImageEncoder(ImageInfo imageInfo)
        {
            _imageInformation = imageInfo;
        }

        public Pixel GetFirstRandomCoordinates()
        {
            Random random = new Random();
            int x = random.Next(_imageInformation.Width);
            int y = random.Next(_imageInformation.Height);
            Color color = _imageInformation.Pixels[y, x];
            Pixel coordinates = new Pixel(x, y, color);
            return coordinates;
        }

        public int CalcuteOffset(Pixel firstCoordinates)
        {
            int offsetCount = 0;
            for (int i = 0; i <= firstCoordinates.Y; i++)
            {
                int predicate = _imageInformation.Width;
                if (i == firstCoordinates.Y)
                {
                    predicate = firstCoordinates.X + 1;
                }
                for (int j = 0; j < predicate; j++)
                {
                    if (_imageInformation.Pixels[i, j].A == firstCoordinates.Color.A &&
                        _imageInformation.Pixels[i, j].R == firstCoordinates.Color.R &&
                        _imageInformation.Pixels[i, j].G == firstCoordinates.Color.G &&
                        _imageInformation.Pixels[i, j].B == firstCoordinates.Color.B)
                    {

                        offsetCount++;
                    }
                }
            }
            return offsetCount;
        }

        public Dictionary<char, List<Pixel>> GetAllColors(string text, Pixel firstCoordinates)
        {
            Dictionary<char, List<Pixel>> keyValues = new Dictionary<char, List<Pixel>>();

            Pixel previousCoordinates = firstCoordinates;
            for (int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];
                previousCoordinates = ImageProcessing.GetNextCoordinates(previousCoordinates, _imageInformation);
                previousCoordinates.UniqueNumber = i + 1;
                if (keyValues.ContainsKey(symbol))
                {
                    keyValues[symbol].Add(previousCoordinates);
                }
                else
                {
                    keyValues.Add(symbol, new List<Pixel>() { previousCoordinates });
                }
            }
            return keyValues;
        }

        public Dictionary<char, List<Pixel>> AddUniqueNumberToPixel(Dictionary<char, List<Pixel>> symbolsAndPixelsInfo)
        {
            List<Pixel> pixels = new List<Pixel>();
            foreach (var symbolAndPixels in symbolsAndPixelsInfo)
            {
                pixels.AddRange(symbolAndPixels.Value);
            }

            for (int i = 0; i < pixels.Count; i++)
            {
                pixels[i].UniqueNumber = i + 1;
            }

            return symbolsAndPixelsInfo;
        }

        public Dictionary<char, List<string>> CreateOutputHashesInfo(Dictionary<char, List<Pixel>> symbolsAndColors)
        {

            Dictionary<char, List<string>> symbolsAndHashes = new Dictionary<char, List<string>>();

            foreach (var symbolWithColors in symbolsAndColors)
            {
                symbolsAndHashes.Add(symbolWithColors.Key, new List<string>());
                foreach (var colorCoord in symbolWithColors.Value)
                {
                    string input = colorCoord.X + colorCoord.Color.Name + colorCoord.Y + colorCoord.UniqueNumber;
                    string hash = HashGenerator.GenerateUniqueValue256(input);
                    symbolsAndHashes[symbolWithColors.Key].Add(hash);
                }
            }

            return symbolsAndHashes;
        }

        public Dictionary<char, List<string>> GenerateSymbolNoise(Dictionary<char, List<Pixel>> symbolsAndColors)
        {
            Dictionary<char, List<string>> noiseSymbolsAndHashes = new Dictionary<char, List<string>>();

            HashSet<char> existingSymbols = new HashSet<char>(symbolsAndColors.Select(x => x.Key));
            int maxAmountOfColors = symbolsAndColors.Max(x => x.Value.Count) + 2;
            HashSet<char> ASCIIchars = new HashSet<char>();
            for (int i = 33; i < 128; i++)
            {
                ASCIIchars.Add((char)i);
            }
            ASCIIchars.ExceptWith(existingSymbols);
            Random random = new Random();
            foreach (var symbol in ASCIIchars)
            {
                noiseSymbolsAndHashes.Add(symbol, new List<string>());
                int hashesAmount = random.Next(1, maxAmountOfColors);
                for (int i = 0; i < hashesAmount; i++)
                {
                    string input = Convert.ToString(i, 2) + new string(symbol, 8) + Convert.ToString(i, 2);
                    string hash = HashGenerator.GenerateUniqueValue256(input);
                    noiseSymbolsAndHashes[symbol].Add(hash);
                }
            }

            return noiseSymbolsAndHashes;
        }
    }

    
}

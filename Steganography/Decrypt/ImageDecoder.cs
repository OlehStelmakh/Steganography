using Steganography.Models;
using Steganography.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Decrypt
{
    class ImageDecoder
    {
        private readonly ParsedData _parsedData;
        private readonly ImageInfo _imageInfo;

        public ImageDecoder(ParsedData parsedData, ImageInfo imageInfo)
        {
            _parsedData = parsedData;
            _imageInfo = imageInfo;
        }

        public Pixel GetCoordinatesOfFirst()
        {
            int count = 1;
            Pixel pixel = new Pixel();

            for (int i = 0; i < _imageInfo.Height; i++)
            {
                for (int j = 0; j < _imageInfo.Width; j++)
                {
                    if (_parsedData.FirstColor.A == _imageInfo.Pixels[i, j].A &&
                        _parsedData.FirstColor.R == _imageInfo.Pixels[i, j].R &&
                        _parsedData.FirstColor.G == _imageInfo.Pixels[i, j].G &&
                        _parsedData.FirstColor.B == _imageInfo.Pixels[i, j].B)
                    {
                        if (_parsedData.Offset == count)
                        {
                            return new Pixel(j, i, _parsedData.FirstColor);
                        }
                        count++;
                    }
                }
            }

            return pixel;
        }

        public string DecryptText(Pixel firstCoordinates)
        {
            StringBuilder stringBuilder = new StringBuilder(_parsedData.LengthOfText);
            Pixel previousCoordinates = firstCoordinates;
            for (int i = 0; i < _parsedData.LengthOfText; i++)
            {
                previousCoordinates = ImageProcessing.GetNextCoordinates(previousCoordinates, _imageInfo);
                previousCoordinates.UniqueNumber = i + 1;
                stringBuilder.Append(FindSymbolInHashes(_parsedData, previousCoordinates));
            }

            return stringBuilder.ToString();
        }

        private char FindSymbolInHashes(ParsedData parsedData, Pixel pixel)
        {
            string input = pixel.X + pixel.Color.Name + pixel.Y + pixel.UniqueNumber;
            string hash = HashGenerator.GenerateUniqueValue256(input);

            foreach (var symbolAndHashes in parsedData.SymbolsAndHashes)
            {
                var values = parsedData.SymbolsAndHashes[symbolAndHashes.Key];
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] == hash)
                    {
                        return symbolAndHashes.Key;
                    }
                }
            }
            throw new Exception("Hash not found");
        }
    }
}

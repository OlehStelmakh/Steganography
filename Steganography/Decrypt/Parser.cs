using System;
using System.Collections.Generic;
using System.Drawing;

namespace Steganography
{
    public class Parser
    {

        private string[] arrayOfLines { get; set; }

        public Parser(string allInfo)
        {
            string[] separator = { "\n" };
            arrayOfLines = allInfo.Split(separator, StringSplitOptions.None);
        }

        public Color GetColorFromString()
        {
            string colorName = arrayOfLines[0].Substring(0, 8);
            int A = Convert.ToInt32(colorName.Substring(0, 2), 16);
            int R = Convert.ToInt32(colorName.Substring(2, 2), 16);
            int G = Convert.ToInt32(colorName.Substring(4, 2), 16);
            int B = Convert.ToInt32(colorName.Substring(6, 2), 16);
            return Color.FromArgb(A, R, G, B);
        }

        public int GetOffsetOfcolor()
        {
            string offset = arrayOfLines[0].Substring(8);
            return Convert.ToInt32(offset);
        }

        public Dictionary<char, List<string>> GetInfoAboutAllSymbols()
        {

            Dictionary<char, List<string>> symbolsAndHashes =
                new Dictionary<char, List<string>>(arrayOfLines.Length - 1);

            for (int i = 1; i < arrayOfLines.Length - 1; i++)
            {
                string[] symbolAndHashes = arrayOfLines[i].Split(' ');
                char symbol;
                if (i == 1)
                {
                    symbol = ' ';
                }
                else
                {
                    symbol = symbolAndHashes[0][0];
                }
                symbolsAndHashes.Add(symbol, new List<string>());
                for (int j = 1; j < symbolAndHashes.Length; j++)
                {
                    if (!string.IsNullOrWhiteSpace(symbolAndHashes[j]))
                    {
                        symbolsAndHashes[symbol].Add(symbolAndHashes[j]);
                    }
                }
            }

            return symbolsAndHashes;
        }

        public int GetLengthOfText()
        {
            string length = arrayOfLines[arrayOfLines.Length - 1];
            return Convert.ToInt32(length);
        }

    }
}
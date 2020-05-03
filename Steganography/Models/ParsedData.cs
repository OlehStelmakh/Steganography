using System.Collections.Generic;
using System.Drawing;

namespace Steganography.Models
{
    public class ParsedData
    {
        public Color FirstColor { get; set; }

        public int Offset { get; set; }

        public Dictionary<char, List<string>> SymbolsAndHashes { get; set; }

        public int LengthOfText { get; set; }

        public ParsedData(Color firstColor, int offset, Dictionary<char, List<string>> symbolsAndHashes,
            int lengthOfText)
        {
            FirstColor = firstColor;
            Offset = offset;
            SymbolsAndHashes = symbolsAndHashes;
            LengthOfText = lengthOfText;
        }
    }
}
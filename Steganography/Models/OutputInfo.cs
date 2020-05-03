using System.Collections.Generic;

namespace Steganography.Models
{
    public class OutputInfo
    {
        public Pixel FirstCoordinates { get; set; }

        public int Offset { get; set; }

        public Dictionary<char, List<string>> SymbolsAndHashes { get; set; }

        public Dictionary<char, List<string>> NoiseSymbols { get; set; }

        public int LengthOfText { get; set; }

        public OutputInfo(Pixel first, int offset, Dictionary<char, List<string>> symbolsAndHashes,
            Dictionary<char, List<string>> noiseSymbols, int lengthOfText)
        {
            FirstCoordinates = first;
            Offset = offset;
            SymbolsAndHashes = symbolsAndHashes;
            NoiseSymbols = noiseSymbols;
            LengthOfText = lengthOfText;
        }
    }
}
﻿using Steganography.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Steganography.Encrypt
{
    class OutputProcessing
    {
        private readonly OutputInfo _outputInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="outputInfo"></param>
        public OutputProcessing(OutputInfo outputInfo)
        {
            _outputInfo = outputInfo;
        }

        /// <summary>
        /// Generate output string that will be encoded by Rijndael algorithm
        /// </summary>
        /// <returns></returns>
        public string CreateOutputString()
        {
            StringBuilder stringBuilder = new StringBuilder(10000);
            var commonInfo = new Dictionary<char, List<string>>(_outputInfo.NoiseSymbols);
            foreach (var instance in _outputInfo.SymbolsAndHashes)
            {
                commonInfo.Add(instance.Key, instance.Value);
            }
            commonInfo = commonInfo.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

            stringBuilder.Append(_outputInfo.FirstCoordinates.Color.Name);
            stringBuilder.Append(_outputInfo.Offset + "\n");
            foreach (var info in commonInfo)
            {
                stringBuilder.Append(info.Key + " ");
                for (int i = 0; i < info.Value.Count; i++)
                {
                    stringBuilder.Append(info.Value[i] + " ");
                }
                stringBuilder.Append("\n");
            }
            stringBuilder.Append(_outputInfo.LengthOfText);
            return stringBuilder.ToString();
        }
    }
}
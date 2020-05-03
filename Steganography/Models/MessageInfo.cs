using System.Collections.Generic;

namespace Steganography.Models
{
    public class MessageInfo
    {
        public MessageInfo(string text, SortedDictionary<char, int> data)
        {
            Text = text;
            MessageData = data;
        }

        public string Text { get; }
        public SortedDictionary<char, int> MessageData { get; }
    }
}

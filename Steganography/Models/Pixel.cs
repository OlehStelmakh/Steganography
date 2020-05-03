using System.Drawing;

namespace Steganography.Models
{
    public class Pixel
    {
        public int X { get; }
        public int Y { get; }
        public Color Color { get; }

        public int UniqueNumber { get; set; } = 1;

        public Pixel(int x, int y, Color color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public Pixel()
        {

        }
    }
}
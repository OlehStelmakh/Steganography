using System.Drawing;
using Color = System.Drawing.Color;

namespace Steganography.Models
{
    public class ImageInfo
    {
        public ImageInfo(Image imageSource, Bitmap bitmap, string path)
        {
            ImageSource = imageSource;
            Bitmap = bitmap;
            Height = bitmap.Height;
            Width = bitmap.Width;
            Path = path;
            CashedBitmap = (Bitmap)bitmap.Clone();
        }
        public Image ImageSource { set; get; }
        public Bitmap Bitmap { set; get; }
        public int Height { set; get; }
        public int Width { set; get; }
        public Color[,] Pixels { set; get; }
        public string Path { set; get; }
        public Bitmap CashedBitmap { set; get; }

    }
}

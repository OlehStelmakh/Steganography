using Steganography.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steganography.Shared
{
    class ImageProcessing
    {
        public static Bitmap Array2DToBitmap(int[,] integers)
        {
            int width = integers.GetLength(0);
            int height = integers.GetLength(1);

            int stride = width * 4;

            Bitmap bitmap = null;

            unsafe
            {
                fixed (int* intPtr = &integers[0, 0])
                {
                    bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppRgb, new IntPtr(intPtr));
                }
            }

            return bitmap;
        }

        public static Color[,] BitmapToArray2D(Bitmap image)
        {
            Color[,] array2D = new Color[image.Height, image.Width];

            BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            unsafe
            {
                byte* address = (byte*)bitmapData.Scan0;

                int paddingOffset = bitmapData.Stride - (image.Width * 4);

                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        byte[] temp = new byte[4];
                        temp[0] = address[0];
                        temp[1] = address[1];
                        temp[2] = address[2];
                        temp[3] = address[3];
                        Color color = Color.FromArgb(temp[3], temp[2], temp[1], temp[0]);

                        array2D[i, j] = color;
                        address += 4;
                    }

                    address += paddingOffset;
                }
            }
            image.UnlockBits(bitmapData);

            return array2D;
        }

        public static Pixel GetNextCoordinates(Pixel previousCoordinates, ImageInfo imageInfo)
        {
            int maxX = imageInfo.Width;
            int maxY = imageInfo.Height;

            string joinedInfo =
                Convert.ToString(previousCoordinates.X, 2) +
                Convert.ToString(previousCoordinates.Y, 2) +
                Convert.ToString(Convert.ToInt64(previousCoordinates.Color.Name, 16), 2);
            int length = joinedInfo.Length;
            string twoBytesX = string.Empty;
            string twoBytesY = string.Empty;

            int capacity = 16;

            if (length >= capacity * 2)
            {
                twoBytesX = joinedInfo.Substring(0, capacity);
                twoBytesY = joinedInfo.Substring(length - capacity, capacity);
            }
            else
            {
                twoBytesX = joinedInfo.Substring(0, length / 2);
                twoBytesY = joinedInfo.Substring(length / 2, length - length / 2);
            }

            int l1 = Convert.ToInt32(twoBytesX.ToString(), 2);
            int l2 = Convert.ToInt32(twoBytesY.ToString(), 2);

            int X = Convert.ToInt32(l1 | l2);
            int Y = Convert.ToInt32(l1 ^ l2);

            while (X >= maxX)
            {
                X -= maxX;
            }

            while (Y >= maxY)
            {
                Y -= maxY;
            }

            return new Pixel(X, Y, imageInfo.Bitmap.GetPixel(X, Y));

        }
    }
}

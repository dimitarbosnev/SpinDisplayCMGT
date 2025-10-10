using System;
using System.Collections.Generic;
using System.Text;

namespace SpinDisplayCMGT
{
    public struct RawImage
    {
        public int Width;
        public int Height;
        public byte[] Pixels; // Typically RGBA8

        public RawImage(int width, int height, byte[] pixels)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
        }

        public RawImage(int Size, byte[] pixels)
        {
            Width = Size;
            Height = Size;
            Pixels = pixels;
        }
    }

}

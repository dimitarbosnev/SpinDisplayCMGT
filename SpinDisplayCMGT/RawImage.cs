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

        public void SaveAsBmp(string path)
        {
            int bytesPerPixel = 4;
            int stride = Width * bytesPerPixel;
            int fileHeaderSize = 14;
            int infoHeaderSize = 40;
            int fileSize = fileHeaderSize + infoHeaderSize + stride * Height;

            using (var fs = new FileStream(path, FileMode.Create))
            using (var bw = new BinaryWriter(fs))
            {
                // BITMAPFILEHEADER (14 bytes)
                bw.Write((ushort)0x4D42);              // 'BM'
                bw.Write(fileSize);                   // file size
                bw.Write((ushort)0);                  // reserved1
                bw.Write((ushort)0);                  // reserved2
                bw.Write(fileHeaderSize + infoHeaderSize); // pixel data offset

                // BITMAPINFOHEADER (40 bytes)
                bw.Write(infoHeaderSize);             // header size
                bw.Write(Width);
                bw.Write(-Height);                    // top-down bitmap (negative)
                bw.Write((ushort)1);                  // planes
                bw.Write((ushort)(bytesPerPixel * 8));// bits per pixel
                bw.Write(0);                          // compression (BI_RGB)
                bw.Write(stride * Height);            // image size
                bw.Write(0);                          // x pixels per meter
                bw.Write(0);                          // y pixels per meter
                bw.Write(0);                          // colors used
                bw.Write(0);                          // important colors

                // Pixel data (BGRA)
                bw.Write(Pixels);
            }
        }

        public void SaveAsJpeg(string path)
        {
            TurboLibjpeg.BytesSaveToJpeg(Pixels, Width, Height, path, 30);
        }

    }
}

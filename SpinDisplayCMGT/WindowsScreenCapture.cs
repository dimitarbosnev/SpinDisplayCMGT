using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SpinDisplayCMGT
{
    public static class WindowsScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        private const int SRCCOPY = 0x00CC0020;

        public static RawImage CaptureScreen(int width, int height)
        {
            IntPtr desktopWnd = GetDesktopWindow();
            IntPtr desktopDC = GetDC(desktopWnd);
            IntPtr memoryDC = CreateCompatibleDC(desktopDC);

            int screenWidth = GetSystemMetrics(0);
            int scrrenHeight = GetSystemMetrics(1);

            IntPtr hBitmap = CreateCompatibleBitmap(desktopDC, width, height);
            SelectObject(memoryDC, hBitmap);

            BitBlt(memoryDC, 0, 0, width, height, desktopDC, 0, 0, SRCCOPY);

            // Copy bitmap bits into managed memory
            var bmpInfo = new BITMAPINFOHEADER
            {
                biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER)),
                biWidth = width,
                biHeight = -height, // top-down bitmap
                biPlanes = 1,
                biBitCount = 32,
                biCompression = 0
            };

            byte[] pixels = new byte[width * height * 4];
            GetDIBits(memoryDC, hBitmap, 0, (uint)height, pixels, ref bmpInfo, 0);

            // Cleanup
            DeleteObject(hBitmap);
            DeleteDC(memoryDC);
            ReleaseDC(desktopWnd, desktopDC);

            return new RawImage(width, height, pixels);
        }

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
            uint cScanLines, byte[] lpvBits, ref BITMAPINFOHEADER lpbi, uint uUsage);

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }
    }
}

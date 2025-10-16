using System.Drawing;
using System.Runtime.InteropServices;

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


        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("gdi32.dll")]
        static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint uStartScan,
        uint cScanLines, IntPtr lpvBits, [In, Out] ref BITMAPINFOHEADER lpbi, uint uUsage);




        private const int SRCCOPY = 0x00CC0020;
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        public static Size size;
        public static Point upperSrc;
        public static Point upperDst;
        static WindowsScreenCapture()
        {
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            Console.WriteLine(string.Format("width:{0},height:{1}", screenWidth, screenHeight));
            int num = (screenWidth > screenHeight) ? screenHeight : screenWidth;
            num = ((num > 672) ? 672 : num);
            upperSrc = new Point((screenWidth - num) / 2, (screenHeight - num) / 2);
            upperDst = new Point(0, 0);
            Console.WriteLine($"InternalRes: {num}");
            size = new Size(num, num);
        }
        public static RawImage CaptureScreenRawImage()
        {
            IntPtr hScreenDC = GetDC(IntPtr.Zero);
            IntPtr hMemDC = CreateCompatibleDC(hScreenDC);

            IntPtr hBitmap = CreateCompatibleBitmap(hScreenDC, size.Width, size.Height);
            IntPtr hOld = SelectObject(hMemDC, hBitmap);

            BitBlt(hMemDC, 0, 0, size.Width, size.Height, hScreenDC, 0, 0, SRCCOPY);

            SelectObject(hMemDC, hOld);

            // copy bitmap bits into memory
            var bmpInfo = new BITMAPINFOHEADER();
            bmpInfo.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmpInfo.biWidth = size.Width;
            bmpInfo.biHeight = -size.Height; // flip vertically
            bmpInfo.biPlanes = 1;
            bmpInfo.biBitCount = 24;
            bmpInfo.biCompression = 0; // BI_RGB

            int bytes = size.Width * size.Height * 3;
            byte[] data = new byte[bytes];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            GetDIBits(hMemDC, hBitmap, 0, (uint)size.Height, handle.AddrOfPinnedObject(), ref bmpInfo, 0);
            handle.Free();

            DeleteObject(hBitmap);
            DeleteDC(hMemDC);
            ReleaseDC(IntPtr.Zero, hScreenDC);

            return new RawImage(size.Width,size.Height, data);
        }

        public static Bitmap CaptureScreenBitmp()
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(upperSrc, upperDst, size);
            graphics.Dispose();
            return bitmap;
        }



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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace SpinDisplayCMGT
{
    public static class TurboLibjpeg
    {

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tjDestroy")]
        private static extern int tjDestroy(IntPtr handle);

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tjInitCompress")]
        private static extern IntPtr tjInitCompress();

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tjCompress2")]
        private static extern int tjCompressInternal(IntPtr handle, IntPtr rgbBuf, int width, int pitch, int height, int pixelFormat, ref IntPtr jpegBuf, ref ulong jpegSize, int jpegSubsamp, int jpegQual, int flags);

        [DllImport("turbojpeg.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "tjFree")]
        private static extern void tjFree(IntPtr buffer);

        public unsafe static byte[] bytesToJpeg(byte[] bytes, int width, int height, int quality)
        {
            int pitch = width * PixelSizes[TJPixelFormats.TJPF_BGR];
            IntPtr handle = tjInitCompress();
            IntPtr rgbBuf = IntPtr.Zero;
            fixed (byte* ptr = &bytes[0])
            {
                void* value = (void*)ptr;
                rgbBuf = (IntPtr)value;
            }
            byte[] result = tjCompress(handle, rgbBuf, width, pitch, height, (int)TJPixelFormats.TJPF_BGR, 2, quality, 0);
            tjDestroy(handle);
            return result;
        }

        public unsafe static void BytesSaveToJpeg(byte[] bytes, int width, int height, string path, int quality)
        {
            byte[] array = bytesToJpeg(bytes, width, height, quality);
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            fileStream.Seek(0L, SeekOrigin.Begin);
            fileStream.Write(array, 0, array.Length);
            fileStream.Close();
        }
        public unsafe static byte[] BitmapToJpeg(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int num = width * PixelSizes[TJPixelFormats.TJPF_BGR];
            PixelFormat pixelFormat = bitmap.PixelFormat;
            byte[] array = new byte[num * height];
            int num2 = 0;
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(0, 0), new Size(width, height)), ImageLockMode.ReadWrite, pixelFormat);
            byte* ptr = (byte*)((void*)bitmapData.Scan0);
            for (int i = 0; i < width * height * 4; i++)
            {
                if ((i + 1) % 4 > 0)
                {
                    array[num2] = ptr[i];
                    num2++;
                }
            }
            bitmap.UnlockBits(bitmapData);
            IntPtr handle = tjInitCompress();
            IntPtr rgbBuf = IntPtr.Zero;
            fixed (byte* ptr2 = &array[0])
            {
                void* value = (void*)ptr2;
                rgbBuf = (IntPtr)value;
            }
            byte[] result = tjCompress(handle, rgbBuf, width, num, height, (int)TJPixelFormats.TJPF_BGR, 2, 30, 0);
            tjDestroy(handle);
            bitmap.Dispose();
            return result;
        }

        public unsafe static void BitmapSaveToJpeg(Bitmap bitmap, string path)
        {
            byte[] array = BitmapToJpeg(bitmap);
            FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            fileStream.Seek(0L, SeekOrigin.Begin);
            fileStream.Write(array, 0, array.Length);
            fileStream.Close();
        }

        public static byte[] tjCompress(IntPtr handle, IntPtr rgbBuf, int width, int pitch, int height, int pixelFormat, int jpegSubsamp, int jpegQual, int flags)
        {
            IntPtr zero = IntPtr.Zero;
            ulong num = 0UL;
            byte[] result;
            try
            {
                tjCompressInternal(handle, rgbBuf, width, pitch, height, pixelFormat, ref zero, ref num, jpegSubsamp, jpegQual, flags);
                byte[] array = new byte[num];
                Marshal.Copy(zero, array, 0, (int)num);
                result = array;
            }
            finally
            {
                tjFree(zero);
            }
            return result;
        }

        public static readonly Dictionary<TJPixelFormats, int> PixelSizes = new Dictionary<TJPixelFormats, int>
        {
            {
                TJPixelFormats.TJPF_RGB,
                3
            },
            {
                TJPixelFormats.TJPF_BGR,
                3
            },
            {
                TJPixelFormats.TJPF_RGBX,
                4
            },
            {
                TJPixelFormats.TJPF_BGRX,
                4
            },
            {
                TJPixelFormats.TJPF_XBGR,
                4
            },
            {
                TJPixelFormats.TJPF_XRGB,
                4
            },
            {
                TJPixelFormats.TJPF_GRAY,
                1
            },
            {
                TJPixelFormats.TJPF_RGBA,
                4
            },
            {
                TJPixelFormats.TJPF_BGRA,
                4
            },
            {
                TJPixelFormats.TJPF_ABGR,
                4
            },
            {
                TJPixelFormats.TJPF_ARGB,
                4
            },
            {
                TJPixelFormats.TJPF_CMYK,
                4
            }
        };

        public enum TJColorSpaces
        {
            TJCS_RGB,
            TJCS_YCbCr,
            TJCS_GRAY,
            TJCS_CMYK,
            TJCS_YCCK
        }

        public enum TJSubsamplingOptions
        {
            TJSAMP_444,
            TJSAMP_422,
            TJSAMP_420,
            TJSAMP_GRAY,
            TJSAMP_440,
            TJSAMP_411
        }

        public enum TJPixelFormats
        {
            TJPF_RGB,
            TJPF_BGR,
            TJPF_RGBX,
            TJPF_BGRX,
            TJPF_XBGR,
            TJPF_XRGB,
            TJPF_GRAY,
            TJPF_RGBA,
            TJPF_BGRA,
            TJPF_ABGR,
            TJPF_ARGB,
            TJPF_CMYK
        }

        [Flags]
        public enum TJFlags
        {
            NONE = 0,
            BOTTOMUP = 2,
            FASTUPSAMPLE = 256,
            NOREALLOC = 1024,
            FASTDCT = 2048,
            ACCURATEDCT = 4096
        }

        public enum TJTransformOperations
        {
            TJXOP_NONE,
            TJXOP_HFLIP,
            TJXOP_VFLIP,
            TJXOP_TRANSPOSE,
            TJXOP_TRANSVERSE,
            TJXOP_ROT90,
            TJXOP_ROT180,
            TJXOP_ROT270
        }

        [Flags]
        public enum TJTransformOptions
        {
            PERFECT = 1,
            TRIM = 2,
            CROP = 4,
            GRAY = 8,
            NOOUTPUT = 16
        }
    }
}

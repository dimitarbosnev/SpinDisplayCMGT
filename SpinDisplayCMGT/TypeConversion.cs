using System;
using System.Collections.Generic;
using System.Text;

namespace SpinDisplayCMGT
{
    public static class TypeConversion
    {
        // Token: 0x06000486 RID: 1158 RVA: 0x00028D9C File Offset: 0x00026F9C
        public static byte[] GetBytes(long value, bool isLittleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian == isLittleEndian)
            {
                return bytes;
            }
            Array.Reverse(bytes);
            return bytes;
        }

        // Token: 0x06000487 RID: 1159 RVA: 0x00028DC4 File Offset: 0x00026FC4
        public static byte[] GetBytes(short value, bool isLittleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian == isLittleEndian)
            {
                return bytes;
            }
            Array.Reverse(bytes);
            return bytes;
        }

        // Token: 0x06000488 RID: 1160 RVA: 0x00028DEC File Offset: 0x00026FEC
        public static byte[] GetBytes(int value, bool isLittleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian == isLittleEndian)
            {
                return bytes;
            }
            Array.Reverse(bytes);
            return bytes;
        }

        // Token: 0x06000489 RID: 1161 RVA: 0x00028E14 File Offset: 0x00027014
        public static byte[] GetBytes(string str, int length)
        {
            byte[] result;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                if (bytes.Length <= length)
                {
                    result = bytes;
                }
                else
                {
                    byte[] array = new byte[length];
                    Array.Copy(bytes, array, length);
                    int length2 = Encoding.UTF8.GetString(array).Length;
                    int bytes2 = Encoding.UTF8.GetBytes(str, 0, length2, bytes, 0);
                    if (bytes2 > length)
                    {
                        bytes2 = Encoding.UTF8.GetBytes(str, 0, length2 - 1, bytes, 0);
                    }
                    array = new byte[bytes2];
                    Array.Copy(bytes, array, bytes2);
                    result = array;
                }
            }
            catch (Exception)
            {
                result = new byte[0];
            }
            return result;
        }

        // Token: 0x0600048A RID: 1162 RVA: 0x00028EB0 File Offset: 0x000270B0
        public static byte[] LongToBytes(long num)
        {
            return new byte[]
            {
                (byte)(num & 255L),
                (byte)((num & 65280L) >> 8),
                (byte)((num & 16711680L) >> 16),
                (byte)((num & 4278190080L) >> 24),
                (byte)((num & 1095216660480L) >> 32),
                (byte)((num & 280375465082880L) >> 40),
                (byte)((num & 71776119061217280L) >> 48),
                (byte)(num >> 56)
            };
        }

        // Token: 0x0600048B RID: 1163 RVA: 0x00028F39 File Offset: 0x00027139
        public static byte[] IntToBytes(int num)
        {
            return new byte[]
            {
                (byte)(num & 255),
                (byte)((num & 65280) >> 8),
                (byte)((num & 16711680) >> 16),
                (byte)(num >> 24)
            };
        }

        // Token: 0x0600048C RID: 1164 RVA: 0x00028F6F File Offset: 0x0002716F
        public static byte[] UIntToBytes(uint num)
        {
            return new byte[]
            {
                (byte)(num & 255U),
                (byte)((num & 65280U) >> 8),
                (byte)((num & 16711680U) >> 16),
                (byte)(num >> 24)
            };
        }

        // Token: 0x0600048D RID: 1165 RVA: 0x00028FA5 File Offset: 0x000271A5
        public static byte[] ShortToBytes(short num)
        {
            return new byte[]
            {
                (byte)(num & 255),
                (byte)(num >> 8)
            };
        }

        // Token: 0x0600048E RID: 1166 RVA: 0x00028FC0 File Offset: 0x000271C0
        public static byte[] HexToBytes(string hex)
        {
            string text = hex.Replace(" ", "").Replace("-", "");
            if (text.Length % 2 != 0)
            {
                text += " ";
            }
            byte[] array = new byte[text.Length / 2];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }
            return array;
        }
    }
}

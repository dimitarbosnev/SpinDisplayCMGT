using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics;
using System.Drawing;
namespace SpinDisplayCMGT
{
    public static class FanCMGT
    {
        public const string TCP_SERVER_IP = "10.10.10.1";
        public const int TCP_SERVER_PORT = 50110;
        public const string UDP_SERVER_IP = "10.10.10.255";
        public const int UDP_SERVER_PORT = 50100;
        private static readonly byte[] CONNECT_UDP = { 0x68, 0x00, 0x04, 0x00, 0x7f, 0x99, 0x5b, 0x36, 0x34, 0xdc };
        private static readonly byte[] CONNECT_TCP = { 0x68, 0x10, 0x01, 0x00, 0x01, 0xd1, 0x09 };
        public static bool isConnected = false;
        public static bool isProjecting = false;
        public const int Width = 672;

        private static TcpClient tcpClient;
        private static NetworkStream tcpStream;
        private static UdpClient udpSender;
        private static readonly IPEndPoint? udpFan = new IPEndPoint(IPAddress.Parse(UDP_SERVER_IP), UDP_SERVER_PORT);
        private static UdpClient udpListener;

        private static void AddCrc(byte[] data)
        {
            byte[] crc16Bytes = Crc16.GetCrc16Bytes(data, 0, data.Length - 2);
            data[data.Length - 2] = crc16Bytes[0];
            data[data.Length - 1] = crc16Bytes[1];
        }
        private static readonly object _writeLock = new object();
        public static void write(byte[] message)
        {
            try
            {
                lock (_writeLock)
                {
                    tcpStream.Write(message, 0, message.Length);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        public static byte[] HeartSend()
        {
            byte[] array = new byte[6];
            array[0] = Protocol.Head;
            array[1] = Protocol.Heart;
            AddCrc(array);
            return array;
        }

        public static byte[] Projection(int isOn)
        {
            byte[] array = new byte[7];
            array[0] = Protocol.Head;
            array[1] = Protocol.Projection;
            array[2] = 1;
            array[4] = (byte)isOn;
            AddCrc(array);
            return array;
        }

        private static readonly byte[] HEARTBEAT = HeartSend();
        private static readonly byte[] PROJECTION = Projection(1);
        public static void Ping()
        {
            while (isConnected)
            {
                write(HEARTBEAT);
                if (isProjecting)
                    write(PROJECTION);
                Thread.Sleep(2000);
            }
        }

        public static string read(byte[] message)
        {
            try
            {
                tcpStream.Write(message, 0, message.Length);
                byte[] data = new byte[256];
                string responseData = string.Empty;
                Int32 bytes = tcpStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                return responseData;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return "";
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return "";
            }
        }

        private static Thread pingThread = new Thread(Ping);

        public static void Connect()
        {
            try
            {
                udpSender = new UdpClient();
                udpSender.Send(CONNECT_UDP, CONNECT_UDP.Length, udpFan);

                udpListener = new UdpClient(50105);
                IPEndPoint? sender = null;
                byte[] recived = udpListener.Receive(ref sender);
                Console.WriteLine(recived);
                tcpClient = new TcpClient(TCP_SERVER_IP, TCP_SERVER_PORT);
                tcpStream = tcpClient.GetStream();
                write(CONNECT_TCP);
                isConnected = true; 

                pingThread.Start();
            }
            catch (ArgumentNullException e) { Console.WriteLine("ArgumentNullException: {0}", e); }
            catch (SocketException e) { Console.WriteLine("SocketException: {0}", e); }
        }

        public static void Disconnect()
        {
            tcpStream.Close();
            tcpClient.Close();
            isConnected = false;
            pingThread.Join();
        }

        public static void PowerOn()
        {
            byte[] array = new byte[7];
            array[0] = Protocol.Head;
            array[1] = Protocol.OnOff;
            array[2] = 1;
            array[4] = (byte)(1 & 255);
            AddCrc(array);
            write(array);
        }

        public static void PowerOff()
        {
            byte[] array = new byte[7];
            array[0] = Protocol.Head;
            array[1] = Protocol.OnOff;
            array[2] = 1;
            array[4] = (byte)(0 & 255);
            AddCrc(array);
            write(array);
        }

        public static void StartProjection()
        {
            isProjecting = true;
        }

        private static readonly byte[] END_PROJECTION = Projection(0);
        public static void EndProjection()
        {
            write(END_PROJECTION);
            isProjecting = false;
        }

        public static void ProjectOnDisplay(in RawImage rawImage)
        {
            if (isProjecting)
            {
                byte[] bytes = TypeConversion.GetBytes(rawImage.Pixels.Length, true);
                IEnumerable<byte> source = new byte[]
                {
                Protocol.ProHeader,
                bytes[0],
                bytes[1],
                bytes[2]
                }.Concat(rawImage.Pixels);
                write(source.ToArray());
            }
        }

    }
}

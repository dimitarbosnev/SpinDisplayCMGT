using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Intrinsics;
using System.Drawing;
using System.Windows.Forms;

namespace SpinDisplayCMGT
{
    public static class FanCMGT
    {
        public static readonly IPAddress TCP_SERVER_IP = IPAddress.Parse("10.10.10.1");
        public static readonly IPAddress TCP_SERVER_NETCARD = IPAddress.Parse("10.10.10.3");
        public const int TCP_SERVER_PORT_1 = 50110;
        public const int TCP_SERVER_PORT_2 = 50115;

        private static readonly byte[] CONNECT_TCP = { 0x68, 0x10, 0x01, 0x00, 0x01, 0xd1, 0x09 };
        public static bool isConnected = false;
        public static bool isProjecting = false;
        public static bool isDataReady = false;

        private static Socket socket_1;
        private static Socket socket_2;


        private static void AddCrc(byte[] data)
        {
            byte[] crc16Bytes = Crc16.GetCrc16Bytes(data, 0, data.Length - 2);
            data[data.Length - 2] = crc16Bytes[0];
            data[data.Length - 1] = crc16Bytes[1];
        }
        private static readonly object writeLock_1 = new object();
        public static void send_command(byte[] message)
        {
            try
            {
                lock (writeLock_1)
                {
                    socket_1.Send(message);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Failed to send command");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to send command");
            }
        }
        private static readonly object writeLock_2 = new object();
        public static void send_data(byte[] message)
        {
            try
            {
                lock (writeLock_2)
                {
                    socket_2.Send(message);
                }
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Failed to send data");
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to send data");
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
                send_command(HEARTBEAT);
                Thread.Sleep(2000);
            }
        }
        private static byte[] recive_buffer = new byte[1024];
        private static void ReciveProc(IAsyncResult ar)
        {
            try
            {
                int num = (ar.AsyncState as Socket).EndReceive(ar);
                byte[] array = new byte[num];
                Array.Copy(recive_buffer, array, num);
                if (array[1] == Protocol.Projection)
                {
                    int bit5 = (array[4] & byte.MaxValue);
                    isDataReady = bit5 == 0;
                }
            }
            catch { }


            if (isConnected)
            {
                socket_1.BeginReceive(recive_buffer, 0, recive_buffer.Length, SocketFlags.None, new AsyncCallback(ReciveProc), socket_1);
            }
        }

        private static Thread pingThread = new Thread(Ping);

        public const string UDP_SERVER_IP = "10.10.10.255";
        public const int UDP_SERVER_PORT_1 = 50100;
        public const int UDP_SERVER_PORT_2 = 50105;
        private static readonly IPEndPoint UDP_ENDPOINT = new IPEndPoint(IPAddress.Parse(UDP_SERVER_IP), UDP_SERVER_PORT_1);
        private static readonly byte[] CONNECT_UDP = { 0x68, 0x00, 0x04, 0x00, 0x7f, 0x99, 0x5b, 0x36, 0x34, 0xdc };
        private static UdpClient udpListener = new UdpClient(UDP_SERVER_PORT_2);
        private static UdpClient udpSender = new UdpClient();
        public static void Connect()
        {
            try
            {
                udpSender.Send(CONNECT_UDP, CONNECT_UDP.Length, UDP_ENDPOINT);

                IPEndPoint? sender = null;
                byte[] recived = udpListener.Receive(ref sender);
                Console.WriteLine(recived);
                socket_1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_1.SendBufferSize = 1024;
                socket_1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                socket_1.Bind(new IPEndPoint(TCP_SERVER_NETCARD, 0));
                socket_1.Connect(TCP_SERVER_IP, TCP_SERVER_PORT_1);
   
                send_command(CONNECT_TCP);
                isConnected = true; 

                pingThread.Start();
                socket_1.BeginReceive(recive_buffer, 0, recive_buffer.Length, SocketFlags.None, new AsyncCallback(ReciveProc), socket_1);
            }
            catch (ArgumentNullException e) { Console.WriteLine("ArgumentNullException: {0}", e); }
            catch (SocketException e) { Console.WriteLine("SocketException: {0}", e); }
        }

        public static void Disconnect()
        {
            EndProjection();
            socket_1.Close();
            isConnected = false;
        }

        public static void PowerOn()
        {
            byte[] array = new byte[7];
            array[0] = Protocol.Head;
            array[1] = Protocol.OnOff;
            array[2] = 1;
            array[4] = (byte)(1 & 255);
            AddCrc(array);
            send_command(array);
        }

        public static void PowerOff()
        {
            byte[] array = new byte[7];
            array[0] = Protocol.Head;
            array[1] = Protocol.OnOff;
            array[2] = 1;
            array[4] = (byte)(0 & 255);
            AddCrc(array);
            send_command(array);
        }

        public static void StartProjection()
        {
            try
            {
                if (socket_2 == null)
                {
                    socket_2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket_2.SendBufferSize = 102400;
                    socket_2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 2000);
                    socket_2.Bind(new IPEndPoint(TCP_SERVER_NETCARD, 0));
                    socket_2.Connect(TCP_SERVER_IP, TCP_SERVER_PORT_2);
                }
                new Thread(() =>
                {

                    while (!isDataReady)
                    {
                        send_command(PROJECTION);
                        Thread.Sleep(2000);
                    }
                    Thread.Sleep(500);
                    isProjecting = true;
                }).Start();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static readonly byte[] END_PROJECTION = Projection(0);
        public static void EndProjection()
        {
            if (socket_2 != null)
            {
                socket_2.Disconnect(true);
                socket_2.Close();
                socket_2 = null;
            }
            send_command(END_PROJECTION);
            isProjecting = false;
            isDataReady = false;
        }

        public static void ProjectOnDisplay(in RawImage rawImage)
        {
            if (isProjecting)
            {
                byte[] array = TurboLibjpeg.bytesToJpeg(rawImage.Pixels, rawImage.Width, rawImage.Height, 30);
                byte[] bytes = TypeConversion.GetBytes(array.Length, true);

                IEnumerable<byte> source = new byte[]
                {
                Protocol.ProHeader,
                bytes[0],
                bytes[1],
                bytes[2]
                }.Concat(array);

                send_data(source.ToArray());
            }
        }

        public static void ProjectOnDisplay(in Bitmap bitmap)
        {
            if (isProjecting && isDataReady)
            {
                byte[] array = TurboLibjpeg.BitmapToJpeg(bitmap);
                byte[] bytes = TypeConversion.GetBytes(array.Length, true);

                IEnumerable<byte> source = new byte[]
                {
                Protocol.ProHeader,
                bytes[0],
                bytes[1],
                bytes[2]
                }.Concat(array);

                send_data(source.ToArray());
            }
        }

    }
}

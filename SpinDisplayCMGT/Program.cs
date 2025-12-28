using System.Diagnostics;
using CMGT;

namespace SpinDisplayCMGT
{


    internal class Program
    {
        private static readonly object imagelock = new object();
        static void Main(string[] args)
        {
            Fan.Connect();
            Fan.PowerOn();

            Console.WriteLine("Press 'q' to quit.");
            bool running = true;
            byte[] array = null;
            Stopwatch sw = Stopwatch.StartNew();
            double lastTime = 0.0;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            running = false;
                            continue;
                            break;
                        case ConsoleKey.E:
                            Fan.EndProjection();
                            break;
                        case ConsoleKey.W:
                            Fan.StartProjection();
                            break;
                        case ConsoleKey.R:
                            Fan.PowerOn();
                            break;
                        case ConsoleKey.T:
                            Fan.PowerOff();
                            break;
                    }
                }
                if (Fan.isProjecting)
                {
                    lock (imagelock)
                    {
                        RawImage image = WindowsScreenCapture.CaptureScreenRawImage();
                        array = Jpeg.bytesToJpeg(image.Pixels, image.Width, image.Height, 30);
                        double now = sw.Elapsed.TotalMilliseconds;
                        double deltaTime = now - lastTime;
                        if (deltaTime >= 80.0) 
                        {
                            lastTime = now;
                   
                            Fan.ProjectOnDisplay(in array);
                        }
                    }
                }
            }
            Fan.Disconnect();
        }
    }
}

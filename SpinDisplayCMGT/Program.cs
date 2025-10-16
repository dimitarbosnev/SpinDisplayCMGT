using System.Drawing;

namespace SpinDisplayCMGT
{


    internal class Program
    {
        private static readonly object imagelock = new object();
        static void Main(string[] args)
        {
            FanCMGT.Connect();
            FanCMGT.PowerOn();

            Console.WriteLine("Press 'q' to quit.");
            bool running = true;
            RawImage image = WindowsScreenCapture.CaptureScreenRawImage();
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
                            FanCMGT.EndProjection();
                            break;
                        case ConsoleKey.W:
                            FanCMGT.StartProjection();
                            break;
                        case ConsoleKey.R:
                            FanCMGT.PowerOn();
                            break;
                        case ConsoleKey.T:
                            FanCMGT.PowerOff();
                            break;
                    }
                }
                if (FanCMGT.isProjecting)
                {
                    lock (imagelock)
                    {
                        image = WindowsScreenCapture.CaptureScreenRawImage();
                        FanCMGT.ProjectOnDisplay(in image);
                    }
                }
            }
            FanCMGT.Disconnect();
        }
    }
}

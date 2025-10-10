namespace SpinDisplayCMGT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FanCMGT.Connect();
            FanCMGT.PowerOn();
            FanCMGT.StartProjection();
            Console.WriteLine("Press 'q' to quit.");
            bool running = true;
            RawImage IMAGE;
            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Q)
                        running = false;
                }
                IMAGE = WindowsScreenCapture.CaptureScreen(FanCMGT.Width, FanCMGT.Width);
                FanCMGT.ProjectOnDisplay(in IMAGE);
            }
           FanCMGT.EndProjection();
           FanCMGT.PowerOff();
           FanCMGT.Disconnect();
        }
    }
}

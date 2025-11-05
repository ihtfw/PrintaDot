using PrintaDot.Shared.Common;
using PrintaDot.Shared.NativeMessaging;
using PrintaDot.Windows;

class Program
{
    static Host Host = null!;

    static void Main(string[] args)
    {
        Log.Active = true;

        Host = new Host()
        {
            PlatformPrintingService = new WindowsPrintingService()
        };

        Host.GenerateManifest();
        Host.RegisterAllSupportedBrowsers();
        Host.MoveHostToLocalAppData();

        if (args.Contains("--unregister"))
        {
            Host.Unregister();
        }
        else
        {
            Host.Listen();
        }
    }
}
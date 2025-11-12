using PrintaDot.Shared.Common;
using PrintaDot.Shared.NativeMessaging;
using PrintaDot.Windows;

class Program
{
    static Host Host = null!;
    static Updater Updater = null!;

    static void Main(string[] args)
    {
        Log.Active = true;

        Updater = new Updater();

        if (args.Contains("--update"))
        {
            Updater.PerformUpdate();
        }

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
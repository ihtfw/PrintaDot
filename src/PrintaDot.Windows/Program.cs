using PrintaDot.Shared.Common;
using PrintaDot.Shared.NativeMessaging;
using PrintaDot.Windows;

class Program
{
    static Host Host = null!;
    static Updater Updater = null!;

    static async Task Main(string[] args)
    {
        Log.Active = true;

        Updater = new Updater();
        Updater.DeleteTempFile();
        if (args.Contains("--update"))
        {
            Updater.PerformUpdate(int.Parse(args[1]));
        }

        Host = new Host()
        {
            PlatformPrintingService = new WindowsPrintingService()
        };

        var currentDirectory = Utils.AssemblyLoadDirectory();

        if (!Utils.IsLocalAppDataDirectory(currentDirectory))
        {
            Host.GenerateManifest();
            Host.RegisterAllSupportedBrowsers();
            Host.MoveHostToLocalAppData(currentDirectory);
        }

        await Updater.Update();

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

using PrintaDot.Shared.Common;
using PrintaDot.Shared.NativeMessaging;
using PrintaDot.Shared.Platform;

namespace PrintaDot;

class Program
{
    static Host Host = null!;

    static void Main(string[] args)
    {
        Log.Active = true;

        Host = new Host()
        {
            PlatformPrintingService = ResolvePlatformPrintingService()
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

    private static IPlatformPrintingService ResolvePlatformPrintingService()
    {
#if BUILD_FOR_WINDOWS
        return new PrintaDot.Windows.WindowsPrintingService();
#else
        throw new InvalidOperationException("Only Windows currently is supported");
#endif
    }
}

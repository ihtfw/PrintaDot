using PrintaDot.Shared.Common;
using PrintaDot.Shared.NativeMessaging;

namespace PrintaDot;

class Program
{
    static Host Host = null!;

    static void Main(string[] args)
    {
        Log.Active = true;

        Host = new Host();

        Host.MoveHostToLocalAppData();
        Host.GenerateManifest();
        Host.RegisterAllSupportedBrowsers();

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

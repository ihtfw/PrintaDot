using PrintaDot.NativeMessaging;

namespace PrintaDot;

class Program
{
    static Host Host = null!;

    static void Main(string[] args)
    {
        Log.Active = true;

        Host = new Host();
        Host.SupportedBrowsers.Add(BrowserCreator.GoogleChrome);
        Host.SupportedBrowsers.Add(BrowserCreator.MicrosoftEdge);

        Host.GenerateManifest();
        Host.Register();

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
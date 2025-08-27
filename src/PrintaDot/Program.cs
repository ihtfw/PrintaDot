using PrintaDot.NativeMessaging;

namespace PrintaDot;

class Program
{
    static Host Host = null!;

    readonly static string[] AllowedOrigins
        = new string[]
        {
                "chrome-extension://ncpdldoackcgjeocgpkjbfimpdjkolpg/"
        };

    readonly static string Description
        = "Host";

    static void Main(string[] args)
    {
        Log.Active = true;

        Host = new Host();
        Host.SupportedBrowsers.Add(ChromiumBrowser.GoogleChrome);
        Host.SupportedBrowsers.Add(ChromiumBrowser.MicrosoftEdge);

        Host.GenerateManifest(Description, AllowedOrigins);
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
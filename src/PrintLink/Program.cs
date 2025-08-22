using PrintLink.NativeMessaging;

namespace NativeMessagingTest
{
    class Program
    {
        readonly static string[] AllowedOrigins
            = new string[]
            {
                "chrome-extension://eehnbljfbemdnegckkpbdcmpelaahpek/"
            };

        readonly static string Description
            = "Host application for printing";

        static void Main(string[] args)
        {
            Log.Active = true;

            var host = new Host("com.vectorbest.printlink");
            host.SupportedBrowsers.Add(BrowserCreator.GoogleChrome);
            host.SupportedBrowsers.Add(BrowserCreator.MicrosoftEdge);

            host.GenerateManifest(Description, AllowedOrigins);
            Console.WriteLine("Manifest file created successfully");

            host.Register();
            Console.WriteLine("Host registry has done. Application are ready to use with extension");

            if (args.Contains("--unregister"))
            {
                host.Unregister();
            }
            else
            {
                host.Listen();
            }
        }
    }
}

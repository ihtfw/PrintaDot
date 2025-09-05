namespace PrintaDot.Shared.NativeMessaging;

public static class BrowserCreator
{
    /// <summary>
    /// <see cref="ChromiumBrowser"/> object for Google Chrome.
    /// </summary>
    public static Browser GoogleChrome => new Browser("Google Chrome", "SOFTWARE\\Google\\Chrome\\");

    /// <summary>
    /// <see cref="ChromiumBrowser"/> object for Microsoft Edge.
    /// </summary>
    public static Browser MicrosoftEdge => new Browser("Microsoft Edge", "SOFTWARE\\Microsoft\\Edge\\");
}

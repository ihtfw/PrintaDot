using PrintaDot.Shared.NativeMessaging;

namespace PrintaDot.Shared.Common;

public static class Extensions
{
    /// <summary>
    /// Checks if the host is registered with all required browsers.
    /// </summary>
    /// <returns><see langword="true"/> if the required information is present in the registry.</returns>
    public static bool IsAnyRegistered(this IEnumerable<Browser> browsers, string hostName, string manifestPath)
    {
        bool result = false;

        foreach (Browser browser in browsers)
        {
            result = result || browser.IsRegistered(hostName, manifestPath);
        }

        return result;
    }

    /// <summary>
    /// Register the application to open with all required browsers.
    /// </summary>
    public static void Register(this IEnumerable<Browser> browsers, string hostName, string manifestPath)
    {
        foreach (Browser browser in browsers)
        {
            browser.Register(hostName, manifestPath);
        }
    }

    /// <summary>
    /// De-register the application to open with all required browsers.
    /// </summary>
    public static void Unregister(this IEnumerable<Browser> browsers, string hostName)
    {
        foreach (Browser browser in browsers)
        {
            browser.Unregister(hostName);
        }
    }
}

using System.Reflection;

namespace PrintaDot.Shared.Common;

public static class Utils
{
    public static string MessageLogLocation => Path.Combine(AssemblyLoadDirectory(), "native-messaging.log");

    static public string AssemblyLoadDirectory()
    {
        string? codeBase = Assembly.GetEntryAssembly()?.Location;

        if (codeBase == null)
        {
            throw new InvalidOperationException("Invalid assembly directory.");
        }

        UriBuilder uri = new UriBuilder(codeBase);
        string path = Uri.UnescapeDataString(uri.Path);

        // Ensuring the return string is never null, since
        // GetDirectoryName will return null if in the root directory
        return Path.GetDirectoryName(path) ?? "";
    }

    static public string AssemblyExecuteablePath()
    {
        string? codeBase = Environment.ProcessPath;

        if (codeBase == null)
        {
            throw new InvalidOperationException("Invalid executable path.");
        }

        UriBuilder uri = new UriBuilder(codeBase);

        return Uri.UnescapeDataString(uri.Path);
    }
}

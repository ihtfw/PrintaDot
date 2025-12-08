using Microsoft.JSInterop;

namespace PrintaDot.Blazor;
public static class Utils
{
    public static Exception MapJsError(JSException ex)
    {
        var msg = ex.Message;

        return msg switch
        {
            string s when s.Contains("Extension connection failed") => new ExtensionConnectionFailedException(),
            string s when s.Contains("Native app connection failed") => new NativeAppConnectionFailedException(),
            string s when s.Contains("Extension is not connected") => new ExtensionConnectionFailedException(),
            string s when s.Contains("Native application is not connected") => new NativeAppConnectionFailedException(),
            _ => new Exception(msg)
        };
    }
}

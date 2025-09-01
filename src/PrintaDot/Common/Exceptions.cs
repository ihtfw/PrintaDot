namespace PrintaDot.Common;

/// <summary>
/// Exception raised when trying to interact with chrome while the 
/// extension is not registered in the Windows Registry
/// </summary>
[Serializable]
public class NotRegisteredWithBrowserException : Exception
{
    public NotRegisteredWithBrowserException(string message) : base(message) { }
}

namespace PrintLink.NativeMessaging;

[Serializable]
public class NotRegisteredWithBrowserException : Exception
{
    public NotRegisteredWithBrowserException(string message) : base(message) { }
}

[Serializable]
public class BadFormatRecivedDataException : Exception
{
    public BadFormatRecivedDataException(string message) : base(message) { }
}

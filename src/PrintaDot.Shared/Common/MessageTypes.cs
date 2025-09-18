namespace PrintaDot.Shared.Common;

public static class MessageTypes
{
    /// <summary>
    /// Variable for supported message protocol version.
    /// All versions lower than SupportedMessageVersion autumaticly supported.
    /// Higher versions than SupportedMessageVersion not supported and can be deserialize to lower version.
    /// </summary>
    public const int SupportedMessageVersion = 1;

    public static readonly Dictionary<string, List<int>> SupportedMessageVersions = new()
    {
        [PrintRequestMessageType] = [1],
        [ProfilesType] = [1],
        [ProfileType] = [1]
    };

    //TODO переделать под enum
    public const string PrintRequestMessageType = "printRequest";
    public const string GetPrintStatusRequestMessageType = "getPrintStatusRequest";
    public const string GetPrintStatusResponseMessageType = "getPrintStatusResponse";
    public const string ProfileType = "profile";
    public const string ProfilesType = "profiles";
    public const string ExceptionType = "exception";
}

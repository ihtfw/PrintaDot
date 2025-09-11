namespace PrintaDot.Shared.Common;

public static class MessageTypes
{
    public static readonly Dictionary<string, List<int>> SupportedMessageVersions = new()
    {
        [PrintRequestMessageType] = [1]
    };

    //TODO переделать под enum
    public const string PrintRequestMessageType = "printRequest";
    public const string GetPrintStatusRequestMessageType = "getPrintStatusRequest";
    public const string GetPrintStatusResponseMessageType = "getPrintStatusResponse";
    public const string ProfileType = "profile";
}

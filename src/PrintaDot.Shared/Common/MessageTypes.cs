namespace PrintaDot.Shared.Common;

public static class MessageTypes
{
    public static readonly Dictionary<string, List<int>> SupportedMessageVersions = new()
    {
        [PrintRequestMessage] = [1]
    };

    public const string PrintRequestMessage = "printRequest";
    public const string GetPrintStatusRequestMessage = "getPrintStatusRequest";
    public const string GetPrintStatusResponseMessage = "getPrintStatusResponse";
}

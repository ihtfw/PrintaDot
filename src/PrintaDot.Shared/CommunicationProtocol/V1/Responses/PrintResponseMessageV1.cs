namespace PrintaDot.Shared.CommunicationProtocol.V1.Responses;

public class PrintResponseMessageV1 : Response
{
    public bool IsSuccess { get; set; }
    public string? MessageText { get; set; }

    public static PrintResponseMessageV1 CreateSuccessResponse(Guid messageIdToResponse) => new PrintResponseMessageV1 
    {
        Version = 1,
        Type = Common.ResponseType.PrintResponse,
        MessageIdToResponse = messageIdToResponse,
        IsSuccess = true,
        MessageText = "All data successfully printed",
    };

    public static  PrintResponseMessageV1 CreateFailedResponse(Guid messageIdToResponse) => new PrintResponseMessageV1
    {
        Version = 1,
        Type = Common.ResponseType.PrintResponse,
        MessageIdToResponse = messageIdToResponse,
        IsSuccess = false,
        MessageText = "Error happened when data was printing",
    };
}

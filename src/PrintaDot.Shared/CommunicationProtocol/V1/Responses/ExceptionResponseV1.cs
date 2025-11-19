using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol.V1.Responses;

public class ExceptionResponseV1 : Response
{
    public static ExceptionResponseV1 Create(string text) => new()
    {
        Version = 1,
        Type = ResponseType.Exception,
        MessageText = text
    };

    public required string MessageText { get; set; }
}

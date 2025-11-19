using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol.V1.Requests;
public class ExceptionMessageV1 : Message
{
    public static ExceptionMessageV1 Create(string text) => new()
    {
        Version = 1,
        Type = MessageType.Exception,
        MessageText = text
    };

    public required string MessageText { get; set; }
}

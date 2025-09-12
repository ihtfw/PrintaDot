using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class ExceptionMessageV1 : Message
{
    public static ExceptionMessageV1 Create(string text) => new()
    {
        Version = 1,
        Type = MessageTypes.ExceptionType,
        MessageText = text
    };

    public required string MessageText { get; set; }
}

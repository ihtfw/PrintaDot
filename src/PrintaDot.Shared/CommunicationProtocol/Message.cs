using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol;

/// <summary>
/// Represents base class for messages.
/// </summary>
public class Message
{
    public required MessageType Type { get; set; }
    public required int Version { get; set; }
}

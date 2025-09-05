namespace PrintaDot.Shared.CommunicationProtocol;

/// <summary>
/// Represents base class for messages.
/// </summary>
public class Message
{
    public required string Type { get; set; }
    public required int Version { get; set; }
}

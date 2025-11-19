using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol;

/// <summary>
/// Represents base class for response to message.
/// </summary>
public class Response
{
    public int Version { get; set; }
    public Guid? MessageIdToResponse { get; set; }
    public ResponseType Type { get; set; }
}

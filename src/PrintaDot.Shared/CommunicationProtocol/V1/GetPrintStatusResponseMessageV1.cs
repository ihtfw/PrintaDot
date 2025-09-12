using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class GetPrintStatusResponseMessageV1 : Message
{
    public required Guid Guid { get; set; }
    public required PrintStatus PrintStatus { get; set; }
    public string? Details { get; set; }
}

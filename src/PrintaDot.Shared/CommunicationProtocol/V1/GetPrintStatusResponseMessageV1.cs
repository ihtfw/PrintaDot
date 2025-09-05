namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class GetPrintStatusResponseMessageV1 : Message
{
    public required Guid Guid { get; set; }
    public required PrintStatus PrintStatus { get; set; }
    public string? Details { get; set; }
}

public enum PrintStatus
{
    Queued = 0,
    Printing = 1,
    Success = 2,
    Error = 3,
    Unknown = 4,
}

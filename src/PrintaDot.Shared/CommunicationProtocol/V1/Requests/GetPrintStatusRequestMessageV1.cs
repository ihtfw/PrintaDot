namespace PrintaDot.Shared.CommunicationProtocol.V1.Requests;

public class GetPrintStatusRequestMessageV1 : Message
{
    public required Guid Guid { get; set; }
}

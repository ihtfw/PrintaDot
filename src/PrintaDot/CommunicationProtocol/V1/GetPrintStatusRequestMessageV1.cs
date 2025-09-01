namespace PrintaDot.NativeMessaging.CommunicationProtocol.V1;

public class GetPrintStatusRequestMessageV1 : Message
{
    public required Guid Guid { get; set; }
}

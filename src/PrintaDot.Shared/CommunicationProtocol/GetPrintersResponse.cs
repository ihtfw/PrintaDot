namespace PrintaDot.Shared.CommunicationProtocol;

public class GetPrintersResponse : Message
{
    public List<string>? Printers { get; set; }
}

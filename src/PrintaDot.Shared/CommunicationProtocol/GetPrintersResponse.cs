namespace PrintaDot.Shared.CommunicationProtocol;

public class GetPrintersResponse : Response
{
    public List<string>? Printers { get; set; }
}

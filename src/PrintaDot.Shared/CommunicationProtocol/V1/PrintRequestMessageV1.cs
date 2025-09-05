namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class PrintRequestMessageV1 : Message
{
    public string? Profile { get; set; }
    public List<Item> Items { get; set; }

    public class Item
    {
        public string? Header { get; set; }
        public required string Barcode { get; set; }
        public string? Figures { get; set; }
    }
}

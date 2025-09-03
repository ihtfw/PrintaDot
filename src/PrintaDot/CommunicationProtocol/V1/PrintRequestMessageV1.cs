using System.Text.Json.Serialization;

namespace PrintaDot.CommunicationProtocol.V1;

[JsonDerivedType(typeof(PrintRequestMessageV1), typeDiscriminator: "printRequestMessageV1")]
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

using PrintaDot.CommunicationProtocol.V1;
using System.Text.Json.Serialization;

namespace PrintaDot.CommunicationProtocol;

[JsonDerivedType(typeof(PrintRequestMessageV1), typeDiscriminator: "printRequestMessageV1")]
[JsonDerivedType(typeof(GetPrintStatusResponseMessageV1), typeDiscriminator: "getPrintStatusResponseMessageV1")]
[JsonDerivedType(typeof(GetPrintStatusRequestMessageV1), typeDiscriminator: "getPrintStatusRequestMessageV1")]
public class Message
{
    public required string Type { get; set; }
    public required int Version { get; set; }
}

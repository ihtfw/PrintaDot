using System.Text.Json.Serialization;

namespace PrintaDot.CommunicationProtocol.V1;

[JsonDerivedType(typeof(GetPrintStatusRequestMessageV1), typeDiscriminator: "getPrintStatusRequestMessageV1")]
public class GetPrintStatusRequestMessageV1 : Message
{
    public required Guid Guid { get; set; }
}

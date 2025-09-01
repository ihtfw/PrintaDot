using PrintaDot.NativeMessaging.CommunicationProtocol.V1;
using System.Text.Json.Serialization;

namespace PrintaDot.NativeMessaging.CommunicationProtocol;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(PrintRequestMessageV1), typeDiscriminator: "printRequestMessageV1")]
public class Message
{
    public required string Type { get; set; }
    public required int Version { get; set; }
}

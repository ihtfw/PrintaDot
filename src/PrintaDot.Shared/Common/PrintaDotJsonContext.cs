using System.Text.Json.Serialization;
using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.CommunicationProtocol.V1;

namespace PrintaDot.Shared.Common;

[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(PrintRequestMessageV1))]
[JsonSerializable(typeof(PrintRequestMessageV1.PrintProfile))]
[JsonSerializable(typeof(PrintRequestMessageV1.Item))]
[JsonSerializable(typeof(GetPrintStatusRequestMessageV1))]
[JsonSerializable(typeof(GetPrintStatusResponseMessageV1))]
[JsonSerializable(typeof(ExceptionMessageV1))]
public partial class PrintaDotJsonContext : JsonSerializerContext
{
}
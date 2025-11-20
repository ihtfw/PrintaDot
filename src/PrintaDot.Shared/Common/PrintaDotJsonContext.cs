using System.Text.Json.Serialization;
using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.CommunicationProtocol.V1.Requests;
using PrintaDot.Shared.CommunicationProtocol.V1.Responses;

namespace PrintaDot.Shared.Common;

[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(PrintRequestMessageV1))]
[JsonSerializable(typeof(PrintRequestMessageV1.PrintProfile))]
[JsonSerializable(typeof(PrintRequestMessageV1.Item))]
[JsonSerializable(typeof(PrintResponseMessageV1))]
[JsonSerializable(typeof(GetPrintStatusRequestMessageV1))]
[JsonSerializable(typeof(GetPrintStatusResponseMessageV1))]
[JsonSerializable(typeof(ExceptionResponseV1))]
[JsonSerializable(typeof(GetPrintersRequest))]
[JsonSerializable(typeof(GetPrintersResponse))]
[JsonSerializable(typeof(UpdateResponseDto))]
public partial class PrintaDotJsonContext : JsonSerializerContext
{
}
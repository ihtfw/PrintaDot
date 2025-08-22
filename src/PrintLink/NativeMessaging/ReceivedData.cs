using Newtonsoft.Json;

namespace PrintLink.NativeMessaging;

public class ReceivedData
{
    [JsonProperty("sampleName")]
    public string SampleName { get; set; }

    [JsonProperty("barcode")]
    public string Barcode { get; set; }
}

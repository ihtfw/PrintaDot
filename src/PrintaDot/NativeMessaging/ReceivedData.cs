using Newtonsoft.Json;

namespace PrintaDot.NativeMessaging;

public class ReceivedData
{
    [JsonProperty("sampleName")]
    public string SampleName { get; set; }

    [JsonProperty("barcode")]
    public string Barcode { get; set; }
}

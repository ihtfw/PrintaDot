using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PrintLink.NativeMessaging;
public class Response
{
    [JsonProperty("isSuccessful")]
    public bool IsSuccessful { get; set; }
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public JObject Data { get; set; }

    public Response(JObject data)
    {
        Data = data;
        Message = "Confirmation";
        IsSuccessful = true;
    }

    public Response(JObject data, string message)
    {
        Data = data;
        Message = message;
    }

    public JObject? GetJObject()
    {
        return JsonConvert.DeserializeObject<JObject>(
            JsonConvert.SerializeObject(this));
    }
}
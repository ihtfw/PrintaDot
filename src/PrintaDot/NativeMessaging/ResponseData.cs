//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;

//namespace PrintaDot.NativeMessaging;

//public class ResponseData
//{
//    [JsonProperty("isSuccessful")]
//    public bool IsSuccessful { get; set; }
//    [JsonProperty("message")]
//    public string Message { get; set; }

//    [JsonProperty("data")]
//    public JObject Data { get; set; }

//    public ResponseData(JObject data)
//    {
//        Data = data;
//        Message = "Confirmation";
//        IsSuccessful = true;
//    }

//    public ResponseData(JObject data, string message)
//    {
//        Data = data;
//        Message = message;
//    }

//    public JObject? GetJObject()
//    {
//        return JsonConvert.DeserializeObject<JObject>(
//            JsonConvert.SerializeObject(this));
//    }

//}
using System.Text.Json.Serialization;

namespace PrintaDot.NativeMessaging
{
    public class Manifest
    {
        [JsonPropertyName("name")]
        public string HostName => "com.printadot";

        [JsonPropertyName("description")]
        public string Description => "PrintaDot host application";

        [JsonPropertyName("path")]
        public string ExecuteablePath => Utils.AssemblyExecuteablePath();

        [JsonPropertyName("type")]
        public string Type => "stdio";
        [JsonPropertyName("allowed_origins")]
        public string[] AllowedOrigins { get; set; } = ["chrome-extension://ncpdldoackcgjeocgpkjbfimpdjkolpg/"];

        [JsonIgnore]
        public string ManifestPath => Path.Combine(Utils.AssemblyLoadDirectory() ?? "", HostName + "-manifest.json");

        public Manifest() { }
    }
}
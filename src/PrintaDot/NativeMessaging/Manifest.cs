using Newtonsoft.Json;

namespace PrintaDot.NativeMessaging
{
    internal class Manifest
    {
        [JsonProperty("name")]
        public string HostName => "com.printadot";

        [JsonProperty("description")]
        public string Description => "PrintaDot host application";

        [JsonProperty("path")]
        public string ExecuteablePath => Utils.AssemblyExecuteablePath();

        [JsonProperty("type")]
        public string Type => "stdio";

        [JsonProperty("allowed_origins")]
        public readonly string[] AllowedOrigins = ["chrome-extension://ncpdldoackcgjeocgpkjbfimpdjkolpg/"];

        [JsonIgnore]
        public string ManifestPath => Path.Combine(Utils.AssemblyLoadDirectory() ?? "", HostName + "-manifest.json");

        public Manifest() { }
    }
}

using PrintaDot.Common;
using System.Text.Json.Serialization;

namespace PrintaDot.NativeMessaging;

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

    public void GenerateManifest(bool overwrite = true)
    {
        if (File.Exists(ManifestPath) && !overwrite)
        {
            Log.LogMessage("Manifest exists already");
        }
        else
        {
            Log.LogMessage("Generating Manifest");

            string manifest = this.ToJson();

            File.WriteAllText(ManifestPath, manifest);

            Log.LogMessage("Manifest Generated");
        }
    }

    public void RemoveManifest()
    {
        if (File.Exists(ManifestPath))
        {
            File.Delete(ManifestPath);
        }
    }

}

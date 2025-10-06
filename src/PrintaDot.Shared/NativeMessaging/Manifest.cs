using PrintaDot.Shared.Common;
using System.Text.Json.Serialization;

namespace PrintaDot.Shared.NativeMessaging;

/// <summary>
/// This class is needed for generation of manifest for native application.
/// </summary>
public class Manifest
{
    [JsonPropertyName("name")]
    public string HostName => "com.printadot";

    [JsonPropertyName("description")]
    public string Description => "PrintaDot host application";

    [JsonPropertyName("path")]
    public string ExecuteablePath => Path.Combine(Utils.TargetApplicationDirectory, "PrintaDot.exe");

    [JsonPropertyName("type")]
    public string Type => "stdio";
    [JsonPropertyName("allowed_origins")]
    public string[] AllowedOrigins { get; set; } = ["chrome-extension://ncpdldoackcgjeocgpkjbfimpdjkolpg/"];

    [JsonIgnore]
    public string ManifestPath => Path.Combine(Utils.TargetApplicationDirectory, HostName + "-manifest.json");

    public Manifest() { }

    public void GenerateManifest()
    {
        var manifest = this.ToJson();

        File.WriteAllText(ManifestPath, manifest);

        Log.LogMessage("Manifest Generated");
    }

    public void RemoveManifest()
    {
        if (File.Exists(ManifestPath))
        {
            File.Delete(ManifestPath);
        }
    }
}
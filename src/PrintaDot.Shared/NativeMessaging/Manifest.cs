using PrintaDot.Shared.Common;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrintaDot.Shared.NativeMessaging;

/// <summary>
/// This class is needed for generation of manifest for native application.
/// </summary>
public static class Manifest
{
    [JsonPropertyName("name")]
    public static string HostName => "com.printadot";

    [JsonPropertyName("description")]
    public static string Description => "PrintaDot host application";

    [JsonPropertyName("path")]
    public static string ExecuteablePath => Path.Combine(Utils.TargetApplicationDirectory, Utils.GetExecutableFileName());

    [JsonPropertyName("type")]
    public static string Type => "stdio";
    [JsonPropertyName("allowed_origins")]
    public static string[] AllowedOrigins { get; set; } = ["chrome-extension://mafhdhphilplnoldeghpeaafffeibaej/"];

    [JsonIgnore]
    public static string ManifestPath => Path.Combine(Utils.TargetApplicationDirectory, ManifestFileName);
    [JsonIgnore]
    public static string ManifestFileName => HostName + "-manifest.json";
    public static void GenerateManifest()
    {
        var manifest = new Dictionary<string, object>
        {
            ["name"] = HostName,
            ["description"] = Description,
            ["path"] = ExecuteablePath,
            ["type"] = Type,
            ["allowed_origins"] = AllowedOrigins
        };

        File.WriteAllText(ManifestFileName, manifest.ToJson());

        Log.LogMessage("Manifest Generated");
    }

    public static void RemoveManifest()
    {
        if (File.Exists(ManifestPath))
        {
            File.Delete(ManifestPath);
        }
    }
}

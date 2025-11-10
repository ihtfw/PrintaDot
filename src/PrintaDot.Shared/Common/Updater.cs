using PrintaDot.Shared.NativeMessaging;
using System.Text.Json;

namespace PrintaDot.Shared.Common;

public class Updater : IDisposable
{
    /// <summary>
    /// Version of application for auto updating.
    /// </summary>
    public static readonly string PrintaDotVersion = "v1.0";

    /// <summary>
    /// Name of the file.
    /// </summary>
    public static readonly string ExeName = "PrintaDot.Windows"; //TODO: must be depends on system (Linux and Windows).

    public static readonly string Path = "C:\\Users\\user\\AppData\\Local\\PrintaDot\\";
    public static string UpdatedExePath(string newVersion) => Path + $"PrintaDot.Windows.{newVersion}.exe";
    /// <summary>;
    /// Url of repo with latest release.
    /// </summary>
    public static readonly string RepoUrl = "https://api.github.com/repos/ihtfw/PrintaDot/releases/latest";

    private readonly Timer _checkTimer;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
    private bool _disposed = false;

    public Updater()
    {
        _ = Update();

        _checkTimer = new Timer(OnTimerElapsed, null, _checkInterval, _checkInterval);
    }

    private void OnTimerElapsed(object? state)
    {
        if (!_disposed)
        {
            _ = Update();
        }
    }

    public async Task Update()
    {
        var version = await DownloadUpdate();

        Manifest.GenerateManifestWithCustomPath(UpdatedExePath(version));

        StreamHandler.Write(new CommunicationProtocol.Message { 
            Type = MessageType.UpdateNativeApp, 
            Version = 1
        });
    }

    private async Task<string?> DownloadUpdate()
    {
        try
        {
            using var httpClient = new HttpClient();
            var urlWithVersion = await GetLatestUpdateUrl(httpClient);
           
            var exeBytes = await httpClient.GetByteArrayAsync(urlWithVersion.Url);
            await File.WriteAllBytesAsync(UpdatedExePath(urlWithVersion.VersionToUpdate), exeBytes);

           return urlWithVersion.VersionToUpdate;
        }
        catch (Exception ex)
        {
            Log.LogMessage(ex.Message);
        }

        return null;
    }

    public async Task<(string? Url, string VersionToUpdate)> GetLatestUpdateUrl(HttpClient httpClient)
    { 
        httpClient.DefaultRequestHeaders.Add("User-Agent", "PrintaDot-Updater");

        var response = await httpClient.GetStringAsync(RepoUrl);
        var jsonDoc = JsonDocument.Parse(response);

        string downloadUrl = null!;
        var latestVersion = jsonDoc.RootElement.GetProperty("tag_name").GetString();

        if (IsNewerVersionExists(latestVersion))
        {
            var assets = jsonDoc.RootElement.GetProperty("assets");

            foreach (var asset in assets.EnumerateArray())
            {
                if (asset.GetProperty("name").GetString().Contains(ExeName))
                {
                    downloadUrl = asset.GetProperty("browser_download_url").GetString();
                    break;
                }
            }
        }

        return (downloadUrl, latestVersion);
    }

    private static bool IsNewerVersionExists(string? latest)
    {
        if (!string.IsNullOrWhiteSpace(latest)) return false;

        var latestParts = latest.TrimStart('v', 'V').Split('.');
        var currentParts = PrintaDotVersion.TrimStart('v', 'V').Split('.');

        for (int i = 0; i < 2; i++)
        {
            var latestPart = int.Parse(latestParts[i]);
            var currentPart = int.Parse(currentParts[i]);

            if (latestPart > currentPart) return true;
            if (latestPart < currentPart) return false;
        }

        return false;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _checkTimer?.Dispose();

            _disposed = true;
        }
    }
}

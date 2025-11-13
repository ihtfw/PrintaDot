using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace PrintaDot.Shared.Common;

public class UpdateResponseDto
{
    [JsonPropertyName("tag_name")]
    public required string LatestVersion { get; set; }

    [JsonPropertyName("assets")]
    public List<Asset>? Assets { get; set; }

    public class Asset
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public required string DownloadUrl { get; set; }
    }
}

public class Updater : IDisposable
{
    public static string TempExePath => Path.Combine(Utils.TargetApplicationDirectory, $"PrintaDot_tmp.exe");

    /// <summary>
    /// Main executable file path
    /// </summary>
    public static string MainExePath => Path.Combine(Utils.TargetApplicationDirectory, "PrintaDot.exe");

    /// <summary>
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
        await DownloadUpdate(); 
    }

    private async Task DownloadUpdate()
    {
        try
        {
            using var httpClient = new HttpClient(); // создаем 1 раз (factory)
            var url = await GetLatestUpdateUrl(httpClient);

            if (url != null)
            {
                var exeBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(TempExePath, exeBytes);

                StartUpdateProcess();
            }
        }
        catch (Exception ex)
        {
            Log.LogMessage(ex.Message);
        }
    }

    private void StartUpdateProcess()
    {
        try
        {
            Process.Start(TempExePath, "--update");

            Environment.Exit(0); //Closing main process...                           
        }
        catch (Exception ex)
        {
            Log.LogMessage($"Failed to start update process: {ex.Message}");
        }
    }

    public void DeleteTempFile()
    {
        if (File.Exists(TempExePath))
        {
            File.Delete(TempExePath);
        }
    }

    public void PerformUpdate()
    {
        try
        {
            //Waiting for main procces is closed..
            Thread.Sleep(2000);

            if (File.Exists(MainExePath))
            {
                File.Delete(MainExePath);
            }

            File.Copy(TempExePath, MainExePath, true);

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Log.LogMessage($"Update failed: {ex.Message}");
        }
    }

    public async Task<string?> GetLatestUpdateUrl(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Add("User-Agent", "PrintaDot-Updater");

        var response = await httpClient.GetStringAsync(RepoUrl);
        var updateResponse = response.FromJson<UpdateResponseDto>(isSkip: true);

        if (updateResponse is null)
        {
            return null;
        }

        if (IsNewerVersionExists(updateResponse.LatestVersion) && updateResponse.Assets is not null)
        {
            foreach (var asset in updateResponse.Assets)
            {
                if (asset.Name == Utils.GetExecutableFileName())
                {
                    return asset.DownloadUrl;
                }
            }
        }

        return null;
    }

    private static bool IsNewerVersionExists(string? latest)
    {
        if (!string.IsNullOrWhiteSpace(latest)) return false;

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        var assemblyVersion = assembly.GetName().Version!;
        var currentAppVersion = Convert.ToInt32($"{assemblyVersion.Major}{assemblyVersion.Minor}{assemblyVersion.Build}");
        var latestAppVersion = Convert.ToInt32(latest.TrimStart('v', '.'));

        return latestAppVersion > currentAppVersion;
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

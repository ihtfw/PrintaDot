using PrintaDot.Common;
using PrintaDot.CommunicationProtocol;
using PrintaDot.CommunicationProtocol.V1;
using PrintaDot.Printing;

namespace PrintaDot.NativeMessaging;

/// <summary>
/// Class to communicate with browsers.
/// </summary>
public class Host
{
    private readonly Manifest _manifest;
    private bool _sendConfirmationReceipt;
    private readonly PrintService _printService;

    /// <summary>
    /// List of supported browsers.
    /// </summary>
    public List<Browser> SupportedBrowsers { get; }

    /// <summary>
    /// Creates the Host object
    /// </summary>
    /// <param name="sendConfirmationReceipt"><see langword="true" /> for the host to automatically send message confirmation receipt.</param>
    public Host(bool sendConfirmationReceipt = true)
    {
        SupportedBrowsers = new List<Browser>(2);

        _sendConfirmationReceipt = sendConfirmationReceipt;

        _manifest = new Manifest();
        _printService = new PrintService();
    }

    /// <summary>
    /// Starts listening for input.
    /// </summary>
    public void Listen()
    {
        if (!SupportedBrowsers.IsAnyRegistered(_manifest.HostName, _manifest.ManifestPath))
        {
            throw new NotRegisteredWithBrowserException(_manifest.HostName);
        }

        Message? message;

        while ((message = StreamHandler.Read()) != null)
        {
            var exactMessage = message as PrintRequestMessageV1;

            if (message != null)
            {
                Print(exactMessage);
            }

            Log.LogMessage("Data Received:" + message.ToJson());

            if (_sendConfirmationReceipt)
            {
                StreamHandler.Write(exactMessage.ToJson());
            }
        }
    }

    private void Print(PrintRequestMessageV1 message)
    {
        try
        {
            _printService.PrintRequestMessageV1(message);
            Log.LogMessage($"Print job completed: {message.Version} - {message.Type}");
        }
        catch (Exception ex)
        {
            Log.LogMessage($"Print error: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates the manifest and saves it to the correct location.
    /// </summary>
    /// <param name="overwrite">Determines if the manifest should be overwritten if it already exists.<br />Defaults to <see langword="false"/>.</param>
    public void GenerateManifest(bool overwrite = true)
    {
        _manifest.GenerateManifest(overwrite);
    }

    /// <summary>
    /// Register all supported Browsers (Chrome and Explorer in this version of application).
    /// </summary>
    public void RegisterAllSupportedBrowsers()
    {
        SupportedBrowsers.Add(BrowserCreator.GoogleChrome);
        SupportedBrowsers.Add(BrowserCreator.MicrosoftEdge);

        SupportedBrowsers.Register(_manifest.HostName, _manifest.ManifestPath);
    }

    /// <summary>
    /// Unregister from all supported Browsers (Chrome and Explorer in this version of application).
    /// </summary>
    public void Unregister()
    {
        SupportedBrowsers.Unregister(_manifest.HostName);
    }
}

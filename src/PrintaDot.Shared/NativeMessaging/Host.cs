using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.CommunicationProtocol.V1;
using PrintaDot.Shared.Printing;

namespace PrintaDot.Shared.NativeMessaging;

/// <summary>
/// Class to communicate with browsers.
/// </summary>
public class Host
{
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

        _printService = new PrintService();
    }

    /// <summary>
    /// Starts listening for input.
    /// </summary>
    public void Listen()
    {
        if (!SupportedBrowsers.IsAnyRegistered(Manifest.HostName, Manifest.ManifestPath))
        {
            throw new NotRegisteredWithBrowserException(Manifest.HostName);
        }

        Message? message;

        while ((message = StreamHandler.Read()) != null)
        {
            if (message is not null)
            {
                if (message.Type == MessageType.Exception)
                {
                    StreamHandler.Write(message);
                }
                else
                {
                    ProcessMessageByType(message);
                }
            }
        }
    }

    /// <summary>
    /// Processes the message based on its version.
    /// </summary>
    /// <param name="message">The message to process and route.</param>
    private void ProcessMessageByType(Message message)
    {
        switch (message.Version)
        {
            case 1:
                ProcessMessageV1(message);
                break;
        }
    }

    /// <summary>
    /// Processes V1 messages.
    /// </summary>
    /// <param name="message">The message to process and route.</param>
    public void ProcessMessageV1(Message message)
    {
        switch (message)
        {
            case ExceptionMessageV1:
                var exceptionMessage = message as ExceptionMessageV1;
                Log.LogMessage(exceptionMessage!.MessageText);

                StreamHandler.Write(exceptionMessage);
                break;
            case PrintRequestMessageV1:
                _printService.PrintRequestMessageV1((message as PrintRequestMessageV1)!);
                break;
            case GetPrintStatusRequestMessageV1:
                // TODO: Add handling for GetPrintStatusRequestMessageV1
                break;
            default:
                Log.LogMessage("Current type of messages is not supported");

                var exception = ExceptionMessageV1.Create("Current type of messages is not supported");
                StreamHandler.Write(exception);
                break;
        }
    }

    /// <summary>
    /// Generates the manifest and saves it to the correct location.
    /// </summary>
    /// <param name="overwrite">Determines if the manifest should be overwritten if it already exists.<br />Defaults to <see langword="false"/>.</param>
    public void GenerateManifest()
    {
        Manifest.GenerateManifest();
    }

    /// <summary>
    /// Register all supported Browsers (Chrome and Explorer in this version of application).
    /// </summary>
    public void RegisterAllSupportedBrowsers()
    {
        SupportedBrowsers.Add(BrowserCreator.GoogleChrome);
        SupportedBrowsers.Add(BrowserCreator.MicrosoftEdge);

        SupportedBrowsers.Register(Manifest.HostName, Manifest.ManifestPath);
    }

    /// <summary>
    /// Unregister from all supported Browsers (Chrome and Explorer in this version of application).
    /// </summary>
    public void Unregister()
    {
        SupportedBrowsers.Unregister(Manifest.HostName);
    }

    public void MoveHostToLocalAppData()
    {
        Utils.MoveApplicationToLocalAppData();
    }
}

using System.Reflection;
using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.CommunicationProtocol.V1.Requests;
using PrintaDot.Shared.CommunicationProtocol.V1.Responses;
using PrintaDot.Shared.ImageGeneration.V1;
using PrintaDot.Shared.Platform;
using PrintaDot.Shared.Printing;

namespace PrintaDot.Shared.NativeMessaging;

/// <summary>
/// Class to communicate with browsers.
/// </summary>
public class Host
{
    
    private bool _sendConfirmationReceipt;

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
        SupportedBrowsers = [BrowserCreator.GoogleChrome, BrowserCreator.MicrosoftEdge];

        _sendConfirmationReceipt = sendConfirmationReceipt;
    }

    public required IPlatformPrintingService PlatformPrintingService { get; init; }

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
                ProcessMessageByType(message);
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
            case GetPrintersRequest getPrintersRequest:
                var printersResponse = PlatformPrintingService.GetInstalledPrinters(getPrintersRequest.Id);

                StreamHandler.Write(printersResponse);
                break;
            case PrintRequestMessageV1 printRequestMessageV1:
                var barcodeImageGenerator = new BarcodeImageGeneratorV1(printRequestMessageV1);

                var paperSettings = new PaperSettings()
                {
                    Height = printRequestMessageV1.Profile.PaperHeight,
                    Width = printRequestMessageV1.Profile.PaperWidth,
                    LabelsPerColumn = printRequestMessageV1.Profile.LabelsPerColumn,
                    LabelsPerRow = printRequestMessageV1.Profile.LabelsPerRow
                };

                var printService = new PrintService(barcodeImageGenerator, PlatformPrintingService, paperSettings);
                var isPrintedSuccessfully = printService.Print(printRequestMessageV1.Profile.PrinterName);

                if (isPrintedSuccessfully)
                {
                    StreamHandler.Write(PrintResponseMessageV1.CreateSuccessResponse(message.Id));
                } else
                {
                    StreamHandler.Write(PrintResponseMessageV1.CreateFailedResponse(message.Id));
                }
                break;
            case GetPrintStatusRequestMessageV1:
                // TODO: Add handling for GetPrintStatusRequestMessageV1
                break;
            default:
                Log.LogMessage("Current type of messages is not supported");

                var exception = ExceptionResponseV1.Create("Current type of messages is not supported");
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
    /// </summary>;
    public void RegisterAllSupportedBrowsers()
    {
        SupportedBrowsers.Register(Manifest.HostName, Manifest.ManifestPath);
    }

    /// <summary>
    /// Unregister from all supported Browsers (Chrome and Explorer in this version of application).
    /// </summary>
    public void Unregister()
    {
        SupportedBrowsers.Unregister(Manifest.HostName);
    }

    public void ConfigureHost()
    {
        Utils.CreatePrintaDotFolderInLocalAppData();
        Manifest.GenerateManifest();
        RegisterAllSupportedBrowsers();
        Utils.MoveApplicationToLocalAppData();
    }
}

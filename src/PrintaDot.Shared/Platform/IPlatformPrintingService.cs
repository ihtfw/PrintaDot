using PrintaDot.Shared.CommunicationProtocol;
using PrintaDot.Shared.Printing;
using SixLabors.ImageSharp;

namespace PrintaDot.Shared.Platform;

public interface IPlatformPrintingService
{
    bool Print(string printerName, List<Image> images, PaperSettings paperSettings);
    GetPrintersResponse GetInstalledPrinters(Guid messageId);
}

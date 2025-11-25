using PrintaDot.Shared.Common;
using PrintaDot.Shared.ImageGeneration;
using SixLabors.ImageSharp;
using PrintaDot.Shared.Platform;

using Image = SixLabors.ImageSharp.Image;
using PrintaDot.Shared.CommunicationProtocol.V1.Requests;
using PrintaDot.Shared.CommunicationProtocol.V1.Responses;

namespace PrintaDot.Shared.Printing;

public class PrintService
{
    private readonly PaperSettings _paperSettings;

    private readonly IPrintaDotImageGenerator _imageGenerator;
    private readonly IPlatformPrintingService _platformPrintingService;

    public PrintService(IPrintaDotImageGenerator imageGenerator, IPlatformPrintingService platformPrintingService, PaperSettings paperSettings)
    {
        _imageGenerator = imageGenerator;
        _platformPrintingService = platformPrintingService;
        _paperSettings = paperSettings;
    }

    public bool Print(string printerName)
    {
        var images = _imageGenerator.GenerateImages();
#if DEBUG
        SaveImageToDesktop(images);
#endif
        return _platformPrintingService.Print(printerName, images, _paperSettings);
    }

    /// <summary>
    /// For debug. Saves generated image on desktop inside "BarcodeImages" directory.
    /// </summary>
    /// <param name="images">Generated  ImageSharp image </param>
    private void SaveImageToDesktop(List<Image> images)
    {
        foreach (var image in images)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                string imagesFolder = Path.Combine(desktopPath, "BarcodeImages");
                Directory.CreateDirectory(imagesFolder);

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                string fileName = $"barcode_{timestamp}.bmp";
                string filePath = Path.Combine(imagesFolder, fileName);

                image.SaveAsBmp(filePath);
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error when saving image: {ex.Message}");
            }
        }
    }

    public GetPrintStatusResponseMessageV1 GetPrintStatusResponseMessageV1(GetPrintStatusRequestMessageV1 request)
    {
        return new GetPrintStatusResponseMessageV1
        {
            Type = MessageType.GetPrintStatusResponse,
            Version = 1,
            Guid = Guid.NewGuid(),
            PrintStatus = PrintStatus.Success,
            Details = null,
        };
    }
}

using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol.V1;
using PrintaDot.Shared.ImageGeneration;
using SixLabors.ImageSharp;
using PrintaDot.Shared.Platform;

using Image = SixLabors.ImageSharp.Image;
using static System.Net.Mime.MediaTypeNames;

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

    public void Print(string printerName)
    {
        var zebra = "ZDesigner ZD411-203dpi ZPL";
        var microsoftToPdf = "Microsoft Print To PDF";

        var images = _imageGenerator.GenerateImages();
#if DEBUG
        SaveImageToDesktop(images);
#endif
        _platformPrintingService.Print(microsoftToPdf, images, _paperSettings);
    }

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

                Console.WriteLine($"Image saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when saving image: {ex.Message}");
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

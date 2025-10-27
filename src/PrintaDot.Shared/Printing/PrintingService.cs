using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol.V1;
using PrintaDot.Shared.ImageGeneration;
using SixLabors.ImageSharp;
using System.Drawing;
using System.Drawing.Printing;
using PrintaDot.Shared.Platform;
using Image = SixLabors.ImageSharp.Image;

namespace PrintaDot.Shared.Printing;

public class PrintService
{
    private readonly BarcodeImageGenerator _barcodeImageGenerator;
    private readonly IPlatformPrintingService _platformPrintingService;

    public PrintService(BarcodeImageGenerator barcodeImageGenerator, IPlatformPrintingService platformPrintingService)
    {
        _barcodeImageGenerator = barcodeImageGenerator;
        _platformPrintingService = platformPrintingService;
    }

    // public void PrintRequestMessageV1(PrintRequestMessageV1 message)
    // {
    //     using var printDocument = new PrintDocument();
    //     printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;
    //
    //     printDocument.PrintPage += (sender, e) =>
    //     {
    //         using var font = new Font("Arial", 14);
    //         e.Graphics?.DrawString(message.Version.ToString(), font, Brushes.Black, 100, 100);
    //         e.Graphics?.DrawString(message.Profile.ProfileName, font, Brushes.Black, 100, 130);
    //         e.Graphics?.DrawString($"Date: {DateTime.Now}", font, Brushes.Black, 100, 160);
    //
    //         if (message.Items.Any())
    //         {
    //             e.Graphics?.DrawString(message.Profile.ProfileName, font, Brushes.Black, 100, 190);
    //             e.Graphics?.DrawString(message.Profile.Id.ToString(), font, Brushes.Black, 100, 220);
    //             e.Graphics?.DrawString(message.Profile.PrinterName.ToString(), font, Brushes.Black, 100, 250);
    //         }
    //     };
    //
    //     printDocument.Print();
    // }

    public void Print(string printerName)
    {
        var zebra = "ZDesigner ZD411-203dpi ZPL";
        var microsoftToPdf = "Microsoft Print To PDF";

        var images = _barcodeImageGenerator.GenerateBarcodeImage();

        foreach (var image in images) 
        {
            _platformPrintingService.Print(zebra, image);
            SaveImageToDesktop(image);
        }
    }

    private void SaveImageToDesktop(Image image)
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

            Console.WriteLine($"Изображение сохранено: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
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

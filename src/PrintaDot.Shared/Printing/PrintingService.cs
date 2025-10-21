using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol.V1;
using PrintaDot.Shared.ImageGeneration;
using SixLabors.ImageSharp;
using System.Drawing;
using System.Drawing.Printing;
using Image = SixLabors.ImageSharp.Image;

namespace PrintaDot.Shared.Printing;

public class PrintService
{
    private BarcodeImageGenerator _barcodeImageGenerator;

    public PrintService(BarcodeImageGenerator barcodeImageGenerator)
    {
        _barcodeImageGenerator = barcodeImageGenerator;
    }

    public void PrintRequestMessageV1(PrintRequestMessageV1 message)
    {
        using var printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;

        printDocument.PrintPage += (sender, e) =>
        {
            using var font = new Font("Arial", 14);
            e.Graphics?.DrawString(message.Version.ToString(), font, Brushes.Black, 100, 100);
            e.Graphics?.DrawString(message.Profile.ProfileName, font, Brushes.Black, 100, 130);
            e.Graphics?.DrawString($"Date: {DateTime.Now}", font, Brushes.Black, 100, 160);

            if (message.Items.Any())
            {
                e.Graphics?.DrawString(message.Profile.ProfileName, font, Brushes.Black, 100, 190);
                e.Graphics?.DrawString(message.Profile.Id.ToString(), font, Brushes.Black, 100, 220);
                e.Graphics?.DrawString(message.Profile.PrinterName.ToString(), font, Brushes.Black, 100, 250);
            }
        };

        printDocument.Print();
    }

    public void Print()
    {
        var images = _barcodeImageGenerator.GenerateBarcodeImage();

        foreach (var image in images) 
        {
            //PrintImage(image);
            SaveImageToDesktop(image);
        }
    }

    private void SaveImageToDesktop(Image image)
    {
        try
        {
            // Получаем путь к рабочему столу
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Создаем папку для изображений, если ее нет
            string imagesFolder = Path.Combine(desktopPath, "BarcodeImages");
            Directory.CreateDirectory(imagesFolder);

            // Генерируем уникальное имя файла с временной меткой
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string fileName = $"barcode_{timestamp}.png";
            string filePath = Path.Combine(imagesFolder, fileName);

            // Сохраняем изображение
            image.SaveAsPng(filePath);

            Console.WriteLine($"Изображение сохранено: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении изображения: {ex.Message}");
        }
    }

    public void PrintImage(SixLabors.ImageSharp.Image image)
    {
        using var printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;

        printDocument.PrintPage += (sender, e) =>
        {
            using var memoryStream = new MemoryStream();
            image.SaveAsPng(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var systemDrawingImage = System.Drawing.Image.FromStream(memoryStream);

            if (e.Graphics != null)
            {
                e.Graphics.DrawImage(systemDrawingImage, 0, 0, systemDrawingImage.Width, systemDrawingImage.Height);
            }
        };

        printDocument.Print();
    }

    public GetPrintStatusResponseMessageV1 GetPrintStatusResponseMessageV1(GetPrintStatusRequestMessageV1 request)
    {
        // TODO Implement logic of getting printing status
        return new GetPrintStatusResponseMessageV1 
        {
            Type = MessageType.GetPrintStatusResponse,
            Version = 1,
            Guid  = Guid.NewGuid(),
            PrintStatus = PrintStatus.Success,
            Details = null,
        };
    }
}
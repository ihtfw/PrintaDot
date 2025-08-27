using PrintaDot.NativeMessaging;
using System.Drawing;
using System.Drawing.Printing;

namespace PrintaDot.Printing;
public class PrintService
{

    public void PrintSample(ReceivedData data)
    {
        using var printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;

        printDocument.PrintPage += (sender, e) =>
        {
            using var font = new Font("Arial", 14);
            e.Graphics?.DrawString($"Sample: {data.SampleName}", font, Brushes.Black, 100, 100);
            e.Graphics?.DrawString($"Barcode: {data.Barcode}", font, Brushes.Black, 100, 130);
            e.Graphics?.DrawString($"Date: {DateTime.Now}", font, Brushes.Black, 100, 160);
        };

        printDocument.Print();
    }
}
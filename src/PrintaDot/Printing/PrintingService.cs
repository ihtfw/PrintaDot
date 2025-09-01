using PrintaDot.NativeMessaging.CommunicationProtocol.V1;
using System.Drawing;
using System.Drawing.Printing;

namespace PrintaDot.Printing;

public class PrintService
{
    public void PrintRequestMessageV1(PrintRequestMessageV1 message)
    {
        using var printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = new PrinterSettings().PrinterName;

        printDocument.PrintPage += (sender, e) =>
        {
            using var font = new Font("Arial", 14);
            e.Graphics?.DrawString(message.Version.ToString(), font, Brushes.Black, 100, 100);
            e.Graphics?.DrawString(message.Profile, font, Brushes.Black, 100, 130);
            e.Graphics?.DrawString($"Date: {DateTime.Now}", font, Brushes.Black, 100, 160);
        };

        printDocument.Print();
    }
}
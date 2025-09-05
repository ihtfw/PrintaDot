using PrintaDot.Shared.CommunicationProtocol.V1;
using System.Drawing;
using System.Drawing.Printing;

namespace PrintaDot.Shared.Printing;

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

            if (message.Items.Any())
            {
                e.Graphics?.DrawString(message.Items[0].Header, font, Brushes.Black, 100, 190);
                e.Graphics?.DrawString(message.Items[0].Barcode, font, Brushes.Black, 100, 220);
                e.Graphics?.DrawString(message.Items[0].Figures, font, Brushes.Black, 100, 250);
            }
        };

        printDocument.Print();
    }

    public GetPrintStatusResponseMessageV1 GetPrintStatusResponseMessageV1(GetPrintStatusRequestMessageV1 request)
    {
        // TODO Implement logic of getting printing status
        return new GetPrintStatusResponseMessageV1 
        {
            Type = "getPrintStatusResponse",
            Version = 1,
            Guid  = Guid.NewGuid(),
            PrintStatus = PrintStatus.Success,
            Details = null,
        };
    }
}
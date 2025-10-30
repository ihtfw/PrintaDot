using PrintaDot.Shared.CommunicationProtocol.V1;
using PrintaDot.Shared.Common;

namespace PrintaDot.Tests.V1;

internal class MessageV1Creator
{
    public PrintRequestMessageV1 CreatePrintRequestMessageV1Thermo()
    {
        return new PrintRequestMessageV1
        {
            Version = 1,
            Type = MessageType.PrintRequest,
            Profile = new PrintRequestMessageV1.PrintProfile
            {
                Id = 1,
                ProfileName = "Default Profile",
                PaperHeight = 24,
                PaperWidth = 45,
                LabelHeight = 24,
                LabelWidth = 45,
                MarginX = 9,
                MarginY = 2,
                OffsetX = 0,
                OffsetY = 0,
                LabelsPerRow = 1,
                LabelsPerColumn = 1,
                TextAlignment = "Left",
                TextMaxLength = 0,
                TextTrimLength = 0,
                TextFontSize = 20,
                TextAngle = 0,
                PrinterName = "default",
                UseDataMatrix = false,
                NumbersAlignment = "Left",
                NumbersFontSize = 20,
                NumbersAngle = 0,
                BarcodeAlignment = "Right",
                BarcodeFontSize = 20,
                BarcodeAngle = 0
            },
            Items = new List<PrintRequestMessageV1.Item>
                {
                    new() { Header = "Atemy 1", Barcode = "1234567890", Figures = "Atem 1" }
                }
        };
    }

    public PrintRequestMessageV1 CreatePrintRequestMessageV1A4()
    {
        return new PrintRequestMessageV1
        {
            Version = 1,
            Type = MessageType.PrintRequest,
            Profile = new PrintRequestMessageV1.PrintProfile
            {
                Id = 2,
                ProfileName = "Fallback Profile",
                PaperHeight = 297,
                PaperWidth = 210,
                LabelHeight = 50,
                LabelWidth = 80,
                MarginX = 2,
                MarginY = 2,
                OffsetX = 0,
                OffsetY = 0,
                LabelsPerRow = 2,
                LabelsPerColumn = 5,
                TextAlignment = "Left",
                TextMaxLength = 20,
                TextTrimLength = 18,
                TextFontSize = 12,
                TextAngle = 0,
                PrinterName = "default",
                UseDataMatrix = false,
                NumbersAlignment = "Left",
                NumbersFontSize = 10,
                NumbersAngle = 0,
                BarcodeAlignment = "Left",
                BarcodeFontSize = 8,
                BarcodeAngle = 0
            },
            Items = new List<PrintRequestMessageV1.Item>
                {
                    new() { Header = "Fallback 1", Barcode = "1122334455", Figures = "C3" }
                }
        };
    }
}

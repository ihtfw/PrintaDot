namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class ProfileMessageV1 : Message
{
    public required string ProfileName { get; set; }

    // Main settings
    public required double PaperHeight { get; set; }
    public required double PaperWidth { get; set; }
    public required double LabelHeight { get; set; }
    public required double LabelWidth { get; set; }
    public required double MarginX { get; set; }
    public required double MarginY { get; set; }
    public required double OffsetX { get; set; }
    public required double OffsetY { get; set; }
    public int LabelsPerRow { get; set; }
    public int LabelsPerColumn { get; set; }

    // Text settings
    public required string TextAlignment { get; set; }
    public int TextMaxLength { get; set; }
    public int TextTrimLength { get; set; }
    public required double TextFontSize { get; set; }
    public required double TextAngle { get; set; }
    
    //Printer
    public required string PrinterName { get; set; }

    // Barcode type
    public bool UseDataMatrix { get; set; }

    // Settings of number
    public required string NumbersAlignment { get; set; }
    public required double NumbersFontSize { get; set; }
    public required double NumbersAngle { get; set; }

    // Settings of barcode
    public required string BarcodeAlignment { get; set; }
    public required double BarcodeFontSize { get; set; }
    public required double BarcodeAngle { get; set; }
}

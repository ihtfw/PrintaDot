using PrintaDot.Shared.CommunicationProtocol.V1;

namespace PrintaDot.Shared.ImageGeneration.V1;

internal class ImageProfileV1
{
    private const float DPI = 300f;

    public ImageProfileV1(PrintRequestMessageV1.PrintProfile profile)
    {
        Id = profile.Id;
        ProfileName = profile.ProfileName;

        PaperHeight = ImageGenerationHelper.MmToDpi(profile.PaperHeight);
        PaperWidth = ImageGenerationHelper.MmToDpi(profile.PaperWidth);
        LabelHeight = ImageGenerationHelper.MmToDpi(profile.LabelHeight);
        LabelWidth = ImageGenerationHelper.MmToDpi(profile.LabelWidth);
        MarginX = ImageGenerationHelper.MmToDpi(profile.MarginX);
        MarginY = ImageGenerationHelper.MmToDpi(profile.MarginY);
        OffsetX = ImageGenerationHelper.MmToDpi(profile.OffsetX);
        OffsetY = ImageGenerationHelper.MmToDpi(profile.OffsetY);
        LabelsPerRow = profile.LabelsPerRow;
        LabelsPerColumn = profile.LabelsPerColumn;

        TextAlignment = profile.TextAlignment;
        TextMaxLength = profile.TextMaxLength;
        TextTrimLength = profile.TextTrimLength;
        TextFontSize = ImageGenerationHelper.MmToDpi(profile.TextFontSize);
        TextAngle = profile.TextAngle;

        PrinterName = profile.PrinterName;

        UseDataMatrix = profile.UseDataMatrix;

        NumbersAlignment = profile.NumbersAlignment;
        NumbersFontSize = ImageGenerationHelper.MmToDpi(profile.NumbersFontSize);
        NumbersAngle = profile.NumbersAngle;

        BarcodeAlignment = profile.BarcodeAlignment;
        BarcodeFontSize = ImageGenerationHelper.MmToDpi(profile.BarcodeFontSize);
        BarcodeAngle = profile.BarcodeAngle;
    }

    public int Id { get; set; }
    public string ProfileName { get; set; }

    // Main settings
    public float PaperHeight { get; set; }
    public float PaperWidth { get; set; }
    public float LabelHeight { get; set; }
    public float LabelWidth { get; set; }
    public float MarginX { get; set; }
    public float MarginY { get; set; }
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public int LabelsPerRow { get; set; }
    public int LabelsPerColumn { get; set; }

    // Text settings
    public string TextAlignment { get; set; }
    public int TextMaxLength { get; set; }
    public int TextTrimLength { get; set; }
    public float TextFontSize { get; set; }
    public double TextAngle { get; set; }

    // Printer
    public string PrinterName { get; set; }

    // Barcode type
    public bool UseDataMatrix { get; set; }

    // Settings of number
    public string NumbersAlignment { get; set; }
    public float NumbersFontSize { get; set; }
    public double NumbersAngle { get; set; }

    // Settings of barcode
    public string BarcodeAlignment { get; set; }
    public float BarcodeFontSize { get; set; }
    public float BarcodeFontSizeWidth => BarcodeFontSize * 5.0f;
    public float BarcodeAngle { get; set; }
}
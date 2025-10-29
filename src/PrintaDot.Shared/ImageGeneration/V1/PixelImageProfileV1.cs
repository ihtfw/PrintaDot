using PrintaDot.Shared.CommunicationProtocol.V1;

namespace PrintaDot.Shared.ImageGeneration.V1;

internal class PixelImageProfileV1
{
    public PixelImageProfileV1(PrintRequestMessageV1.PrintProfile profile)
    {
        Id = profile.Id;
        ProfileName = profile.ProfileName;

        PaperHeight = ImageGenerationHelper.FromMmToPixels(profile.PaperHeight);
        PaperWidth = ImageGenerationHelper.FromMmToPixels(profile.PaperWidth);
        LabelHeight = ImageGenerationHelper.FromMmToPixels(profile.LabelHeight);
        LabelWidth = ImageGenerationHelper.FromMmToPixels(profile.LabelWidth);
        MarginX = ImageGenerationHelper.FromMmToPixels(profile.MarginX);
        MarginY = ImageGenerationHelper.FromMmToPixels(profile.MarginY);
        OffsetX = ImageGenerationHelper.FromMmToPixels(profile.OffsetX);
        OffsetY = ImageGenerationHelper.FromMmToPixels(profile.OffsetY);
        LabelsPerRow = profile.LabelsPerRow;
        LabelsPerColumn = profile.LabelsPerColumn;

        TextAlignment = profile.TextAlignment;
        TextMaxLength = profile.TextMaxLength;
        TextTrimLength = profile.TextTrimLength;
        TextFontSize = ImageGenerationHelper.FontSizeToPixels(profile.TextFontSize);
        TextAngle = profile.TextAngle;

        PrinterName = profile.PrinterName;

        UseDataMatrix = profile.UseDataMatrix;

        NumbersAlignment = profile.NumbersAlignment;
        NumbersFontSize = ImageGenerationHelper.FontSizeToPixels(profile.NumbersFontSize);
        NumbersAngle = profile.NumbersAngle;

        BarcodeAlignment = profile.BarcodeAlignment;
        BarcodeFontSize = ImageGenerationHelper.FontSizeToPixels(profile.BarcodeFontSize);
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
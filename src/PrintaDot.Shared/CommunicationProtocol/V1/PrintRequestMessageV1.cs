namespace PrintaDot.Shared.CommunicationProtocol.V1;

public class PrintRequestMessageV1 : Message
{
    public required PrintProfile Profile { get; set; }
    public List<Item> Items { get; set; }

    public class Item
    {
        public string? Header { get; set; }
        public required string Barcode { get; set; }
        public string? Figures { get; set; }
    }

    public class PrintProfile
    {
        public required int Id { get; set; }
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
        public required int LabelsPerRow { get; set; }
        public required int LabelsPerColumn { get; set; }

        // Text settings
        public required string TextAlignment { get; set; }
        public required int TextMaxLength { get; set; }
        public required int TextTrimLength { get; set; }
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
}

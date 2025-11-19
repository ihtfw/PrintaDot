namespace PrintaDot.Shared.CommunicationProtocol.V1.Requests;

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
        public required float PaperHeight { get; set; }
        public required float PaperWidth { get; set; }
        public required float LabelHeight { get; set; }
        public required float LabelWidth { get; set; }
        public required float MarginX { get; set; }
        public required float MarginY { get; set; }
        public required float OffsetX { get; set; }
        public required float OffsetY { get; set; }
        public required int LabelsPerRow { get; set; }
        public required int LabelsPerColumn { get; set; }

        // Text settings
        public required string TextAlignment { get; set; }
        public required int TextMaxLength { get; set; }
        public required int TextTrimLength { get; set; }
        public required float TextFontSize { get; set; }
        public required float TextAngle { get; set; }

        //Printer
        public required string PrinterName { get; set; }

        // Barcode type
        public bool UseDataMatrix { get; set; }

        // Settings of number
        public required string NumbersAlignment { get; set; }
        public required float NumbersFontSize { get; set; }
        public required float NumbersAngle { get; set; }

        // Settings of barcode
        public required string BarcodeAlignment { get; set; }
        public required float BarcodeFontSize { get; set; }
        public required float BarcodeAngle { get; set; }
    }
}

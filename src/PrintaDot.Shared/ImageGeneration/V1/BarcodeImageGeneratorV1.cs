using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ZXing;
using ZXing.Common;
using ZXing.Datamatrix.Encoder;
using PrintaDot.Shared.CommunicationProtocol.V1;
using ZXing.Datamatrix;
using ZXing.ImageSharp.Rendering;
using System.Reflection.Emit;

namespace PrintaDot.Shared.ImageGeneration.V1;

public class BarcodeImageGeneratorV1 : BarcodeImageGenerator
{
    private List<PrintRequestMessageV1.Item> _items;
    private ImageProfileV1 _profile;

    private const float MARGIN_BETWEEN_TEXT_AND_BARCODE = 2.0f;

    public BarcodeImageGeneratorV1(PrintRequestMessageV1 message)
    {
        _profile = new ImageProfileV1(message.Profile);
        _items = message.Items;
    }

    public List<Image> GenerateBarcodeImage()
    {
        var images = new List<Image>();

        foreach (var item in _items)
        {
            var image = new Image<Rgba32>((int)_profile.LabelWidth, (int)_profile.LabelHeight);

            image.Mutate(ctx => ctx.BackgroundColor(Color.White));

            DrawLabel(image, item);

            images.Add(image);
        }

        return images;
    }

    private Figure CalculateTextSize(string text, float fontSize)
    {
        var font = SystemFonts.CreateFont("Arial", fontSize);
        var textSize = TextMeasurer.MeasureSize(text, new TextOptions(font));

        return new Figure
        {
            Width = textSize.Width,
            Height = textSize.Height
        };
    }

    private void DrawLabel(Image<Rgba32> image, PrintRequestMessageV1.Item item)
    {
        var headerTextSize = CalculateTextSize(item.Header, _profile.TextFontSize);
        var figuresTextSize = CalculateTextSize(item.Figures, _profile.NumbersFontSize);

        var barcodeSize = new Figure { Width = 150, Height = _profile.BarcodeFontSize };

        var heightCenter = _profile.LabelHeight / 2;

        var barcodeHalfAndHeaderHeight = headerTextSize.Height + barcodeSize.Height / 2.0f + MARGIN_BETWEEN_TEXT_AND_BARCODE;
        var barcodeHalfAndFiguresHeight = figuresTextSize.Height + barcodeSize.Height / 2.0f + MARGIN_BETWEEN_TEXT_AND_BARCODE;
        
        var allElementsHeight = 2.0f * MARGIN_BETWEEN_TEXT_AND_BARCODE + figuresTextSize.Height + headerTextSize.Height + barcodeSize.Height;

        PointF headerCenter = new(), figuresCenter = new(), barcodeTopLeft= new();

        if (allElementsHeight > _profile.LabelHeight)
        {
            //ЕСЛИ СУММА ВЫСОТ ВСЕХ ЭЛЕМЕНТОВ БОЛЬШЕ ЧЕМ ВЫСОТА ЛЕЙБЛА
        }
        else if (barcodeHalfAndHeaderHeight > heightCenter)
        {
            headerCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height / 2);      

            var barcodeCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height + MARGIN_BETWEEN_TEXT_AND_BARCODE);
            barcodeTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(barcodeCenter, barcodeSize.Width, barcodeSize.Height);

            figuresCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height + MARGIN_BETWEEN_TEXT_AND_BARCODE * 2.0f + barcodeSize.Height);           
        }
        else if (barcodeHalfAndFiguresHeight > heightCenter) 
        { 

        }
        else
        {
            headerCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f - barcodeSize.Height / 2.0f - headerTextSize.Height / 2.0f - MARGIN_BETWEEN_TEXT_AND_BARCODE);
            //Label.HeaderTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(headerCenter, headerTextSize.Width, headerTextSize.Height);

            var barcodeCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f);
            barcodeTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(barcodeCenter, barcodeSize.Width, barcodeSize.Height);

            figuresCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f + barcodeSize.Height / 2.0f + figuresTextSize.Height / 2.0f + MARGIN_BETWEEN_TEXT_AND_BARCODE);
            //Label.FiguresTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(figuresCenter, figuresTextSize.Width, figuresTextSize.Height);

            //DrawText(image, item.Header, headerCenter, _profile.TextFontSize);
            //DrawBarcode(image, item.Barcode, Label.BarcodeTopLeft);
            //DrawText(image, item.Figures, figuresCenter, _profile.NumbersFontSize);
        }


        DrawText(image, item.Header, headerCenter, _profile.TextFontSize);
        DrawBarcode(image, item.Barcode, barcodeTopLeft);
        DrawText(image, item.Figures, figuresCenter, _profile.NumbersFontSize);
    }

    private void DrawText(Image<Rgba32> image, string text, PointF topLeft, float fontSize)
    {
        var font = SystemFonts.CreateFont("Arial", fontSize);

        var textOptions = new RichTextOptions(font)
        {
            Origin = topLeft,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Start,
        };

        //// Обрезаем текст если нужно
        //var displayText = item.Header.Length > _profile.TextMaxLength
        //    ? item.Header.Substring(0, _profile.TextTrimLength)
        //    : item.Header;

        image.Mutate(ctx => ctx.DrawText(textOptions, text, Color.Black));
    }

    private void DrawBarcode(Image<Rgba32> image, string barcode, PointF topLeft)
    {
        var barcodeImage = GenerateBarcode(barcode);
        if (barcodeImage == null) return;

        var point = new Point((int)topLeft.X, (int)topLeft.Y);
        image.Mutate(ctx => ctx.DrawImage(barcodeImage, point, 1f));
    }

    private void DrawFigures(Image<Rgba32> image, PrintRequestMessageV1.Item item)
    {
        if (string.IsNullOrEmpty(item.Figures)) return;

        var font = SystemFonts.CreateFont("Arial", _profile.NumbersFontSize);

        // Позиция под штрих-кодом
        var barcodeHeight = GetBarcodeHeight();
        var yPosition = barcodeHeight + 2f; // 2px отступ

        var textOptions = new RichTextOptions(font)
        {
            Origin = new PointF(image.Width / 2, yPosition),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top
        };

        image.Mutate(ctx => ctx.DrawText(textOptions, item.Figures, Color.Black));
    }

    private Image? GenerateBarcode(string barcodeText)
    {
        try
        {
            var writer = _profile.UseDataMatrix ? CreateDataMatrixWriter() : CreateStandardBarcodeWriter();

            var barcodeBitmap = writer.Write(barcodeText);
            return barcodeBitmap;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка генерации штрих-кода: {ex.Message}");
            return null;
        }
    }

    private BarcodeWriter<Image> CreateStandardBarcodeWriter()
    {
        return new BarcodeWriter<Image>
        {
            Format = BarcodeFormat.CODE_128,
            Renderer = new ImageSharpRenderer<Rgba32>(),
            Options = new EncodingOptions
            {
                Height = (int)_profile.BarcodeFontSize,
                Width = 40, // TODO: СДЕЛАТЬ ВЫЧИСЛЯЕМЫМ
                Margin = 1,
                PureBarcode = true
            }
        };
    }

    private BarcodeWriter<Image> CreateDataMatrixWriter()
    {
        return new BarcodeWriter<Image>
        {
            Format = BarcodeFormat.DATA_MATRIX,
            Renderer = new ImageSharpRenderer<Rgba32>(),
            Options = new DatamatrixEncodingOptions
            {
                Height = (int)_profile.BarcodeFontSize,
                Width = (int)_profile.BarcodeFontSize,
                Margin = 1,
                SymbolShape = SymbolShapeHint.FORCE_SQUARE
            }
        };
    }

    private PointF CalculateTextPosition(string text, Font font, string alignment, float verticalOffset)
    {
        var marginPx = 0f; // 0px отступ

        return alignment.ToLower() switch
        {
            "Left" => new PointF(marginPx, verticalOffset),
            "Right" => new PointF(_profile.LabelWidth - marginPx, verticalOffset),
            "Center" => new PointF(_profile.LabelWidth / 2, marginPx),
            "Stretched" => new PointF(_profile.LabelWidth / 2, _profile.LabelHeight - 5f),
            _ => new PointF(_profile.LabelWidth / 2, verticalOffset)
        };
    }

    private PointF CalculateBarcodePosition(Image barcodeImage, PrintRequestMessageV1.Item item)
    {
        var center = new PointF(_profile.LabelWidth / 2, _profile.LabelHeight / 2);
        return center;
    }

    private HorizontalAlignment GetHorizontalAlignment(string alignment)
    {
        return alignment.ToLower() switch
        {
            "Left" => HorizontalAlignment.Left,
            "Right" => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Center
        };
    }

    private VerticalAlignment GetVerticalAlignment(string alignment)
    {
        return alignment.ToLower() switch
        {
            "Top" => VerticalAlignment.Top,
            "Bottom" => VerticalAlignment.Bottom,
            _ => VerticalAlignment.Center
        };
    }

    private float GetBarcodeHeight()
    {
        return 15f;
    }
}
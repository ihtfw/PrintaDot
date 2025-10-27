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

namespace PrintaDot.Shared.ImageGeneration.V1;

public class BarcodeImageGeneratorV1 : BarcodeImageGenerator
{
    private List<PrintRequestMessageV1.Item> _items;
    private ImageProfileV1 _profile;

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

    private (float Width, float Height) CalculateTextSize(string text, float fontSize)
    {
        var font = SystemFonts.CreateFont("Arial", fontSize);
        var textSize = TextMeasurer.MeasureBounds(text, new TextOptions(font));

        return (textSize.Width, textSize.Height);
    }

    private void DrawLabel(Image<Rgba32> image, PrintRequestMessageV1.Item item)
    {
        var headerTextSize = CalculateTextSize(item.Header, _profile.TextFontSize);
        var figuresTextSize = CalculateTextSize(item.Figures, _profile.NumbersFontSize);

        var heightCenter = _profile.LabelHeight / 2;

        var barcodeImage = GenerateBarcode(item.Barcode);

        var barcodeHalfAndHeaderHeight = headerTextSize.Height + _profile.BarcodeFontSize / 2.0f;
        var barcodeHalfAndFiguresHeight = figuresTextSize.Height + _profile.BarcodeFontSize / 2.0f;
        
        var allElementsHeight = figuresTextSize.Height + headerTextSize.Height + _profile.BarcodeFontSize;

        PointF headerCenter = new(), figuresCenter = new(), barcodeCenter= new();

        if (allElementsHeight > _profile.LabelHeight)
        {
            //ЕСЛИ СУММА ВЫСОТ ВСЕХ ЭЛЕМЕНТОВ БОЛЬШЕ ЧЕМ ВЫСОТА ЛЕЙБЛА
        }
        else if (barcodeHalfAndHeaderHeight > heightCenter)
        {
            headerCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height / 2.0f);
            barcodeCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height + _profile.BarcodeFontSize / 2.0f);
            figuresCenter = new PointF(_profile.LabelWidth / 2.0f, headerTextSize.Height + _profile.BarcodeFontSize + figuresTextSize.Height / 2.0f);
        }
        else if (barcodeHalfAndFiguresHeight > heightCenter) 
        {
            headerCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight - figuresTextSize.Height - _profile.BarcodeFontSize- headerTextSize.Height / 2);
            barcodeCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight - figuresTextSize.Height - _profile.BarcodeFontSize / 2.0f);
            figuresCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight - figuresTextSize.Height / 2.0f);
        }
        else
        {
            headerCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f - _profile.BarcodeFontSize / 2.0f - headerTextSize.Height / 2.0f);
            barcodeCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f);
            figuresCenter = new PointF(_profile.LabelWidth / 2.0f, _profile.LabelHeight / 2.0f + _profile.BarcodeFontSize / 2.0f + figuresTextSize.Height / 2.0f);
        }

        var headerTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(headerCenter, headerTextSize.Width, _profile.TextFontSize);
        var barcodeTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(barcodeCenter, barcodeImage.Width, _profile.BarcodeFontSize);
        var figuresTopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(figuresCenter, figuresTextSize.Width, _profile.NumbersFontSize);

        headerTopLeft.X = GetWidthAligment(_profile.TextAlignment, _profile.LabelWidth, headerTextSize.Width, headerTopLeft.X);
        barcodeTopLeft.X = GetWidthAligment(_profile.TextAlignment, _profile.LabelWidth, _profile.BarcodeFontSizeWidth, barcodeTopLeft.X);
        figuresTopLeft.X = GetWidthAligment(_profile.TextAlignment, _profile.LabelWidth, figuresTextSize.Width, figuresTopLeft.X);

        DrawText(image, item.Header, headerTopLeft, _profile.TextFontSize);
        DrawBarcode(image, barcodeImage, barcodeTopLeft);
        DrawText(image, item.Figures, figuresTopLeft, _profile.NumbersFontSize);
    }

    private void DrawText(Image<Rgba32> image, string text, PointF topLeft, float fontSize)
    {
        var font = SystemFonts.CreateFont("Arial", fontSize);

        var textOptions = new RichTextOptions(font)
        {
            Origin = topLeft,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Center,
        };

        //// Обрезаем текст если нужно
        //var displayText = item.Header.Length > _profile.TextMaxLength
        //    ? item.Header.Substring(0, _profile.TextTrimLength)
        //    : item.Header;

        image.Mutate(ctx => ctx.DrawText(textOptions, text, Color.Black));
    }

    private void DrawBarcode(Image<Rgba32> image, Image barcodeImage, PointF topLeft)
    {
        if (barcodeImage == null) return;

        var point = new Point((int)topLeft.X, (int)topLeft.Y);
       
        image.Mutate(ctx => ctx.DrawImage(barcodeImage, point, 1f));
    }

    private Image? GenerateBarcode(string barcodeText)
    {
        try
        {
            var writer = _profile.UseDataMatrix ? CreateDataMatrixWriter() : CreateStandardBarcodeWriter();

            var barcodeRaw = writer.Write(barcodeText);

            using var tempStream = new MemoryStream();
            barcodeRaw.SaveAsPng(tempStream);
            tempStream.Position = 0;

            var barcodeTyped = Image.Load<Rgba32>(tempStream);

            return barcodeTyped;
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
                Width = (int)_profile.BarcodeFontSizeWidth,
                Margin = 0,
                NoPadding = true,
                PureBarcode = true,
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

    private float GetWidthAligment(string aligment, float labelWdith, float elementWidth, float alligmentValue)
    {
        switch (aligment)
        {
            case "Left":
                return 0.0f;
            case "Right":
                return labelWdith - elementWidth;
            case "Center":
                return alligmentValue;
            case "Stretched":
                throw new NotImplementedException();
            default:
                return 0.0f;
        }
    }
}
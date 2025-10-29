using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing.Common;
using ZXing.Datamatrix.Encoder;
using ZXing.Datamatrix;
using ZXing.ImageSharp.Rendering;
using ZXing;
using SixLabors.ImageSharp.Processing;
using PrintaDot.Shared.ImageGeneration.V1;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class BarcodeElement : IDrawElement
{
    public PointF TopLeft { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
    public Image BarcodeImage { get; set; }

    public BarcodeElement(PixelImageProfileV1 profile, string barcodeText)
    {
        BarcodeImage = GenerateBarcode(profile, barcodeText);
        CalculateTopLeft(profile);
    }

    public void Draw(Image<Rgba32> image)
    {
        var point = new Point((int)TopLeft.X, (int)TopLeft.Y);

        image.Mutate(ctx => ctx.DrawImage(BarcodeImage, point, 1f));
    }

    internal void CalculateTopLeft(PixelImageProfileV1 profile)
    {
        var center = new PointF(profile.LabelWidth / 2.0f, profile.LabelHeight / 2.0f);

        TopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(center, BarcodeImage.Width, BarcodeImage.Height);
    }

    private Image GenerateBarcode(PixelImageProfileV1 profile, string barcodeText)
    {
        try
        {
            var writer = profile.UseDataMatrix ? CreateDataMatrixWriter(profile.BarcodeFontSize) : CreateStandardBarcodeWriter(profile.BarcodeFontSize, profile.BarcodeFontSizeWidth);

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

    private BarcodeWriter<Image> CreateStandardBarcodeWriter(float barcodeFontSize, float barcodeFontSizeWidth)
    {
        return new BarcodeWriter<Image>
        {
            Format = BarcodeFormat.CODE_128,
            Renderer = new ImageSharpRenderer<Rgba32>(),
            Options = new EncodingOptions
            {
                Height = (int)barcodeFontSize,
                Width = (int)barcodeFontSizeWidth,
                Margin = 0,
                NoPadding = true,
                PureBarcode = true,
            }
        };
    }

    private BarcodeWriter<Image> CreateDataMatrixWriter(float barcodeFontSize)
    {
        return new BarcodeWriter<Image>
        {
            Format = BarcodeFormat.DATA_MATRIX,
            Renderer = new ImageSharpRenderer<Rgba32>(),
            Options = new DatamatrixEncodingOptions
            {
                Height = (int)barcodeFontSize,
                Width = (int)barcodeFontSize,
                Margin = 1,
                SymbolShape = SymbolShapeHint.FORCE_SQUARE
            }
        };
    }
}

// 1 - Сгенерировати штрихкод по высоте и ширине
// 2 - Взять оттуда высоту и ширину и подставить в вычисления
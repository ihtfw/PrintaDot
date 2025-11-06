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

internal class BarcodeElement : Element, IDrawElement
{
    public Image BarcodeImage { get; set; }

    public BarcodeElement(PixelImageProfileV1 profile, string barcodeText)
    {
        Rotation = profile.BarcodeAngle;
        Offset = new PointF(profile.OffsetX, profile.OffsetY);

        BarcodeImage = GenerateBarcode(profile, barcodeText);
        CalculateTopLeft(profile);
    }

    public void Draw(Image<Rgba32> image)
    {
        var rotatedBarcode = BarcodeImage.Clone(ctx => ctx.Rotate(Rotation));

        var newPoint = CalculateTopLeftRotated(rotatedBarcode, BarcodeImage.Width, BarcodeImage.Height);

        image.Mutate(ctx => {
            ctx.DrawImage(rotatedBarcode, newPoint, 1f);
        });
    }

    internal void CalculateTopLeft(PixelImageProfileV1 profile)
    {
        Center = new PointF(profile.LabelWidth / 2.0f, profile.LabelHeight / 2.0f);

        TopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(Center, BarcodeImage.Width, BarcodeImage.Height);

        TopLeft = new PointF(GetHorizontalAligment(profile.BarcodeAlignment, profile.LabelWidth, BarcodeImage.Width, TopLeft.X), TopLeft.Y);
    }

    private Image GenerateBarcode(PixelImageProfileV1 profile, string barcodeText)
    {
        var writer = profile.UseDataMatrix ? CreateDataMatrixWriter(profile.BarcodeFontSize) : CreateStandardBarcodeWriter(profile.BarcodeFontSize, profile.BarcodeFontSizeWidth);

        var barcodeRaw = writer.Write(barcodeText);

        using var tempStream = new MemoryStream();
        barcodeRaw.SaveAsPng(tempStream);
        tempStream.Position = 0;

        var barcodeTyped = Image.Load<Rgba32>(tempStream);
#if DEBUG
        barcodeTyped.Mutate(ctx => ctx.DrawBbox(0, 0, barcodeTyped.Width, barcodeTyped.Height));
#endif
        return barcodeTyped;
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

using PrintaDot.Shared.ImageGeneration.V1;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


namespace PrintaDot.Shared.ImageGeneration.DrawElements;
internal class FiguresElement : TextElement
{
    public FiguresElement(PixelImageProfileV1 profile, string? text, PointF barcodeTopLeft, float barcodeHeight)
    {
        if (string.IsNullOrEmpty(text)) {
            return;
        }

        Text = text;
        Rotation = profile.NumbersAngle;
        Offset = new PointF(profile.OffsetX, profile.OffsetY);

        Font = SystemFonts.CreateFont(ImageGenerationHelper.DEFAULT_FONT, profile.NumbersFontSize);
        TextBbox = TextMeasurer.MeasureAdvance(text, new TextOptions(Font));

        ReadOnlySpan<GlyphBounds> textSize;
        TextMeasurer.TryMeasureCharacterBounds(text, new TextOptions(Font), out textSize);

        CalculateTopLeft(profile, barcodeTopLeft, barcodeHeight);
    }

    private void CalculateTopLeft(PixelImageProfileV1 profile, PointF barcodeTopLeft, float barcodeHeight)
    {
        var y = barcodeTopLeft.Y + barcodeHeight + TextBbox.Height / 2.0f;

        Center = new PointF(profile.LabelWidth / 2.0f, y);

        TopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(Center, TextBbox.Width, TextBbox.Height);
        TopLeft += new PointF(0, ImageGenerationHelper.MARGIM_FROM_BARCODE);
        TopLeft = new PointF(GetHorizontalAligment(profile.NumbersAlignment, profile.LabelWidth, TextBbox.Width, TopLeft.X), TopLeft.Y);
    }

    public override void Draw(Image<Rgba32> image)
    {
        if (!string.IsNullOrEmpty(Text))
        {
            base.Draw(image);
        }
    }
}

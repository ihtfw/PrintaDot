using PrintaDot.Shared.ImageGeneration.V1;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class HeaderElement : TextElement
{
    public HeaderElement(PixelImageProfileV1 profile, string text, PointF barcodeTopLeft)
    {
        Text = text;

        Font = SystemFonts.CreateFont(ImageGenerationHelper.DEFAULT_FONT, profile.TextFontSize);
        TextBbox = TextMeasurer.MeasureAdvance(text, new TextOptions(Font));

        ReadOnlySpan<GlyphBounds> textSize;
        TextMeasurer.TryMeasureCharacterBounds(text, new TextOptions(Font), out textSize);

        CalculateTopLeft(profile, barcodeTopLeft);
    }

    private void CalculateTopLeft(PixelImageProfileV1 profile, PointF barcodeTopLeft)
    {
        var y = barcodeTopLeft.Y - TextBbox.Height / 2.0f;

        if (y < 0)
        {
            y = 0;
        }

        var center = new PointF(profile.LabelWidth / 2.0f, y);

        TopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(center, TextBbox.Width, TextBbox.Height);

        TopLeft -= new PointF(0, ImageGenerationHelper.MARGIM_FROM_BARCODE);
    }
}

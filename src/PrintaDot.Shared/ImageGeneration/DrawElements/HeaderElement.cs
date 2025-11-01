using PrintaDot.Shared.ImageGeneration.V1;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using System.Text;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class HeaderElement : TextElement
{
    public HeaderElement(PixelImageProfileV1 profile, string text, PointF barcodeTopLeft)
    {
        Text = profile.TextTrimLength > 0 && text.Length > profile.TextTrimLength
            ? text.Substring(0, profile.TextTrimLength)
            : text;

        Text = InsertLineBreaks(Text, profile.TextMaxLength);

        Rotation = profile.TextAngle;
        Offset = new PointF(profile.OffsetX, profile.OffsetY);

        Font = SystemFonts.CreateFont(ImageGenerationHelper.DEFAULT_FONT, profile.TextFontSize);
        TextBbox = TextMeasurer.MeasureAdvance(Text, new TextOptions(Font));

        CalculateTopLeft(profile, barcodeTopLeft);
    }

    private void CalculateTopLeft(PixelImageProfileV1 profile, PointF barcodeTopLeft)
    {
        var y = barcodeTopLeft.Y - TextBbox.Height / 2.0f;

        Center = new PointF(profile.LabelWidth / 2.0f, y);

        TopLeft = ImageGenerationHelper.CalculateTopLeftFromCenter(Center, TextBbox.Width, TextBbox.Height);
        TopLeft -= new PointF(0, ImageGenerationHelper.MARGIM_FROM_BARCODE);

        TopLeft = new PointF(GetHorizontalAligment(profile.TextAlignment, profile.LabelWidth, TextBbox.Width, TopLeft.X), TopLeft.Y);
    }

    private string InsertLineBreaks(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || maxLength <= 0)
            return text;

        var result = new StringBuilder();
        int currentIndex = 0;
        int textLength = text.Length;

        while (currentIndex < textLength)
        {
            int segmentLength = Math.Min(maxLength, textLength - currentIndex);

            if (currentIndex > 0)
            {
                while (currentIndex < textLength && char.IsWhiteSpace(text[currentIndex]))
                {
                    currentIndex++;
                }
            }

            if (currentIndex >= textLength)
                break;

            segmentLength = Math.Min(maxLength, textLength - currentIndex);

            result.Append(text, currentIndex, segmentLength);
            currentIndex += segmentLength;

            if (currentIndex < textLength)
            {
                result.Append('\n');
            }
        }

        return result.ToString();
    }
}

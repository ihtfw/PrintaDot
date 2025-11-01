using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class TextElement : Element, IDrawElement
{
    public string? Text { get; set; }
    public FontRectangle TextBbox { get; set; }
    public Font Font { get; set; }

    public void Draw(Image<Rgba32> image)
    {
        var textImage = new Image<Rgba32>((int)TextBbox.Width, (int)TextBbox.Height);

        textImage.Mutate(ctx =>
        {
            ctx.Clear(Color.Transparent);

            var textOptions = new RichTextOptions(Font)
            {
                Origin = new PointF(0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center,
            };

            var brush = Brushes.Solid(Color.Black);
            ctx.DrawText(textOptions, Text, brush, null);

#if DEBUG
            ctx.DrawBbox(0f, 0f, TextBbox.Width, TextBbox.Height);
#endif
        });

        var rotatedText = textImage.Clone(ctx => ctx.Rotate(Rotation));

        var newPoint = CalculateTopLeftRotated(rotatedText, TextBbox.Width, TextBbox.Height);

        image.Mutate(ctx => {
            ctx.DrawImage(rotatedText, newPoint, 1f);

        });

        textImage.Dispose();
        rotatedText.Dispose();
    }
}

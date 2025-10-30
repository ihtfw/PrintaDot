using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using PrintaDot.Shared.Common;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

internal class TextElement : Element, IDrawElement
{
    public PointF TopLeft { get; set; }
    public string? Text { get; set; }
    public FontRectangle TextBbox { get; set; }
    public Font Font { get; set; }

    public void Draw(Image<Rgba32> image)
    {
        var textOptions = new RichTextOptions(Font)
        {
            Origin = TopLeft,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Center,
        };

        //// Обрезаем текст если нужно
        //var displayText = item.Header.Length > _profile.TextMaxLength
        //    ? item.Header.Substring(0, _profile.TextTrimLength)
        //    : item.Header;

        image.Mutate(ctx => {
            ctx.DrawText(textOptions, Text, Color.Black);
#if DEBUG
            ctx.DrawBbox(TopLeft.X, TopLeft.Y, TextBbox.Width, TextBbox.Height);
#endif
        });
    }
}

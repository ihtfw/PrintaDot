using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace PrintaDot.Shared.ImageGeneration.DrawElements;

public interface IDrawElement
{
    public void Draw(Image<Rgba32> image);
}

using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration;

public interface IPrintaDotImageGenerator
{
    public List<Image> GenerateImage();
}

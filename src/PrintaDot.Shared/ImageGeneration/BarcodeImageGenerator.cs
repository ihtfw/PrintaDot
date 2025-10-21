using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration;

public interface BarcodeImageGenerator
{
    public List<Image> GenerateBarcodeImage();
}

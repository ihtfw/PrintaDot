using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration;

public static class ImageGenerationHelper
{
    public static float MmToDpi(double mm, float dpi = 300f) => (float)(mm * dpi) / 25.4f;

    //public static float MmToDpi(double mm) => 96.0f * (float)mm / 25.4f;

    public static float FontSizeToPixels(double fontSize, float dpi = 300f) => (float)(fontSize * dpi) / 72.0f;

    public static PointF CalculateCenterFromTopLeft(PointF topLeft, float width, float height) => new (topLeft.X + width / 2.0f, topLeft.Y + height / 2.0f);

    public static PointF CalculateTopLeftFromCenter(PointF center, float width, float height) => new (center.X - width / 2.0f, center.Y - height / 2.0f);
}

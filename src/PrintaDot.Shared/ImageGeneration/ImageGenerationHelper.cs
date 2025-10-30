using SixLabors.ImageSharp;

namespace PrintaDot.Shared.ImageGeneration;

public static class ImageGenerationHelper
{
    public const string DEFAULT_FONT = "Arial";
    public const float DEFAULT_DPI = 300.0f;
    public const float MM_PER_INCH = 25.4f;
    public const float POINTS_PER_INCH = 72.0f;
    public const float MARGIM_FROM_BARCODE = 5.0f;

    public static int FromPixelsToHundredthsInch(float pixels, float dpi = DEFAULT_DPI) => (int)Math.Round(pixels / dpi * 100);
    public static float FromMmToPixels(float mm, float dpi = DEFAULT_DPI) => mm * dpi / MM_PER_INCH;
    public static float FromPixelsToMm(float pixels, float dpi = DEFAULT_DPI) => pixels * MM_PER_INCH / dpi;
    public static float FontSizeToPixels(float fontSize, float dpi = DEFAULT_DPI) => fontSize * dpi / POINTS_PER_INCH;
    public static float FontSizeToPixels(double fontSize, float dpi = DEFAULT_DPI) => (float)(fontSize * dpi / POINTS_PER_INCH);
    public static PointF CalculateCenterFromTopLeft(PointF topLeft, float width, float height)
        => new PointF(topLeft.X + width / 2.0f, topLeft.Y + height / 2.0f);
    public static PointF CalculateTopLeftFromCenter(PointF center, float width, float height)
        => new PointF(center.X - width / 2.0f, center.Y - height / 2.0f);
}
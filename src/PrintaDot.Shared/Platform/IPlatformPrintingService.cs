using SixLabors.ImageSharp;

namespace PrintaDot.Shared.Platform;

interface IPlatformPrintingService
{
    void Print(Image iamge);
}

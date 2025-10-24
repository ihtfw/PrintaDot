using SixLabors.ImageSharp;

namespace PrintaDot.Shared.Platform;

public interface IPlatformPrintingService
{
    void Print(string printerName, Image image);
}

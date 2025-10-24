using PrintaDot.Shared.NativeMessaging;
using PrintaDot.Shared.Platform;

namespace PrintaDot.Tests.V1;

internal class TestsRunnerV1
{
    private MessageV1Creator _creator = new();

    internal void PrintA4(string printerName)
    {
        var message = _creator.CreatePrintRequestMessageV1A4();
        message.Profile.PrinterName = printerName;

        var host = new Host()
        {
            PlatformPrintingService = ResolvePlatformPrintingService()
        };

        host.ProcessMessageV1(message);
    }

    private static IPlatformPrintingService ResolvePlatformPrintingService()
    {
#if BUILD_FOR_WINDOWS
        return new PrintaDot.Windows.WindowsPrintingService();
#else
        throw new InvalidOperationException("Only Windows currently is supported");
#endif
    }

    internal void PrintThermo(string printerName)
    {
        var message = _creator.CreatePrintRequestMessageV1Thermo();
        message.Profile.PrinterName = printerName;

        var host = new Host()
        {
            PlatformPrintingService = ResolvePlatformPrintingService()
        };

        host.ProcessMessageV1(message);
    }
}

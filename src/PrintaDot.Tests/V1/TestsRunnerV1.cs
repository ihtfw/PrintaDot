using PrintaDot.Shared.NativeMessaging;

namespace PrintaDot.Tests.V1;

internal class TestsRunnerV1
{
    private MessageV1Creator _creator = new();

    internal void PrintA4()
    {
        var message = _creator.CreatePrintRequestMessageV1A4();

        var host = new Host();

        host.ProcessMessageV1(message);
    }

    internal void PrintThermo()
    {
        var message = _creator.CreatePrintRequestMessageV1Thermo();

        var host = new Host();

        host.ProcessMessageV1(message);
    }
}

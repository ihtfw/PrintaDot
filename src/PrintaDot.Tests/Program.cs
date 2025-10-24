using PrintaDot.Tests.V1;

internal class Program
{
    private static void Main(string[] args)
    {
        var printerName = "Microsoft Print to PDF";

        var testsRunner = new TestsRunnerV1();
        testsRunner.PrintThermo(printerName);
        //testsRunnerV1.PrintA4();
    }
}
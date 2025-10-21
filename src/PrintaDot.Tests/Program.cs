using PrintaDot.Tests.V1;

internal class Program
{
    static TestsRunnerV1 testsRunnerV1 = new();
    private static void Main(string[] args)
    {
        testsRunnerV1.PrintThermo();
        //testsRunnerV1.PrintA4();
    }
}
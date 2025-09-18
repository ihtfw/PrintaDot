namespace PrintaDot.Shared.Common;

/// <summary>
/// Controls the logging behavior of the application
/// </summary>
public static class Log
{
    /// <summary>
    /// The path to the generated log file.
    /// </summary>
    public static string MessageLogLocation => Utils.MessageLogLocation;

    /// <summary>
    /// Activate the logging if set to <see langword="true"/>
    /// </summary>
    public static bool Active { get; set; } = false;

    internal static void LogMessage(string msg, string? nameof = null)
    {
        if (!Active)
        {
            return;
        }

        try
        {
            if (nameof is not null)
            {
                var messageLine = $"[{nameof}]: {msg} {Environment.NewLine}";

                File.AppendAllText(MessageLogLocation, messageLine);
            }
            else
            {
                var messageLine = $"[NATIVEHOST]: {msg} {Environment.NewLine}";

                File.AppendAllText(MessageLogLocation, messageLine);
            }
            
        }
        catch (IOException)
        {
            Console.WriteLine("Could not log to file");
            //Supress Exception
        }
    }
}

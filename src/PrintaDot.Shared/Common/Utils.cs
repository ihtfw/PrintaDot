using System.Reflection;

namespace PrintaDot.Shared.Common;

public static class Utils
{
    /// <summary>
    /// Variable for supported message protocol version.
    /// All versions lower than SupportedMessageVersion autumaticly supported.
    /// Higher versions than SupportedMessageVersion not supported and can be deserialize to lower version.
    /// </summary>
    public const int SupportedMessageVersion = 1;

    /// <summary>
    /// Logs from application.
    /// </summary>
    public static string MessageLogLocation => Path.Combine(AssemblyLoadDirectory(), "native-messaging.log");

    /// <summary>
    /// Target application path.
    /// </summary>
    public static string TargetApplicationDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PrintaDot");

    public static string AssemblyLoadDirectory()
    {
        string baseDirectory = AppContext.BaseDirectory;

        if (baseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
            baseDirectory.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        {
            baseDirectory = baseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        return baseDirectory;
    }
    public static string AssemblyExecuteablePath()
    {
        string? processPath = Environment.ProcessPath;

        if (!string.IsNullOrEmpty(processPath))
        {
            return processPath;
        }

        string? assemblyPath = Assembly.GetEntryAssembly()?.Location;
        if (!string.IsNullOrEmpty(assemblyPath))
        {
            return assemblyPath;
        }

        throw new InvalidOperationException("Invalid executable path.");
    }

    /// <summary>
    /// Moving application to %LocalAppData%/PrintaDot/ directory.
    /// </summary>
    /// <returns></returns>
    public static bool MoveApplicationToLocalAppData()
    {
        var currentDirectory = AssemblyLoadDirectory();

        if (string.Equals(currentDirectory, TargetApplicationDirectory, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Native application is already in right folder");
            return true;
        }

        try
        {
            if (Directory.Exists(TargetApplicationDirectory))
            {
                RemoveDirectory(TargetApplicationDirectory);
            }

            Directory.CreateDirectory(TargetApplicationDirectory);

            CopyAllFiles(currentDirectory, TargetApplicationDirectory);

            Console.WriteLine($"Application moved to: {TargetApplicationDirectory}");

            Environment.Exit(0);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mooving application: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Recursivly copying files to target folder.
    /// </summary>
    /// <param name="sourceDir">Directory from copy files.</param>
    /// <param name="targetDir">Directory to copy files.</param>
    private static void CopyAllFiles(string sourceDir, string targetDir)
    {
        try
        {
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(targetDir, fileName);

                File.Copy(file, destFile, true);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                var dirName = Path.GetFileName(directory);
                var destDirPath = Path.Combine(targetDir, dirName);

                if (!Directory.Exists(destDirPath))
                {
                    Directory.CreateDirectory(destDirPath);
                }

                CopyAllFiles(directory, destDirPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving files");
        }
    }

    /// <summary>
    /// Used for removing directory with files.
    /// </summary>
    private static void RemoveDirectory(string directoryPath)
    {
        try
        {
            Directory.Delete(directoryPath, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Directory cannot be removed: {ex.Message}");
        }
    }
}

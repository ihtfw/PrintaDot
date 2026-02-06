using PrintaDot.Shared.NativeMessaging;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

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

    public static string GetExecutableFileName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "PrintaDot.exe";
        }

        return "PrintaDot";
    }

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

    public static bool IsLocalAppDataDirectory(string directory) =>
        string.Equals(directory, TargetApplicationDirectory, StringComparison.OrdinalIgnoreCase);

    public static void CreatePrintaDotFolderInLocalAppData()
    {
        var currentDirectory = AssemblyLoadDirectory();

        if (Directory.Exists(TargetApplicationDirectory))
        {
            return;
        }

        Directory.CreateDirectory(TargetApplicationDirectory);
    }

    /// <summary>
    /// Moving application to %LocalAppData%/PrintaDot/ directory.
    /// </summary>
    /// <returns></returns>
    public static bool MoveApplicationToLocalAppData()
    {
        var currentDirectory = AssemblyLoadDirectory();

        if (IsLocalAppDataDirectory(currentDirectory))
        {
            Log.LogMessage("Application is alredy in local app data");
            return true;
        }

        try
        {

#if DEBUG
            var isFilesMoved =  CopyAllFiles(currentDirectory, TargetApplicationDirectory);
#else
            var isFilesMoved = CopyReleaseFiles(currentDirectory, TargetApplicationDirectory);
#endif
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            bool isRussian = culture.TwoLetterISOLanguageName == "ru" ||
                             culture.ThreeLetterISOLanguageName == "rus";

            if (isRussian)
            {
                Console.WriteLine("Все файлы скопированы. Приложение готово к работе. Закройте это окно.");
            }
            else
            {
                Console.WriteLine("All files are copied. Application ready to work. Close this window.");
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Copy only publish files (exe and pdb files) in Release mode
    /// </summary>
    /// <param name="sourceDir">Directory from copy files.</param>
    /// <param name="targetDir">Directory to copy files.</param>
    private static bool CopyReleaseFiles(string sourceDir, string targetDir)
    {
        var currentDirectory = AssemblyLoadDirectory();

        if (IsLocalAppDataDirectory(currentDirectory))
        {
            return true;
        }

        try
        {
            string exeName = Process.GetCurrentProcess().MainModule.FileName;

            var sourceFile = Path.Combine(sourceDir, exeName);
            var destFile = Path.Combine(targetDir, GetExecutableFileName());

            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, destFile, true);
            }
            else
            {
                throw new FileNotFoundException(GetExecutableFileName());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error copying publish files: {ex.Message}");

            return false;
        }

        return true;
    }

    /// <summary>
    /// Recursivly copying files to target folder.
    /// </summary>
    /// <param name="sourceDir">Directory from copy files.</param>
    /// <param name="targetDir">Directory to copy files.</param>
    private static bool CopyAllFiles(string sourceDir, string targetDir)
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

            return false;
        }

        return true;
    }
}
using System.Diagnostics;
using Common.BasicHelper.Utils.Extensions;

namespace Cheese.Utils.Cheese;

internal class PathHelper
{
    private static PathHelper? _instance;

    public static PathHelper Instance => _instance ??= new();

    private string? baseSlnDir;

    public string? BaseSlnDir => baseSlnDir;

    private bool FindSolutionUpward(string startLocation, string solutionFileName)
    {
        var currentDirInfo = new DirectoryInfo(startLocation);

        if (!currentDirInfo.Exists || currentDirInfo.Parent is null || CheckForFile(currentDirInfo, solutionFileName))
        {
            var path = Path.Combine(currentDirInfo.FullName, solutionFileName);
            if (File.Exists(path))
            {
                baseSlnDir = currentDirInfo.FullName;
                return true;
            }
            else return false;
        }

        return FindSolutionUpward(currentDirInfo.Parent.FullName, solutionFileName);
    }

    private static bool CheckForFile(DirectoryInfo directory, string fileNameIgnoreCase) => directory.GetFiles().Any(
        x => x.Name.Equals(fileNameIgnoreCase, StringComparison.OrdinalIgnoreCase)
    );

    public PathHelper()
    {
        var location = Environment.CurrentDirectory.GetFullPath();

        _ = FindSolutionUpward(location, "KitX.sln");
    }

    public PathHelper ReadFile(string relativePath, Action<FileInfo>? onReadIn = null, Action<Exception>? onError = null)
    {
        if (BaseSlnDir is null)
        {
            onError?.Invoke(new InvalidOperationException("We're not in KitX repo."));

            return this;
        }

        var path = Path.Combine(BaseSlnDir, relativePath);

        if (File.Exists(path) == false)
        {
            onError?.Invoke(new FileNotFoundException("File not found.", Path.GetFileName(path)));

            return this;
        }

        onReadIn?.Invoke(new FileInfo(path));

        return this;
    }

    public PathHelper WriteFile(string relativePath, string content, Action<Exception>? onError = null, Action? onSucceeded = null)
    {
        if (BaseSlnDir is null)
        {
            onError?.Invoke(new InvalidOperationException("We're not in KitX repo."));

            return this;
        }

        var path = Path.Combine(BaseSlnDir, relativePath);

        try
        {
            File.WriteAllText(path, content);

            onSucceeded?.Invoke();
        }
        catch (Exception e)
        {
            onError?.Invoke(e);
        }

        return this;
    }

    public PathHelper ExecuteCommand(string relativeBaseDir, string cmd, string args)
    {
        if (BaseSlnDir is null) return this;

        var old_location = Path.GetFullPath(".");

        var new_location = Path.Combine(BaseSlnDir, relativeBaseDir);

        Environment.CurrentDirectory = new_location;

        var StartInfo = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = true,
        };

        Console.WriteLine($"Executing: {cmd} {args}");

        var process = Process.Start(StartInfo);

        process?.WaitForExit();

        if (process?.ExitCode != 0)
        {
            Console.WriteLine($"Process exited with return value {process?.ExitCode}");
        }

        Environment.CurrentDirectory = old_location;

        return this;
    }
}

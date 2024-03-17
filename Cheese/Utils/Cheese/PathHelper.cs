using System.Diagnostics;
using Common.BasicHelper.Utils.Extensions;

namespace Cheese.Utils.Cheese;

public class PathHelper
{
    private static PathHelper? _instance;

    public static PathHelper Instance => _instance ??= new();

    private string? _baseSlnDir;

    public string? BaseSlnDir => _baseSlnDir;

    private bool FindSolutionUpward(string startLocation, string cheeseFolderName)
    {
        var currentDirInfo = new DirectoryInfo(startLocation);

        if (!currentDirInfo.Exists || currentDirInfo.Parent is null || CheckDirectory(currentDirInfo, cheeseFolderName))
        {
            var path = Path.Combine(currentDirInfo.FullName, cheeseFolderName);

            if (Directory.Exists(path))
            {
                _baseSlnDir = currentDirInfo.FullName;
                return true;
            }
            else return false;
        }

        return FindSolutionUpward(currentDirInfo.Parent.FullName, cheeseFolderName);
    }

    private static bool CheckDirectory(DirectoryInfo directory, string fileNameIgnoreCase) => directory
        .GetDirectories()
        .Any(
            x => x.Name.Equals(fileNameIgnoreCase, StringComparison.OrdinalIgnoreCase)
        );

    public PathHelper()
    {
        var location = Environment.CurrentDirectory.GetFullPath();

        _ = FindSolutionUpward(location, ".cheese");
    }

    public PathHelper ReadFile(string relativePath, Action<FileInfo>? onReadIn = null,
        Action<Exception>? onError = null)
    {
        if (BaseSlnDir is null)
        {
            onError?.Invoke(new InvalidOperationException("We're not in a cheese project."));

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

    public PathHelper WriteFile(string relativePath, string content, Action<Exception>? onError = null,
        Action? onSucceeded = null)
    {
        if (BaseSlnDir is null)
        {
            onError?.Invoke(new InvalidOperationException("We're not in a cheese project."));

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

        var oldLocation = Path.GetFullPath(".");

        var newLocation = Path.Combine(BaseSlnDir, relativeBaseDir);

        Environment.CurrentDirectory = newLocation;

        var startInfo = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = false,
            UseShellExecute = true,
            CreateNoWindow = true,
        };

        Console.WriteLine($"Executing: {cmd} {args}");

        var process = Process.Start(startInfo);

        process?.WaitForExit();

        if (process?.ExitCode != 0)
        {
            Console.WriteLine($"Process exited with return value {process?.ExitCode}");
        }

        Environment.CurrentDirectory = oldLocation;

        return this;
    }
}
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

    public static string? WorkBase => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName)?.GetFullPath();

    public PathHelper()
    {
        var location = Environment.CurrentDirectory.GetFullPath();

        _ = FindSolutionUpward(location, ".cheese");
    }
    
    public string? GetPath(string relativePath)
    {
        return BaseSlnDir is null ? null : Path.Combine(BaseSlnDir, relativePath);
    }
    
    public PathHelper GetPath(string relativePath, out string? finalPath)
    {
        if (BaseSlnDir is null)
        {
            finalPath = null;
            return this;
        }

        finalPath = Path.Combine(BaseSlnDir, relativePath);
        return this;
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

    public PathHelper ExecuteCommand(string relativeBaseDir, string cmd, string args, out string? stdOutput, out string? stdError, out int exitCode)
    {
        stdOutput = null;
        stdError = null;
        exitCode = -255;
        
        if (BaseSlnDir is null) return this;

        var oldLocation = Path.GetFullPath(".");

        var newLocation = Path.Combine(BaseSlnDir, relativeBaseDir);

        Environment.CurrentDirectory = newLocation;

        var startInfo = new ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        ConsoleHelper.Instance.AccentLine($"@ Executing: {cmd} {args}");

        var process = Process.Start(startInfo);

        stdOutput = process?.StandardOutput.ReadToEnd();
        
        stdError = process?.StandardError.ReadToEnd();

        process?.WaitForExit();

        exitCode = process?.ExitCode ?? -255;

        if (process?.ExitCode != 0)
        {
            ConsoleHelper.Instance.ErrorLine($"@ Process exited with return value {process?.ExitCode}");
        }

        Environment.CurrentDirectory = oldLocation;

        return this;
    }
}
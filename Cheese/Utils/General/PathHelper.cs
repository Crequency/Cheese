using System.Diagnostics;
using Common.BasicHelper.Utils.Extensions;
using Spectre.Console;

namespace Cheese.Utils.General;

public class PathHelper
{
    private static PathHelper? _instance;

    public static PathHelper Instance => _instance ??= new();

    public string? BaseSlnDir { get; private set; }

    private bool FindSolutionUpward(string startLocation, string cheeseFolderName)
    {
        const int deepestDepth = 1000;

        var currentDepth = 0;

        while (currentDepth <= deepestDepth)
        {
            currentDepth += 1;

            var currentDirInfo = new DirectoryInfo(startLocation);

            if (currentDirInfo is { Exists: true, Parent: not null } && !CheckDirectory(currentDirInfo, cheeseFolderName))
            {
                startLocation = currentDirInfo.Parent.FullName;
                continue;
            }

            var path = Path.Combine(currentDirInfo.FullName, cheeseFolderName);

            if (!Directory.Exists(path)) return false;

            BaseSlnDir = currentDirInfo.FullName;
            return true;
        }

        throw new IOException($"Meet max depth limit ({deepestDepth}) when looking upward for `{cheeseFolderName}` folder.");
    }

    private static bool CheckDirectory(DirectoryInfo directory, string fileNameIgnoreCase) => directory
        .GetDirectories()
        .Any(
            x => x.Name.Equals(fileNameIgnoreCase, StringComparison.OrdinalIgnoreCase)
        );

    public Dictionary<string, FileInfo> GetSubFiles(DirectoryInfo dir)
    {
        var result = new Dictionary<string, FileInfo>();

        SearchFiles(dir);

        return result;

        void SearchFiles(DirectoryInfo dirInfo)
        {
            foreach (var file in dirInfo.GetFiles())
                result.Add(file.FullName, file);

            foreach (var subDir in dirInfo.GetDirectories())
                SearchFiles(subDir);
        }
    }

    public Tree GetSubFilesTree(DirectoryInfo dir, string label)
    {
        var tree = new Tree(label);

        SearchFiles(dir);

        return tree;

        void SearchFiles(DirectoryInfo dirInfo, TreeNode? parent = null)
        {
            var node = parent?.AddNode($"[yellow]\ud83d\udcc2 {dirInfo.Name}[/]") ?? tree.AddNode($"[yellow]\ud83d\udcc2 {dirInfo.Name}[/]");

            foreach (var file in dirInfo.GetFiles())
                node.AddNode($"[white]\ud83d\udcc4 {file.Name}[/]");

            foreach (var subDir in dirInfo.GetDirectories())
                SearchFiles(subDir, node);
        }
    }

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

        onReadIn?.Invoke(new(path));

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

    public PathHelper ExecuteCommand(string relativeBaseDir, string cmd, string args, out string? stdOutput, out string? stdError, out int exitCode, bool showText = true)
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

        if (showText) ConsoleHelper.Instance.AccentLine($"@ Executing: {cmd} {args}");

        var process = Process.Start(startInfo);

        stdOutput = process?.StandardOutput.ReadToEnd();

        stdError = process?.StandardError.ReadToEnd();

        process?.WaitForExit();

        exitCode = process?.ExitCode ?? -255;

        if (process?.ExitCode != 0)
        {
            if (showText) ConsoleHelper.Instance.ErrorLine($"@ Process exited with return value {process?.ExitCode}");
        }

        Environment.CurrentDirectory = oldLocation;

        return this;
    }

    public PathHelper AssertInSlnDirectory(out bool assert)
    {
        assert = BaseSlnDir is not null;

        if (BaseSlnDir is null)
            throw new InvalidOperationException("We're not in a cheese project (make sure you have `.cheese` folder).");

        return this;
    }
}
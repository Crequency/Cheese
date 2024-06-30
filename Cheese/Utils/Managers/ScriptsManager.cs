using System.Text.RegularExpressions;
using Cheese.Options;
using Cheese.Utils.General;
using Common.BasicHelper.Utils.Extensions;
using Spectre.Console;

namespace Cheese.Utils.Managers;

public class ScriptsManager
{
    private static ScriptsManager? _instance;

    public static ScriptsManager Instance => _instance ??= new();

    private const string ScriptsFolder = ".cheese/scripts";

    public ScriptsManager List(ScriptsOptions options)
    {
        var fullPath = PathHelper.Instance.AssertInSlnDirectory(out _).GetPath(ScriptsFolder);

        var filesTree = PathHelper.Instance.GetSubFilesTree(new(fullPath!), "\ud83d\udcc2 .cheese");

        AnsiConsole.Write(filesTree);

        return this;
    }

    public ScriptsManager Execute(ScriptsOptions options)
    {
        var fullPath = PathHelper.Instance.AssertInSlnDirectory(out _).GetPath(ScriptsFolder);

        var files = PathHelper.Instance.GetSubFiles(new(fullPath!));

        var mostMatchedFiles = new List<FileInfo>();
        var visitedMaxLcsLen = 0;

        foreach (var file in files.Values)
        {
            var lcs = GetLcs(file.Name, options.Execute ?? "");

            if (lcs < visitedMaxLcsLen || lcs == 0) continue;

            if (lcs == visitedMaxLcsLen)
            {
                mostMatchedFiles.Add(file);
                continue;
            }

            visitedMaxLcsLen = lcs;
            mostMatchedFiles.Clear();
            mostMatchedFiles.Add(file);
        }

        var finalFileToExecute = string.Empty;

        switch (mostMatchedFiles.Count)
        {
            case > 1:
            {
                var tree = new Tree("The most matched files are:");
                foreach (var file in mostMatchedFiles)
                    tree.AddNode($"[white]\ud83d\udcc4 {file.Name}[/]");
                AnsiConsole.Write(tree);

                var userChoice = AnsiConsole.Ask("Which one you want to execute (type exact file name) ?", mostMatchedFiles.First().Name);
                finalFileToExecute = mostMatchedFiles.FirstOrDefault(file => file.Name.Equals(userChoice))?.FullName;

                break;
            }
            case 1:
                AnsiConsole.MarkupLine($"The most matched file is [green]{mostMatchedFiles.First().Name}[/].");
                finalFileToExecute = mostMatchedFiles.First().FullName;
                break;
            default:
                ConsoleHelper.Instance.ErrorLine("We found no matched files.");

                if (!options.FailFast) return this;

                Environment.ExitCode = 30;
                throw new IOException("No matched files found");
        }

        if (finalFileToExecute.IsNullOrWhiteSpace())
        {
            ConsoleHelper.Instance.ErrorLine("No this file");
            return this;
        }

        if (options.DryRun) return this;

        var script = File.ReadAllText(finalFileToExecute!);

        ConsoleHelper.Instance.AccentLine("Executing ...");

        var failed = false;

        var task = ScriptHost.Instance.ExecuteCodesAsync(script, onError: _ => failed = true, includeTimestamp: false);

        task.Wait();

        if (failed && options.FailFast)
            ConsoleHelper.Instance.ErrorLine(task.Result!.ToString()!);

        return this;

        int GetLcs(string a, string b)
        {
            if (a.IsNullOrWhiteSpace() || b.IsNullOrWhiteSpace())
                return 0;

            a = Regex.Escape(a);
            b = Regex.Escape(b);

            var w = Math.Max(a.Length, b.Length);
            var h = Math.Min(a.Length, b.Length);

            var sa = a.Length >= b.Length ? a : b;
            var sb = a.Length >= b.Length ? b : a;

            var mat = new int[w + 1, w + 1];

            for (var i = 0; i <= h; ++i)
            for (var j = 0; j <= w; ++j)
                mat[i, j] = 0;

            for (var i = 1; i <= h; ++i)
            for (var j = 1; j <= w; ++j)
            {
                var isSame = sa[j - 1] == sb[i - 1];
                mat[i, j] = isSame ? mat[i - 1, j - 1] + 1 : Math.Max(mat[i, j - 1], mat[i - 1, j]);
            }

            var max = 0;
            for (var j = 1; j <= w; ++j)
                max = Math.Max(max, mat[h, j]);

            return max;
        }
    }
}
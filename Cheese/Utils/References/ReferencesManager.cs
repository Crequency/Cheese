using System.Text;
using System.Text.RegularExpressions;
using Cheese.Options;
using Cheese.Shared.References;
using Cheese.Utils.Cheese;

namespace Cheese.Utils.References;

public partial class ReferencesManager
{
    private static ReferencesManager? _instance;

    public static ReferencesManager Instance => _instance ??= new();

    private List<ReferenceItem>? _references;

    private const string ProviderPath = ".cheese/references.cs";

    private ReferencesManager()
    {
        PathHelper.Instance.ReadFile(
            ProviderPath,
            file =>
            {
                var script = File.ReadAllText(file.FullName);
                
                // ToDo: Execute provider script
            },
            error =>
            {
                Console.WriteLine(
                    new StringBuilder()
                        .AppendLine($"Error occured: {error.Message}")
                        .AppendLine(error.StackTrace)
                        .ToString()
                );
            }
        );
    }

    public ReferencesManager SetupAll(ReferenceOptions options)
    {
        if (_references is null) return this;

        foreach (var item in _references)
        {
            ConsoleHelper.Instance.AccentLine($"@ {item.Name}");

            switch (item.Type)
            {
                case ReferenceType.Unknown:
                    break;
                case ReferenceType.GitRepo:
                    var targetUrl = item.Url;

                    if (targetUrl is null) continue;

                    if (options.ConvertSslLinkToHttpsLink && SslGitLinkRegex().IsMatch(targetUrl))
                    {
                        targetUrl = SslGitLinkRegex().Replace(targetUrl, "https://${hostname}/${username}/$1.git");
                    }

                    var argsClone = $"clone {targetUrl} \"{item.Location}\"";
                    var argsCheckout = $"checkout {item.Branch}";

                    var dir = PathHelper.Instance.GetPath(item.Location ?? "");

                    if (dir is null) continue;

                    if (options.DryRun)
                    {
                        ConsoleHelper.Instance.DebugLine($"# git {argsClone}");
                    }
                    else
                    {
                        PathHelper.Instance.ExecuteCommand("", "git", argsClone, out var stdOutput, out var stdError, out var exitCode);
                        ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(stdOutput ?? string.Empty).GoBack();

                        if (exitCode != 0)
                        {
                            ConsoleHelper.Instance.ErrorLine(stdError ?? string.Empty);

                            continue;
                        }
                    }

                    if (options.DryRun)
                    {
                        ConsoleHelper.Instance.DebugLine($"# git {argsCheckout}");
                    }
                    else
                    {
                        PathHelper.Instance.ExecuteCommand(dir, "git", argsCheckout, out var cStdOutput, out var cStdError, out var cExitCode);
                        ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(cStdOutput ?? string.Empty).GoBack();

                        if (cExitCode != 0) ConsoleHelper.Instance.ErrorLine(cStdError ?? string.Empty);
                    }

                    break;
                case ReferenceType.Binary:
                    break;
            }
        }

        return this;
    }

    public ReferencesManager UpdateAll()
    {
        if (_references is null) return this;

        foreach (var item in _references)
        {
            ConsoleHelper.Instance.AccentLine($"@ {item.Name}");

            switch (item.Type)
            {
                case ReferenceType.Unknown:
                    break;
                case ReferenceType.GitRepo:
                    var dir = PathHelper.Instance.GetPath(item.Location ?? "");

                    if (dir is null) continue;

                    PathHelper.Instance.ExecuteCommand(dir, "git", "pull", out var stdOutput, out var stdError, out var exitCode);

                    ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(stdOutput ?? string.Empty).GoBack();

                    if (exitCode != 0) ConsoleHelper.Instance.ErrorLine(stdError ?? string.Empty);
                    break;
                case ReferenceType.Binary:
                    break;
            }
        }

        return this;
    }

    public ReferencesManager Status(ReferenceOptions options)
    {
        if (_references is null) return this;

        var finalResult = new StringBuilder();

        foreach (var item in _references)
        {
            ConsoleHelper.Instance.AccentLine($"@ {item.Name}");

            switch (item.Type)
            {
                case ReferenceType.Unknown:
                    break;
                case ReferenceType.GitRepo:
                    if (item.Branch is null)
                    {
                        ConsoleHelper.Instance.ErrorLine($"@ {nameof(item.Branch)} property of {item.Name} is null");

                        continue;
                    }

                    var dir = PathHelper.Instance.GetPath(item.Location ?? "");

                    if (dir is null) continue;

                    if (options.Fetch)
                    {
                        PathHelper.Instance.ExecuteCommand(dir, "git", "fetch", out _, out var fStdError, out var fExitCode, showText: options.Verbose);

                        if (fExitCode != 0)
                            ConsoleHelper.Instance
                                .ErrorLine($"@ Failed to fetch remote repo with exit code {fExitCode}")
                                .WriteLine("")
                                .ErrorLine(fStdError ?? string.Empty);
                    }

                    GitHelper.Instance.GetLatestCommitHash(dir, item.Branch, out var latestCommitHash, showText: options.Verbose);
                    GitHelper.Instance.GetLatestCommitHash(dir, item.RemoteBranch, out var remoteCommitHash, showText: options.Verbose);

                    var remoteText = remoteCommitHash ?? "none";

                    if (remoteText.Equals(latestCommitHash)) remoteText = item.RemoteBranch;

                    finalResult.AppendLine($"{latestCommitHash} {item.Name} ({remoteText})");

                    break;
                case ReferenceType.Binary:
                    break;
            }
        }

        ConsoleHelper.Instance.WriteLine("").WriteLine(finalResult.ToString());

        return this;
    }

    [GeneratedRegex(@"^git@(?<hostname>[\w\.]+):(?<username>[\w-]+)/([\w-\.]+).git$")]
    private static partial Regex SslGitLinkRegex();
}
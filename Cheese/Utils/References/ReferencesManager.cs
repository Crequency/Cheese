using System.ComponentModel.Composition.Hosting;
using System.Text;
using System.Text.Json;
using Cheese.Contract.References;
using Cheese.Options;
using Cheese.Shared.References;
using Cheese.Utils.Cheese;

namespace Cheese.Utils.References;

public class ReferencesManager
{
    private static ReferencesManager? _instance;

    public static ReferencesManager Instance => _instance ??= new();

    private List<ReferenceItem>? _references;

    private const string ConfigPath = ".cheese/references.json";

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private ReferencesManager()
    {
        PathHelper.Instance.ReadFile(
            ConfigPath,
            file =>
            {
                var content = File.ReadAllText(file.FullName);

                _references = JsonSerializer.Deserialize<List<ReferenceItem>>(content) ??
                              throw new FormatException("Bad format for `.cheese/references.json`.");
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

    public ReferencesManager GenerateWithFlavor(ReferenceOptions options)
    {
        var path = PathHelper.WorkBase;

        ArgumentNullException.ThrowIfNull(path, nameof(path));

        if (options.Verbose)
            ConsoleHelper.Instance
                .DebugLine($"# Going to load plugins from `*.dll` with {nameof(IReferencesProvider)}")
                .DebugLine($"# Plugins located in {path}")
                .DebugLine("")
                ;

        var catalog = new DirectoryCatalog(path, "*.dll");

        var container = new CompositionContainer(catalog);

        var sub = container.GetExportedValues<IReferencesProvider>().ToList();

        var target = sub.FirstOrDefault(
            x => x.GetProviderIdentity().ToLower().Equals(options.Flavor?.ToLower())
        );

        ArgumentNullException.ThrowIfNull(target, nameof(target));

        if (options.DryRun)
        {
            ConsoleHelper.Instance
                .DebugLine($"# We found {sub.Count} plugins in {path} with `*.dll` pattern")
                .DebugLine($"# Going to generate below content at {PathHelper.Instance.GetPath(ConfigPath)}")
                .WriteLine("")
                .DryRunLine(JsonSerializer.Serialize(target.GetReferences().ToList(), SerializerOptions))
                ;

            return this;
        }

        _references = target.GetReferences().ToList();

        SaveAll();

        return this;
    }

    public ReferencesManager SetupAll()
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
                    var argsClone = $"clone {item.Url} \"{item.Location}\"";
                    var argsCheckout = $"checkout {item.Branch}";
                    
                    var dir = PathHelper.Instance.GetPath(item.Location ?? "");
                    
                    if (dir is null) continue;

                    PathHelper.Instance.ExecuteCommand("", "git", argsClone, out var stdOutput, out var stdError, out var exitCode);
                    ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(stdOutput ?? string.Empty).GoBack();
                    
                    if (exitCode != 0)
                    {
                        ConsoleHelper.Instance.ErrorLine(stdError ?? string.Empty);
                        
                        continue;
                    }
                    
                    PathHelper.Instance.ExecuteCommand(dir, "git", argsCheckout, out var cStdOutput, out var cStdError, out var cExitCode);
                    ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(cStdOutput ?? string.Empty).GoBack();
                    
                    if (cExitCode != 0) ConsoleHelper.Instance.ErrorLine(cStdError ?? string.Empty);
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

    public ReferencesManager SaveAll()
    {
        PathHelper.Instance.WriteFile(
            ConfigPath,
            JsonSerializer.Serialize(_references, SerializerOptions),
            error =>
            {
                Console.WriteLine(
                    new StringBuilder()
                        .AppendLine($"Error occured: {error.Message}")
                        .AppendLine(error.StackTrace)
                        .ToString()
                );
            },
            () => Console.WriteLine($"References saved at `{ConfigPath}`")
        );

        return this;
    }
}
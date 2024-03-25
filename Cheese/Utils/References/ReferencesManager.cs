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
                .DebugLine($"# Plugins located in {path}");

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
                    var args = $"clone {item.Url} \"{item.Location}\"";
                    PathHelper.Instance.ExecuteCommand("", "git", args, out var stdOutput, out var stdError, out var exitCode);
                    ConsoleHelper.Instance.SetForeground(ConsoleColor.DarkGray).WriteLine(stdOutput ?? string.Empty).GoBack();
                    if (exitCode != 0) ConsoleHelper.Instance.ErrorLine(stdError ?? string.Empty);
                    break;
                case ReferenceType.Binary:
                    break;
            }
        }

        return this;
    }

    public ReferencesManager Status(string pattern = "*")
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
                    break;
                case ReferenceType.Binary:
                    break;
            }
        }
        
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
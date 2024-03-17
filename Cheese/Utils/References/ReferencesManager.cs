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

    public ReferencesManager GenerateWithFlavor(SetupOptions options)
    {
        var path = PathHelper.WorkBase;

        ArgumentNullException.ThrowIfNull(path, nameof(path));

        if (options.Verbose)
            Console.WriteLine($"# Going to load plugins from `*.dll` with {nameof(IReferencesProvider)} implemented.");

        var catalog = new DirectoryCatalog(path, "*.dll");

        var container = new CompositionContainer(catalog);

        var sub = container.GetExportedValues<IReferencesProvider>().ToList();

        var target = sub.FirstOrDefault(
            x => x.GetProviderIdentity().ToLower().Equals(options.ReferenceFlavor?.ToLower())
        );

        ArgumentNullException.ThrowIfNull(target, nameof(target));

        if (options.DryRun)
        {
            Console.WriteLine(
                new StringBuilder()
                    .AppendLine($"# We found {sub.Count} plugins in {path} with `*.dll` pattern")
                    .AppendLine($"# Going to generate below content at {PathHelper.Instance.GetPath(ConfigPath)}")
                    .AppendLine()
                    .AppendLine(JsonSerializer.Serialize(target.GetReferences().ToList(), SerializerOptions))
                    .ToString()
            );

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
            switch (item.Type)
            {
                case ReferenceType.Unknown:
                    break;
                case ReferenceType.GitRepo:
                    var args = $"clone {item.Url} \"{item.Location}\"";
                    PathHelper.Instance.ExecuteCommand("", "git", args);
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
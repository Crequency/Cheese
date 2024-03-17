using System.Text;
using System.Text.Json;
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

                _references = JsonSerializer.Deserialize<List<ReferenceItem>>(content) ?? throw new FormatException("Bad format for `.cheese/references.json`.");
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

    public ReferencesManager GenerateDefault()
    {
        _references =
        [
            new ReferenceItem
            {
                Name = "Common.Activity",
                Location = "Reference/Common.Activity",
                Url = "git@github.com:Crequency/Common.Activity.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
            },
            new ReferenceItem
            {
                Name = "Common.Algorithm",
                Location = "Reference/Common.Algorithm",
                Url = "git@github.com:Crequency/Common.Algorithm.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
            },
            new ReferenceItem
            {
                Name = "Common.BasicHelper",
                Location = "Reference/Common.BasicHelper",
                Url = "git@github.com:Crequency/Common.BasicHelper.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
            },
            new ReferenceItem
            {
                Name = "Common.Update",
                Location = "Reference/Common.Update",
                Url = "git@github.com:Crequency/Common.Update.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
            },
            new ReferenceItem
            {
                Name = "Csharpell",
                Location = "Reference/Csharpell",
                Url = "git@github.com:Dynesshely/Csharpell.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
            },
            new ReferenceItem
            {
                Name = "XamlMultiLanguageEditor",
                Location = "KitX SDK/Reference/XamlMultiLanguageEditor",
                Url = "git@github.com:Dynesshely/XamlMultiLanguageEditor.git",
                Branch = "dev=main",
                Type = ReferenceType.GitRepo,
                InSubmodule = true,
            },
        ];

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

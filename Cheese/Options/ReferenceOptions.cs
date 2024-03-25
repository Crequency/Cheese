using Cheese.Utils.References;
using CommandLine;
using CommandLine.Text;

namespace Cheese.Options;

[Verb("reference", aliases: ["refer", "ref"], HelpText = "References management.")]
public class ReferenceOptions : Options
{
    [Option('s', "setup", Group = "act", HelpText = "Setup references.")]
    public bool Setup { get; set; }

    [Option('g', "gen", Group = "act", HelpText = "Generate references with flavor.")]
    public bool Generate { get; set; }

    [Option("flavor", Default = null, HelpText = "Select a reference flavor.")]
    public string? Flavor { get; set; }
}

public static class ReferenceOptionsExtensions
{
    public static ReferenceOptions Execute(this ReferenceOptions options)
    {
        if (options.Generate)
            ReferencesManager.Instance.GenerateWithFlavor(options);

        if (options.Setup)
            ReferencesManager.Instance.SetupAll();

        return options;
    }
}
using CommandLine;

namespace Cheese.Options;

[Verb("setup", HelpText = "Setup dev environment.")]
public class SetupOptions : Options
{
    [Option("reference", Default = false, HelpText = "Setup reference projects.")]
    public bool SetupReference { get; set; }

    [Option('f', "flavor", Default = null, Group = "gen", HelpText = "Select a reference flavor.")]
    public string? ReferenceFlavor { get; set; }

    [Option("gen", Default = false, Group = "gen", HelpText = "Generate references with flavor.")]
    public bool GenerateDefaultReferences { get; set; }
}

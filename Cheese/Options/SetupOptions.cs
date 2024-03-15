using CommandLine;

namespace Cheese.Options;

[Verb("setup", HelpText = "Setup KitX dev environment.")]
public class SetupOptions : Options
{
    [Option("reference", Default = false, HelpText = "Setup reference projects.")]
    public bool SetupReference { get; set; }

    [Option("generate-default-references", Default = false, HelpText = "Generate default references.")]
    public bool GenerateDefaultReferences { get; set; }
}

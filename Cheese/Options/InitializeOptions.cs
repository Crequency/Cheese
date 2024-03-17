using CommandLine;

namespace Cheese.Options;

[Verb("init", HelpText = "Initialize a repo env.")]
public class InitializeOptions : Options
{
    [Option('t', "initialize-target", Required = false, Default = null, HelpText = "Indicate initialize target.")]
    public string? InitializeTarget { get; set; }
}

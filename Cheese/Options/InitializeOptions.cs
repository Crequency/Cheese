using Cheese.Utils.Initializer;
using CommandLine;

namespace Cheese.Options;

[Verb("init", HelpText = "Initialize a repo env.")]
public class InitializeOptions : Options
{
    [Option('t', "initialize-target", Required = false, Default = null, HelpText = "Indicate initialize target.")]
    public string? InitializeTarget { get; set; }
}

public static class InitializeOptionsExtensions
{
    public static InitializeOptions Execute(this InitializeOptions options)
    {
        Initializer.Instance.Execute(options: options);
        
        return options;
    }
}

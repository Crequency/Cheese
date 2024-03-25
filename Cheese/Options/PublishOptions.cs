using CommandLine;

namespace Cheese.Options;

[Verb("publish", HelpText = "Run publisher and get the releases.")]
public class PublishOptions : Options
{
    [Option("skip-generate", Default = false, HelpText = "Skip generating process.")]
    public bool SkipGenerating { get; set; }

    [Option("skip-packing", Default = false, HelpText = "Skip packing process.")]
    public bool SkipPacking { get; set; }
}

public static class PublishOptionsExtensions
{
    public static PublishOptions Execute(this PublishOptions options)
    {
        Publisher.Instance.Execute(options);
        
        return options;
    }
}

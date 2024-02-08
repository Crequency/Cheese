using CommandLine;

namespace Cheese.Options;

public class Options
{
    [Option('v', "verbose", Required = false, HelpText = "More details will be printed out.")]
    public bool Verbose { get; set; }
}

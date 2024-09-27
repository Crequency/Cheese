using CommandLine;
using Cheese.Utils.Managers;

namespace Cheese.Options;

[Verb("setup", HelpText = "Setup dev environment.")]
public class SetupOptions : Options
{
    [Option("reference", Default = false, HelpText = "Setup reference projects.")]
    public bool SetupReference { get; set; }
}

public static class SetupOptionsExtensions
{
    public static SetupOptions Execute(this SetupOptions options)
    {
        if (options.SetupReference)
            ReferencesManager.Instance.SetupAll(new());

        return options;
    }
}
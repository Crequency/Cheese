using Cheese.Utils.Managers;
using CommandLine;

namespace Cheese.Options;

[Verb("scripts", aliases: ["script", "services"], HelpText = "Manage scripts.")]
public class ScriptsOptions : Options
{
    [Option('l', "list", Group = "act", HelpText = "List scripts.")]
    public bool List { get; set; }

    [Option('e', "execute", Group = "act", HelpText = "Execute a script")]
    public string? Execute { get; set; }

    [Option('a', "alone", HelpText = "Execute in an alone process")]
    public bool InAloneProcess { get; set; }
}

public static class ScriptsOptionsExtensions
{
    public static ScriptsOptions Execute(this ScriptsOptions options)
    {
        if (options.List)
            ScriptsManager.Instance.List(options);

        if (options.Execute is not null)
            ScriptsManager.Instance.Execute(options);

        return options;
    }
}
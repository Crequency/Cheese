using CommandLine;
using Cheese.Utils.Managers;

namespace Cheese.Options;

[Verb("reference", aliases: ["refer", "ref"], HelpText = "References management.")]
public class ReferenceOptions : Options
{
    [Option("status", Group = "act", HelpText = "Show references status.")]
    public bool Status { get; set; }
    
    [Option("fetch", HelpText = "Fetch repo from remote.")]
    public bool Fetch { get; set; }
    
    [Option('u', "update", Group = "act", HelpText = "Update references.")]
    public bool Update { get; set; }
    
    [Option('s', "setup", Group = "act", HelpText = "Setup references.")]
    public bool Setup { get; set; }
    
    [Option('h', "convert-ssl-link-to-https-link", HelpText = "Indicate will cheese convert ssl link to https link.")]
    public bool ConvertSslLinkToHttpsLink { get; set; }
}

public static class ReferenceOptionsExtensions
{
    public static ReferenceOptions Execute(this ReferenceOptions options)
    {
        if (options.Setup)
            ReferencesManager.Instance.SetupAll(options);

        if (options.Status)
            ReferencesManager.Instance.Status(options);

        if (options.Update)
            ReferencesManager.Instance.UpdateAll(options);

        return options;
    }
}
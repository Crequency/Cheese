using Cheese.Utils.References;
using CommandLine;

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
    
    [Option("convert-ssl-link-to-https-link", HelpText = "Indicate will cheese convert ssl link to https link.")]
    public bool ConvertSslLinkToHttpsLink { get; set; }

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
            ReferencesManager.Instance.SetupAll(options);

        if (options.Status)
            ReferencesManager.Instance.Status(options);

        if (options.Update)
            ReferencesManager.Instance.UpdateAll();

        return options;
    }
}
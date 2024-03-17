using Cheese.Utils.Cheese;
using CommandLine;

namespace Cheese.Options;

public class Options
{
    [Option('v', "verbose", HelpText = "More details will be printed out.")]
    public bool Verbose { get; set; }
    
    [Option('V', HelpText = "Display the version information.")]
    public bool ShowVersionText { get; set; }
    
    [Option('d', "dry-run", HelpText = "Display what the command will do instead of executing directly.")]
    public bool DryRun { get; set; }
}

public static class OptionsExtensions
{
    public static Options Execute(this Options options, string? versionText = null)
    {
        if (options.Verbose)
            Console.WriteLine(
                $"""
                 # sudo: {Environment.IsPrivilegedProcess}
                 # exe:  {Environment.ProcessPath}
                 # cmd:  {Environment.CommandLine}
                 # dir:  {Environment.CurrentDirectory}

                 Current cheese project directory: {PathHelper.Instance.BaseSlnDir}
                 """
            );

        if (options.ShowVersionText)
            Console.WriteLine(
                $"""
                 Cheese {versionText} {Environment.OSVersion}
                 A new generation of project scaffolding tool.

                 """
            );
        
        return options;
    }
}

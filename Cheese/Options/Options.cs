using Cheese.Utils.General;
using CommandLine;

namespace Cheese.Options;

public class Options
{
    [Option("verbose", HelpText = "More details will be printed out.")]
    public bool Verbose { get; set; }
    
    [Option("dry-run", HelpText = "Display what the command will do instead of executing directly.")]
    public bool DryRun { get; set; }
}

public static class OptionsExtensions
{
    public static Options Execute(this Options options)
    {
        if (options.Verbose)
            ConsoleHelper.Instance.DebugLine(
                $"""
                 # sudo: {Environment.IsPrivilegedProcess}
                 # exe:  {Environment.ProcessPath}
                 # cmd:  {Environment.CommandLine}
                 # dir:  {Environment.CurrentDirectory}

                 # Current cheese project directory: {PathHelper.Instance.BaseSlnDir}

                 """
            );
        
        return options;
    }
}

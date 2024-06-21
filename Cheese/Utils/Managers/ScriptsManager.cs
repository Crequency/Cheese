using Cheese.Options;
using Cheese.Utils.General;
using Spectre.Console;

namespace Cheese.Utils.Managers;

public class ScriptsManager
{
    private static ScriptsManager? _instance;

    public static ScriptsManager Instance => _instance ??= new();

    private const string ScriptsFolder = ".cheese/scripts";

    public ScriptsManager List(ScriptsOptions options)
    {
        var fullPath = PathHelper.Instance.AssertInSlnDirectory(out _).GetPath(ScriptsFolder);

        var filesTree = PathHelper.Instance.GetSubFilesTree(new(fullPath!), "+ .cheese");
        
        AnsiConsole.Write(filesTree);

        return this;
    }

    public ScriptsManager Execute(ScriptsOptions options)
    {
        // ToDo: Use LCS algorithm to find most related script and execute
        
        return this;
    }
}
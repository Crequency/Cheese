namespace Cheese.Utils.General;

public class GitHelper
{
    private static GitHelper? _instance;

    public static GitHelper Instance => _instance ??= new();

    public GitHelper GetLatestCommitHash(string location, string? branch, out string? result, bool showText = true)
    {
        if (branch is null)
        {
            result = null;

            return this;
        }
        
        var args = $"rev-parse {branch}";

        PathHelper.Instance.ExecuteCommand(location, "git", args, out var stdOut, out var stdError, out var exitCode, showText);

        if (exitCode != 0)
        {
            result = null;

            ConsoleHelper.Instance
                .ErrorLine($"@ Failed to fetch latest commit hash with exit code {exitCode}")
                .WriteLine("")
                .ErrorLine(stdError ?? string.Empty);
        }
        else result = stdOut?.Replace("\r", "").Replace("\n", "");
        
        return this;
    }
}
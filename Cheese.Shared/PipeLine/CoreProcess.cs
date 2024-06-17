namespace Cheese.Shared.PipeLine;

public class CoreProcess
{
    /// <summary>
    /// The root path of current project, if not in any cheese project, you get null
    /// </summary>
    public string? ProjectRootPath { get; set; }

    /// <summary>
    /// Where cheese startup
    /// For example:
    /// $ cd */xxx
    /// $ cheese ...
    /// Then, `StartupPath` will be "*/xxx"
    /// </summary>
    public string? StartupPath { get; set; }

    /// <summary>
    /// The absolute path for `.cheese` folder in current project
    /// If not in any cheese project, you get null
    /// </summary>
    public string? CheeseDataPath { get; set; }
    
    /// <summary>
    /// Basic information for current repo/project
    /// </summary>
    public RepoInfo? RepoInfo { get; set; }
}
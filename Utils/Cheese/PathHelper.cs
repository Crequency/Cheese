using Common.BasicHelper.Utils.Extensions;

namespace Cheese.Utils.Cheese;

internal class PathHelper
{
    private string? baseSlnDir;

    public string? BaseSlnDir => baseSlnDir;

    private bool FindSolutionUpward(string startLocation, string solutionFileName)
    {
        var currentDirInfo = new DirectoryInfo(startLocation);

        if (!currentDirInfo.Exists || currentDirInfo.Parent is null || CheckForFile(currentDirInfo, solutionFileName))
        {
            var path = Path.Combine(currentDirInfo.FullName, solutionFileName);
            if (File.Exists(path))
            {
                baseSlnDir = currentDirInfo.FullName;
                return true;
            }
            else return false;
        }

        return FindSolutionUpward(currentDirInfo.Parent.FullName, solutionFileName);
    }

    private static bool CheckForFile(DirectoryInfo directory, string fileNameIgnoreCase) => directory.GetFiles().Any(
        x => x.Name.Equals(fileNameIgnoreCase, StringComparison.OrdinalIgnoreCase)
    );

    public PathHelper()
    {
        var location = Environment.CurrentDirectory.GetFullPath();

        _ = FindSolutionUpward(location, "KitX.sln");
    }
}

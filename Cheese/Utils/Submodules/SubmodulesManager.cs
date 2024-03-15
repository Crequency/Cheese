namespace Cheese.Utils.Submodules;

public class SubmodulesManager
{
    private static SubmodulesManager? _instance;

    public static SubmodulesManager Instance => _instance ??= new();

    public SubmodulesManager()
    {

    }

    private void ScanAndBuildSubmodulesTree()
    {

    }

    public SubmodulesManager Update(string path)
    {


        return this;
    }
}

using System.Text;

namespace Cheese.Utils.Cheese;

public class DebugHelper
{
    private static DebugHelper? _instance;

    public static DebugHelper Instance => _instance ??= new();

    public DebugHelper RequestAnyKey(bool onlyInDebugMode = true)
    {
        var appendText = new StringBuilder()
#if DEBUG
                .Append("(debug mode) ")
#endif
                .ToString()
            ;

        if (onlyInDebugMode)
        {
#if DEBUG
            Action();
#endif
        }
        else Action();

        return this;

        void Action()
        {
            Console.WriteLine();
            Console.WriteLine($"Press any key to exit cheese {appendText}...");
            Console.ReadLine();
        }
    }
}
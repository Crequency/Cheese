using Cheese.Options;

namespace Cheese.Utils.Initializer;

public class Initializer
{
    private static Initializer? _instance;

    public static Initializer Instance => _instance ??= new();

    public Initializer Execute(InitializeOptions options)
    {
        
        
        return this;
    }
}
namespace Cheese.Utils.Cheese;

public class ConsoleHelper
{
    private static ConsoleHelper? _instance;

    public static ConsoleHelper Instance => _instance ??= new();

    private readonly Stack<ConsoleColor> _foregroundStack = [];

    public ConsoleHelper SetForeground(ConsoleColor newColor = ConsoleColor.White)
    {
        _foregroundStack.Push(Console.ForegroundColor);

        Console.ForegroundColor = newColor;

        return this;
    }

    public ConsoleHelper GoBack()
    {
        Console.ForegroundColor = _foregroundStack.Pop();

        return this;
    }

    public ConsoleHelper Write(string content)
    {
        Console.Write(content);

        return this;
    }

    public ConsoleHelper WriteLine(string content)
    {
        Console.WriteLine(content);

        return this;
    }

    public ConsoleHelper Debug(string content)
    {
        SetForeground(ConsoleColor.DarkGray)
            .Write(content)
            .GoBack();
        
        return this;
    }

    public ConsoleHelper DebugLine(string content)
    {
        SetForeground(ConsoleColor.DarkGray)
            .WriteLine(content)
            .GoBack();
        
        return this;
    }

    public ConsoleHelper Error(string content)
    {
        SetForeground(ConsoleColor.Red)
            .Write(content)
            .GoBack();
        
        return this;
    }

    public ConsoleHelper ErrorLine(string content)
    {
        SetForeground(ConsoleColor.Red)
            .WriteLine(content)
            .GoBack();
        
        return this;
    }

    public ConsoleHelper Accent(string content)
    {
        SetForeground(ConsoleColor.Cyan)
            .Write(content)
            .GoBack();
        
        return this;
    }
    
    public ConsoleHelper AccentLine(string content)
    {
        SetForeground(ConsoleColor.Cyan)
            .WriteLine(content)
            .GoBack();
        
        return this;
    }
    
    public ConsoleHelper DryRun(string content)
    {
        SetForeground(ConsoleColor.DarkBlue)
            .Write(content)
            .GoBack();
        
        return this;
    }
    
    public ConsoleHelper DryRunLine(string content)
    {
        SetForeground(ConsoleColor.DarkBlue)
            .WriteLine(content)
            .GoBack();
        
        return this;
    }
}
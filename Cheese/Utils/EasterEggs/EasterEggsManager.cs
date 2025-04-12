using Spectre.Console;

namespace Cheese.Utils.EasterEggs;

public class EasterEggsManager
{
    private static EasterEggsManager? _instance;

    public static EasterEggsManager Instance => _instance ??= new();

    public EasterEggsManager Enter()
    {
        var choices = new List<string>
        {
            "\ud83e\udd5a 1"
        };

        var targets = new List<IEasterEgg>
        {
            new DragonCurve(),
        };

        var egg = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What's your [green]favorite egg[/]?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more egg)[/]")
                .AddChoices(choices));

        AnsiConsole.MarkupLine($"I agree. [blue]{egg}[/] is tasty!");
        AnsiConsole.MarkupLine($"Prepare for your tour in [blue]{egg}[/]");

        Thread.Sleep(1000);

        var index = choices.IndexOf(egg);

        if (index >= targets.Count || index < 0)
        {
            AnsiConsole.MarkupLine($"Nothing here due to your selected index: [red]{index}[/]");
            return this;
        }

        targets[index].Run();

        return this;
    }
}
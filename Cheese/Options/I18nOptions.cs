using Cheese.Utils.I18n;
using CommandLine;

namespace Cheese.Options;

[Verb("i18n", HelpText = "i18n related command.")]
public class I18nOptions : Options
{
    [Option('t', "target", Required = true, HelpText = "Indicate i18n command's target field.")]
    public string? Target { get; set; }
}

public static class I18nOptionsExtensions
{
    public static I18nOptions Execute(this I18nOptions options)
    {
        I18nManager.Instance.Execute(options: options);
        
        return options;
    }
}

using CommandLine;

namespace Cheese.Options;

[Verb("i18n", HelpText = "i18n related command.")]
public class I18nOptions : Options
{
    [Option('t', "target", Required = true, HelpText = "Indicate i18n command's target field.")]
    public string? Target { get; set; }
}

using Cheese.Options;
using Common.BasicHelper.Core.Shell;
using Common.BasicHelper.Utils.Extensions;
using System.Text;

namespace Cheese.Utils.I18n;

internal class I18nManager
{
    internal I18nManager Execute(I18nOptions options)
    {
        if (options.Verbose)
            Console.WriteLine(
                $"""
                verbose: {options.Verbose}
                target:  {options.Target}
                """
            );

        var location = Instances.PathHelper!.BaseSlnDir;

        if (location is null)
        {
            Console.WriteLine("! You're not in KitX repo.");
            return this;
        }

        switch (options.Target?.ToLower())
        {
            case "dashboard":
                var relativePath = "%/KitX SDK/Reference/XamlMultiLanguageEditor";

                Instances.SubmodulesManager?.Update(relativePath);

                var exePath = relativePath.Replace("%", location).GetFullPath();
                var scriptPath = $"{exePath}/run.ps1".GetFullPath();

                var args = new StringBuilder()
                    .Append($" -WorkingDirectory \"{exePath}\"")
                    .Append($" -c \"& '{scriptPath}'\"")
                    .ToString()
                    ;

                if (options.Verbose)
                    Console.WriteLine($"# Passed args to pwsh: {args}");

                "pwsh".ExecuteAsCommand(args: args, findInPath: true);
                break;
            default:
                break;
        }

        return this;
    }
}

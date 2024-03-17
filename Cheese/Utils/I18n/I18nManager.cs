using System.Text;
using Cheese.Options;
using Cheese.Utils.Cheese;
using Cheese.Utils.Submodules;
using Common.BasicHelper.Core.Shell;
using Common.BasicHelper.Utils.Extensions;

namespace Cheese.Utils.I18n;

public class I18nManager
{
    private static I18nManager? _instance;

    public static I18nManager Instance => _instance ??= new();

    public I18nManager Execute(I18nOptions options)
    {
        if (options.Verbose)
            Console.WriteLine(
                $"""
                verbose: {options.Verbose}
                target:  {options.Target}
                """
            );

        var location = PathHelper.Instance.BaseSlnDir;

        if (location is null)
        {
            Console.WriteLine("! You're not in KitX repo.");
            return this;
        }

        switch (options.Target?.ToLower())
        {
            case "dashboard":
                var relativePath = "%/KitX SDK/Reference/XamlMultiLanguageEditor";

                SubmodulesManager.Instance.Update(relativePath);

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
        }

        return this;
    }
}

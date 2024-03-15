using System.Reflection;
using Cheese.Options;
using Cheese.Utils.Cheese;
using Cheese.Utils.I18n;
using Cheese.Utils.References;
using CommandLine;

var version = Assembly.GetExecutingAssembly().GetName().Version;

var versionText = $"v{version?.Major}.{version?.Minor}.{version?.MinorRevision} ({version?.Build})";

Console.WriteLine(
    $"""
    Cheese {versionText} {Environment.OSVersion}
    The new generation cli tool for KitX Project.

    """
);

Parser.Default.ParseArguments<Options, SetupOptions, PublishOptions, I18nOptions, object>(args)
    .WithParsed<Options>(options =>
    {
        if (options.Verbose)
            Console.WriteLine(
                $"""
                # sudo: {Environment.IsPrivilegedProcess}
                # exe:  {Environment.ProcessPath}
                # cmd:  {Environment.CommandLine}
                # dir:  {Environment.CurrentDirectory}

                Current KitX repo directory: {PathHelper.Instance.BaseSlnDir}
                """
            );

        if (PathHelper.Instance.BaseSlnDir is null)
            throw new InvalidOperationException("You must run Cheese in a KitX repo directory.");
    })
    .WithParsed<SetupOptions>(options =>
    {
        if (options.GenerateDefaultReferences)
            ReferencesManager.Instance.GenerateDefault();

        if (options.SetupReference)
            ReferencesManager.Instance.SetupAll();
    })
    .WithParsed<PublishOptions>(options => Publisher.Instance.Execute(options))
    .WithParsed<I18nOptions>(options => I18nManager.Instance.Execute(options))
    ;

#if DEBUG
// In debug mode, read the console output before closing the app
Console.WriteLine();
Console.WriteLine("Press any key to exit cheese ...");
Console.ReadLine();
#endif

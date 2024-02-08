using Cheese;
using Cheese.Options;
using CommandLine;

Console.WriteLine(
    $"""
    Cheese v0.1.0 {Environment.OSVersion}
    """
);

Instances.Init();

Parser.Default.ParseArguments<Options, PublishOptions, I18nOptions, object>(args)
    .WithParsed<Options>(options =>
    {
        if (options.Verbose)
            Console.WriteLine(
                $"""
                # sudo: {Environment.IsPrivilegedProcess}
                # exe:  {Environment.ProcessPath}
                # cmd:  {Environment.CommandLine}
                # dir:  {Environment.CurrentDirectory}
                """
            );
    })
    .WithParsed<PublishOptions>(options => new Publisher().Execute(options))
    .WithParsed<I18nOptions>(options => Instances.I18nManager?.Execute(options))
    ;

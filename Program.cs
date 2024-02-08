using Cheese.Options;
using CommandLine;

Console.WriteLine(
    $"""
    Cheese v0.1.0 {Environment.OSVersion}
    """
);

Parser.Default.ParseArguments<PublishOptions, object>(args)
    .WithParsed<PublishOptions>(options =>
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

        new Publisher().Execute(options);
    });

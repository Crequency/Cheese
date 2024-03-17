using System.Reflection;
using Cheese.Options;
using Cheese.Utils.Cheese;
using Cheese.Utils.I18n;
using Cheese.Utils.Initializer;
using Cheese.Utils.References;
using CommandLine;

var assembly = Assembly.GetExecutingAssembly();

var version = assembly.GetName().Version;

var versionText = $"v{version?.Major}.{version?.Minor}.{version?.Build} ({version?.MinorRevision})";

Parser.Default.ParseArguments<Options, InitializeOptions, SetupOptions, PublishOptions, I18nOptions, object>(args)
    .WithParsed<Options>(options => options.Execute(versionText: versionText))
    // Parse command "init"
    .WithParsed<InitializeOptions>(options => Initializer.Instance.Execute(options))
    // Parse command "setup"
    .WithParsed<SetupOptions>(options =>
    {
        if (options.GenerateDefaultReferences)
            ReferencesManager.Instance.GenerateWithFlavor(options);

        if (options.SetupReference)
            ReferencesManager.Instance.SetupAll();
    })
    // Parse command "publish"
    .WithParsed<PublishOptions>(options => Publisher.Instance.Execute(options))
    // Parse command "i18n"
    .WithParsed<I18nOptions>(options => I18nManager.Instance.Execute(options))
    ;

// In debug mode, read the console output before closing the app
DebugHelper.Instance.RequestAnyKey(onlyInDebugMode: true);

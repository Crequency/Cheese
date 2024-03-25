using System.Reflection;
using Cheese.Options;
using Cheese.Utils.Cheese;
using CommandLine;

var assembly = Assembly.GetExecutingAssembly();

var version = assembly.GetName().Version;

var versionText = $"v{version?.Major}.{version?.Minor}.{version?.Build} ({version?.MinorRevision})";

Parser.Default.ParseArguments<Options, InitializeOptions, SetupOptions, ReferenceOptions, PublishOptions, I18nOptions, object>(args)
    .WithParsed<Options>(options => options.Execute())
    // Parse command "init"
    .WithParsed<InitializeOptions>(options => options.Execute())
    // Parse command "setup"
    .WithParsed<SetupOptions>(options => options.Execute())
    // Parse command "reference"
    .WithParsed<ReferenceOptions>(options => options.Execute())
    // Parse command "publish"
    .WithParsed<PublishOptions>(options => options.Execute())
    // Parse command "i18n"
    .WithParsed<I18nOptions>(options => options.Execute())
    ;

// In debug mode, read the console output before closing the app
DebugHelper.Instance.RequestAnyKey(onlyInDebugMode: true);

using System.Reflection;
using Cheese.Options;
using Cheese.Utils.General;
using CommandLine;

var assembly = Assembly.GetExecutingAssembly();

var version = assembly.GetName().Version;

var versionText = $"v{version?.Major}.{version?.Minor}.{version?.Build} ({version?.MinorRevision})";

Parser.Default.ParseArguments<Options, SetupOptions, ScriptsOptions, ReferenceOptions, object>(args)
    .WithParsed<Options>(options => options.Execute())
    // Parse command "setup"
    .WithParsed<SetupOptions>(options => options.Execute())
    // Parse command "scripts"
    .WithParsed<ScriptsOptions>(options => options.Execute())
    // Parse command "reference"
    .WithParsed<ReferenceOptions>(options => options.Execute())
    ;

// In debug mode, read the console output before closing the app
DebugHelper.Instance.RequestAnyKey(onlyInDebugMode: true);
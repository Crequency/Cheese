using System.Diagnostics;
using System.Reflection;
using System.Text;
using Common.BasicHelper.Core.Shell;
using Csharpell.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Cheese.Utils.General;

public class ScriptHost
{
    private static ScriptHost? _instance;

    public static ScriptHost Instance => _instance ??= new();

    private CSharpScriptEngine Engine => new();

    public async Task<object?> ExecuteCodesAsync(
        string code,
        bool includeTimestamp = true,
        CancellationToken cancellationToken = default,
        Action<Exception>? onError = null,
        Func<ScriptOptions, ScriptOptions>? optionsProcessor = null
    )
    {
        var sw = new Stopwatch();

        var begin = DateTime.Now;

        sw.Start();

        try
        {
            var result = await Engine.ExecuteAsync(
                code,
                options =>
                {
                    options = options
                            // This line and below references importer are to make sure namespaces are all imported
                            .WithReferences(Assembly.GetExecutingAssembly()) // This lian and next one are about Cheese assembly
                            .WithReferences(Assembly.GetAssembly(typeof(ScriptHost)))
                            .WithImports(
                                "Cheese",
                                "Cheese.Utils",
                                "Cheese.Utils.General",
                                "Cheese.Utils.Managers",
                                "Cheese.Shared",
                                "Cheese.Shared.PipeLine",
                                "Cheese.Shared.References"
                            )
                            .WithLanguageVersion(LanguageVersion.Preview)
                        ;

                    options = optionsProcessor?.Invoke(options) ?? options;

                    return options;
                },
                addDefaultImports: true,
                runInReplMode: false,
                cancellationToken: cancellationToken
            );

            sw.Stop();

            return includeTimestamp
                    ? new StringBuilder()
                        .AppendLine($"[{begin:yyyy-MM-dd HH:mm:ss}] [I] Posted.")
                        .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [I] Ended, took {sw.ElapsedMilliseconds} ms.")
                        .AppendLine(result?.ToString())
                        .ToString()
                    : result
                ;
        }
        catch (Exception e)
        {
            sw.Stop();

            onError?.Invoke(e);

            return new StringBuilder()
                .AppendLine($"[{begin:yyyy-MM-dd HH:mm:ss}] [I] Posted.")
                .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [E] Exception caught after {sw.ElapsedMilliseconds} ms, Message: {e.Message}")
                .AppendLine(e.StackTrace)
                .ToString();
            ;
        }

        ;
    }
}
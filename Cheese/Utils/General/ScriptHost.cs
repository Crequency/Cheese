using System.Diagnostics;
using System.Reflection;
using System.Text;
using Csharpell.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;

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
        Action<Exception>? onError = null
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
                            .WithReferences(Assembly.GetExecutingAssembly())
                            // This line is to make sure namespaces are all imported
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
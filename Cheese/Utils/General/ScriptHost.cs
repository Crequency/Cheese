using System.Diagnostics;
using System.Reflection;
using System.Text;
using Cheese.Contract;
using Csharpell.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Cheese.Utils.General;

public class ScriptHost
{
    private static ScriptHost? _instance;

    public static ScriptHost Instance => _instance ??= new();

    private CSharpScriptEngine Engine => new();

    public async Task<string?> ExecuteCodesAsync(string code, bool includeTimestamp = true, CancellationToken cancellationToken = default)
    {
        var sw = new Stopwatch();

        var begin = DateTime.Now;

        sw.Start();

        try
        {
            var result = (await Engine.ExecuteAsync(
                code,
                options =>
                {
                    options = options
                            .WithReferences(Assembly.GetExecutingAssembly())
                            // This line and next line are to make sure namespaces all imported
                            .WithReferences(Assembly.GetAssembly(typeof(IProvider)))
                            .WithReferences(Assembly.GetAssembly(typeof(ScriptHost)))
                            .WithImports(
                                "Cheese",
                                "Cheese.Utils",
                                "Cheese.Utils.General",
                                "Cheese.Utils.References",
                                "Cheese.Contract",
                                "Cheese.Contract.Providers"
                            )
                            .WithLanguageVersion(LanguageVersion.Preview)
                        ;

                    return options;
                },
                addDefaultImports: true,
                runInReplMode: false,
                cancellationToken: cancellationToken
            ))?.ToString();

            sw.Stop();

            return includeTimestamp
                    ? new StringBuilder()
                        .AppendLine($"[{begin:yyyy-MM-dd HH:mm:ss}] [I] Posted.")
                        .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [I] Ended, took {sw.ElapsedMilliseconds} ms.")
                        .AppendLine(result)
                        .ToString()
                    : result
                ;
        }
        catch (Exception e)
        {
            sw.Stop();

            return new StringBuilder()
                    .AppendLine($"[{begin:yyyy-MM-dd HH:mm:ss}] [I] Posted.")
                    .AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [E] Exception caught after {sw.ElapsedMilliseconds} ms, Message: {e.Message}")
                    .AppendLine(e.StackTrace)
                    .ToString()
                ;
        }

        ;
    }
}
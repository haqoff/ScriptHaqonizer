using System.Text.Json;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Logging;

/// <summary>
/// Provides extension methods for logging actions.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Declares a scope for logging the operation of executing the specified script.
    /// </summary>
    public static IDisposable? BeginScriptExecutingScope(this ILogger logger, Script script, ExecutionMode executionMode)
    {
        var dict = GetScriptDictionary(script);
        dict["ScriptExecutionMode"] = executionMode;
        return logger.BeginScope(dict);
    }

    /// <summary>
    /// Declares a scope for migrations.
    /// </summary>
    public static IDisposable? BeginMigrationScope(this ILogger logger, string sourceName, AvailableEnvironment environment)
    {
        return logger.BeginScope(new ScopeDictionary()
        {
            {"Source", sourceName},
            {"Environment", environment}
        });
    }

    private static Dictionary<string, object> GetScriptDictionary(Script script)
    {
        var srcDict = JsonSerializer.SerializeToDocument(script).Deserialize<Dictionary<string, object>>()!;
        var newDict = new ScopeDictionary();
        foreach (var pair in srcDict)
        {
            var key = "Script" + pair.Key;
            newDict[key] = pair.Value;
        }

        return newDict;
    }

    private class ScopeDictionary : Dictionary<string, object>
    {
        private readonly Func<ScopeDictionary, string> _toStringFunc;

        public ScopeDictionary()
        {
            _toStringFunc = d => JsonSerializer.Serialize(d);
        }

        public ScopeDictionary(Func<ScopeDictionary, string> toStringFunc)
        {
            _toStringFunc = toStringFunc;
        }

        public override string ToString()
        {
            return _toStringFunc(this);
        }
    }
}
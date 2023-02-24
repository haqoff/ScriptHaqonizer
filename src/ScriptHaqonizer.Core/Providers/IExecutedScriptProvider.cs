using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Providers;

/// <summary>
/// Provides functionality for getting already executed scripts.
/// </summary>
public interface IExecutedScriptProvider
{
    /// <summary>
    /// Gets a list of all executed scripts.
    /// </summary>
    /// <exception cref="CannotFetchExecutedScriptsException">Exception that occurs when it was not possible to get the scripts executed earlier</exception>
    IReadOnlyList<ExecutedScript> GetExecutedScripts();
}
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Providers;

/// <summary>
/// Provides functionality for getting parsed scripts.
/// </summary>
public interface IParsedScriptProvider
{
    /// <summary>
    /// Gets all parsed scripts.
    /// </summary>
    /// <exception cref="ScriptListValidationException">The exception that is thrown if duplicates are found in the list.</exception>
    /// <exception cref="CannotGetScriptFilesException">The exception that is thrown if the list of scripts could not be retrieved.</exception>
    /// <exception cref="ScriptNameValidationException">The exception that is thrown if the filename does not match the pattern.</exception>
    /// <exception cref="ScriptLoadException">Exception that is thrown when the contents of the script failed to load.</exception>
    IReadOnlyCollection<Script> GetParsedScripts();
}
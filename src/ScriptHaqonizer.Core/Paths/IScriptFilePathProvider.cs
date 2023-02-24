using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.Core.Paths;

/// <summary>
/// Provides the functionality to get the paths of all scripts.
/// </summary>
public interface IScriptFilePathProvider
{
    /// <summary>
    /// Gets the paths of found scripts.
    /// </summary>
    /// <exception cref="CannotGetScriptFilesException">The exception that is thrown if the list of scripts could not be retrieved.</exception>
    IReadOnlyList<string> GetScriptPaths();
}
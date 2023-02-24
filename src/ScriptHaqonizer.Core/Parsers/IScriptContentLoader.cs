using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.Core.Parsers;

/// <summary>
/// Provides a method by which to load script content.
/// </summary>
public interface IScriptContentLoader
{
    /// <summary>
    /// Loads script content by given path.
    /// </summary>
    /// <exception cref="ScriptLoadException">Exception that is thrown when the contents of the script failed to load.</exception>
    string Load(string path);
}
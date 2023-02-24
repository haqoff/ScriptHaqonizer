using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.Core.Parsers;

/// <summary>
/// Represents a class that loads a script using the file system.
/// </summary>
public class ScriptContentLoader : IScriptContentLoader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptContentLoader"/> class.
    /// </summary>
    public ScriptContentLoader()
    {
    }

    /// <summary>
    /// Loads script content by given path.
    /// </summary>
    /// <exception cref="ScriptLoadException">Exception that is thrown when the contents of the script failed to load.</exception>
    public string Load(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            throw new ScriptLoadException(path, ex);
        }
    }
}
using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.Core.Paths;

/// <summary>
/// Represents a mechanism for obtaining the paths of all scripts.
/// </summary>
public class ScriptFilePathProvider : IScriptFilePathProvider
{
    private readonly string _scriptDirectoryPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptFilePathProvider"/> class with the specified migration script path.
    /// </summary>
    public ScriptFilePathProvider(string scriptDirectoryPath)
    {
        _scriptDirectoryPath = scriptDirectoryPath;
    }

    /// <summary>
    /// Gets the paths of found scripts.
    /// </summary>
    /// <exception cref="CannotGetScriptFilesException">The exception that is thrown if the list of scripts could not be retrieved.</exception>
    public IReadOnlyList<string> GetScriptPaths()
    {
        if (!Directory.Exists(_scriptDirectoryPath))
        {
            throw new CannotGetScriptFilesException($"The migration scripts folder at {_scriptDirectoryPath} was not found.");
        }

        try
        {
            var scriptPaths = Directory.GetFiles(_scriptDirectoryPath);
            return scriptPaths;
        }
        catch (Exception ex)
        {
            throw new CannotGetScriptFilesException("Failed to get scripts.", ex);
        }
    }
}
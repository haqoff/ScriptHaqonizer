namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents the exception that is thrown when the contents of the script failed to load.
/// </summary>
public class ScriptLoadException : MigrationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptLoadException"/> class with the specified script path and the internal error that caused it.
    /// </summary>
    /// <param name="path">Path to the script.</param>
    /// <param name="innerException">The error that caused it.</param>
    public ScriptLoadException(string path, Exception innerException) : base($"An error occurred while loading the script at path {path}.", innerException)
    {
    }
}
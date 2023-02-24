namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents the exception that occurs when the script files could not be retrieved.
/// </summary>
public class CannotGetScriptFilesException : MigrationBaseException
{
    /// <summary>
    /// Initialize new instance of <see cref="CannotGetScriptFilesException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public CannotGetScriptFilesException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initialize new instance of <see cref="CannotGetScriptFilesException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="innerException">Actual error.</param>
    public CannotGetScriptFilesException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
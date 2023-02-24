namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents an exception that occurs when it was not possible to get the scripts executed earlier.
/// </summary>
public class CannotFetchExecutedScriptsException : MigrationBaseException
{
    /// <summary>
    /// Initialize new instance of <see cref="CannotFetchExecutedScriptsException"/> class.
    /// </summary>
    /// <param name="innerException">Actual error.</param>
    public CannotFetchExecutedScriptsException(Exception innerException) : base("An error occurred while fetching previously executed scripts.", innerException)
    {
    }
}
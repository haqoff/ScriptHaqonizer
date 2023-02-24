namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents the exception that is thrown when a connection to the database fails.
/// </summary>
public class CannotConnectException : MigrationBaseException
{
    /// <summary>
    /// Initialize new instance of <see cref="CannotConnectException"/> class.
    /// </summary>
    /// <param name="innerException">Actual connection error.</param>
    public CannotConnectException(Exception innerException) : base("Failed to connect to database.", innerException)
    {
    }
}
namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents an exception that occurs when a database backup fails.
/// </summary>
public class DatabaseBackupException : MigrationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseBackupException"/> class with the specified message.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    public DatabaseBackupException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseBackupException"/> class with the specified message and the internal error that caused it.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="innerException">The error that caused it.</param>
    public DatabaseBackupException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
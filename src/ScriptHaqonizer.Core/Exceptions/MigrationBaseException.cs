namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents a base migration run error.
/// </summary>
public abstract class MigrationBaseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationBaseException"/> class with the specified message.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    protected MigrationBaseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationBaseException"/> class with the specified message and the internal error that caused it.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="innerException">The error that caused it.</param>
    protected MigrationBaseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
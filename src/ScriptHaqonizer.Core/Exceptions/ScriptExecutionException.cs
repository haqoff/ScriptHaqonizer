namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents an exception that occurs during script execution against the database.
/// </summary>
public class ScriptExecutionException : MigrationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptExecutionException"/> class.
    /// </summary>
    /// <param name="scriptId">Script id.</param>
    /// <param name="details">Details of error.</param>
    /// <param name="innerException">The error that caused it.</param>
    public ScriptExecutionException(int scriptId, string details, Exception innerException) : base($"An error occurred while executing script {scriptId}." + details, innerException)
    {
        ScriptId = scriptId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptExecutionException"/> class with the specified script id and the internal error that caused it.
    /// </summary>
    /// <param name="scriptId">Script id.</param>
    /// <param name="innerException">The error that caused it.</param>
    public ScriptExecutionException(int scriptId, Exception innerException) : base($"An error occurred while executing script {scriptId}.", innerException)
    {
        ScriptId = scriptId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptExecutionException"/> class with message.
    /// </summary>
    /// <param name="scriptId">Script id.</param>
    /// <param name="message">Message.</param>
    public ScriptExecutionException(int scriptId, string message) : base(message)
    {
        ScriptId = scriptId;
    }

    /// <summary>
    /// Script id.
    /// </summary>
    public int ScriptId { get; }
}
namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when the contents of a script file are invalid.
/// </summary>
public class ScriptContentValidationException : MigrationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptContentValidationException"/> class with the specified message and script id.
    /// </summary>
    /// <param name="scriptId">Script ID.</param>
    /// <param name="message">Message.</param>
    public ScriptContentValidationException(int scriptId, string message) : base(message)
    {
        ScriptId = scriptId;
    }

    /// <summary>
    /// Script ID.
    /// </summary>
    public int ScriptId { get; }
}
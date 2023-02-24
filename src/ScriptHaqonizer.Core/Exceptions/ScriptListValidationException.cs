namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a particular script is invalid relative to other scripts.
/// <br/>
/// For example, duplication of identifiers.
/// </summary>
public class ScriptListValidationException : MigrationBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptListValidationException"/> class with the specified message and script id.
    /// </summary>
    /// <param name="scriptId">Script ID.</param>
    /// <param name="message">Message.</param>
    public ScriptListValidationException(int scriptId, string message) : base(message)
    {
        ScriptId = scriptId;
    }

    /// <summary>
    /// Script ID.
    /// </summary>
    public int ScriptId { get; }
}
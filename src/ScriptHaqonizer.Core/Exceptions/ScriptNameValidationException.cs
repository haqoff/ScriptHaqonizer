namespace ScriptHaqonizer.Core.Exceptions;

/// <summary>
/// Represents a script name parsing error.
/// </summary>
public class ScriptNameValidationException : MigrationBaseException
{
    private const string TemplateRefer = "Refer to the documentation for the correct script names.";

    /// <summary>
    /// Initializes a new instance of the class <see cref="ScriptNameValidationException"/> with the specified filename and reason.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="reason">Error reason.</param>
    public ScriptNameValidationException(string fileName, string reason) : base($"The file name {fileName} does not match the pattern. {reason} {TemplateRefer}")
    {
    }
}
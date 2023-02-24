using ScriptHaqonizer.Core.Exceptions;

namespace ScriptHaqonizer.Core.Parsers;

/// <summary>
/// Provides a method for parsing script syntax.
/// </summary>
public interface IScriptContentParser
{
    /// <summary>
    /// Parses the script and also checks the contents of the script: only syntax, lexical errors and other assigned rules that can be checked without access to the actual database and its state.
    /// </summary>
    /// <exception cref="ScriptContentValidationException">Exception that is thrown when the contents of a script file are invalid.</exception>
    IParsedScriptContent ParseAndThrowIfNotValid(int scriptId, string content, bool allowScriptTransactions);
}
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Parsers;

/// <summary>
/// Represents a mechanism for parsing how a migration script is applied. 
/// </summary>
public interface IScriptParser
{
    /// <summary>
    /// Gets a script by parsing its execution options.
    /// </summary>
    /// <param name="path">Path to the script.</param>
    /// <returns>Script with parsed information.</returns>
    /// <exception cref="ScriptNameValidationException">The exception that is thrown if the filename does not match the pattern.</exception>
    /// <exception cref="ScriptLoadException">Exception that is thrown when the contents of the script failed to load.</exception>
    Script Parse(string path);
}
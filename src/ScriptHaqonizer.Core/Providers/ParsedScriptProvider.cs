using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.Core.Paths;

namespace ScriptHaqonizer.Core.Providers;

/// <summary>
/// Represents a mechanism for extracting and obtaining a list of scripts.
/// </summary>
public class ParsedScriptProvider : IParsedScriptProvider
{
    private readonly IScriptFilePathProvider _pathProvider;
    private readonly IScriptParser _scriptParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParsedScriptProvider"/> class.
    /// </summary>
    public ParsedScriptProvider(IScriptFilePathProvider pathProvider, IScriptParser scriptParser)
    {
        _pathProvider = pathProvider;
        _scriptParser = scriptParser;
    }

    /// <summary>
    /// Gets all parsed scripts.
    /// </summary>
    /// <exception cref="ScriptListValidationException">The exception that is thrown if duplicates are found in the list.</exception>
    /// <exception cref="CannotGetScriptFilesException">The exception that is thrown if the list of scripts could not be retrieved.</exception>
    /// <exception cref="ScriptNameValidationException">The exception that is thrown if the filename does not match the pattern.</exception>
    /// <exception cref="ScriptLoadException">The exception that is thrown when the contents of the script failed to load.</exception>
    public IReadOnlyCollection<Script> GetParsedScripts()
    {
        var scriptPaths = _pathProvider.GetScriptPaths();
        var parsedScripts = new HashSet<Script>();

        foreach (var parsedScript in scriptPaths.Select(_scriptParser.Parse))
        {
            if (parsedScripts.Contains(parsedScript))
            {
                throw new ScriptListValidationException(parsedScript.Id, $"Duplicate migration script found with id {parsedScript.Id}.");
            }

            parsedScripts.Add(parsedScript);
        }

        return parsedScripts;
    }
}
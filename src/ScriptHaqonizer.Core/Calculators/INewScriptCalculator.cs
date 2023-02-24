using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Calculators;

/// <summary>
/// Provides functionality for getting new, not executed scripts.
/// </summary>
public interface INewScriptCalculator
{
    /// <summary>
    /// Gets new scripts in sorted order to apply.
    /// </summary>
    /// <param name="alreadyExecutedScripts">Scripts that have already been applied.</param>
    /// <param name="parsedScripts">All scripts.</param>
    /// <exception cref="ScriptListValidationException">The exception that occurs when the IDs of new migration scripts are invalid.</exception>
    IReadOnlyList<Script> GetNotAppliedSortedScripts(IReadOnlyList<ExecutedScript> alreadyExecutedScripts, IEnumerable<Script> parsedScripts);
}
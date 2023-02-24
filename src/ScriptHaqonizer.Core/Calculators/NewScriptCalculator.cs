using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Calculators;

/// <summary>
/// Represents a mechanism for obtaining new, not executed scripts.
/// </summary>
public class NewScriptCalculator : INewScriptCalculator
{
    private readonly AvailableEnvironment _currentEnvironment;

    /// <summary>
    /// Initialize new instance of <see cref="NewScriptCalculator"/> class.
    /// </summary>
    public NewScriptCalculator(AvailableEnvironment currentEnvironment)
    {
        _currentEnvironment = currentEnvironment;
    }

    /// <summary>
    /// Gets new scripts in sorted order to apply.
    /// </summary>
    /// <param name="alreadyExecutedScripts">Scripts that have already been applied.</param>
    /// <param name="parsedScripts">All scripts.</param>
    /// <exception cref="ScriptListValidationException">The exception that occurs when the IDs of new migration scripts are invalid.</exception>
    public IReadOnlyList<Script> GetNotAppliedSortedScripts(IReadOnlyList<ExecutedScript> alreadyExecutedScripts, IEnumerable<Script> parsedScripts)
    {
        var newSortedParsedScripts = parsedScripts
            .Where(parsedScript => alreadyExecutedScripts.All(appliedScript => parsedScript.Id != appliedScript.Id))
            .Where(parsedScript => parsedScript.EnvironmentContainer.IsRelatedTo(_currentEnvironment))
            .OrderBy(s => s.Id)
            .ToArray();

        ThrowIfNewScriptIdsWrong(alreadyExecutedScripts, newSortedParsedScripts);
        return newSortedParsedScripts;
    }

    private static void ThrowIfNewScriptIdsWrong(IReadOnlyCollection<ExecutedScript> alreadyExecutedScripts, IEnumerable<Script> newSortedParsedScripts)
    {
        var maxExecutedScriptId = alreadyExecutedScripts.Count > 0 ? alreadyExecutedScripts.Max(s => s.Id) : 0;
        foreach (var newParsedScript in newSortedParsedScripts)
        {
            if (newParsedScript.Id < maxExecutedScriptId)
            {
                throw new ScriptListValidationException(newParsedScript.Id,
                    $"A new migration ID {newParsedScript.Id} was encountered that is less than the last applied migration {maxExecutedScriptId}. New migration scripts must have an ID greater than the previously applied ones.");
            }
        }
    }
}
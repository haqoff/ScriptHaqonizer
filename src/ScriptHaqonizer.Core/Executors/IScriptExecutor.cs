using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Executors;

/// <summary>
/// Provides functionality for checking and executing scripts.
/// </summary>
public interface IScriptExecutor
{
    /// <summary>
    /// Applies the script.
    /// </summary>
    /// <param name="script">Script.</param>
    /// <param name="mode">Launch mode.</param>
    /// <exception cref="MigrationBaseException">Exception thrown when a migration execution fails.</exception>
    public void Apply(Script script, ExecutionMode mode);
}
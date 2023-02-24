using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core;

/// <summary>
/// Provides a method for running migrations.
/// </summary>
public interface IMigrationFacade
{
    /// <summary>
    /// Attempts to migrate the database by executing the found scripts in the specified mode.
    /// <br/>
    /// If an error occurs, it is logged.
    /// </summary>
    /// <returns><c>True</c> if the migration applied without errors; <c>false</c> - otherwise.</returns>
    bool TryMigrate(ExecutionMode mode);

    /// <summary>
    /// Migrates the database, executing the found scripts in the specified mode.
    /// </summary>
    /// <exception cref="MigrationBaseException">The exception that occurs if the operation failed in the specified mode.</exception>
    void Migrate(ExecutionMode mode);
}
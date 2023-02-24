using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Backups;

/// <summary>
/// Provides functionality for backing up databases that will be modified.
/// </summary>
public interface IDatabaseBackupExecutor
{
    /// <summary>
    /// Performs a backup of databases that will be modified by the specified scripts.
    /// </summary>
    /// <exception cref="DatabaseBackupException">Exception that occurs when a database backup fails.</exception>
    /// <exception cref="CannotConnectException">Exception that is thrown when a connection to the database fails.</exception>
    /// <returns><c>True</c>, if backup was made; <c>false</c> - otherwise.</returns>
    bool TryExecute(IReadOnlyCollection<Script> scripts);
}
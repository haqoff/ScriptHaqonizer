using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Backups;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.MsSql.Database.Helpers;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql;

/// <summary>
/// Represents a mechanism for performing backups of MSSQL databases.
/// </summary>
public class MsSqlDatabaseBackupExecutor : IDatabaseBackupExecutor
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly string? _backupFolderPath;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="MsSqlDatabaseBackupExecutor"/>.
    /// </summary>
    public MsSqlDatabaseBackupExecutor(string connectionString, string databaseName, string? backupFolderPath, ILogger logger)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
        _backupFolderPath = backupFolderPath;
        _logger = logger;
    }

    /// <summary>
    /// Performs a backup of databases that will be modified by the specified scripts.
    /// </summary>
    /// <exception cref="DatabaseBackupException">Exception that occurs when a database backup fails.</exception>
    /// <exception cref="CannotConnectException">Exception that is thrown when a connection to the database fails.</exception>
    /// <returns><c>True</c>, if backup was made; <c>false</c> - otherwise.</returns>
    public bool TryExecute(IReadOnlyCollection<Script> scripts)
    {
        if (_backupFolderPath is null)
        {
            return false;
        }

        if (scripts.Count == 0)
        {
            return false;
        }

        var existingDatabaseNames = ExtractUniqueExistingDatabaseNames(scripts);
        if (existingDatabaseNames.Count == 0)
        {
            return false;
        }

        _logger.LogInformation("The following databases have been modified in scripts: {databaseNames}.", string.Join(", ", existingDatabaseNames));

        var currentBackupFolderPath = Path.Combine(_backupFolderPath, DateTime.Now.ToString("dd.MM.yy HH:mm"));
        Parallel.ForEach(existingDatabaseNames, new ParallelOptions {MaxDegreeOfParallelism = 2}, databaseName => BackupCore(databaseName, currentBackupFolderPath));
        return true;
    }

    private void BackupCore(string databaseName, string currentBackupFolderPath)
    {
        var backupPath = Path.Combine(currentBackupFolderPath, databaseName + ".bak");
        _logger.LogInformation("Starting backing up database {databaseName} to path {databaseBackupPath}.", databaseName, backupPath);
        using var connection = MsSqlDbHelper.GetOpenedConnection(_connectionString);

        try
        {
            var sw = Stopwatch.StartNew();
            var command = connection.CreateCommand();
            command.CommandText = $@"
BACKUP DATABASE [{databaseName}]
TO DISK = '{backupPath}'
WITH COPY_ONLY;
";
            command.ExecuteNonQuery();
            sw.Stop();
            _logger.LogInformation("Backup of database {databaseName} completed successfully in {duration} minutes.", databaseName, sw.Elapsed.TotalMinutes);
        }
        catch (Exception e)
        {
            throw new DatabaseBackupException($"An error occurred during backup {databaseName}.", e);
        }
    }

    private HashSet<string> ExtractUniqueExistingDatabaseNames(IEnumerable<Script> scripts)
    {
        using var connection = MsSqlDbHelper.GetOpenedConnection(_connectionString);
        var changedDatabaseNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var script in scripts)
        {
            if (script.NoBackup)
            {
                continue;
            }

            if (script.ParsedContent is not MsSqlParsedScriptContent parsedContent)
            {
                throw new DatabaseBackupException($"Script with id {script.Id} is not MSSQL.");
            }

            var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(parsedContent.Root, _databaseName);
            foreach (var source in sources)
            {
                foreach (var databaseName in source.DatabaseNames)
                {
                    if (!changedDatabaseNames.Contains(databaseName) && MsSqlDbHelper.IsDatabaseExist(connection, databaseName))
                    {
                        changedDatabaseNames.Add(databaseName);
                    }
                }
            }
        }

        return changedDatabaseNames;
    }
}
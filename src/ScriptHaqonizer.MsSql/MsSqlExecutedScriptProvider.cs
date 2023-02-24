using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Providers;
using ScriptHaqonizer.MsSql.Database.Constants;
using ScriptHaqonizer.MsSql.Database.Helpers;

namespace ScriptHaqonizer.MsSql;

/// <summary>
/// Represents a class that can be used to extract already applied scripts from the MSSQL database.
/// </summary>
public class MsSqlExecutedScriptProvider : IExecutedScriptProvider
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="MsSqlExecutedScriptProvider"/>.
    /// </summary>
    public MsSqlExecutedScriptProvider(string connectionString, string databaseName, ILogger logger)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of all executed scripts.
    /// </summary>
    /// <exception cref="CannotFetchExecutedScriptsException">Exception that occurs when it was not possible to get the scripts executed earlier</exception>
    public IReadOnlyList<ExecutedScript> GetExecutedScripts()
    {
        using var connection = MsSqlDbHelper.GetOpenedConnection(_connectionString);
        if (!MsSqlDbHelper.IsDatabaseExist(connection, _databaseName))
        {
            LogNoDatabasePresent();
            return Array.Empty<ExecutedScript>();
        }

        try
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @$"
SELECT 
    [Id],
    [ScriptName],
    [AppliedAtUtc]
FROM [{_databaseName}].[dbo].[{MsSqlTableConstants.MigrationScriptTableName}]
";

            using var reader = cmd.ExecuteReader();
            var executedScripts = new List<ExecutedScript>();
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var appliedAtUtc = reader.GetDateTime(2);
                executedScripts.Add(new ExecutedScript(id, name, appliedAtUtc));
            }

            return executedScripts;
        }
        catch (SqlException sqlException) when (sqlException.Number == MsSqlErrorCodeConstants.InvalidObjectName)
        {
            LogNoMigrationTablePresent();
            return Array.Empty<ExecutedScript>();
        }
        catch (Exception ex)
        {
            throw new CannotFetchExecutedScriptsException(ex);
        }
    }

    private void LogNoDatabasePresent()
    {
        _logger.LogWarning("The database named {databaseName} was not found. Hopefully the database will be created using the migration scripts.", _databaseName);
    }

    private void LogNoMigrationTablePresent()
    {
        _logger.LogWarning(
            "The migration script table {tableName} was not found in the {databaseName} database. Hopefully the table will be created using the migration scripts.",
            MsSqlTableConstants.MigrationScriptTableName, _databaseName);
    }
}
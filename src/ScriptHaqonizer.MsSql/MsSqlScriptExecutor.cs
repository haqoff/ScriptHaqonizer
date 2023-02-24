using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Executors;
using ScriptHaqonizer.Core.Logging;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.MsSql.Database.Constants;
using ScriptHaqonizer.MsSql.Database.Helpers;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql;

/// <summary>
/// Represents a mechanism for checking and executing migration scripts in MSSQL.
/// </summary>
public class MsSqlScriptExecutor : IScriptExecutor
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="MsSqlScriptExecutor"/>.
    /// </summary>
    public MsSqlScriptExecutor(string connectionString, string databaseName, ILogger logger)
    {
        _connectionString = connectionString;
        _databaseName = databaseName;
        _logger = logger;
    }

    /// <summary>
    /// Applies the script.
    /// </summary>
    /// <param name="script">Script.</param>
    /// <param name="mode">Launch mode.</param>
    /// <exception cref="MigrationBaseException">The exception that is thrown in case of a script application error.</exception>
    public void Apply(Script script, ExecutionMode mode)
    {
        using var executingScope = _logger.BeginScriptExecutingScope(script, mode);
        _logger.LogInformation("Starting script...");

        var parsedContent = CastToMsSqlContentOrThrow(script.ParsedContent, script.Id);
        if (mode == ExecutionMode.OnlyCheck && (script.SkipExecutionWithRollbackWhenChecking || script.NoTransaction))
        {
            _logger.LogInformation("This script cannot be run in OnlyCheck mode and will be skipped.");
            return;
        }

        using var connection = MsSqlDbHelper.GetOpenedConnection(_connectionString);
        using var transaction = script.NoTransaction ? null : connection.BeginTransaction();
        var dbExist = MsSqlDbHelper.IsDatabaseExist(connection, _databaseName, transaction);
        IDbCommand? currentExecutingCommand = null;

        if (!dbExist)
        {
            ValidationHelper.ThrowIfDatabaseNameIsNotClear(script.Id, parsedContent.Root);
        }

        var commands = new List<IDbCommand>();
        var scriptCommands = CreateCommandsFromBatches(connection, transaction, parsedContent.Batches);
        var insertScriptHistoryCommand = CreateInsertScriptExecutionHistoryCommand(connection, transaction, _databaseName, script);

        if (dbExist)
        {
            var useMainDbCommand = CreateUseDbCommand(connection, transaction, _databaseName);
            commands.Add(useMainDbCommand);
        }

        commands.AddRange(scriptCommands);
        commands.Add(insertScriptHistoryCommand);

        try
        {
            var sw = Stopwatch.StartNew();
            foreach (var command in commands)
            {
                currentExecutingCommand = command;
                var affectedRows = command.ExecuteNonQuery();
                LogExecutedCommand(script, command, affectedRows);
                currentExecutingCommand = null;
            }

            if (mode == ExecutionMode.OnlyCheck)
            {
                transaction?.Rollback();
            }
            else
            {
                transaction?.Commit();
            }

            sw.Stop();
            _logger.LogInformation("Script was successfully executed in {duration} seconds.", sw.Elapsed.TotalSeconds);
        }
        catch (Exception ex)
        {
            transaction?.Rollback();

            if (currentExecutingCommand != null)
            {
                throw new ScriptExecutionException(script.Id, $"Command is \"{currentExecutingCommand.CommandText}\".", ex);
            }

            throw new ScriptExecutionException(script.Id, ex);
        }
    }

    private static IEnumerable<IDbCommand> CreateCommandsFromBatches(SqlConnection connection, SqlTransaction? transaction, IEnumerable<string> batches)
    {
        return batches.Select(text =>
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = text;
            return command;
        });
    }

    private static IDbCommand CreateUseDbCommand(SqlConnection connection, SqlTransaction? transaction, string databaseName)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $@"USE [{databaseName}]";
        return command;
    }

    public static IDbCommand CreateInsertScriptExecutionHistoryCommand(SqlConnection connection, SqlTransaction? transaction, string databaseName, Script script)
    {
        var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.Parameters.AddWithValue("@Id", script.Id);
        command.Parameters.AddWithValue("@ScriptName", script.Name);
        command.Parameters.AddWithValue("@AppliedAtUtc", DateTime.UtcNow);
        command.CommandText = $@"
INSERT INTO [{databaseName}].[dbo].[{MsSqlTableConstants.MigrationScriptTableName}]
([Id], [ScriptName], [AppliedAtUtc])
VALUES
(@Id, @ScriptName, @AppliedAtUtc) 
";
        return command;
    }

    private void LogExecutedCommand(Script script, IDbCommand command, int affectedRows)
    {
        _logger.LogInformation("""
Script: {script}
Command: "{commandText}"
Number of rows affected: {affectedRows}
""", script.SourceFileName, command.CommandText, affectedRows);
    }

    private static MsSqlParsedScriptContent CastToMsSqlContentOrThrow(IParsedScriptContent content, int scriptId)
    {
        if (content is not MsSqlParsedScriptContent parsedContent)
        {
            throw new ScriptExecutionException(scriptId, "The specified script cannot be run for MSSQL.");
        }

        return parsedContent;
    }
}
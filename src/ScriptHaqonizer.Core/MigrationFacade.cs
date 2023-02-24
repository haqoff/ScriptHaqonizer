using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Backups;
using ScriptHaqonizer.Core.Calculators;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Executors;
using ScriptHaqonizer.Core.Logging;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Providers;

namespace ScriptHaqonizer.Core;

/// <summary>
/// Provides a mechanism for tracking and executing unapplied migration scripts.
/// </summary>
public class MigrationFacade : IMigrationFacade
{
    private readonly IExecutedScriptProvider _executedScriptProvider;
    private readonly IParsedScriptProvider _parsedScriptProvider;
    private readonly INewScriptCalculator _scriptsCalculator;
    private readonly IDatabaseBackupExecutor _backupExecutor;
    private readonly IScriptExecutor _scriptExecutor;
    private readonly MigrationLoggerOptions _loggerOptions;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="MigrationFacade"/>.
    /// </summary>
    public MigrationFacade(
        IExecutedScriptProvider executedScriptProvider,
        IParsedScriptProvider parsedScriptProvider,
        INewScriptCalculator scriptsCalculator,
        IDatabaseBackupExecutor backupExecutor,
        IScriptExecutor scriptExecutor,
        MigrationLoggerOptions loggerOptions,
        ILogger logger
    )
    {
        _executedScriptProvider = executedScriptProvider;
        _parsedScriptProvider = parsedScriptProvider;
        _scriptsCalculator = scriptsCalculator;
        _backupExecutor = backupExecutor;
        _scriptExecutor = scriptExecutor;
        _loggerOptions = loggerOptions;
        _logger = logger;
    }

    /// <summary>
    /// Attempts to migrate the database by executing the found scripts in the specified mode.
    /// <br/>
    /// If an error occurs, it is logged.
    /// </summary>
    /// <returns><c>True</c> if the migration applied without errors; <c>false</c> - otherwise.</returns>
    public bool TryMigrate(ExecutionMode mode)
    {
        try
        {
            Migrate(mode);
            _logger.LogInformation("Migration done.");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Migration failed.");
            return false;
        }
    }

    /// <summary>
    /// Migrates the database, executing the found scripts in the specified mode.
    /// </summary>
    /// <exception cref="MigrationBaseException">The exception that occurs if the operation failed in the specified mode.</exception>
    public void Migrate(ExecutionMode mode)
    {
        using var sourceContext = _logger.BeginMigrationScope(_loggerOptions.SourceName, _loggerOptions.Environment);

        var executedScripts = _executedScriptProvider.GetExecutedScripts();
        var allScripts = _parsedScriptProvider.GetParsedScripts();
        var newScripts = _scriptsCalculator.GetNotAppliedSortedScripts(executedScripts, allScripts);

        if (newScripts.Count == 0)
        {
            _logger.LogInformation("No new migration scripts were found.");
        }
        else
        {
            var scriptsString = string.Join("; ", newScripts.Select(s => s.SourceFileName));
            _logger.LogInformation("New migration scripts found: [{scripts}]", scriptsString);
        }

        if (mode == ExecutionMode.Full)
        {
            _backupExecutor.TryExecute(newScripts);
        }

        foreach (var newScript in newScripts)
        {
            _scriptExecutor.Apply(newScript, mode);
        }
    }
}
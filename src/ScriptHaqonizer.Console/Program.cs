using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Console;
using ScriptHaqonizer.Core;
using ScriptHaqonizer.Core.Calculators;
using ScriptHaqonizer.Core.Executors;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.Core.Paths;
using ScriptHaqonizer.Core.Providers;
using ScriptHaqonizer.MsSql;
using ScriptHaqonizer.Core.Backups;
using Microsoft.Extensions.Logging.Console;
using ScriptHaqonizer.Core.Models;

Parser.Default.ParseArguments<Options>(args).WithParsed(StartMigration).WithNotParsed(OnArgParseError);

static void StartMigration(Options o)
{
    var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddSimpleConsole(options => { options.IncludeScopes = true; });
        builder.Services.Configure((ConsoleLoggerOptions options) => { options.LogToStandardErrorThreshold = LogLevel.Error; });
    });
    var logger = loggerFactory.CreateLogger("ScriptHaqonizer");

    IScriptContentParser scriptContentParser;
    IExecutedScriptProvider executedScriptProvider;
    IScriptExecutor scriptExecutor;
    IDatabaseBackupExecutor backupExecutor;

    switch (o.DbType)
    {
        case SupportedDb.MsSql:
            scriptContentParser = new MsSqlScriptContentParser(o.SpecifyingDatabaseNameValidation, logger);
            executedScriptProvider = new MsSqlExecutedScriptProvider(o.ConnectionString, o.DatabaseName, logger);
            scriptExecutor = new MsSqlScriptExecutor(o.ConnectionString, o.DatabaseName, logger);
            backupExecutor = new MsSqlDatabaseBackupExecutor(o.ConnectionString, o.DatabaseName, o.BackupPath, logger);
            break;

        default:
            throw new ArgumentOutOfRangeException(nameof(o.DbType), "Unsupported database type.");
    }

    var scriptPathProvider = new ScriptFilePathProvider(o.ScriptDirectoryPath);
    var scriptContentLoader = new ScriptContentLoader();
    var scriptParser = new ScriptParser(scriptContentLoader, scriptContentParser);
    var parsedScriptProvider = new ParsedScriptProvider(scriptPathProvider, scriptParser);
    var newScriptCalculator = new NewScriptCalculator(o.EnvironmentName);
    var migrationOptions = new MigrationLoggerOptions(o.Source, o.EnvironmentName);
    var migrationFacade = new MigrationFacade(executedScriptProvider, parsedScriptProvider, newScriptCalculator, backupExecutor, scriptExecutor, migrationOptions, logger);

    var success = migrationFacade.TryMigrate(o.Mode);
    Environment.Exit(success ? ExitConstants.MigrationSuccess : ExitConstants.MigrationFailed);
}

static void OnArgParseError(IEnumerable<Error> errors)
{
    Environment.Exit(ExitConstants.ArgumentParseError);
}
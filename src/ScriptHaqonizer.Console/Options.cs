using CommandLine;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Console;

/// <Summary>
/// Arguments of launching the migration console.
/// </Summary>
public class Options
{
    /// <Summary>
    /// The connection line to the database server.
    /// </Summary>
    [Option('c', "connectionString", Required = true, HelpText = "Database connection string")]
    public string ConnectionString { get; init; } = default!;

    /// <Summary>
    /// The name of the database in which the history of the used scripts will be stored.
    /// It may not exist at the time of launch.
    /// </Summary>
    [Option('d', "databaseName", Required = true, HelpText = "The name of the database for which the migrations are being performed.")]
    public string DatabaseName { get; init; } = default!;

    /// <summary>
    /// The full path to the folder in which migration scripts are located. The folder must exist.
    /// </summary>
    [Option('s', "scriptDirectoryPath", Required = true, HelpText = "Full path to the folder where the scripts are located")]
    public string ScriptDirectoryPath { get; init; } = default!;

    /// <summary>
    /// The name of the current environment for which migration will be performed.
    /// </summary>
    [Option('e', "environment", Required = true, HelpText = "The name of the environment for which you want to migrate")]
    public AvailableEnvironment EnvironmentName { get; init; } = default!;

    /// <summary>
    /// Migration launch mode.
    /// <br/>
    /// <see cref="ExecutionMode.OnlyCheck"/> is used to validate and check the possibility of starting scripts.
    /// <br/>
    /// <see cref="ExecutionMode.Full"/> is used to actually execute scripts.
    /// </summary>
    [Option('m', "mode", Required = true, HelpText = "Launch mode. OnlyCheck or Full.")]
    public ExecutionMode Mode { get; init; }

    /// <summary>
    /// Type of database.
    /// </summary>
    [Option('t', "dbType", Required = false, HelpText = "Database type.", Default = SupportedDb.MsSql)]
    public SupportedDb DbType { get; init; }

    /// <summary>
    /// If set to true, all statements in scripts must explicitly specify which database to use.
    /// </summary>
    [Option("specifyingDatabaseNameValidation", Required = false, HelpText = "If set to true, all statements in scripts must explicitly specify which database to use.", Default = false)]
    public bool SpecifyingDatabaseNameValidation { get; init; }

    /// <summary>
    /// The path to the folder where full database backups will be stored.
    /// If the path is specified and the startup type is full, then if new migration scripts are available, the affected databases will be backed up.
    /// </summary>
    [Option("backupPath", Required = false, HelpText = "The path to the folder where full database backups will be stored.")]
    public string? BackupPath { get; init; }

    /// <summary>
    /// Launch source, additionally displayed in the logs.
    /// </summary>
    [Option("from", Required = false, HelpText = "Launch source, additionally displayed in the logs.", Default = "Unknown")]
    public string Source { get; init; } = default!;
}
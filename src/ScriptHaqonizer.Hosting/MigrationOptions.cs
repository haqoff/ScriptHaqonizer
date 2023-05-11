using System.ComponentModel.DataAnnotations;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Hosting;

/// <summary>
/// Contains parameters for starting the migration.
/// </summary>
public class MigrationOptions
{
    /// <Summary>
    /// The connection line to the database server.
    /// </Summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Connection string must be specified.")]
    public string ConnectionString { get; init; } = default!;

    /// <Summary>
    /// The name of the database in which the history of the used scripts will be stored.
    /// It may not exist at the time of launch.
    /// </Summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Database name must be specified.")]
    public string DatabaseName { get; init; } = default!;

    /// <summary>
    /// The full path to the folder in which migration scripts are located. The folder must exist.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Script directory path must be specified.")]
    public string ScriptDirectoryPath { get; init; } = default!;

    /// <summary>
    /// The name of the current environment for which migration will be performed.
    /// </summary>
    public AvailableEnvironment EnvironmentName { get; init; } = AvailableEnvironment.Local;

    /// <summary>
    /// Migration launch mode.
    /// <br/>
    /// <see cref="ExecutionMode.OnlyCheck"/> is used to validate and check the possibility of starting scripts.
    /// <br/>
    /// <see cref="ExecutionMode.Full"/> is used to actually execute scripts.
    /// </summary>
    public ExecutionMode Mode { get; init; } = ExecutionMode.Full;

    /// <summary>
    /// If set to true, all statements in scripts must explicitly specify which database to use.
    /// </summary>
    public bool SpecifyingDatabaseNameValidation { get; init; } = false;

    /// <summary>
    /// The path to the folder where full database backups will be stored.
    /// If the path is specified and the startup type is full, then if new migration scripts are available, the affected databases will be backed up.
    /// </summary>
    public string? BackupPath { get; init; } = null;

    /// <summary>
    /// Launch source, additionally displayed in the logs.
    /// </summary>
    public string Source { get; init; } = "Unknown";
}
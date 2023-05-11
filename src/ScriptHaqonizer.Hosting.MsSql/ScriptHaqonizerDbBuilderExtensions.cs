using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Backups;
using ScriptHaqonizer.Core.Executors;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.Core.Providers;
using ScriptHaqonizer.MsSql;

namespace ScriptHaqonizer.Hosting.MsSql;

/// <summary>
/// Contains the registration method for the MsSql provider.
/// </summary>
public static class ScriptHaqonizerDbBuilderExtensions
{
    /// <summary>
    /// Registers MsSql as a scripting engine.
    /// </summary>
    public static void WithMsSql(this ScriptHaqonizerDbBuilder builder)
    {
        var serviceCollection = builder.ServiceCollection;
        var migrationOptions = builder.MigrationOptions;
        serviceCollection.AddSingleton<IScriptContentParser, MsSqlScriptContentParser>(p =>
            new MsSqlScriptContentParser(migrationOptions.SpecifyingDatabaseNameValidation, p.GetRequiredService<ILogger>()));
        serviceCollection.AddSingleton<IExecutedScriptProvider, MsSqlExecutedScriptProvider>(p =>
            new MsSqlExecutedScriptProvider(migrationOptions.ConnectionString, migrationOptions.DatabaseName, p.GetRequiredService<ILogger>()));
        serviceCollection.AddSingleton<IScriptExecutor, MsSqlScriptExecutor>(p =>
            new MsSqlScriptExecutor(migrationOptions.ConnectionString, migrationOptions.DatabaseName, p.GetRequiredService<ILogger>()));
        serviceCollection.AddSingleton<IDatabaseBackupExecutor, MsSqlDatabaseBackupExecutor>(p =>
            new MsSqlDatabaseBackupExecutor(migrationOptions.ConnectionString, migrationOptions.DatabaseName, migrationOptions.BackupPath, p.GetRequiredService<ILogger>()));
    }
}
﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core;
using ScriptHaqonizer.Core.Backups;
using ScriptHaqonizer.Core.Calculators;
using ScriptHaqonizer.Core.Executors;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.Core.Paths;
using ScriptHaqonizer.Core.Providers;
using ScriptHaqonizer.MsSql;

namespace ScriptHaqonizer.Hosting;

/// <summary>
/// Provides an extension method for adding a migration mechanism.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a ScriptHaqonizer to the services collection.
    /// <br/>
    /// Registers a hosted service <see cref="MigrationHostedService"/> that will perform the migration at application startup.
    /// </summary>
    public static IServiceCollection AddScriptHaqonizer(this IServiceCollection serviceCollection, MigrationOptions migrationOptions)
    {
        var validationContext = new ValidationContext(migrationOptions);
        Validator.ValidateObject(migrationOptions, validationContext, true);

        switch (migrationOptions.DbType)
        {
            case SupportedDb.MsSql:
                serviceCollection.AddSingleton<IScriptContentParser, MsSqlScriptContentParser>(p =>
                    new MsSqlScriptContentParser(migrationOptions.SpecifyingDatabaseNameValidation, p.GetRequiredService<ILogger>()));
                serviceCollection.AddSingleton<IExecutedScriptProvider, MsSqlExecutedScriptProvider>(p =>
                    new MsSqlExecutedScriptProvider(migrationOptions.ConnectionString, migrationOptions.DatabaseName, p.GetRequiredService<ILogger>()));
                serviceCollection.AddSingleton<IScriptExecutor, MsSqlScriptExecutor>(p =>
                    new MsSqlScriptExecutor(migrationOptions.ConnectionString, migrationOptions.DatabaseName, p.GetRequiredService<ILogger>()));
                serviceCollection.AddSingleton<IDatabaseBackupExecutor, MsSqlDatabaseBackupExecutor>(p =>
                    new MsSqlDatabaseBackupExecutor(migrationOptions.ConnectionString, migrationOptions.DatabaseName, migrationOptions.BackupPath, p.GetRequiredService<ILogger>()));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(migrationOptions.DbType), "Unsupported database type.");
        }

        serviceCollection.AddSingleton<IScriptFilePathProvider, ScriptFilePathProvider>(_ => new ScriptFilePathProvider(migrationOptions.ScriptDirectoryPath));
        serviceCollection.AddSingleton<IScriptContentLoader, ScriptContentLoader>();
        serviceCollection.AddSingleton<IScriptParser, ScriptParser>();
        serviceCollection.AddSingleton<IParsedScriptProvider, ParsedScriptProvider>();
        serviceCollection.AddSingleton<INewScriptCalculator, NewScriptCalculator>(_ => new NewScriptCalculator(migrationOptions.EnvironmentName));
        var loggerOptions = new MigrationLoggerOptions(migrationOptions.Source, migrationOptions.EnvironmentName);
        serviceCollection.AddSingleton<IMigrationFacade, MigrationFacade>(p => new MigrationFacade
            (
                p.GetRequiredService<IExecutedScriptProvider>(),
                p.GetRequiredService<IParsedScriptProvider>(),
                p.GetRequiredService<INewScriptCalculator>(),
                p.GetRequiredService<IDatabaseBackupExecutor>(),
                p.GetRequiredService<IScriptExecutor>(),
                loggerOptions,
                p.GetRequiredService<ILogger>()
            )
        );

        serviceCollection.AddHostedService(p => new MigrationHostedService(p.GetRequiredService<IMigrationFacade>(), migrationOptions.Mode));
        return serviceCollection;
    }
}
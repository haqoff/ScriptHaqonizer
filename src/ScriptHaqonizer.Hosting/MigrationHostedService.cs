using Microsoft.Extensions.Hosting;
using ScriptHaqonizer.Core;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Hosting;

/// <summary>
/// Represents a hosted service that starts the migration when the application starts.
/// </summary>
public class MigrationHostedService : IHostedService
{
    private readonly IMigrationFacade _migrationFacade;
    private readonly ExecutionMode _mode;

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationHostedService"/> class.
    /// </summary>
    public MigrationHostedService(IMigrationFacade migrationFacade, ExecutionMode mode)
    {
        _migrationFacade = migrationFacade;
        _mode = mode;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _migrationFacade.Migrate(_mode);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
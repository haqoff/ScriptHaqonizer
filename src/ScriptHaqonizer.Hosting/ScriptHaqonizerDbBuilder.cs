using Microsoft.Extensions.DependencyInjection;

namespace ScriptHaqonizer.Hosting;

/// <summary>
/// Class that contains the necessary parameters for registering the script execution engine.
/// </summary>
public record ScriptHaqonizerDbBuilder(MigrationOptions MigrationOptions, IServiceCollection ServiceCollection);
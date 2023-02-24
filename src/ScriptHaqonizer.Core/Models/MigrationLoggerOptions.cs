namespace ScriptHaqonizer.Core.Models;

/// <summary>
/// Contains options for logging migration operations.
/// </summary>
/// <param name="SourceName">Source to run migrations.</param>
/// <param name="Environment">Current environment.</param>
public record MigrationLoggerOptions(string SourceName, AvailableEnvironment Environment);
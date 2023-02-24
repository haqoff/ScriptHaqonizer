namespace ScriptHaqonizer.Console;

/// <summary>
/// Contains constants for exit codes of the console process.
/// </summary>
public static class ExitConstants
{
    /// <summary>
    /// Operation completed successfully.
    /// </summary>
    public const int MigrationSuccess = 0;

    /// <summary>
    /// Invalid console arguments.
    /// </summary>
    public const int ArgumentParseError = 1;

    /// <summary>
    /// Operation failed.
    /// </summary>
    public const int MigrationFailed = 2;
}
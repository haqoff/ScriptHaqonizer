namespace ScriptHaqonizer.Core.Models;

/// <summary>
/// Script launch type.
/// </summary>
public enum ExecutionMode
{
    /// <summary>
    /// The script is run only to check its execution and then rollback the transaction.
    /// </summary>
    OnlyCheck,

    /// <summary>
    /// The script is run for final application to the database. The transaction will be committed.
    /// </summary>
    Full
}
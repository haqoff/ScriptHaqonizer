using ScriptHaqonizer.Core.Parsers;

namespace ScriptHaqonizer.Core.Models;

/// <summary>
/// Represents a migration script obtained from a file.
/// </summary>
public class Script
{
    /// <summary>
    /// Represents a migration script obtained from a file.
    /// </summary>
    /// <param name="id">Migration ID. Is greater than zero.</param>
    /// <param name="name">Name of the migration. It is in Ascii encoding from 3 to 50 characters.</param>
    /// <param name="environmentContainer">Information about the environments in which the script should be used.</param>
    /// <param name="sourceFileName">The original filename of the script, excluding the file extension.</param>
    /// <param name="parsedContent">Contains the parsed result of the script content depending on the database.</param>
    /// <param name="skipExecutionWithRollbackWhenChecking">Indicates that in check mode, it is necessary to skip the script launch with transaction rollback.</param>
    /// <param name="noTransaction">An indication that a transaction is not required to execute this script.</param>
    /// <param name="noBackup">Indicates that this script does not need to back up databases if backup is enabled.</param>
    public Script
    (
        int id,
        string name,
        ScriptEnvironmentContainer environmentContainer,
        string sourceFileName,
        IParsedScriptContent parsedContent,
        bool skipExecutionWithRollbackWhenChecking,
        bool noTransaction,
        bool noBackup
    )
    {
        Id = id;
        Name = name;
        EnvironmentContainer = environmentContainer;
        SourceFileName = sourceFileName;
        ParsedContent = parsedContent;
        SkipExecutionWithRollbackWhenChecking = skipExecutionWithRollbackWhenChecking;
        NoTransaction = noTransaction;
        NoBackup = noBackup;
    }

    public virtual bool Equals(Script? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id;
    }

    /// <summary>Migration ID. Is greater than zero.</summary>
    public int Id { get; init; }

    /// <summary>Name of the migration. It is in Ascii encoding from 3 to 50 characters.</summary>
    public string Name { get; init; }

    /// <summary>Information about the environments in which the script should be used.</summary>
    public ScriptEnvironmentContainer EnvironmentContainer { get; init; }

    /// <summary>The original filename of the script, excluding the file extension.</summary>
    public string SourceFileName { get; init; }

    /// <summary>Contains the parsed result of the script content depending on the database.</summary>
    public IParsedScriptContent ParsedContent { get; init; }

    /// <summary>Indicates that in check mode, it is necessary to skip the script launch with transaction rollback.</summary>
    public bool SkipExecutionWithRollbackWhenChecking { get; init; }

    /// <summary>An indication that a transaction is not required to execute this script.</summary>
    public bool NoTransaction { get; init; }

    /// <summary>Indicates that this script does not need to back up databases if backup is enabled.</summary>
    public bool NoBackup { get; init; }
}
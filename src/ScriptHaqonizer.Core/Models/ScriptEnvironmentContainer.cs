namespace ScriptHaqonizer.Core.Models;

/// <summary>
/// Represents information about script application environments.
/// </summary>
public class ScriptEnvironmentContainer
{
    /// <summary>
    /// All environments. The script will be applied everywhere.
    /// </summary>
    public static readonly ScriptEnvironmentContainer All = new(Array.Empty<AvailableEnvironment>(), EntryType.Excluded);

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptEnvironmentContainer"/> class.
    /// </summary>
    /// <param name="environments">Environments.</param>
    /// <param name="entryType">Type of application of environments: include / exclude.</param>
    public ScriptEnvironmentContainer(IEnumerable<AvailableEnvironment> environments, EntryType entryType)
    {
        Environments = new HashSet<AvailableEnvironment>(environments);
        EntryType = entryType;
    }

    /// <summary>
    /// Environments.
    /// </summary>
    public IReadOnlyCollection<AvailableEnvironment> Environments { get; }

    /// <summary>
    /// Type of specified environments.
    /// If include is specified, then the script should only be used in the listed environments.
    /// If exclude is specified, then the script must not be used in the listed environments, but be used in any others.
    /// </summary>
    public EntryType EntryType { get; }

    /// <summary>
    /// Gets an indication that the specified <paramref name="environment"/> matches the rules specified in this container.
    /// </summary>
    public bool IsRelatedTo(AvailableEnvironment environment)
    {
        var hasAny = Environments.Any(e => e == environment);
        return EntryType == EntryType.Excluded ? !hasAny : hasAny;
    }
}
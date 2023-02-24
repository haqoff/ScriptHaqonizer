namespace ScriptHaqonizer.Core.Models;

/// <summary>
/// Represents information about the already executed migration script.
/// </summary>
/// <param name="Id">Script ID.</param>
/// <param name="ScriptName">Script name.</param>
/// <param name="AppliedAtUtc">Script application time.</param>
public record ExecutedScript(int Id, string ScriptName, DateTime AppliedAtUtc);
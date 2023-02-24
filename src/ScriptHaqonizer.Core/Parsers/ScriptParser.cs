using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;

namespace ScriptHaqonizer.Core.Parsers;

/// <summary>
/// Represents a mechanism for parsing how a migration script is applied. 
/// </summary>
public class ScriptParser : IScriptParser
{
    private readonly record struct BoolParams(bool SkipExecutionWithRollbackWhenChecking, bool NoTransaction, bool NoBackup)
    {
        public const string SkipExecutionWithRollbackWhenCheckingParameter = "nocheck";
        public const string NoTransactionParameter = "notransaction";
        public const string NoBackupParameter = "nobackup";

        public static BoolParams Empty => new(false, false, false);
        public bool AnyParameterPresent => SkipExecutionWithRollbackWhenChecking || NoTransaction || NoBackup;
    }

    private readonly IScriptContentLoader _contentLoader;
    private readonly IScriptContentParser _contentParser;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptParser"/> class.
    /// </summary>
    public ScriptParser(IScriptContentLoader contentLoader, IScriptContentParser contentParser)
    {
        _contentLoader = contentLoader;
        _contentParser = contentParser;
    }

    /// <summary>
    /// Gets a script by parsing its execution options.
    /// </summary>
    /// <param name="path">Path to the script.</param>
    /// <returns>Script with parsed information.</returns>
    /// <exception cref="ScriptNameValidationException">The exception that is thrown if the filename does not match the pattern.</exception>
    /// <exception cref="ScriptLoadException">Exception that is thrown when the contents of the script failed to load.</exception>
    public Script Parse(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);

        var tokens = fileName.Split('_');
        switch (tokens.Length)
        {
            case < 2:
                throw new ScriptNameValidationException(fileName, "The file name must contain at least one underscore.");
            case > 4:
                throw new ScriptNameValidationException(fileName, "Encountered too many underscores.");
        }

        var firstToken = tokens[0];
        var secondToken = tokens[1];
        var thirdToken = ExtractOptionalToken(tokens, 2);
        var fourthToken = ExtractOptionalToken(tokens, 3);

        var id = ParseId(firstToken, fileName);
        var migrationName = ParseMigrationName(secondToken, fileName);
        var (scriptEnvironmentContainer, boolParams) = ParseEnvironmentAndBoolParams(thirdToken, fourthToken, fileName);
        var contentText = _contentLoader.Load(path);
        var parsedContent = _contentParser.ParseAndThrowIfNotValid(id, contentText, boolParams.NoTransaction);
        var script = new Script
        (
            id,
            migrationName,
            scriptEnvironmentContainer,
            fileName,
            parsedContent,
            boolParams.SkipExecutionWithRollbackWhenChecking,
            boolParams.NoTransaction,
            boolParams.NoBackup
        );
        return script;
    }

    private static int ParseId(string firstToken, string fileName)
    {
        if (!int.TryParse(firstToken, out var id))
        {
            throw new ScriptNameValidationException(fileName, "Failed to cast identifier to a number.");
        }

        if (id < 1)
        {
            throw new ScriptNameValidationException(fileName, "Script ID must be greater than 0.");
        }

        return id;
    }

    private static string ParseMigrationName(string secondToken, string fileName)
    {
        switch (secondToken.Length)
        {
            case < 3:
                throw new ScriptNameValidationException(fileName, "The migration name must contain at least 3 characters.");
            case > 50:
                throw new ScriptNameValidationException(fileName, "Migration name must not exceed 50 characters.");
        }

        if (!IsAnsiString(secondToken))
        {
            throw new ScriptNameValidationException(fileName, "The migration name must be in ASCII encoding.");
        }

        return secondToken;
    }

    private static (ScriptEnvironmentContainer environmentContainer, BoolParams parsedParams) ParseEnvironmentAndBoolParams(string? thirdToken, string? fourthToken, string fileName)
    {
        var thirdTokenParams = ParseParams(thirdToken, fileName, true);
        if (thirdTokenParams.AnyParameterPresent)
        {
            if (fourthToken is not null)
            {
                throw new ScriptNameValidationException(fileName, "After parameters, more text was encountered through the underscore.");
            }

            var skippedEnvironment = ParseEnvironment(null, fileName);
            return (skippedEnvironment, thirdTokenParams);
        }

        var environment = ParseEnvironment(thirdToken, fileName);
        var parsedParams = ParseParams(fourthToken, fileName, false);
        return (environment, parsedParams);
    }

    private static ScriptEnvironmentContainer ParseEnvironment(string? environmentSource, string fileName)
    {
        if (environmentSource is null)
        {
            return ScriptEnvironmentContainer.All;
        }

        string tokenString;
        EntryType type;

        if (environmentSource.StartsWith("!"))
        {
            tokenString = environmentSource.Substring(1);
            type = EntryType.Excluded;
        }
        else
        {
            tokenString = environmentSource;
            type = EntryType.Included;
        }

        var tokens = tokenString.Split('&');
        var parsedEnvironments = new List<AvailableEnvironment>(tokens.Length);

        foreach (var token in tokens)
        {
            if (!Enum.TryParse<AvailableEnvironment>(token, true, out var environment))
            {
                throw new ScriptNameValidationException(fileName,
                    $"If an environment is present, it must be one of the following: {string.Join(", ", Enum.GetValues<AvailableEnvironment>())}.");
            }

            parsedEnvironments.Add(environment);
        }

        return new ScriptEnvironmentContainer(parsedEnvironments, type);
    }

    private static BoolParams ParseParams(string? src, string fileName, bool noThrow)
    {
        if (src is null)
        {
            return BoolParams.Empty;
        }

        var sourceTokens = src.Split('&');
        var uniqueTokens = new HashSet<string>(sourceTokens, StringComparer.OrdinalIgnoreCase);
        if (sourceTokens.Length != uniqueTokens.Count)
        {
            if (noThrow)
            {
                return BoolParams.Empty;
            }

            throw new ScriptNameValidationException(fileName, "Duplicate parameters encountered.");
        }

        var noCheck = uniqueTokens.Remove(BoolParams.SkipExecutionWithRollbackWhenCheckingParameter);
        var noTransaction = uniqueTokens.Remove(BoolParams.NoTransactionParameter);
        var noBackup = uniqueTokens.Remove(BoolParams.NoBackupParameter);

        if (uniqueTokens.Count > 0)
        {
            if (noThrow)
            {
                return BoolParams.Empty;
            }

            throw new ScriptNameValidationException(fileName, $"Unknown parameters encountered: {string.Join(",", uniqueTokens)}");
        }

        return new BoolParams(noCheck, noTransaction, noBackup);
    }

    private static bool IsAnsiString(ReadOnlySpan<char> str)
    {
        const int maxAnsiCode = 255;
        foreach (var c in str)
        {
            if (c > maxAnsiCode) return false;
        }

        return true;
    }

    private static string? ExtractOptionalToken(IReadOnlyList<string> tokens, int index)
    {
        return index < tokens.Count ? tokens[index] : null;
    }
}
using Microsoft.Extensions.Logging;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Parsers;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Extensions;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql;

/// <summary>
/// Provides a method for parsing of a TSQL script.
/// </summary>
public class MsSqlScriptContentParser : IScriptContentParser
{
    private readonly bool _validateThatAllDatabaseNamesSpecified;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the class <see cref="MsSqlScriptContentParser"/>.
    /// </summary>
    public MsSqlScriptContentParser(bool validateThatAllDatabaseNamesSpecified, ILogger logger)
    {
        _validateThatAllDatabaseNamesSpecified = validateThatAllDatabaseNamesSpecified;
        _logger = logger;
    }

    /// <summary>
    /// Parses the script and also checks the contents of the script: only syntax, lexical errors and other assigned rules that can be checked without access to the actual database and its state.
    /// </summary>
    /// <exception cref="ScriptContentValidationException">Exception that is thrown when the contents of a script file are invalid.</exception>
    public IParsedScriptContent ParseAndThrowIfNotValid(int scriptId, string content, bool allowScriptTransactions)
    {
        var result = CreateFromText(content);

        if (!allowScriptTransactions)
        {
            result = RemoveTransactionStatementsIfAny(result, _logger);
        }

        if (result.Errors.Count > 0)
        {
            var errorText = FormatErrors(result.Errors);
            throw new ScriptContentValidationException(scriptId, errorText);
        }

        if (_validateThatAllDatabaseNamesSpecified)
        {
            ValidationHelper.ThrowIfDatabaseNameIsNotClear(scriptId, result.Root);
        }

        if (result.Root.Fragment.Batches.All(b => b.Statements.Count == 0))
        {
            throw new ScriptContentValidationException(scriptId, $"Script {scriptId} contains no statements.");
        }

        return result;
    }

    private static MsSqlParsedScriptContent CreateFromText(string sql)
    {
        var parser = new TSql160Parser(true, SqlEngineType.All);
        var root = parser.Parse(new StringReader(sql), out var errors);
        var traversableSqlNode = TraversableSqlNodeHelper.Create(root);
        var newLines = PositionHelper.GetNewLines(sql);
        var script = (TSqlScript) root;
        var batches = script.Batches.Select(b =>
        {
            var start = b.GetStartLocation();
            var end = b.GetEndLocation();

            var startIndex = PositionHelper.ToTextPosition(newLines, start);
            var endIndex = PositionHelper.ToTextPosition(newLines, end);

            return sql.Substring(startIndex, endIndex - startIndex);
        }).ToList();

        return new MsSqlParsedScriptContent(traversableSqlNode, errors, newLines, sql, batches);
    }

    private static MsSqlParsedScriptContent RemoveTransactionStatementsIfAny(MsSqlParsedScriptContent result, ILogger logger)
    {
        var statementNodes = result.Root.EnumerateAll<TransactionStatement>().ToList();

        if (statementNodes.Count > 0)
        {
            LogExtraTransactionDeclared(logger);
            var newSql = SqlModifierHelper.RemoveFragments(result.Content, statementNodes.Select(s => s.Fragment), result.NewLines);
            result = CreateFromText(newSql);
        }

        return result;
    }

    private static string FormatErrors(IEnumerable<ParseError> errors)
    {
        var sb = new StringBuilder();
        foreach (var error in errors)
        {
            sb.AppendLine(FormatError(error));
        }

        return sb.ToString();
    }

    private static string FormatError(ParseError error)
    {
        return $"[Line: {error.Line}, Column: {error.Column}, Number: {error.Number}]: {error.Message}";
    }

    private static void LogExtraTransactionDeclared(ILogger logger)
    {
        logger.LogWarning("TRANSACTION statements found in SQL script. It makes no sense to declare a transaction in a script, since the entire script is already executed in a transaction.");
    }
}
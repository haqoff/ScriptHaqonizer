using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.Core.Parsers;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql;

/// <summary>
/// Contains the parsed result of the TSQL script content.
/// </summary>
internal record MsSqlParsedScriptContent
(
    TraversableSqlNode<TSqlScript> Root,
    IList<ParseError> Errors,
    IReadOnlyList<int> NewLines,
    string Content,
    IReadOnlyList<string> Batches
) : IParsedScriptContent;
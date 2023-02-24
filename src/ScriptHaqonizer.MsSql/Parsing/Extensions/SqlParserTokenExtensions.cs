using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="TSqlParserToken"/>.
/// </summary>
internal static class SqlParserTokenExtensions
{
    /// <summary>
    /// Gets the location of the start of the token.
    /// </summary>
    public static SqlLocation GetStartLocation(this TSqlParserToken token)
    {
        return new SqlLocation(token.Line, token.Column);
    }

    /// <summary>
    /// Gets the location of the end of the token.
    /// </summary>
    public static SqlLocation GetEndLocation(this TSqlParserToken token)
    {
        return new SqlLocation(token.Line, token.Column + token.Text.Length);
    }
}
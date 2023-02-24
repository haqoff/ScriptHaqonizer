using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Extensions;

/// <summary>
/// Provides extension methods for <see cref="TSqlFragment"/>.
/// </summary>
internal static class SqlFragmentExtensions
{
    /// <summary>
    /// Gets the fragment's starting location.
    /// </summary>
    internal static SqlLocation GetStartLocation(this TSqlFragment fragment)
    {
        return fragment.ScriptTokenStream[fragment.FirstTokenIndex].GetStartLocation();
    }

    /// <summary>
    /// Gets the final location of the fragment.
    /// </summary>
    internal static SqlLocation GetEndLocation(this TSqlFragment fragment)
    {
        return fragment.ScriptTokenStream[fragment.LastTokenIndex].GetEndLocation();
    }
}
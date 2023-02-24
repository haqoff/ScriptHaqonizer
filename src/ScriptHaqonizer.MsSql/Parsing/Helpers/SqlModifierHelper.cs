using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Extensions;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Helpers;

/// <summary>
/// Provides helper methods for modifying the contents of the SQL script.
/// </summary>
internal static class SqlModifierHelper
{
    /// <summary>
    /// Removes the specified occurrences from the text, returning a new string.
    /// </summary>
    internal static string RemoveFragments(string content, IEnumerable<TSqlFragment> entries, IReadOnlyList<int> newLines)
    {
        var removeRanges = new List<MutableIntRange>();
        foreach (var entry in entries)
        {
            var startIndex = PositionHelper.ToTextPosition(newLines, entry.GetStartLocation());
            var endIndex = PositionHelper.ToTextPosition(newLines, entry.GetEndLocation());
            var range = new MutableIntRange(startIndex, endIndex);

            var existed = removeRanges.FirstOrDefault(r => r.IntersectOrBorderTo(range));
            if (existed is null)
            {
                removeRanges.Add(range);
            }
            else
            {
                existed.TryMerge(range);
            }
        }

        removeRanges.Sort();

        var offset = 0;
        foreach (var range in removeRanges)
        {
            content = content.Remove(range.Start - offset, range.Length);
            offset += range.Length;
        }

        return content;
    }
}
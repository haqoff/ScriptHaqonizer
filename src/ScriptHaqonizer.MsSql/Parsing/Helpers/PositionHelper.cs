using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Helpers;

/// <summary>
/// Provides helper methods for working with text position.
/// </summary>
internal static class PositionHelper
{
    /// <summary>
    /// Gets indexes of newline characters.
    /// The value -1 is always present under index 0.
    /// </summary>
    internal static List<int> GetNewLines(string content)
    {
        var result = new List<int> {-1};

        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];
            if (c == '\n')
            {
                result.Add(i);
            }
        }

        return result;
    }

    /// <summary>
    /// Converts the location of the token to a character index in the text.
    /// </summary>
    internal static int ToTextPosition(IReadOnlyList<int> newLines, SqlLocation location)
    {
        if (location.LineNumber < 1 || location.LineNumber - 1 >= newLines.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(location.LineNumber));
        }

        return newLines[location.LineNumber - 1] + location.ColumnNumber;
    }
}
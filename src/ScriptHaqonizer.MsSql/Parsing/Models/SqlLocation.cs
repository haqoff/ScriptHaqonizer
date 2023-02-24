using System.Globalization;

namespace ScriptHaqonizer.MsSql.Parsing.Models;

/// <summary>
/// Represents a location in the text of the SQL script.
/// </summary>
internal class SqlLocation : IComparable<SqlLocation>
{
    /// <summary>Gets the line number.</summary>
    /// <return>The line number.</return>
    internal readonly int LineNumber;

    /// <summary>Gets the column number.</summary>
    /// <return>The column number.</return>
    internal readonly int ColumnNumber;

    /// <summary>
    /// Initializes a new instance of the Location class with the specified line number, column number and offset.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="columnNumber">The column number.</param>
    internal SqlLocation(int lineNumber, int columnNumber)
    {
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    /// <summary>
    /// Returns a string representation of this Location object.
    /// </summary>
    /// <return>A string representation of this Location object.</return>
    public override string ToString() => string.Format(CultureInfo.CurrentCulture, "({0},{1})", LineNumber, ColumnNumber);

    /// <summary>
    /// Compares this Location object with the specified Location object and returns an integer that indicates
    /// their relative positions to one another.
    /// </summary>
    /// <param name="other">The Location object to compare against.</param>
    /// <return>An integer that indicates the relative order of the objects being compared.
    /// 0 indicates that both objects have the same location. A negative number
    /// indicates that this instance precedes the specified Location object and a positive number indicates that
    /// this instance follows the specified Location object.</return>
    public int CompareTo(SqlLocation? other)
    {
        if (other is null)
        {
            return 1;
        }

        return LineNumber == other.LineNumber ? ColumnNumber - other.ColumnNumber : LineNumber - other.LineNumber;
    }
}
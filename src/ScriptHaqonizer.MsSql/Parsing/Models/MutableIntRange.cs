namespace ScriptHaqonizer.MsSql.Parsing.Models;

/// <summary>
/// Represents a range of integers.
/// </summary>
internal class MutableIntRange : IComparable<MutableIntRange>
{
    /// <summary>
    /// Initializes a new instance of the class <see cref="MutableIntRange"/>.
    /// </summary>
    /// <param name="start">Range start incl.</param>
    /// <param name="end">End of range, not including.</param>
    public MutableIntRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Range start incl.
    /// </summary>
    public int Start { get; private set; }

    /// <summary>
    /// End of range, not including.
    /// </summary>
    public int End { get; private set; }

    /// <summary>
    /// Range length.
    /// </summary>
    public int Length => End - Start;

    /// <summary>
    /// Gets an indication that the ranges intersect or border each other.
    /// </summary>
    public bool IntersectOrBorderTo(MutableIntRange other)
    {
        return Start <= other.End && End >= other.Start;
    }

    /// <summary>
    /// Merges the specified range into this one if they intersect or border each other.
    /// </summary>
    public void TryMerge(MutableIntRange other)
    {
        if (!IntersectOrBorderTo(other))
        {
            return;
        }

        var minStart = Math.Min(Start, other.Start);
        var maxEnd = Math.Max(End, other.End);

        Start = minStart;
        End = maxEnd;
    }

    /// <summary>
    /// Compares ranges relative to their start.
    /// </summary>
    public int CompareTo(MutableIntRange? other)
    {
        return Start.CompareTo(other?.Start);
    }
}
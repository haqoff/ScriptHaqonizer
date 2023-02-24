using Microsoft.SqlServer.TransactSql.ScriptDom;
using System.Text;

namespace ScriptHaqonizer.MsSql.Parsing.Models;

/// <summary>
/// Represents a tree node that stores a fragment, as well as parent and child nodes.
/// </summary>
internal record TraversableSqlNode<T>(T Fragment, TraversableSqlNode? Parent, IReadOnlyList<TraversableSqlNode> Children) where T : TSqlFragment
{
    /// <summary>
    /// Finds the first node of the specified type in the script.
    /// </summary>
    internal TraversableSqlNode<TSearch>? FindFirst<TSearch>() where TSearch : TSqlFragment
    {
        return EnumerateAll<TSearch>().FirstOrDefault();
    }

    /// <summary>
    /// Finds the first node whose fragment matches the specified <paramref name="fragment"/>.
    /// </summary>
    internal TraversableSqlNode<TSearch>? FindFirst<TSearch>(TSearch fragment) where TSearch : TSqlFragment
    {
        return EnumerateAll<TSearch>().FirstOrDefault(t => t.Fragment == fragment);
    }

    /// <summary>
    /// Gets an enumeration of all nodes, as well as all nodes in depth, starting from the current one.
    /// </summary>
    internal IEnumerable<TraversableSqlNode> EnumerateAll()
    {
        yield return Cast();
        foreach (var childNode in Children.SelectMany(child => child.EnumerateAll()))
        {
            yield return childNode;
        }
    }

    /// <summary>
    /// Gets an enumeration of all nodes, as well as all nodes down, starting from the current one, that have the specified fragment type.
    /// </summary>
    internal IEnumerable<TraversableSqlNode<TSearch>> EnumerateAll<TSearch>() where TSearch : TSqlFragment
    {
        if (Fragment is TSearch)
        {
            yield return Cast<TSearch>();
        }

        foreach (var childNode in Children.SelectMany(child => child.EnumerateAll<TSearch>()))
        {
            yield return childNode;
        }
    }

    /// <summary>
    /// Finds a fragment of the specified type among all parent nodes in the hierarchy above.
    /// </summary>
    internal TraversableSqlNode<TSearch>? FindInParentNodes<TSearch>() where TSearch : TSqlFragment
    {
        for (var current = Parent; current != null; current = current.Parent)
        {
            if (current.Fragment is TSearch)
            {
                return current.Cast<TSearch>();
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a sign that there is a fragment of the specified type among the parents in the hierarchy above.
    /// </summary>
    internal bool AnyInParentNodes<TSearch>() where TSearch : TSqlFragment
    {
        return FindInParentNodes<TSearch>() != null;
    }

    /// <summary>
    /// Converts the given node to a <see cref="TraversableSqlNode"/> node.
    /// </summary>
    internal TraversableSqlNode Cast()
    {
        return new TraversableSqlNode(Fragment, Parent, Children);
    }

    /// <summary>
    /// Attempts to convert the node to a node of the specified type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Exception if the conversion to a node of the specified type failed.</exception>
    internal TraversableSqlNode<TCast> Cast<TCast>() where TCast : TSqlFragment
    {
        if (Fragment is not TCast castFragment)
        {
            throw new InvalidOperationException($"Cannot cast {Fragment.GetType().Name} to {typeof(TCast).Name}");
        }

        return new TraversableSqlNode<TCast>(castFragment, Parent, Children);
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        FormatTo(sb, 0);
        return sb.ToString();
    }

    private void FormatTo(StringBuilder builder, int level)
    {
        builder.Append(' ', level * 2);
        builder.Append(Fragment.GetType().Name);
        builder.AppendLine();

        foreach (var node in Children)
        {
            node.FormatTo(builder, level + 1);
        }
    }
}

/// <summary>
/// Represents a tree node that stores a fragment, as well as parent and child nodes.
/// </summary>
internal record TraversableSqlNode(TSqlFragment Fragment, TraversableSqlNode? Parent, IReadOnlyList<TraversableSqlNode> Children)
    : TraversableSqlNode<TSqlFragment>(Fragment, Parent, Children);
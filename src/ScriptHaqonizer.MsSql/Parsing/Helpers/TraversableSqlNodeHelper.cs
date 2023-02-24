using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Models;
using System.Reflection;

namespace ScriptHaqonizer.MsSql.Parsing.Helpers;

/// <summary>
/// Provides helper methods for building an AST tree that can be traversed.
/// </summary>
internal static class TraversableSqlNodeHelper
{
    /// <summary>
    /// Creates a tree that can be navigated.
    /// </summary>
    internal static TraversableSqlNode<TSqlScript> Create(TSqlFragment fragment)
    {
        return Create(fragment, null).Cast<TSqlScript>();
    }

    private static TraversableSqlNode Create(TSqlFragment fragment, TraversableSqlNode? parent)
    {
        var fragmentBaseType = typeof(TSqlFragment);
        var enumerableFragments = typeof(IEnumerable<>).MakeGenericType(fragmentBaseType);
        var currentNodeProps = fragment.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var uniqueChildren = new HashSet<TSqlFragment>();
        var childrenList = new List<TraversableSqlNode>();
        var thisNode = new TraversableSqlNode(fragment, parent, childrenList);

        void AddChild(TSqlFragment sqlFragment)
        {
            if (!uniqueChildren.Add(sqlFragment))
            {
                return;
            }

            var node = Create(sqlFragment, thisNode);
            childrenList.Add(node);
        }

        foreach (var prop in currentNodeProps)
        {
            if (prop.PropertyType.IsAssignableTo(fragmentBaseType) && prop.GetIndexParameters().Length == 0)
            {
                var childFragment = (TSqlFragment?) prop.GetValue(fragment);
                if (childFragment is not null)
                {
                    AddChild(childFragment);
                }
            }
            else if (prop.PropertyType.IsAssignableTo(enumerableFragments))
            {
                var childrenFragments = (IEnumerable<TSqlFragment>?) prop.GetValue(fragment);
                if (childrenFragments is null)
                {
                    continue;
                }

                foreach (var v in childrenFragments)
                {
                    AddChild(v);
                }
            }
        }

        return thisNode;
    }
}
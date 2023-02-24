using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Extensions;

/// <summary>
/// Provides extension methods for the AST parsed tree node.
/// </summary>
internal static class SqlNodeExtensions
{
    /// <summary>
    /// Gets an enumeration of all existing database objects (tables, views, procedures, etc.) that are referenced. Excluding temporary objects.
    /// </summary>
    internal static IEnumerable<TraversableSqlNode<SchemaObjectName>> GetPhysicalDatabaseObjectIdentifiers<TR>(this TraversableSqlNode<TR> node) where TR : TSqlFragment
    {
        if (node.Fragment is SchemaObjectName && !node.AnyInParentNodes<DataTypeReference>())
        {
            yield return node.Cast<SchemaObjectName>();
        }
        else
        {
            foreach (var foundItem in node.Children.SelectMany(GetPhysicalDatabaseObjectIdentifiers))
            {
                yield return foundItem;
            }
        }
    }
}
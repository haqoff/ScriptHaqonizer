using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.MsSql.Parsing.Extensions;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Helpers;

/// <summary>
/// Provides helper methods for validating MSSQL script expressions.
/// </summary>
internal static class ValidationHelper
{
    /// <summary>
    /// Throws an error if the script contains database object names without specifying a database name.
    /// </summary>
    /// <exception cref="ScriptContentValidationException">The exception being thrown.</exception>
    internal static void ThrowIfDatabaseNameIsNotClear(int scriptId, TraversableSqlNode<TSqlScript> root)
    {
        var firstUseStatement = root.FindFirst<UseStatement>();
        var firstUseStatementLocation = firstUseStatement?.Fragment.GetStartLocation();

        var nodes = root.GetPhysicalDatabaseObjectIdentifiers();
        foreach (var node in nodes)
        {
            var name = node.Fragment;
            var nameLocation = name.GetStartLocation();

            if (name.DatabaseIdentifier?.Value is null && (firstUseStatementLocation is null || nameLocation.CompareTo(firstUseStatementLocation) < 0))
            {
                throw new ScriptContentValidationException(scriptId,
                    $"Specify the database to use for {name.BaseIdentifier.Value}. You can specify it explicitly, for example: MyDbServer.dbo.{name.BaseIdentifier.Value}, or write the USE command.");
            }
        }
    }
}
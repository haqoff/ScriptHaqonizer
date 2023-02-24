using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Models;

namespace ScriptHaqonizer.MsSql.Parsing.Helpers;

/// <summary>
/// Represents a helper class for looking up database names that are subject to change.
/// </summary>
internal static class MigrationSourceHelper
{
    /// <summary>
    /// Gets all statements that can make changes to the database and also gets the names of the databases.
    /// </summary>
    /// <param name="root">Root node.</param>
    /// <param name="defaultDbName">The name of the default database if the database is not explicitly mentioned in the script.</param>
    internal static List<StatementMigrationSource> GetStatementsThatMigrateDatabase<TR>(TraversableSqlNode<TR> root, string defaultDbName) where TR : TSqlFragment
    {
        var result = new List<StatementMigrationSource>();

        UseStatement? lastUseStatement = null;
        foreach (var statementNode in root.EnumerateAll<TSqlStatement>())
        {
            List<string>? migratedDatabases = null;

            void AddDbName(SchemaObjectName identifier)
            {
                var usedName = identifier.DatabaseIdentifier?.Value ?? lastUseStatement?.DatabaseName?.Value ?? defaultDbName;
                (migratedDatabases ??= new List<string>()).Add(usedName);
            }

            switch (statementNode.Fragment)
            {
                case UseStatement useStatement:
                    lastUseStatement = useStatement;
                    break;

                case SelectStatement:
                    break;

                case MergeStatement mergeStatement:
                    var targetFragment = mergeStatement.MergeSpecification.Target;
                    var targetNode = statementNode.FindFirst(targetFragment)!;
                    foreach (var targetNameNode in targetNode.EnumerateAll<SchemaObjectName>())
                    {
                        AddDbName(targetNameNode.Fragment);
                    }

                    break;

                default:
                    foreach (var nameNode in statementNode.EnumerateAll<SchemaObjectName>())
                    {
                        if (nameNode.AnyInParentNodes<SelectInsertSource>())
                        {
                            continue;
                        }

                        if (nameNode.AnyInParentNodes<DataTypeReference>())
                        {
                            continue;
                        }

                        if (nameNode.AnyInParentNodes<JoinParenthesisTableReference>())
                        {
                            continue;
                        }

                        if (nameNode.AnyInParentNodes<JoinTableReference>())
                        {
                            continue;
                        }

                        AddDbName(nameNode.Fragment);
                    }

                    break;
            }

            if (migratedDatabases is not null)
            {
                result.Add(new StatementMigrationSource(statementNode, migratedDatabases));
            }
        }

        return result;
    }
}
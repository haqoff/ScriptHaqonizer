using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace ScriptHaqonizer.MsSql.Parsing.Models;

/// <summary>
/// Contains information about the statement, as well as all the database names that this statement can change (for example, add something, remove something, and so on).
/// </summary>
/// <param name="ReasonStatementNode">Statement node.</param>
/// <param name="DatabaseNames">Database names.</param>
internal record StatementMigrationSource(TraversableSqlNode<TSqlStatement> ReasonStatementNode, List<string> DatabaseNames);
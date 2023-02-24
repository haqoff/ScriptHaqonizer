using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;

namespace ScriptHaqonizer.Playground;

internal static class MicrosoftSqlServerManagementSqlParser
{
    public static void Test_SelectStatement()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
    SELECT * FROM Sex
");

        foreach (var s in parser.Script.Batches[0].Statements)
        {
            var sqlSelect = (SqlSelectStatement) s;
            var specs = (SqlQuerySpecification) sqlSelect.SelectSpecification.QueryExpression;
            var f = specs.FromClause;
            var t = f.TableExpressions;
            var reft = (SqlTableRefExpression) t[0];
            var o = reft.ObjectIdentifier;
            if (o.DatabaseName.Value is null)
            {
                Console.WriteLine(o.DatabaseName.Value);
            }

            Console.WriteLine(o.Xml);
        }
    }

    public static void Test_SqlDmlStatement()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
    INSERT INTO Sex(ID) Values (1);
");

        foreach (var s in parser.Script.Batches[0].Statements)
        {
            var sqlSelect = (SqlInsertStatement) s;
            var specs = (SqlTableRefExpression) sqlSelect.InsertSpecification.Target;
            var obj = specs.ObjectIdentifier;
            if (obj.DatabaseName is null)
            {
                Console.WriteLine("insert db null");
            }

            var abs = (SqlDmlStatement) s;
        }
    }

    public static void Test_SqlDdlStatement()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
CREATE TABLE NICEDB.dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
");

        foreach (var s in parser.Script.Batches[0].Statements)
        {
            var stat = (SqlDdlStatement) s;
            Console.WriteLine(s);
        }
    }

    public static void Test_CreateDatabase()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
CREATE DATABASE MyDb
");

        foreach (var s in parser.Script.Batches[0].Statements)
        {
            Console.WriteLine(s);
        }
    }


    public static void TestExtractTableRef()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
CREATE TABLE dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
GO

INSERT INTO InsertTable(Col1)
Values('1');
GO

CREATE LOGIN MyLogin FROM WINDOWS;
GO
");
        var firstUseStatement = FindFirstStatement<SqlUseStatement>(parser.Script);
        var pos = firstUseStatement?.StartLocation;

        var refs = Traverse(parser.Script.Children).ToArray();
        foreach (var identifier in refs)
        {
            if (identifier.DatabaseName.Value is null && (pos is null || identifier.StartLocation.CompareTo(pos.Value) < 0))
            {
                Console.WriteLine(
                    $"Specify the database to use for {identifier.ObjectName}. You can specify it explicitly, for example: MyDbServer.dbo.{identifier.ObjectName}, or write the USE command.");
            }
        }

        Console.WriteLine(refs);


        static T? FindFirstStatement<T>(SqlScript script) where T : SqlStatement
        {
            foreach (var batch in script.Batches)
            {
                foreach (var statement in batch.Statements)
                {
                    if (statement is T typedStatement)
                    {
                        return typedStatement;
                    }
                }
            }

            return null;
        }
    }

    public static IEnumerable<SqlObjectIdentifier> Traverse(IEnumerable<SqlCodeObject> objects)
    {
        foreach (var codeObject in objects)
        {
            if (codeObject is SqlObjectIdentifier {Parent: null or not SqlDataType and not SqlColumnRefExpression} expression)
            {
                yield return expression;
            }
            else
            {
                foreach (var inner in Traverse(codeObject.Children))
                {
                    yield return inner;
                }
            }
        }
    }

    public static void TestTruncateTable()
    {
        SqlNullStatement? s = null;
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
TRUNCATE TABLE MyTable
");
    }


    public static void Test_ExtractChangedDatabases()
    {
        var parser = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(@"
CREATE TABLE dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
GO

USE InsertDb;
GO;

INSERT INTO InsertTable(Col1)
Values('1');
GO

DELETE FROM DeleteDb.dbo.DeleteTable WHERE 1=0
GO

UPDATE UpdateDb.dbo.UpdateTable Set Col1 = '1' WHERE 1=1
GO

INSERT INTO InsertSecondDb.dbo.NiceTable
SELECT * FROM SelectDb.dbo.SelectTable
");

        const string defaultDbName = "DefaultDb";
        var changedDbNames = new List<FoundStatement>();


        SqlUseStatement? lastUseStatement = null;
        foreach (var batch in parser.Script.Batches)
        {
            foreach (var statement in batch.Statements)
            {
                var usedNames = new List<string>();
                switch (statement)
                {
                    case SqlUseStatement useStatement:
                        lastUseStatement = useStatement;
                        break;
                    case SqlDmlStatement:
                    case SqlDdlStatement:
                        var identifiers = Traverse(statement.Children).ToArray();
                        foreach (var identifier in identifiers)
                        {
                            var usedName = identifier.DatabaseName.Value ?? lastUseStatement?.DatabaseName.Value ?? defaultDbName;
                            usedNames.Add(usedName);
                        }

                        break;
                }

                if (usedNames.Count > 0)
                {
                    changedDbNames.Add(new FoundStatement(statement, usedNames));
                }
            }
        }

        Console.WriteLine(string.Join(", ", changedDbNames));
    }


    public record FoundStatement(SqlStatement Statement, List<string> ChangedDbNames);
}
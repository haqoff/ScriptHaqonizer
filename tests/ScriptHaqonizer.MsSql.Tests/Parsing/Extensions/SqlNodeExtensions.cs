using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Extensions;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql.Tests.Parsing.Extensions;

public class SqlNodeExtensions
{
    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_ShouldNotConsiderTempTablesOnlyPhysicalOnes()
    {
        const string sql = @"
DECLARE @TempTable TABLE
(
    Code NVARCHAR(MAX)
)

INSERT INTO @TempTable
VALUES ('1');
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var tempTableInsertIdentifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Empty(tempTableInsertIdentifiers);
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_ShouldNotConsiderDataTypes()
    {
        const string sql = @"
CREATE TABLE dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "__MigrationScripts");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_ShouldNotConsiderColumnRefs()
    {
        const string sql = @"
INSERT INTO InsertTable(Col1)
Values('1');
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "InsertTable");
    }


    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_SelectWithSubQuery()
    {
        const string sql = @"
SELECT * FROM
(
    SELECT * FROM MyTable m
    INNER JOIN OtherTable o ON o.ID = m.ID
) t
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Equal(2, identifiers.Length);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "MyTable");
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "OtherTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_Merge()
    {
        const string sql = @"
MERGE MergeTargetDb.dbo.TargetTable T
USING MergeSourceDb.dbo.SourceTable S ON T.LocationID=S.LocationID
WHEN MATCHED THEN
UPDATE SET LocationName=S.LocationName;
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Equal(2, identifiers.Length);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "TargetTable");
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "SourceTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_Delete()
    {
        const string sql = @"
DELETE FROM DeleteDb.dbo.DeleteTable
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "DeleteTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_Insert()
    {
        const string sql = @"
INSERT INTO MyDb.dbo.MyTable
VALUES ('1');
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "MyTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_UpdateSimple()
    {
        const string sql = @"
UPDATE UpdateDb.dbo.UpdateTable Set Col1 = '1' WHERE 1=1
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "UpdateTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_UpdateWithJoin()
    {
        const string sql = @"
UPDATE UpdateDb.dbo.UpdateTable
SET assid = s.assid
FROM UpdateDb.dbo.UpdateTable u
JOIN OtherDb.dbo.OtherTable s ON u.id=s.id
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        // UpdateTable meet twice
        Assert.Equal(3, identifiers.Length);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "UpdateTable");
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "OtherTable");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_UseStatement_ShouldBeEmpty()
    {
        const string sql = @"
USE [MyDb]
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Empty(identifiers);
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_DropView()
    {
        const string sql = @"
DROP VIEW MyView
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "MyView");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_StoredProcedure()
    {
        const string sql = @"
EXEC @ReturnCode = msdb.dbo.sp_update_job 1, @start_step_id = 1
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "sp_update_job");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_TruncateTable()
    {
        const string sql = @"
TRUNCATE TABLE Categories;
";
        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Single(identifiers);
        Assert.Contains(identifiers, i => i.Fragment.BaseIdentifier.Value == "Categories");
    }

    [Fact]
    public void Test_GetPhysicalDatabaseObjectIdentifiers_CreateDatabase()
    {
        const string sql = @"
CREATE DATABASE MyDb
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);

        var identifiers = node.GetPhysicalDatabaseObjectIdentifiers().ToArray();
        Assert.Empty(identifiers);
    }
}
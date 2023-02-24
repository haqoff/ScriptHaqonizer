using FluentAssertions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql.Tests.Parsing.Helpers;

public class MigrationSourceHelperTests
{
    [Fact]
    public void Test_CreateTableStatementWithNoDbNameSpecified()
    {
        var sql = @"
CREATE TABLE dbo.[__MigrationScripts] (
    [Id] int NOT NULL,
    [ScriptName] varchar(50) NOT NULL,
    [AppliedAtUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY ([Id])
);
GO
";
        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);

        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo(defaultDbName);
    }

    [Fact]
    public void Test_InsertStatementWithValuesWithUseCommandBefore()
    {
        var sql = @"
USE InsertDb;
GO;

INSERT INTO InsertTable(Col1)
Values('1');
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);

        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("InsertDb");
    }

    [Fact]
    public void Test_InsertStatementWithSelect_DatabaseSpecified()
    {
        var sql = @"
INSERT INTO InsertSecondDb.dbo.NiceTable
SELECT * FROM SelectDb.dbo.SelectTable
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("InsertSecondDb");
    }

    [Fact]
    public void Test_DeleteSimpleStatement_DatabaseSpecified()
    {
        var sql = @"
DELETE FROM DeleteDb.dbo.DeleteTable WHERE 1=0
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";

        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("DeleteDb");
    }

    [Fact]
    public void Test_UpdateSimpleStatement_DatabaseSpecified()
    {
        var sql = @"
UPDATE UpdateDb.dbo.UpdateTable Set Col1 = '1' WHERE 1=1
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        sources[0].DatabaseNames.Should().BeEquivalentTo("UpdateDb");
    }

    [Fact]
    public void Test_UpdateWithJoinStatement_DatabaseSpecified()
    {
        var sql = @"
UPDATE UpdateDb.dbo.UpdateTable
SET assid = s.assid
FROM UpdateDb.dbo.UpdateTable u
JOIN OtherDb.dbo.OtherTable s ON u.id=s.id
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("UpdateDb");
    }

    [Fact]
    public void Test_MergeSimpleStatement_DatabaseSpecified()
    {
        var sql = @"
MERGE MergeTargetDb.dbo.TargetTable T
USING MergeSourceDb.dbo.SourceTable S ON T.LocationID=S.LocationID
WHEN MATCHED THEN
UPDATE SET LocationName=S.LocationName;
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("MergeTargetDb");
    }

    [Fact]
    public void Test_TruncateTable()
    {
        var sql = @"
TRUNCATE TABLE MyTable
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var result = parser.Parse(new StringReader(sql), out var errors);
        Assert.Empty(errors);
        var node = TraversableSqlNodeHelper.Create(result);
        const string defaultDbName = "asd";
        var sources = MigrationSourceHelper.GetStatementsThatMigrateDatabase(node, defaultDbName);
        Assert.Single(sources);
        sources[0].DatabaseNames.Should().BeEquivalentTo("asd");
    }
}
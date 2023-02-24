using FluentAssertions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using ScriptHaqonizer.MsSql.Parsing.Helpers;

namespace ScriptHaqonizer.MsSql.Tests.Parsing.Helpers;

public class SqlModifierHelperTests
{
    [Fact]
    public void Test_RemoveFragments()
    {
        const string sourceSql = @"
SELECT * FROM MyTable AS My
JOIN (SELECT * FROM OtherTable) As Other ON My.Id = Other.Id
GO

UPDATE UpdateTable
SET MyCol = '1'
WHERE 1 <> 1
GO

INSERT INTO InsertTable
VALUES ('1')
GO

SELECT * FROM MyOtherTable
GO
";

        var parser = new TSql160Parser(true, SqlEngineType.All);
        var root = (TSqlScript) parser.Parse(new StringReader(sourceSql), out var errors);
        Assert.Empty(errors);

        var removingFragments = new TSqlFragment[] {root.Batches[0], root.Batches[1]};
        var newLines = PositionHelper.GetNewLines(sourceSql);

        var actualTargetSql = SqlModifierHelper.RemoveFragments(sourceSql, removingFragments, newLines);
        actualTargetSql.Should().Be(@"

GO


GO

INSERT INTO InsertTable
VALUES ('1')
GO

SELECT * FROM MyOtherTable
GO
");
    }
}
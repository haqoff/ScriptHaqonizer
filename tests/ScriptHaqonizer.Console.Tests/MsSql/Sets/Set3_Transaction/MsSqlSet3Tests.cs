using FluentAssertions;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Console.Tests.MsSql.Helpers;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Sets.Set3_Transaction;

public class MsSqlSet4Tests : MsSqlTestBase
{
    public MsSqlSet4Tests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void ShouldMigrateSuccessfullyAndShowWarningAboutTransaction_WhenTransactionStatementInsideScript()
    {
        CreateDatabase("NICEDB");
        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d NICEDB -s {ScriptDirPath.Escape()} -e Development -m Full -t MsSql --from Test");

        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
        result.StandardOutput.Should().Contain("TRANSACTION statements found in SQL script");
        GetExecutedScripts("NICEDB").Should().Contain(s => s.Id == 1);
    }
}
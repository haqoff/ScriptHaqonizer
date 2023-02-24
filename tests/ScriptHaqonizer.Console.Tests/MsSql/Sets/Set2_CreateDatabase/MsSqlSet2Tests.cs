using FluentAssertions;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Console.Tests.MsSql.Helpers;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Sets.Set2_CreateDatabase;

public class MsSqlSet2Tests : MsSqlTestBase
{
    public MsSqlSet2Tests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void ShouldCreateDatabaseSuccessfully_WhenNoDatabasesAndFullMode()
    {
        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d NICEDB -s {ScriptDirPath.Escape()} -e Development -m Full -t MsSql --from Test");

        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
        GetExecutedScripts("NICEDB").Should().Contain(s => s.Id == 1);
        GetExecutedScripts("NICEDB").Should().Contain(s => s.Id == 2);
    }
}
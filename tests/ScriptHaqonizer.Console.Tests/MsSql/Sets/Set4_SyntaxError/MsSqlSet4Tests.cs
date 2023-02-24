using FluentAssertions;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Console.Tests.MsSql.Helpers;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Sets.Set4_SyntaxError;

public class MsSqlSet5Tests : MsSqlTestBase
{
    public MsSqlSet5Tests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void ShouldLogError_WhenSyntaxError()
    {
        CreateDatabase("NICEDB");
        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d NICEDB -s {ScriptDirPath.Escape()} -e Development -m Full -t MsSql --from Test");
        result.ErrorOutput.Should().Contain("DROP does not allow a database name to be specified.");
        Assert.Equal(ExitConstants.MigrationFailed, result.ExitCode);
    }
}
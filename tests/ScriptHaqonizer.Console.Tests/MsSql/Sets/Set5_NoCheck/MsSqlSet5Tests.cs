using FluentAssertions;
using ScriptHaqonizer.Console.Tests.Base;
using ScriptHaqonizer.Console.Tests.MsSql.Helpers;
using Xunit.Abstractions;

namespace ScriptHaqonizer.Console.Tests.MsSql.Sets.Set5_NoCheck;

public class MsSqlSet5Tests : MsSqlTestBase
{
    public MsSqlSet5Tests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void ShouldSkipChecking_WhenNoCheckParameterSpecified()
    {
        CreateDatabase("NICEDB");
        var result = ExecuteConsole(@$"-c {ConnectionString.Escape()} -d NICEDB -s {ScriptDirPath.Escape()} -e Development -m OnlyCheck -t MsSql --from Test");
        result.StandardOutput.Should().Contain("This script cannot be run in OnlyCheck mode and will be skipped.");
        Assert.Equal(ExitConstants.MigrationSuccess, result.ExitCode);
    }
}
using FluentAssertions;
using Moq;
using ScriptHaqonizer.Core.Calculators;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Parsers;

namespace ScriptHaqonizer.Core.Tests.Calculators;

public class NewScriptCalculatorTest
{
    [Fact]
    public void ShouldReturnUnappliedScriptsSuitableForDevelopmentEnvironment()
    {
        var context = CreateTestContext(AvailableEnvironment.Development);
        var contentMock = new Mock<IParsedScriptContent>();
        var alreadyExecuted = new ExecutedScript[]
        {
            new(1, "FirstScript", DateTime.Now),
            new(2, "SecondScript", DateTime.Now)
        };
        var allScripts = new Script[]
        {
            new(1, "FirstScript", ScriptEnvironmentContainer.All, "1_FirstScript", contentMock.Object, false, false, false),
            new(2, "SecondScript", ScriptEnvironmentContainer.All, "2_SecondScript", contentMock.Object, false, false, false),
            new(3, "ThirdScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Production}, EntryType.Included), "3_ThirdScript_Production", contentMock.Object, false, false, false),
            new(4, "FourScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development}, EntryType.Included), "3_FourScript_Development", contentMock.Object, false, false, false),
        };

        var actualNewScripts = context.ScriptCalculator.GetNotAppliedSortedScripts(alreadyExecuted, allScripts);
        actualNewScripts.Should().ContainSingle(s => s.Id == 4);
    }

    [Fact]
    public void ShouldThrowExceptionWhenNewScriptIdEqualOrLessToLastApplied()
    {
        var context = CreateTestContext(AvailableEnvironment.Development);
        var contentMock = new Mock<IParsedScriptContent>();
        var alreadyExecuted = new ExecutedScript[]
        {
            new(1, "FirstScript", DateTime.Now),
            new(5, "SecondScript", DateTime.Now)
        };
        var allScripts = new Script[]
        {
            new(1, "FirstScript", ScriptEnvironmentContainer.All, "1_FirstScript", contentMock.Object, false, false, false),
            new(5, "SecondScript", ScriptEnvironmentContainer.All, "2_SecondScript", contentMock.Object, false, false, false),
            new(3, "ThirdScript", ScriptEnvironmentContainer.All, "3_ThirdScript", contentMock.Object, false, false, false),
        };

        var ex = Assert.Throws<ScriptListValidationException>(() => context.ScriptCalculator.GetNotAppliedSortedScripts(alreadyExecuted, allScripts));
        ex.ScriptId.Should().Be(3);
    }


    private static TestContext CreateTestContext(AvailableEnvironment currentEnvironment)
    {
        var scriptCalculator = new NewScriptCalculator(currentEnvironment);
        return new TestContext(currentEnvironment, scriptCalculator);
    }

    private record TestContext
    (
        AvailableEnvironment Environment,
        NewScriptCalculator ScriptCalculator
    );
}
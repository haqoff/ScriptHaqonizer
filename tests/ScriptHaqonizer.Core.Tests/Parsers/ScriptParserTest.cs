using FakeItEasy;
using FluentAssertions;
using ScriptHaqonizer.Core.Exceptions;
using ScriptHaqonizer.Core.Models;
using ScriptHaqonizer.Core.Parsers;

namespace ScriptHaqonizer.Core.Tests.Parsers;

public class ScriptParserTest
{
    private record ContentMock : IParsedScriptContent;

    private static readonly IParsedScriptContent Content = new ContentMock();

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void ShouldParseCorrect(TestCase testCase)
    {
        var contentLoader = A.Fake<IScriptContentLoader>();
        var contentParser = A.Fake<IScriptContentParser>();
        A.CallTo(() => contentLoader.Load(A<string>._)).Returns("");
        A.CallTo(() => contentParser.ParseAndThrowIfNotValid(A<int>._, A<string>._, A<bool>._)).Returns(Content);

        switch (testCase)
        {
            case SuccessTestCase successTestCase:
            {
                var actual = new ScriptParser(contentLoader, contentParser).Parse(successTestCase.Path);
                actual.Should().BeEquivalentTo(successTestCase.Expected);
                break;
            }
            case ExceptionTestCase exceptionTestCase:
            {
                var ex = Assert.Throws(exceptionTestCase.ExceptionType, () => new ScriptParser(contentLoader, contentParser).Parse(exceptionTestCase.Path));
                ex.Message.Should().Contain(exceptionTestCase.ExceptionMessagePart);
                break;
            }
            default:
            {
                Assert.Fail("Unknown test case type.");
                break;
            }
        }
    }

    public static object[][] GetTestCases()
    {
        var testCases = new TestCase[]
        {
            new SuccessTestCase
            (
                "directory/1_NiceScript.sql",
                new Script(1, "NiceScript", ScriptEnvironmentContainer.All, "1_NiceScript", Content, false, false, false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_development.sql",
                new Script(1, "NiceScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development}, EntryType.Included), "1_NiceScript_development", Content, false, false,
                    false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_development&local.sql",
                new Script(1, "NiceScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development, AvailableEnvironment.Local}, EntryType.Included), "1_NiceScript_development&local",
                    Content, false, false, false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_!development&local.sql",
                new Script(1, "NiceScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development, AvailableEnvironment.Local}, EntryType.Excluded), "1_NiceScript_!development&local",
                    Content, false, false, false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_development_nocheck.sql",
                new Script(1, "NiceScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development}, EntryType.Included), "1_NiceScript_development_nocheck", Content, true, false,
                    false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_development_nocheck&notransaction&nobackup.sql",
                new Script(1, "NiceScript", new ScriptEnvironmentContainer(new[] {AvailableEnvironment.Development}, EntryType.Included), "1_NiceScript_development_nocheck&notransaction&nobackup",
                    Content, true, true, true)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_nocheck.sql",
                new Script(1, "NiceScript", ScriptEnvironmentContainer.All, "1_NiceScript_nocheck", Content, true, false, false)
            ),
            new SuccessTestCase
            (
                "directory/1_NiceScript_nocheck&notransaction.sql",
                new Script(1, "NiceScript", ScriptEnvironmentContainer.All, "1_NiceScript_nocheck&notransaction", Content, true, true, false)
            ),
            new ExceptionTestCase
            (
                "directory/0_NiceScript_nocheck.sql",
                typeof(ScriptNameValidationException),
                "Script ID must be greater than 0."
            )
        };

        return testCases.Select(c => new object[] {c}).ToArray();
    }

    public abstract record TestCase(string Path);

    public record SuccessTestCase(string Path, Script Expected) : TestCase(Path);

    public record ExceptionTestCase(string Path, Type ExceptionType, string ExceptionMessagePart) : TestCase(Path);
}
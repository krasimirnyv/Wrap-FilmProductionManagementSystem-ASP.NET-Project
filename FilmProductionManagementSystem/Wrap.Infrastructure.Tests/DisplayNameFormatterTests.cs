namespace Wrap.Infrastructure.Tests;

using NUnit.Framework;

using GCommon.Text;
using GCommon.Enums;

[TestFixture]
public class DisplayNameFormatterTests
{
    [TestCase("", "")]
    [TestCase("   ", "")]
    [TestCase("DirectorOfPhotography", "Director Of Photography")]
    [TestCase("FirstAssistantDirector", "First Assistant Director")]
    [TestCase("ADRSupervisor", "ADR Supervisor")]
    [TestCase("Version2Alpha", "Version 2 Alpha")]
    [TestCase("Already Split", "Already Split")]
    public void ToDisplayName_String_FormatsAsExpected(string input, string expected)
    {
        string result = DisplayNameFormatter.ToDisplayName(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(EnumCases))]
    public void ToDisplayName_Enum_FormatsAsExpected(Enum input, string expected)
    {
        string result = DisplayNameFormatter.ToDisplayName(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    private static object[] EnumCases =>
    [
        new object[]
        {
            CrewRoleType.DirectorOfPhotography,
            "Director Of Photography"
        },
        new object[]
        {
            CrewRoleType.FirstAssistantDirector,
            "First Assistant Director"
        }
    ];
}
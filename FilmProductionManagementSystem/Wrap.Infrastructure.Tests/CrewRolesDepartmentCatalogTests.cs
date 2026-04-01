namespace Wrap.Infrastructure.Tests;

using NUnit.Framework;

using GCommon.Enums;
using GCommon.UI;

using static GCommon.ApplicationConstants;

[TestFixture]
public class CrewRolesDepartmentCatalogTests
{
    [Test]
    public void GetRolesByDepartment_ReturnsAllExpectedKeys()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> map =
            CrewRolesDepartmentCatalog.GetRolesByDepartment();
        
        Assert.That(map.ContainsKey(DirectionAndProduction), Is.True);
        Assert.That(map.ContainsKey(WritingAndDevelopment), Is.True);
        Assert.That(map.ContainsKey(CameraDepartment), Is.True);
        Assert.That(map.ContainsKey(LightingDepartment), Is.True);
        Assert.That(map.ContainsKey(GripDepartment), Is.True);
        Assert.That(map.ContainsKey(ArtDepartment), Is.True);
        Assert.That(map.ContainsKey(CostumeAndMakeup), Is.True);
        Assert.That(map.ContainsKey(SoundDepartment), Is.True);
        Assert.That(map.ContainsKey(PostProductionDepartment), Is.True);
        Assert.That(map.ContainsKey(MusicDepartment), Is.True);
        Assert.That(map.ContainsKey(LocationsDepartment), Is.True);
        Assert.That(map.ContainsKey(LogisticsAndTransportationDepartment), Is.True);
        Assert.That(map.ContainsKey(SetOperationsAndSupportDepartment), Is.True);
        Assert.That(map.ContainsKey(OtherDepartment), Is.True);
        
        foreach (KeyValuePair<string, IReadOnlyCollection<CrewRoleType>> kvp in map)
        {
            Assert.That(kvp.Value, Is.Not.Empty);
        }
    }

    [Test]
    public void GetRolesByDepartment_CoversEveryEnumValueExactlyOnce()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> map =
            CrewRolesDepartmentCatalog.GetRolesByDepartment();

        CrewRoleType[] allFromMap = map.SelectMany(kvp => kvp.Value).ToArray();
        CrewRoleType[] allEnumValues = Enum.GetValues<CrewRoleType>();

        // missing?
        Assert.That(allEnumValues, Is.SubsetOf(allFromMap));

        // duplicates across departments?
        Assert.That(allFromMap.Length, Is.EqualTo(allFromMap.Distinct().Count()),
            "Some CrewRoleType values appear in more than one department.");

        // extra? (symmetry)
        Assert.That(allFromMap, Is.SubsetOf(allEnumValues));
    }

    [Test]
    public void GetRolesByDepartment_OtherDepartment_ContainsOnlyOther()
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> map =
            CrewRolesDepartmentCatalog.GetRolesByDepartment();

        Assert.That(map[OtherDepartment], Is.EquivalentTo(new[] { CrewRoleType.Other }));
    }

    [TestCase(CrewRoleType.FirstAssistantDirector, "First Assistant Director")]
    [TestCase(CrewRoleType.DirectorOfPhotography, "Director Of Photography")]
    [TestCase(CrewRoleType.PostProductionSupervisor, "Post Production Supervisor")]
    public void GetDisplayName_FormatsRoleName(CrewRoleType role, string expected)
    {
        string result = CrewRolesDepartmentCatalog.GetDisplayName(role);

        Assert.That(result, Is.EqualTo(expected));
    }
}
namespace Wrap.Services.Models.Profile;

using GCommon.Enums;

public class EditSkillsDto
{
    public IReadOnlyCollection<CrewRoleType> CurrentSkills { get; set; } 
        = new HashSet<CrewRoleType>();

    public IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> AllDepartments { get; set; }
        = new Dictionary<string, IReadOnlyCollection<CrewRoleType>>();
}
namespace Wrap.Services.Models.LoginAndRegistration;

using GCommon.Enums;

public class CrewRegistrationStepTwoDto
{
    public IReadOnlyCollection<int> SelectedSkills { get; set; } =
        new List<int>();
    
    public IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> SkillsByDepartment { get; set; }
        = new Dictionary<string, IReadOnlyCollection<CrewRoleType>>();
}
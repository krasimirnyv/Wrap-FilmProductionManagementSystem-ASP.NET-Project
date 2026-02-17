namespace Wrap.ViewModels.Profile;

using HelperViewModels;

using GCommon.Enums;

public class CrewProfileViewModel
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfileImagePath { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Biography { get; set; }

    /// <summary>
    /// Dictionary of departments with user's skills in each
    /// Key: Department name (e.g., "Direction & Production")
    /// Value: List of skills user has in that department
    /// </summary>
    public IDictionary<string, ICollection<CrewRoleType>> DepartmentSkills { get; set; }
        = new Dictionary<string, ICollection<CrewRoleType>>();

    public int SkillsCount
        => DepartmentSkills?.Sum(d => d.Value.Count) ?? 0;

    public ICollection<CrewMemberProduction>? CrewMemberProductions { get; set; }
        = new List<CrewMemberProduction>();

    public int CrewMemberProductionsCount 
        => CrewMemberProductions?.Count ?? 0;

    public ICollection<CrewMemberScene>? CrewMemberScenes { get; set; }
        = new List<CrewMemberScene>();
    
    public int CrewMemberScenesCount
        => CrewMemberScenes?.Count ?? 0;
}
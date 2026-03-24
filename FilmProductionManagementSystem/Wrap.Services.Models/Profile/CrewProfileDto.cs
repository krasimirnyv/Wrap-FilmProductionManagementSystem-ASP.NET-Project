namespace Wrap.Services.Models.Profile;

using NestedDtos;
using GCommon.Enums;
using CommonProperties;

public class CrewProfileDto : CommonUserInfo
{
    public string ProfileImagePath { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public bool IsActive { get; set; }
    
    public IDictionary<string, ICollection<CrewRoleType>> DepartmentSkills { get; set; }
        = new Dictionary<string, ICollection<CrewRoleType>>();
    
    public IReadOnlyCollection<CrewMemberProductionDto> Productions { get; set; } = [];
    
    public IReadOnlyCollection<CrewMemberSceneDto> Scenes { get; set; } = [];
}
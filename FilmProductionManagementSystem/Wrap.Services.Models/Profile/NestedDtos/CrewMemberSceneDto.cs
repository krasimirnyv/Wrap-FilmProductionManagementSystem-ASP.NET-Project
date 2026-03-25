namespace Wrap.Services.Models.Profile.NestedDtos;

using GCommon.Enums;

public class CrewMemberSceneDto
{
    public string SceneId { get; set; } = null!;

    public string SceneName { get; set; } = null!;

    public string ProductionTitle { get; set; } = null!;
    
    public CrewRoleType RoleType { get; set; }
}
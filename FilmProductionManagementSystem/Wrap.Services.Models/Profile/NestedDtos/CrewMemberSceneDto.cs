namespace Wrap.Services.Models.Profile.NestedDtos;

using GCommon.Enums;

public class CrewMemberSceneDto
{
    public string? SceneId { get; set; }

    public string? SceneName { get; set; }

    public string? ProductionTitle { get; set; }
    
    public CrewRoleType? RoleType { get; set; }
}
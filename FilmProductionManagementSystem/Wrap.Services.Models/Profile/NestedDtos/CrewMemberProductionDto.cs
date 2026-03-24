namespace Wrap.Services.Models.Profile.NestedDtos;

using GCommon.Enums;

public class CrewMemberProductionDto
{
    public string? ProductionId { get; set; }
    
    public string? ProductionTitle { get; set; }
    
    public CrewRoleType? RoleType { get; set; }
    
    public string? ProjectStatus { get; set; }
}
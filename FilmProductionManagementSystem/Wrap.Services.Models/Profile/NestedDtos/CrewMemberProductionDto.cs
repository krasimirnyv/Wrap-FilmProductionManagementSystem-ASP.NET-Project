namespace Wrap.Services.Models.Profile.NestedDtos;

using GCommon.Enums;

public class CrewMemberProductionDto
{
    public string ProductionId { get; set; } = null!;
    
    public string ProductionTitle { get; set; } = null!;
    
    public CrewRoleType RoleType { get; set; }
    
    public string ProjectStatus { get; set; } = null!;
}
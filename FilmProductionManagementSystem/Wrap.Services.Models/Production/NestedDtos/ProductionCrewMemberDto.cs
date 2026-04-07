namespace Wrap.Services.Models.Production.NestedDtos;

using GCommon.Enums;

public class ProductionCrewMemberDto
{
    public string Id { get; set; } = null!;
    
    public string ProfileImagePath { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public CrewRoleType Role { get; set; }
}
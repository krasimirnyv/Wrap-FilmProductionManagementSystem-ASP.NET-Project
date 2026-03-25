namespace Wrap.Services.Models.Production.NestedDtos;

using GCommon.Enums;

public class ProductionCastMemberDto
{
    public string ProfileImagePath { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;

    public string Role { get; set; } = null!;
    
    public byte Age { get; set; }
    
    public GenderType Gender { get; set; }
}
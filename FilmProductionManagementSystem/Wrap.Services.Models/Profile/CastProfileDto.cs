namespace Wrap.Services.Models.Profile;

using NestedDtos;
using CommonProperties;

public class CastProfileDto : CommonUserInfo
{
    public string ProfileImagePath { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public string Age { get; set; } = null!;

    public string Gender { get; set; } = null!;
    
    public bool IsActive { get; set; }
    
    public IReadOnlyCollection<CastMemberProductionDto> Productions { get; set; } = [];

    public IReadOnlyCollection<CastMemberSceneDto> Scenes { get; set; } = [];
}
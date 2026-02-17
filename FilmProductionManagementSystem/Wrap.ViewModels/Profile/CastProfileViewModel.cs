namespace Wrap.ViewModels.Profile;

using HelperViewModels;

public class CastProfileViewModel
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfileImagePath { get; set; } = null!;

    public string Nickname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    
    public string Age { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string? Role { get; set; }

    public bool IsActive { get; set; }

    public string? Biography { get; set; }

    public IEnumerable<CastMemberProduction>? CastMemberProductions { get; set; }
        = new HashSet<CastMemberProduction>();

    public int CastMemberProductionsCount
        => CastMemberProductions?.Count() ?? 0;
    
    public IEnumerable<CastMemberScene>? CastMemberScenes { get; set; }
        = new HashSet<CastMemberScene>();

    public int CastMemberScenesCount
        => CastMemberScenes?.Count() ?? 0;
}
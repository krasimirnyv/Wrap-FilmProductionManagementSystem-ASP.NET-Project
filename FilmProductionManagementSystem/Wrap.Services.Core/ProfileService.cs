namespace Wrap.Services.Core;

using Interfaces;
using Data.Models;
using Data.Repository.Interfaces;
using Models.Profile;

public class ProfileService(IProfileRepository profileRepository) : IProfileService
{
    public async Task<bool> IsUserCrewAsync(string username)
    {
        Crew? crewMembers = await profileRepository.GetCrewByUsernameAsync(username);
        return crewMembers is not null;
    }
    
    public async Task<bool> IsUserCastAsync(string username)
    {
        Cast? castMember = await profileRepository.GetCastByUsernameAsync(username);
        return castMember is not null;
    }

    public async Task<ProfileRoleDto> GetRoleInfoAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsync(username);
        if (crew is not null)
            return new ProfileRoleDto { IsCrew = true, IsCast = false };

        Cast? cast = await profileRepository.GetCastByUsernameAsync(username);
        if (cast is not null)
            return new ProfileRoleDto { IsCrew = false, IsCast = true };
        
        return new ProfileRoleDto { IsCrew = false, IsCast = false };
    }
}
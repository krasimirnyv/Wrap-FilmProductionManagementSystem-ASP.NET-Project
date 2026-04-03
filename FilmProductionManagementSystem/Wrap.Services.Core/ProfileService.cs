namespace Wrap.Services.Core;

using Interfaces;
using Data.Models;
using Data.Repository.Interfaces;
using Models.Profile;

public class ProfileService(IProfileRepository profileRepository) : IProfileService
{
    public async Task<bool> IsUserCrewAsync(string username)
    {
        Crew? crewMembers = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        return crewMembers is not null;
    }
    
    public async Task<bool> IsUserCastAsync(string username)
    {
        Cast? castMember = await profileRepository.GetCastByUsernameAsNoTrackingAsync(username);
        return castMember is not null;
    }

    public async Task<ProfileRoleDto> GetRoleInfoAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        if (crew is not null)
            return new ProfileRoleDto { IsCrew = true, IsCast = false };

        Cast? cast = await profileRepository.GetCastByUsernameAsNoTrackingAsync(username);
        if (cast is not null)
            return new ProfileRoleDto { IsCrew = false, IsCast = true };
        
        return new ProfileRoleDto { IsCrew = false, IsCast = false };
    }
}
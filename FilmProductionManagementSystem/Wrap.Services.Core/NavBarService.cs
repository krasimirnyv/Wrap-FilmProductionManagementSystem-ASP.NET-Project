namespace Wrap.Services.Core;

using Microsoft.Extensions.Logging;

using Interfaces;
using Wrap.Data.Repository.Interfaces;
using Wrap.Data.Models;
using Models.NavBar;

using static GCommon.OutputMessages;
using static GCommon.OutputMessages.NavBar;
    
public class NavBarService(INavBarRepository navBarRepository, ILogger<NavBarService> logger) : INavBarService
{
    public async Task<NavBarUserDto?> GetNavBarUserAsync(Guid userId)
    {
        Crew? crew = await navBarRepository.GetCrewUserAsync(userId);
        if (crew is not null)
        {
            NavBarUserDto navBarDto = new NavBarUserDto
            {
                UserName = crew.User.UserName!,
                ProfileImagePath = crew.ProfileImagePath!,
                Role = CrewString
            };
            
            return navBarDto;
        }
        
        Cast? cast = await navBarRepository.GetCastUserAsync(userId);
        if (cast is not null)
        {
            NavBarUserDto navBarDto = new NavBarUserDto
            {
                UserName = cast.User.UserName!,
                ProfileImagePath = cast.ProfileImagePath!,
                Role = CastString
            };
            
            return navBarDto;
        }
        
        logger.LogError(string.Format(UserNotFoundMessage, userId));
        throw new ArgumentNullException(string.Format(UserNotFoundMessage, userId));
    }
}
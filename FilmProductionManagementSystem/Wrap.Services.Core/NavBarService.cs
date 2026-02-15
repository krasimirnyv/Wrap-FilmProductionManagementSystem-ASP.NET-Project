namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore;

using Interface;

using Data;

using ViewModels.NavBar;

public class NavBarService(FilmProductionDbContext context) : INavBarService
{
    public async Task<NavBarUserViewModel?> GetNavBarUserAsync(string userId)
    {
        NavBarUserViewModel? crew = await context
            .CrewMembers
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new NavBarUserViewModel
            {
                UserName = c.User.UserName!,
                ProfileImagePath = c.ProfileImagePath!,
                Role = "Crew"
            })
            .FirstOrDefaultAsync();

        if (crew is not null)
            return crew;

        NavBarUserViewModel? cast = await context
            .CastMembers
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new NavBarUserViewModel
            {
                UserName = c.User.UserName!,
                ProfileImagePath = c.ProfileImagePath,
                Role = "Cast"
            })
            .FirstOrDefaultAsync();
        
        if (cast is not null)
            return cast;
        
        throw new Exception($"Unable to find user with ID: {userId}");
    }
}
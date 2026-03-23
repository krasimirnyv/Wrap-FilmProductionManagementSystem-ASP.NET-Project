namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Wrap.Services.Models.NavBar;
using static GCommon.OutputMessages;

public class NavBarRepository(FilmProductionDbContext dbContext) 
    : BaseRepository(dbContext), INavBarRepository
{
    public async Task<NavBarUserDto?> GetNavBarCrewUserAsync(Guid userId)
    {
        NavBarUserDto crewDto = await Context!
            .CrewMembers
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new NavBarUserDto
            {
                UserName = c.User.UserName!,
                ProfileImagePath = c.ProfileImagePath!,
                Role = CrewString
            })
            .SingleOrDefaultAsync();
        
        return crewDto;
    }

    public async Task<NavBarUserDto?> GetNavBarCastUserAsync(Guid userId)
    {
        NavBarUserDto castDto = await Context!
            .CastMembers
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .Select(c => new NavBarUserDto
            {
                UserName = c.User.UserName!,
                ProfileImagePath = c.ProfileImagePath,
                Role = CastString
            })
            .SingleOrDefaultAsync();
        
        return castDto;
    }
}
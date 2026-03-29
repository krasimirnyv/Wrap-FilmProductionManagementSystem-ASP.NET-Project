namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;

public class NavBarRepository(FilmProductionDbContext dbContext) 
    : BaseRepository(dbContext), INavBarRepository
{
    public async Task<Crew?> GetCrewUserAsync(Guid userId)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(cm => cm.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.UserId == userId);
        
        return crew;
    }

    public async Task<Cast?> GetCastUserAsync(Guid userId)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(cm => cm.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.UserId == userId);
        
        return cast;
    }
}
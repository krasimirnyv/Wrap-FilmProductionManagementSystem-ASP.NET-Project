namespace Wrap.Services.Core;

using Models.NavBar;
using Interfaces;
using Data;
using Wrap.Data.Repository.Interfaces;

using static GCommon.OutputMessages.NavBar;
    
public class NavBarService(FilmProductionDbContext context,
                           INavBarRepository repository) : INavBarService
{
    public async Task<NavBarUserDto?> GetNavBarUserAsync(Guid userId)
    {
        NavBarUserDto? crew = await repository.GetNavBarCrewUserAsync(userId);
        if (crew is not null)
            return crew;

        NavBarUserDto? cast = await repository.GetNavBarCastUserAsync(userId);
        if (cast is not null)
            return cast;
        
        throw new Exception(string.Format(UserNotFoundMessage, userId));
    }
}
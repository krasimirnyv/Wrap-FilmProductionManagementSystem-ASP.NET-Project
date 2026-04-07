namespace Wrap.Data.Repository.Interfaces;

using Services.Models.FindPeople;
using GCommon.Enums;

public interface IFindPeopleRepository
{
    Task<(IReadOnlyCollection<FilmmakerListDto> Filmmakers, int TotalCount)> GetFilmmakersPagedAsync
        (int skip, int take, string? search, CrewRoleType? roleType, Guid? productionId);

    Task<(IReadOnlyCollection<ActorListDto> Actors, int TotalCount)> GetActorsPagedAsync
        (int skip, int take, string? search, byte? age, string? gender, Guid? productionId);

    Task AddCrewToProductionAsync(Guid productionId, Guid crewId, CrewRoleType roleType);
    
    Task RemoveCrewFromProductionAsync(Guid productionId, Guid crewId);

    Task AddCastToProductionAsync(Guid productionId, Guid castId, string roleName);
    
    Task RemoveCastFromProductionAsync(Guid productionId, Guid castId);

    Task<int> SaveAllChangesAsync();
}
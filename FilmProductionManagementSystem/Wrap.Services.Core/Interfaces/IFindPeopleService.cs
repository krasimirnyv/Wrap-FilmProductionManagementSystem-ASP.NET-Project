namespace Wrap.Services.Core.Interfaces;

using Models.FindPeople;
using GCommon.Enums;

using static GCommon.ApplicationConstants;

public interface IFindPeopleService
{
    Task<FindFilmmakersDto> GetFilmmakersAsync
        (int pageNumber = 1, string? search = null, CrewRoleType? roleType = null, Guid? productionId = null, int peoplePerPage = DefaultPeoplePerPage);

    Task<FindActorsDto> GetActorsAsync
        (int pageNumber = 1, string? search = null, byte? age = null, string? gender = null, Guid? productionId = null, int peoplePerPage = DefaultPeoplePerPage);

    Task<bool> CanManageProductionAsync(Guid productionId, string userId);
    
    Task AddCrewAsync(AddFilmmakerDto dto);
    
    Task RemoveCrewAsync(Guid productionId, Guid crewId);

    Task AddCastAsync(AddActorDto dto);
    
    Task RemoveCastAsync(Guid productionId, Guid castId);
}
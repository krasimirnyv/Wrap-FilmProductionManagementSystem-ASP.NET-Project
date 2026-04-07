namespace Wrap.Services.Core;

using Interfaces;
using Models.FindPeople;
using Data.Repository.Interfaces;
using GCommon.Enums;

using static GCommon.ApplicationConstants;

public class FindPeopleService(IFindPeopleRepository findPeopleRepository, IProductionRepository productionRepository) : IFindPeopleService
{
    public async Task<FindFilmmakersDto> GetFilmmakersAsync
        (int pageNumber = 1, string? search = null, CrewRoleType? roleType = null, Guid? productionId = null, int peoplePerPage = DefaultPeoplePerPage)
    {
        int safePage = Math.Max(pageNumber, 1);
        int skip = (safePage - 1) * peoplePerPage;

        (IReadOnlyCollection<FilmmakerListDto> filmmakers, int totalCount) = await findPeopleRepository
            .GetFilmmakersPagedAsync(skip, peoplePerPage, search, roleType, productionId);

        FindFilmmakersDto filmmakersDto = new FindFilmmakersDto
        {
            FilmmakerListDtos = filmmakers,
            TotalCount = totalCount
        };
        
        return filmmakersDto;
    }

    public async Task<FindActorsDto> GetActorsAsync
        (int pageNumber = 1, string? search = null, byte? age = null, string? gender = null, Guid? productionId = null, int peoplePerPage = DefaultPeoplePerPage)
    {
        int safePage = Math.Max(pageNumber, 1);
        int skip = (safePage - 1) * peoplePerPage;

        (IReadOnlyCollection<ActorListDto> actors, int totalCount) = await findPeopleRepository
            .GetActorsPagedAsync(skip, peoplePerPage, search, age, gender, productionId);

        FindActorsDto actorsDto = new FindActorsDto
        {
            ActorListDtos = actors,
            TotalCount = totalCount
        };
        
        return actorsDto;
    }

    public async Task<bool> CanManageProductionAsync(Guid productionId, string userId)
    {
        Guid? userGuidId = ValidateGuid(userId);
        if (userGuidId is null)
            return false;
        
        bool isAllowed = await productionRepository.IsUserProductionLeaderAsync(productionId, userGuidId.Value);
        return isAllowed;
    }

    public async Task AddCrewAsync(AddFilmmakerDto dto)
    {
        await findPeopleRepository.AddCrewToProductionAsync
        (
            productionId: dto.ProductionId,
            crewId: dto.CrewId,
            roleType: dto.RoleType
        );

        await findPeopleRepository.SaveAllChangesAsync();
    }

    public async Task RemoveCrewAsync(Guid productionId, Guid crewId)
    {
        await findPeopleRepository.RemoveCrewFromProductionAsync(productionId, crewId);
        await findPeopleRepository.SaveAllChangesAsync();
    }

    public async Task AddCastAsync(AddActorDto dto)
    {
        await findPeopleRepository.AddCastToProductionAsync
        (
            productionId: dto.ProductionId,
            castId: dto.CastId,
            roleName: dto.RoleName
        );

        await findPeopleRepository.SaveAllChangesAsync();
    }

    public async Task RemoveCastAsync(Guid productionId, Guid castId)
    {
        await findPeopleRepository.RemoveCastFromProductionAsync(productionId, castId);
        await findPeopleRepository.SaveAllChangesAsync();
    }

    private static Guid? ValidateGuid(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        bool isValidId = Guid.TryParse(id, out Guid productionId);
        if (!isValidId)
            return null;
        
        return productionId;
    }
}
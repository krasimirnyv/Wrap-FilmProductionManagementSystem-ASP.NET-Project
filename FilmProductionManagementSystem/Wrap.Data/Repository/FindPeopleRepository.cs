namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;
using Models.MappingEntities;
using Services.Models.FindPeople;
using GCommon.Enums;

public class FindPeopleRepository(FilmProductionDbContext dbContext) : BaseRepository(dbContext), IFindPeopleRepository
{
    public async Task<(IReadOnlyCollection<FilmmakerListDto> Filmmakers, int TotalCount)> GetFilmmakersPagedAsync
        (int skip, int take, string? search, CrewRoleType? roleType, Guid? productionId)
    {
        IQueryable<Crew> query = Context!
            .CrewMembers
            .Include(c => c.Skills)
            .AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            string normalized = search.Trim().ToLower();
            
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(normalized) ||
                c.LastName.ToLower().Contains(normalized)  ||
                (c.Nickname != null && c.Nickname.ToLower().Contains(normalized)));
        }

        if (roleType.HasValue)
            query = query.Where(c => c.Skills.Any(s => s.RoleType == roleType));
        
        HashSet<Guid> alreadyInProduction = [];
        if (productionId.HasValue)
        {
            alreadyInProduction = (await Context!
                    .ProductionsCrewMembers
                    .AsNoTracking()
                    .Where(pc => pc.ProductionId == productionId.Value)
                    .Select(pc => pc.CrewMemberId)
                    .ToListAsync())
                .ToHashSet();
        };
        
        IReadOnlyCollection<FilmmakerListDto> filmmakers = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip(skip)
            .Take(take)
            .Select(c => new FilmmakerListDto
            {
                CrewId = c.Id,
                ProfileImagePath = c.ProfileImagePath!,
                FullName = c.FirstName + " " + c.LastName,
                Nickname = c.Nickname,
                Role = c.Skills
                    .OrderByDescending(s => s.RoleType)
                    .Select(s => s.RoleType.ToString())
                    .FirstOrDefault(),
                IsAlreadyInProduction = alreadyInProduction.Contains(c.Id)
            })
            .ToListAsync();

        int totalCount = await query.CountAsync();

        return (filmmakers, totalCount);
    }

    public async Task<(IReadOnlyCollection<ActorListDto> Actors, int TotalCount)> GetActorsPagedAsync
        (int skip, int take, string? search, byte? age, string? gender, Guid? productionId)
    {
        IQueryable<Cast> query = Context!
            .CastMembers
            .Include(c => c.User)
            .AsNoTracking();
        
         if (!string.IsNullOrWhiteSpace(search))
        {
            string normalized = search.Trim().ToLower();
            
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(normalized) ||
                c.LastName.ToLower().Contains(normalized)  ||
                (c.Nickname != null && c.Nickname.ToLower().Contains(normalized)));
        }

        if (!string.IsNullOrWhiteSpace(gender) && Enum.TryParse(gender, out GenderType genderType))
            query = query.Where(c => c.Gender == genderType);

        if (age.HasValue)
        {
            DateTime today = DateTime.Today;

            DateTime minBirthDate = today.AddYears(-(age.Value + 1)).AddDays(1);
            DateTime maxBirthDateExclusive = today.AddYears(-age.Value).AddDays(1);

            query = query.Where(c => c.BirthDate >= minBirthDate &&
                                     c.BirthDate < maxBirthDateExclusive);
        }
        
        HashSet<Guid> alreadyInProduction = [];
        if (productionId.HasValue)
        {
            alreadyInProduction = (await Context
                    .ProductionsCastMembers
                    .AsNoTracking()
                    .Where(pc => pc.ProductionId == productionId.Value)
                    .Select(pc => pc.CastMemberId)
                    .ToListAsync())
                .ToHashSet();
        };
        
        List<ActorListDto> actors = await query
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .Skip(skip)
            .Take(take)
            .Select(c => new ActorListDto
            {
                CastId = c.Id,
                ProfileImagePath = c.ProfileImagePath!,
                FullName = c.FirstName + " " + c.LastName,
                Nickname = c.Nickname,
                Age = c.Age,
                Gender = c.Gender.ToString(),
                IsAlreadyInProduction = alreadyInProduction.Contains(c.Id)
            })
            .ToListAsync();

        int totalCount = await query.CountAsync();

        return (actors, totalCount);
    }

    public async Task AddCrewToProductionAsync(Guid productionId, Guid crewId, CrewRoleType roleType)
    {
        bool isAlreadyExists = await Context!
            .ProductionsCrewMembers
            .AnyAsync(pc => pc.ProductionId == productionId && pc.CrewMemberId == crewId);

        if (isAlreadyExists)
            return;

        ProductionCrew newProductionCrew = new()
        {
            ProductionId = productionId,
            CrewMemberId = crewId,
            RoleType = roleType
        };
        
        await Context!.ProductionsCrewMembers.AddAsync(newProductionCrew);
    }

    public async Task RemoveCrewFromProductionAsync(Guid productionId, Guid crewId)
    {
        ProductionCrew? productionCrew = await Context!
            .ProductionsCrewMembers
            .SingleOrDefaultAsync(pc => pc.ProductionId == productionId && pc.CrewMemberId == crewId);
        
        if (productionCrew is not null)
            Context!.ProductionsCrewMembers.Remove(productionCrew);
    }

    public async Task AddCastToProductionAsync(Guid productionId, Guid castId, string roleName)
    {
        bool isAlreadyExists = await Context!
            .ProductionsCastMembers
            .AnyAsync(pc => pc.ProductionId == productionId && pc.CastMemberId == castId);

        if (isAlreadyExists)
            return;

        ProductionCast newProductionCast = new()
        {
            ProductionId = productionId,
            CastMemberId = castId,
            Role = roleName
        };
        
        await Context!.ProductionsCastMembers.AddAsync(newProductionCast);
    }

    public async Task RemoveCastFromProductionAsync(Guid productionId, Guid castId)
    {
        ProductionCast? productionCast = await Context!
            .ProductionsCastMembers
            .SingleOrDefaultAsync(pc => pc.ProductionId == productionId && pc.CastMemberId == castId);

        if (productionCast is not null)
            Context!.ProductionsCastMembers.Remove(productionCast);
    }

    public async Task<int> SaveAllChangesAsync()
    {
        int affectedRows = await SaveChangesAsync();
        return affectedRows;
    }
}
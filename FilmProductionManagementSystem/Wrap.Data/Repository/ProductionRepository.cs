namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Interfaces;
using Models;
using Models.MappingEntities;
using GCommon.Enums;

public class ProductionRepository(FilmProductionDbContext dbContext) 
    : BaseRepository(dbContext), IProductionRepository
{
    public async Task<IReadOnlyCollection<Production>> GetAllProductionsAsync
        (int? skipCount = null, int? takeCount = null, IReadOnlyCollection<ProductionStatusType>? statuses = null, bool? isActive = null)
    {
        IQueryable<Production> productionsQuery = Context!
            .Productions
            .AsNoTracking()
            .OrderBy(p => p.Title)
            .ThenBy(p => p.StatusType)
            .ThenBy(p => p.Id);

        productionsQuery = ProductionsQuery(statuses, isActive, productionsQuery);
        
        if (skipCount.HasValue && skipCount > 0)
            productionsQuery = productionsQuery.Skip(skipCount.Value);

        if (takeCount.HasValue && takeCount > 0)
            productionsQuery = productionsQuery.Take(takeCount.Value);

        IReadOnlyCollection<Production> productions = await productionsQuery.ToArrayAsync();
        
        return productions;
    }

    public async Task<int> ProductionCountAsync(IReadOnlyCollection<ProductionStatusType>? statuses = null, bool? isActive = null)
    {
        IQueryable<Production> productionsQuery = Context!
            .Productions
            .AsNoTracking();
        
        productionsQuery = ProductionsQuery(statuses, isActive, productionsQuery);
        
        int productionsCount = await productionsQuery.CountAsync();
        
        return productionsCount;
    }

    public async Task<Production?> GetProductionByIdAsNoTrackingAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        return production;
    }
    
    public async Task<Production?> GetProductionByIdAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        return production;
    }
    
    public async Task<Production?> GetProductionWithDataByIdAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .Include(p => p.ProductionCrewMembers)
            .ThenInclude(pc => pc.CrewMember)
            .Include(p => p.ProductionCastMembers)
            .ThenInclude(pc => pc.CastMember)
            .Include(p => p.ProductionAssets)
            .Include(p => p.Scenes)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        return production;
    }

    public async Task<(Production?       production, 
   IReadOnlyCollection<ProductionCrew>?  productionCrews,
   IReadOnlyCollection<ProductionCast>?  productionCasts,
   IReadOnlyCollection<Scene>?           productionScenes,
   IReadOnlyCollection<ProductionAsset>? productionAssets,
   IReadOnlyCollection<ShootingDay>?     productionShootingDays)>
        GetDetailsAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .Include(p => p.Script)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        if (production is null)
            return (production:             null,
                    productionCrews:        null,
                    productionCasts:        null, 
                    productionScenes:       null,
                    productionAssets:       null, 
                    productionShootingDays: null);

        IReadOnlyCollection<ProductionCrew> productionCrews = await Context!
            .ProductionsCrewMembers
            .Include(pc => pc.CrewMember)
            .AsNoTracking()
            .Where(pc => pc.ProductionId == productionId)
            .ToArrayAsync();
        
        IReadOnlyCollection<ProductionCast> productionCasts = await Context!
            .ProductionsCastMembers
            .Include(pc => pc.CastMember)
            .AsNoTracking()
            .Where(pc => pc.ProductionId == productionId)
            .ToArrayAsync();

        IReadOnlyCollection<Scene> productionScenes = await Context!
            .Scenes
            .AsNoTracking()
            .Where(s => s.ProductionId == productionId)
            .ToArrayAsync();
        
        IReadOnlyCollection<ProductionAsset> productionAssets = await Context!
            .ProductionsAssets
            .AsNoTracking()
            .Where(pa => pa.ProductionId == productionId)
            .ToArrayAsync();

        IReadOnlyCollection<ShootingDay> productionShootingDays = await Context!
            .ShootingDays
            .AsNoTracking()
            .Where(sd => sd.ProductionId == productionId)
            .ToArrayAsync();
        
        return (production, productionCrews, productionCasts, productionScenes, productionAssets, productionShootingDays);
    }

    public async Task AddAsync(Production production)
    {
        await Context!.Productions.AddAsync(production);
    }

    public Task DeleteAsync(Production production)
    {
        Context!.Productions.Remove(production);
        return Task.CompletedTask;
    }

    public async Task<Crew?> GetCrewByUserIdAsync(Guid applicationUserId)
    {
        Crew? crew = await Context!
            .CrewMembers
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.UserId == applicationUserId);
        
        return crew;
    }

    public async Task AddDirectorToProductionAsync(Guid productionId, Crew creator, CrewRoleType roleType)
    {
        ProductionCrew newProductionCrew = new ProductionCrew
        {
            ProductionId = productionId,
            CrewMemberId = creator.Id,
            RoleType = roleType
        };
        
        await Context!.ProductionsCrewMembers.AddAsync(newProductionCrew);
    }

    public async Task<bool> IsUserProductionLeaderAsync(Guid productionId, Guid applicationUserId)
    {
        Guid? crewId = await Context!
            .CrewMembers
            .AsNoTracking()
            .Where(c => c.UserId == applicationUserId && !c.IsDeleted)
            .Select(c => c.Id)
            .SingleOrDefaultAsync();

        if (crewId == Guid.Empty)
            return false;

        CrewRoleType[] allowedLeaderRoles =
        [
            CrewRoleType.Director,
            CrewRoleType.Producer,
        ];

        bool isLeader = await Context
                            .Productions
                            .AsNoTracking()
                            .Where(p => p.Id == productionId)
                            .AnyAsync(p => p.CreatedByUserId == applicationUserId) ||
                        await Context
                            .ProductionsCrewMembers
                            .AsNoTracking()
                            .AnyAsync(pc => pc.ProductionId == productionId
                                            && pc.CrewMemberId == crewId
                                            && allowedLeaderRoles.Contains(pc.RoleType));
        return isLeader;
    }

    public async Task<int> SaveAllChangesAsync()
    {
        int affectedRows = await SaveChangesAsync();
        return affectedRows;
    }
    
    private static IQueryable<Production> ProductionsQuery(IReadOnlyCollection<ProductionStatusType>? statuses, bool? isActive, IQueryable<Production> productionsQuery)
    {
        if (statuses is not null && statuses.Count > 0)
        {
            productionsQuery = productionsQuery
                .Where(p => statuses.Contains(p.StatusType));
        }
        else if (isActive.HasValue)
        { 
            productionsQuery = productionsQuery
                .Where(p =>
                    p.StatusType == ProductionStatusType.Production ||
                    p.StatusType == ProductionStatusType.OnHold ||
                    p.StatusType == ProductionStatusType.Reshoots);
        } 

        return productionsQuery;
    }
}
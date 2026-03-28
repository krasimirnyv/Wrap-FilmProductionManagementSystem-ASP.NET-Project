namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;

using Models;
using Interfaces;
using Wrap.Services.Models.Production;
using Wrap.Services.Models.Production.NestedDtos;

public class ProductionRepository(FilmProductionDbContext dbContext) 
    : BaseRepository(dbContext), IProductionRepository
{
    public async Task<ICollection<ProductionDto>> GetAllAsync(int? skipCount = null, int? takeCount = null)
    {
        IQueryable<Production> productionsQuery = Context!
            .Productions
            .AsNoTracking()
            .OrderBy(p => p.Title)
            .ThenBy(p => p.Budget)
            .ThenBy(p => p.Description)
            .ThenBy(p => p.StatusType)
            .ThenBy(p => p.Id);
        
        if (skipCount.HasValue && skipCount > 0)
        {
            productionsQuery = productionsQuery
                .Skip(skipCount.Value)
                .AsQueryable();
        }

        if (takeCount.HasValue && takeCount > 0)
        {
            productionsQuery = productionsQuery
                .Take(takeCount.Value)
                .AsQueryable();
        }
        
        ICollection<ProductionDto> dto = await productionsQuery
            .Select(p => new ProductionDto
            {
                Id = p.Id,
                Title = p.Title,
                ThumbnailPath = p.Thumbnail,
                StatusType = p.StatusType
            })
            .ToArrayAsync();
        
        return dto;
    }

    public async Task<int> CountAsync()
    {
        IQueryable<Production> productionsQuery = Context!
            .Productions
            .AsNoTracking()
            .AsQueryable();
        
        int productionsCount = await productionsQuery.CountAsync();
        
        return productionsCount;
    }

    public async Task<Production?> GetByIdAsNoTrackingAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        return production;
    }
    
    public async Task<Production?> GetByIdAsync(Guid productionId)
    {
        Production? production = await Context!
            .Productions
            .SingleOrDefaultAsync(p => p.Id == productionId);
        
        return production;
    }

    public async Task<DetailsProductionDto?> GetDetailsAsync(Guid productionId)
    {
        DetailsProductionDto? baseDto = await Context!
            .Productions
            .AsNoTracking()
            .Where(p => p.Id == productionId)
            .Select(p => new DetailsProductionDto
            {
                Id = p.Id,
                Thumbnail = p.Thumbnail,
                Title = p.Title,
                Description = p.Description,
                Budget = p.Budget,
                StatusType = p.StatusType,
                StatusStartDate = p.StatusStartDate,
                StatusEndDate = p.StatusEndDate,
                ScriptId = p.Script != null ? p.Script.Id : null
            })
            .SingleOrDefaultAsync();
        
        if (baseDto is null)
            return null;

        IReadOnlyCollection<ProductionCrewMemberDto> crewDtos = await Context!
            .ProductionsCrewMembers
            .AsNoTracking()
            .Where(pc => pc.ProductionId == productionId)
            .Select(pc => new ProductionCrewMemberDto
            {
                ProfileImagePath = pc.CrewMember.ProfileImagePath,
                FirstName = pc.CrewMember.FirstName,
                LastName = pc.CrewMember.LastName,
                Role = pc.RoleType
            })
            .ToArrayAsync();
        
        IReadOnlyCollection<ProductionCastMemberDto> castDtos = await Context!
            .ProductionsCastMembers
            .AsNoTracking()
            .Where(pc => pc.ProductionId == productionId)
            .Select(pc => new ProductionCastMemberDto
            {
                ProfileImagePath = pc.CastMember.ProfileImagePath,
                FirstName = pc.CastMember.FirstName,
                LastName = pc.CastMember.LastName,
                Role = pc.Role,
                Age = pc.CastMember.Age,
                Gender = pc.CastMember.Gender
            })
            .ToArrayAsync();

        IReadOnlyCollection<ProductionSceneDto> sceneDtos = await Context!
            .Scenes
            .AsNoTracking()
            .Where(s => s.ProductionId == productionId)
            .Select(s => new ProductionSceneDto
            {
                SceneNumber = s.SceneNumber,
                SceneType = s.SceneType,
                SceneName = s.SceneName,
                Location = s.Location
            })
            .ToArrayAsync();
        
        IReadOnlyCollection<ProductionAssetDto> assetDtos = await Context!
            .ProductionsAssets
            .AsNoTracking()
            .Where(pa => pa.ProductionId == productionId)
            .Select(pa => new ProductionAssetDto
            {
                Title = pa.Title,
                AssetType = pa.AssetType
            })
            .ToArrayAsync();

        IReadOnlyCollection<ProductionShootingDayDto> shootingDayDtos = await Context!
            .ShootingDays
            .AsNoTracking()
            .Where(sd => sd.ProductionId == productionId)
            .Select(sd => new ProductionShootingDayDto
            {
                Date = sd.Date
            })
            .ToArrayAsync();

        baseDto.ProductionCrewMembers = crewDtos;
        baseDto.ProductionCastMembers = castDtos;
        baseDto.ProductionScenes = sceneDtos;
        baseDto.ProductionAssets = assetDtos;
        baseDto.ProductionShootingDays = shootingDayDtos;
        
        return baseDto;
    }

    public async Task<EditProductionDto?> GetEditAsync(Guid productionId)
    {
        EditProductionDto? dto = await Context!
            .Productions
            .Where(p => p.Id == productionId)
            .Select(p => new EditProductionDto
            {
                Title = p.Title,
                ThumbnailImage = null,
                Description = p.Description,
                StatusType = p.StatusType,
                Budget = p.Budget,
                StatusStartDate = p.StatusStartDate,
                StatusEndDate = p.StatusEndDate,
                CurrentThumbnailPath = p.Thumbnail
            })
            .SingleOrDefaultAsync();
        
        return dto;
    }

    public async Task<DeleteProductionDto?> GetDeleteAsync(Guid productionId)
    {
        DeleteProductionDto? dto = await Context!
            .Productions
            .AsNoTracking()
            .Where(p => p.Id == productionId)
            .Select(p => new DeleteProductionDto
            {
                Id = p.Id,
                Title = p.Title,
                Thumbnail = p.Thumbnail,
                Description = p.Description,
                StatusType = p.StatusType,
                Budget = p.Budget,
                CrewMembersCount = p.ProductionCrewMembers.Count,
                CastMembersCount = p.ProductionCastMembers.Count,
                ScenesCount = p.Scenes.Count
            })
            .SingleOrDefaultAsync();
        
        return dto;
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

    public async Task<int> SaveAllChangesAsync()
    {
        int effectedRows = await SaveChangesAsync();
        return effectedRows;
    }
}
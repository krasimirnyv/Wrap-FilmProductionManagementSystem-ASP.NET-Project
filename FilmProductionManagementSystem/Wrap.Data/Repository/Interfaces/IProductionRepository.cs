namespace Wrap.Data.Repository.Interfaces;

using Models;
using Models.MappingEntities;
using GCommon.Enums;

public interface IProductionRepository
{
    Task<IReadOnlyCollection<Production>> GetAllProductionsAsync(int? skipCount = null, int? takeCount = null, IReadOnlyCollection<ProductionStatusType>? statuses = null, bool? isActive = null);
    
    Task<int> ProductionCountAsync(IReadOnlyCollection<ProductionStatusType>? statuses = null, bool? isActive = null);
    
    Task<Production?> GetProductionByIdAsNoTrackingAsync(Guid productionId);

    Task<Production?> GetProductionWithDataByIdAsync(Guid productionId);
    
    Task<Production?> GetProductionByIdAsync(Guid productionId);

              Task<(Production?       production,
IReadOnlyCollection<ProductionCrew>?  productionCrews, 
IReadOnlyCollection<ProductionCast>?  productionCasts, 
IReadOnlyCollection<Scene>?           productionScenes, 
IReadOnlyCollection<ProductionAsset>? productionAssets, 
IReadOnlyCollection<ShootingDay>?     productionShootingDays)> 
        GetDetailsAsync(Guid productionId);
    
    Task AddAsync(Production production);

    Task DeleteAsync(Production production);
    
    Task<Crew?> GetCrewByUserIdAsync(Guid applicationUserId);

    Task AddDirectorToProductionAsync(Guid productionId, Crew creator, CrewRoleType roleType);
    
    Task<bool> IsUserProductionLeaderAsync(Guid productionId, Guid applicationUserId);
    
    Task<int> SaveAllChangesAsync();
}
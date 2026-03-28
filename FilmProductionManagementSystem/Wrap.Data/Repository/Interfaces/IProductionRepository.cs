namespace Wrap.Data.Repository.Interfaces;

using Models;
using Models.MappingEntities;

public interface IProductionRepository
{
    Task<IReadOnlyCollection<Production>> GetAllAsync(int? skipCount = null, int? takeCount = null);

    Task<int> CountAsync();
    
    Task<Production?> GetByIdAsNoTrackingAsync(Guid productionId);
    
    Task<Production?> GetByIdAsync(Guid productionId);

              Task<(Production?       production,
IReadOnlyCollection<ProductionCrew>?  productionCrews, 
IReadOnlyCollection<ProductionCast>?  productionCasts, 
IReadOnlyCollection<Scene>?           productionScenes, 
IReadOnlyCollection<ProductionAsset>? productionAssets, 
IReadOnlyCollection<ShootingDay>?     productionShootingDays)> 
        GetDetailsAsync(Guid productionId);
    
    Task AddAsync(Production production);

    Task DeleteAsync(Production production);

    Task<int> SaveAllChangesAsync();
}
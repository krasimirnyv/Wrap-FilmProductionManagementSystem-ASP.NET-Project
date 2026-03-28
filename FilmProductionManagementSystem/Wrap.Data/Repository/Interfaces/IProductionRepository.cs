namespace Wrap.Data.Repository.Interfaces;

using Models;
using Wrap.Services.Models.Production;

public interface IProductionRepository
{
    Task<ICollection<ProductionDto>> GetAllAsync(int? skipCount = null, int? takeCount = null);

    Task<int> CountAsync();
    
    Task<Production?> GetByIdAsNoTrackingAsync(Guid productionId);
    
    Task<Production?> GetByIdAsync(Guid productionId);

    Task<DetailsProductionDto?> GetDetailsAsync(Guid productionId);

    Task<EditProductionDto?> GetEditAsync(Guid productionId);

    Task<DeleteProductionDto?> GetDeleteAsync(Guid productionId);
    
    Task AddAsync(Production production);

    Task DeleteAsync(Production production);

    Task<int> SaveAllChangesAsync();
}
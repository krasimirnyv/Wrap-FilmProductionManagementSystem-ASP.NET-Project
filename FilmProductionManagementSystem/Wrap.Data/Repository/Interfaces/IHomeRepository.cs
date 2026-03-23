namespace Wrap.Data.Repository.Interfaces;

using Wrap.Services.Models.General;

public interface IHomeRepository
{
    Task<int> GetCrewCountAsync();
    
    Task<int> GetCastCountAsync();
    
    Task<IReadOnlyCollection<ProductionDashboardDto>> GetProductionSummaryAsync(DateTime now);
}
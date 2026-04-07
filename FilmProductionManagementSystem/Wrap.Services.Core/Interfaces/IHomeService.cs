namespace Wrap.Services.Core.Interfaces;

using Models.General;

public interface IHomeService
{
    /// <summary>
    /// Gets the data for the home page,
    /// including the number of crew members, cast members, and productions.
    /// </summary>
    /// <param name="userId">string</param>
    /// <returns>DashboardDataDto</returns>
    Task<DashboardDataDto> GetDashboardDataAsync(string userId);
}
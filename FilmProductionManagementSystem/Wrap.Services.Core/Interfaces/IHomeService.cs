namespace Wrap.Services.Core.Interfaces;

using Models.Home;

public interface IHomeService
{
    /// <summary>
    /// Gets the data for the home page,
    /// including the number of crew members, cast members, and productions.
    /// </summary>
    /// <returns>DashboardDataDto</returns>
    Task<DashboardDataDto> GetDashboardDataAsync();
}
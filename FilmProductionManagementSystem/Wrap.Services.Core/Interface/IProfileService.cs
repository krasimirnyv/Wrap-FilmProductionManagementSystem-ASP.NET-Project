namespace Wrap.Services.Core.Interface;

using Data.Models;

using ViewModels.Profile;

public interface IProfileService
{
    /// <summary>
    /// Check if user is a crew member
    /// </summary>
    /// <param name="username"></param>
    /// <returns>bool isCrew = true/false</returns>
    Task<bool> IsUserCrewAsync(string username);

    /// <summary>
    /// Check if user is a cast member
    /// </summary>
    /// <param name="username"></param>
    /// <returns>bool isCast = true/false</returns>
    Task<bool> IsUserCastAsync(string username);

    /// <summary>
    /// Get complete profile data for Crew member
    /// </summary>
    /// <param name="username">Crew entity</param>
    /// <returns>CrewProfileViewModel</returns>
    Task<CrewProfileViewModel> GetCrewProfileDataAsync(string username);

    /// <summary>
    /// Get complete profile data for Cast member
    /// </summary>
    /// <param name="username">Cast entity</param>
    /// <returns>CastProfileViewModel</returns>
    Task<CastProfileViewModel> GetCastProfileDataAsync(string username);
}
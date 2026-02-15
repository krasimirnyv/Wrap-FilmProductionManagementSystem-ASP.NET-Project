namespace Wrap.Services.Core.Interface;

using ViewModels.Profile;

public interface IProfileService
{
    /// <summary>
    /// Check if user is a crew member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>bool isCrew = true/false</returns>
    Task<bool> IsUserCrewAsync(string username);

    /// <summary>
    /// Check if user is a cast member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>bool isCast = true/false</returns>
    Task<bool> IsUserCastAsync(string username);

    /// <summary>
    /// Get complete profile data for Crew member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CrewProfileViewModel</returns>
    Task<CrewProfileViewModel> GetCrewProfileDataAsync(string username);

    /// <summary>
    /// Get complete profile data for Cast member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CastProfileViewModel</returns>
    Task<CastProfileViewModel> GetCastProfileDataAsync(string username);

    /// <summary>
    /// Gets the data for editing a crew profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCrewProfileViewModel</returns>
    Task<EditCrewProfileViewModel> GetEditCrewProfileAsync(string username);

    /// <summary>
    /// Updates the crew profile with the provided information from the EditCrewProfileViewModel
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="model">EditCrewProfileViewModel</param>
    Task UpdateCrewProfileAsync(string username, EditCrewProfileViewModel model);
    
    /// <summary>
    /// Gets the data for editing a cast profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCastProfileViewModel</returns>
    Task<EditCastProfileViewModel> GetEditCastProfileAsync(string username);

    /// <summary>
    /// Updates the cast profile with the provided information from the EditCrewProfileViewModel
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="model">EditCastProfileViewModel</param>
    Task UpdateCastProfileAsync(string username, EditCastProfileViewModel model);
}
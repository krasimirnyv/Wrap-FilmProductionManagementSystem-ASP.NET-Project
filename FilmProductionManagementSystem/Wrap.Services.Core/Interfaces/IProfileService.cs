namespace Wrap.Services.Core.Interfaces;

using Models.Profile;

public interface IProfileService
{
    /// <summary>
    /// Check if user is a crew member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>bool isCrew = true/false</returns>
    [Obsolete($"{nameof(IsUserCrewAsync)} is deprecated, please use {nameof(GetRoleInfoAsync)} instead.")]
    Task<bool> IsUserCrewAsync(string username);

    /// <summary>
    /// Check if user is a cast member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>bool isCast = true/false</returns>
    [Obsolete($"{nameof(IsUserCastAsync)} is deprecated, please use {nameof(GetRoleInfoAsync)} instead.")]
    Task<bool> IsUserCastAsync(string username);
    
    /// <summary>
    /// Gets the role information of an user
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>ProfileRoleDto</returns>
    Task<ProfileRoleDto> GetRoleInfoAsync(string username);
}
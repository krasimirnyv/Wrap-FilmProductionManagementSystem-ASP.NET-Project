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
    
    /// <summary>
    /// Get complete profile data for Crew member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CrewProfileDto</returns>
    Task<CrewProfileDto> GetCrewProfileDataAsync(string username);

    /// <summary>
    /// Get complete profile data for Cast member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CastProfileDto</returns>
    Task<CastProfileDto> GetCastProfileDataAsync(string username);

    /// <summary>
    /// Gets the data for editing a crew profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCrewProfileDto</returns>
    Task<EditCrewProfileDto> GetEditCrewProfileAsync(string username);

    /// <summary>
    /// Gets the data for editing a cast profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCastProfileDto</returns>
    Task<EditCastProfileDto> GetEditCastProfileAsync(string username);
    
    /// <summary>
    /// Updates the crew profile with the provided information from the EditCrewProfileDto
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="crewDto">EditCrewProfileDto</param>
    Task UpdateCrewProfileAsync(string username, EditCrewProfileDto crewDto);

    /// <summary>
    /// Updates the cast profile with the provided information from the EditCastProfileDto
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="castDto">EditCastProfileDto</param>
    Task UpdateCastProfileAsync(string username, EditCastProfileDto castDto);
    
    /// <summary>
    /// Gets the data for editing a crew member's skills including the current skills
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditSkillsDto</returns>
    Task<EditSkillsDto> GetEditSkillsAsync(string username);

    /// <summary>
    /// Updates the crew member's skills with the provided information from the EditSkillsViewModel
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="skillsDto">UpdateSkillsDto</param>
    Task UpdateSkillsAsync(string username, UpdateSkillsDto skillsDto);
}
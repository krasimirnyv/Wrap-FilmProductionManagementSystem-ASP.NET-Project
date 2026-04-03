namespace Wrap.Services.Core.Interfaces;

using Models.Profile;

public interface ICrewProfileService
{
    /// <summary>
    /// Get complete profile data for Crew member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CrewProfileDto</returns>
    Task<CrewProfileDto> GetCrewProfileDataAsync(string username);

    /// <summary>
    /// Gets the data for editing a crew profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCrewProfileDto</returns>
    Task<EditCrewProfileDto> GetEditCrewProfileAsync(string username);
    
    /// <summary>
    /// Updates the crew profile with the provided information from the EditCrewProfileDto
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="crewDto">EditCrewProfileDto</param>
    Task UpdateCrewProfileAsync(string username, EditCrewProfileDto crewDto);
    
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
    
    /// <summary>
    /// Gets the data for deleting a crew profile.
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>DeleteProfileDto</returns>
    Task<DeleteProfileDto> GetDeleteCrewProfileAsync(string username);
    
    /// <summary>
    /// Deletes the crew profile
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="dto">DeleteProfileDto</param>
    /// <returns>false if password is incorrect and true if success</returns>
    Task<bool> DeleteCrewProfileAsync(string username, DeleteProfileDto dto);
    
    /// <summary>
    /// Downloads the complete profile data for a crew member as a JSON string.
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>JSON as string</returns>
    Task<string> DownloadCrewProfileDataAsync(string username);
}
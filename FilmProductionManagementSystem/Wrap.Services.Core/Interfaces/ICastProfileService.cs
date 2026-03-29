namespace Wrap.Services.Core.Interfaces;

using Models.Profile;

public interface ICastProfileService
{
    /// <summary>
    /// Get complete profile data for Cast member
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>CastProfileDto</returns>
    Task<CastProfileDto> GetCastProfileDataAsync(string username);

    /// <summary>
    /// Gets the data for editing a cast profile,
    /// including the current profile information
    /// </summary>
    /// <param name="username">string</param>
    /// <returns>EditCastProfileDto</returns>
    Task<EditCastProfileDto> GetEditCastProfileAsync(string username);
    
    /// <summary>
    /// Updates the cast profile with the provided information from the EditCastProfileDto
    /// </summary>
    /// <param name="username">string</param>
    /// <param name="castDto">EditCastProfileDto</param>
    Task UpdateCastProfileAsync(string username, EditCastProfileDto castDto);
}
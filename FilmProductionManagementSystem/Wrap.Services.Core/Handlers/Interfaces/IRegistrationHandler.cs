namespace Wrap.Services.Core.Handlers.Interfaces;

using Microsoft.AspNetCore.Identity;

public interface IRegistrationHandler<in TRegistrationDto>
{
    /// <summary>
    /// Creates and completes a new user in the system based on the dto.
    /// </summary>
    /// <param name="registrationDto">TRegistrationDto</param>
    /// <returns>IdentityResult -> Success or not </returns>
    Task<IdentityResult> CompleteRegistrationAsync(TRegistrationDto registrationDto);
}
namespace Wrap.Services.Core.Interfaces;

using Microsoft.AspNetCore.Identity;

using Models.LoginAndRegistration;

public interface ILoginRegisterService
{
    /// <summary>
    /// Builds a draft of crew registration data based on the dto.
    /// This method is responsible for creating a temporary representation of the crew member's registration information,
    /// which can be used for further processing or validation before finalizing the registration.
    /// </summary>
    /// <param name="dto">CrewRegistrationStepOneDto</param>
    /// <returns>CrewRegistrationStepOneDraft</returns>
    Task<CrewRegistrationDraftDto?> BuildCrewDraftAsync(CrewRegistrationStepOneDto dto);

    /// <summary>
    /// Used for the second part of crew registration - get new dto with skills for the dropdowns in the view.
    /// </summary>
    /// <returns>CrewRegistrationStepTwoDto</returns>
    CrewRegistrationStepTwoDto GetNewModelWithSkills();

    /// <summary>
    /// Helper method to get skills for the dropdowns in the view.
    /// It populates the SkillsByDepartment property of the dto.
    /// </summary>
    /// <param name="dto">CrewRegistrationStepTwoDto</param>
    void GetSkills(CrewRegistrationStepTwoDto dto);

    /// <summary>
    /// Helper method to get skills for the dropdowns in the view.
    /// It populates the SkillsByDepartment property of the dto.
    /// </summary>
    /// <param name="dto">CrewRegistrationCompleteDto</param>
    void GetSkills(CrewRegistrationCompleteDto dto);
    
    /// <summary>
    /// Completes the crew registration process by taking the dto that contains
    /// the draft and the selected skills for creating a new crew member in the system vie transaction.
    /// </summary>
    /// <param name="registrationDto">CrewRegistrationCompleteDto</param>
    /// <returns>IdentityResult -> Success or not </returns>
    Task<IdentityResult> CompleteCrewRegistrationAsync(CrewRegistrationCompleteDto registrationDto);

    /// <summary>
    /// Creates and completes a new cast member in the system based on the dto.
    /// </summary>
    /// <param name="registrationDto">CastRegistrationDto</param>
    /// <returns>IdentityResult -> Success or not </returns>
    Task<IdentityResult> CompleteCastRegistrationAsync(CastRegistrationDto registrationDto);

    /// <summary>
    /// Checks the Login status and role in the application (cast or crew)
    /// and returns a LoginStatusDto with the result and the role as string.
    /// </summary>
    /// <param name="loginDto">LoginRequestDto</param>
    /// <returns>LoginStatusDto</returns>
    Task<LoginStatusDto> LoginStatusAsync(LoginRequestDto loginDto);

    /// <summary>
    /// Logout the current user from the system.
    /// </summary>
    Task LogoutAsync();
}
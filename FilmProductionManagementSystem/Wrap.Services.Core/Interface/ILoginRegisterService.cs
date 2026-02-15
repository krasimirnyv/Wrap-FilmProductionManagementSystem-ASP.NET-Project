namespace Wrap.Services.Core.Interface;

using Microsoft.AspNetCore.Identity;

using ViewModels.LoginAndRegistration;

public interface ILoginRegisterService
{
    /// <summary>
    /// Builds a draft of crew registration data based on the input model.
    /// This method is responsible for creating a temporary representation of the crew member's registration information,
    /// which can be used for further processing or validation before finalizing the registration.
    /// </summary>
    /// <param name="model">CrewRegistrationStepOneInputModel</param>
    /// <returns>CrewRegistrationStepOneDraft</returns>
    Task<CrewRegistrationStepOneDraft> BuildCrewDraftAsync(CrewRegistrationStepOneInputModel model);

    /// <summary>
    /// Used for the second part of crew registration - get new model-view with skills for the dropdowns in the view.
    /// </summary>
    /// <returns>CrewRegistrationStepTwoInputModel</returns>
    CrewRegistrationStepTwoInputModel GetNewModelWithSkills();

    /// <summary>
    /// Helper method to get skills for the dropdowns in the view.
    /// It populates the SkillsByDepartment property of the input model.
    /// </summary>
    /// <param name="inputModel">CrewRegistrationStepTwoInputModel</param>
    void GetSkills(CrewRegistrationStepTwoInputModel inputModel);

    /// <summary>
    /// Completes the crew registration process by taking the draft and the selected skills
    /// and creating a new crew member in the system vie transaction.
    /// </summary>
    /// <param name="draft">CrewRegistrationStepOneDraft</param>
    /// <param name="skills">IReadOnlyCollection<int></param>
    /// <returns>IdentityResult -> Success or not </returns>
    Task<IdentityResult> CompleteCrewRegistrationAsync(CrewRegistrationStepOneDraft draft, IReadOnlyCollection<int> skills);

    /// <summary>
    /// Creates and completes a new cast member in the system based on the input model.
    /// </summary>
    /// <param name="model">CastRegistrationInputModel</param>
    /// <returns>IdentityResult -> Success or not </returns>
    Task<IdentityResult> CompleteCastRegistrationAsync(CastRegistrationInputModel model);

    /// <summary>
    /// In Login process, checks if the provided username and password are correct.
    /// </summary>
    /// <param name="model">AccountLogInInputModel</param>
    /// <returns>bool -> user is not null</returns>
    Task<bool> IsUsernameAndPasswordCorrectAsync(AccountLogInInputModel model);
    
    /// <summary>
    /// Checks the Login status and role in the application (cast or crew)
    /// and returns a tuple with the result and the role as string.
    /// </summary>
    /// <param name="model">AccountLogInInputModel</param>
    /// <returns>Tuple<bool, string></returns>
    Task<(bool, string)> LoginStatusAsync(AccountLogInInputModel model);

    /// <summary>
    /// Logout the current user from the system.
    /// </summary>
    Task LogoutAsync();
}
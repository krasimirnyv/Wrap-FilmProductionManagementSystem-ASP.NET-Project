namespace Wrap.Services.Core.Interface;

using Microsoft.AspNetCore.Identity;

using ViewModels.LoginAndRegistration;

public interface IWrapAccountService
{
    Task<CrewRegistrationStepOneDraft> BuildCrewDraftAsync(CrewRegistrationStepOneInputModel model);

    CrewRegistrationStepTwoInputModel GetNewModelWithSkills();

    void GetSkills(CrewRegistrationStepTwoInputModel inputModel);

    Task<IdentityResult> CompleteCrewRegistrationAsync(CrewRegistrationStepOneDraft draft, IReadOnlyCollection<int> skills);

    Task<IdentityResult> CompleteCastRegistrationAsync(CastRegistrationInputModel model);

    Task<bool> IsUsernameAndPasswordCorrectAsync(AccountLogInInputModel model);
    
    Task<(bool, string)> LoginStatusAsync(AccountLogInInputModel model);

    Task LogoutAsync();
}
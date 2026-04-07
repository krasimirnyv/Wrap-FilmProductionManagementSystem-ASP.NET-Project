namespace Wrap.Services.Core.Handlers;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Models.LoginAndRegistration;
using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;

using static GCommon.OutputMessages.Register;
using static GCommon.ApplicationConstants.IdentityRoles;

public class CrewRegistrationHandler(UserManager<ApplicationUser> userManager,
                                     SignInManager<ApplicationUser> signInManager,
                                     ILoginRegisterRepository loginRegisterRepository,
                                     ILogger<CrewRegistrationHandler> logger) : RegistrationHandlerBase<CrewRegistrationCompleteDto>(userManager,
                                                                                                                                     signInManager,
                                                                                                                                     loginRegisterRepository,
                                                                                                                                     logger)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILoginRegisterRepository _loginRegisterRepository = loginRegisterRepository;

    protected override IdentityResult ValidateDto(CrewRegistrationCompleteDto? registrationDto)
    {
        if (registrationDto is null)
        {
            logger.LogError(ErrorCreatingCrew);
            return IdentityResult.Failed(new IdentityError
            {
                Description = ErrorCreatingCrew
            });
        }

        if (registrationDto.Draft is null)
        {
            logger.LogError(ErrorFoundingCrewDraft);
            return IdentityResult.Failed(new IdentityError
            {
                Description = ErrorFoundingCrewDraft
            });
        }
        
        if (registrationDto.SkillNumbers is null || registrationDto.SkillNumbers.Count == 0)
        {
            logger.LogError(NoSelectedSkills);
            return IdentityResult.Failed(new IdentityError
            {
                Description = NoSelectedSkills
            });
        }
        
        return IdentityResult.Success;
    }
    
    protected override ApplicationUser BuildUser(CrewRegistrationCompleteDto registrationDto)
    {
        CrewRegistrationDraftDto draft = registrationDto.Draft!;
        
        ApplicationUser user = new ApplicationUser
        {
            UserName = draft.UserName,
            Email = draft.Email,
            PhoneNumber = draft.PhoneNumber
        };

        return user;
    }

    protected override async Task<IdentityResult> AssignRolesAsync(ApplicationUser user, CrewRegistrationCompleteDto registrationDto)
        => await _userManager.AddToRoleAsync(user, Filmmaker);
    
    protected override string GetPassword(CrewRegistrationCompleteDto registrationDto)
    {
        string password = registrationDto.Draft!.Password;
        return password;
    }

    protected override async Task<int> PersistDomainDataAsync(CrewRegistrationCompleteDto registrationDto, ApplicationUser user)
    {
        CrewRegistrationDraftDto draft = registrationDto.Draft!;
        IReadOnlyCollection<int> skills = registrationDto.SkillNumbers!;
        
        Crew newCrew = new Crew
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ProfileImagePath = draft.ProfilePicturePath,
            FirstName = draft.FirstName,
            LastName = draft.LastName,
            Nickname = draft.Nickname,
            Biography = draft.Biography,
            IsActive = false,
            IsDeleted = false
        };
        
        await _loginRegisterRepository.CreateCrewAsync(newCrew);
        await _loginRegisterRepository.AddCrewSkillsAsync(newCrew.Id, skills);
        
        int expectedRows = 1 + skills.Count;
        return expectedRows;
    }
}
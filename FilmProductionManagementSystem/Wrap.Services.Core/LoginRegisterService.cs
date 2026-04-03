namespace Wrap.Services.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Interfaces;
using Utilities.ImageLogic.Interfaces;
using Handlers.Interfaces;
using Models.LoginAndRegistration;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using GCommon.UI;

using static GCommon.OutputMessages;
using static GCommon.OutputMessages.Register;
using static GCommon.DataFormat;

public class LoginRegisterService(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  ILoginRegisterRepository loginRegisterRepository,
                                  IImageService imageService,
                                  IVariantImageStrategyResolver imageStrategyResolver,
                                  IRegistrationHandlerResolver registrationHandlerResolver,
                                  ILogger<LoginRegisterService> logger) : ILoginRegisterService
{
 
    public async Task<CrewRegistrationDraftDto?> BuildCrewDraftAsync(CrewRegistrationStepOneDto dto)
    {
        try
        {
            IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ProfileFolderName);
            string profilePath = await imageService.SaveImageAsync(dto.ProfilePicture, strategy);
            
            CrewRegistrationDraftDto draft = new CrewRegistrationDraftDto
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Password = dto.Password,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Nickname = dto.Nickname,
                Biography = dto.Biography,
                ProfilePicturePath = profilePath
            };

            return draft;
        }
        catch (NotSupportedException nse)
        {
            throw new NotSupportedException(nse.Message, nse);
        }
        catch (Exception e)
        {
            throw new Exception(ExceptionBuildingCrewDraft, e);
        }
    }

    public CrewRegistrationStepTwoDto GetNewModelWithSkills()
    {
        CrewRegistrationStepTwoDto dto = new CrewRegistrationStepTwoDto();
        
        GetSkills(dto);
        return dto;
    }

    public void GetSkills(CrewRegistrationStepTwoDto dto)
    {
        dto.SkillsByDepartment = CrewRolesDepartmentCatalog.GetRolesByDepartment();
    }

    public void GetSkills(CrewRegistrationCompleteDto dto)
    {
        dto.SkillsByDepartment = CrewRolesDepartmentCatalog.GetRolesByDepartment();
    }

    public async Task<IdentityResult> CompleteRegistrationAsync<TRegistrationDto>(TRegistrationDto registrationDto)
    {
        IRegistrationHandler<TRegistrationDto> handler = registrationHandlerResolver.Resolve<TRegistrationDto>();
        
        IdentityResult result = await handler.CompleteRegistrationAsync(registrationDto);
        
        return result;
    }

    public async Task<LoginStatusDto> LoginStatusAsync(LoginRequestDto? loginDto)
    {
        LoginStatusDto statusDto = new LoginStatusDto();
        if (loginDto is null || (loginDto.Role != CrewString && loginDto.Role != CastString))
        {
            statusDto.IsSucceeded = false;
            statusDto.Role = string.Empty;
            return statusDto;
        }
        
        ApplicationUser? user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user is null)
        {
            logger.LogError(string.Format(UserNotFound, loginDto.UserName));
            statusDto.IsSucceeded = false;
            statusDto.Role = string.Empty;
            return statusDto;
        }

        bool isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
        {
            logger.LogError(string.Format(LoginFailedPass, loginDto.UserName));
            statusDto.IsSucceeded = false;
            statusDto.Role = string.Empty;
            return statusDto;
        }
        
        switch (loginDto.Role)
        {
            case CrewString:
            {
                bool crewExist = await loginRegisterRepository.CrewExistsByUserIdAsync(user.Id);
                if (!crewExist)
                {
                    statusDto.IsSucceeded = false;
                    statusDto.Role = CrewString;
                    return statusDto;
                }
                
                SignInResult result = await signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
                if (!result.Succeeded)
                {
                    logger.LogError(string.Format(LoginFailedPass, loginDto.UserName));
                    statusDto.IsSucceeded = false;
                    statusDto.Role = string.Empty;
                    return statusDto;
                }
                
                statusDto.IsSucceeded = true;
                statusDto.Role = CrewString;
                return statusDto;
            }
            case CastString:
            {
                bool castExist = await loginRegisterRepository.CastExistsByUserIdAsync(user.Id);
                if (!castExist)
                {
                    statusDto.IsSucceeded = false;
                    statusDto.Role = CastString;
                    return statusDto;
                }

                SignInResult result = await signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
                if (!result.Succeeded)
                {
                    logger.LogError(string.Format(LoginFailedPass, loginDto.UserName));
                    statusDto.IsSucceeded = false;
                    statusDto.Role = string.Empty;
                    return statusDto;
                }
                
                statusDto.IsSucceeded = true;
                statusDto.Role = CastString;
                return statusDto;
            }
            default:
                logger.LogError(LoginFailedRole);
                statusDto.IsSucceeded = false;
                statusDto.Role = string.Empty;
                return statusDto;
        }
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}
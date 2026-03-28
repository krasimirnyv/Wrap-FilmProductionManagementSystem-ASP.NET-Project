namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using Interfaces;
using Models.LoginAndRegistration;
using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using GCommon.UI;

using static Utilities.HelperSaveProfile;
using static GCommon.OutputMessages;
using static GCommon.OutputMessages.Register;

public class LoginRegisterService(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  IWebHostEnvironment environment,
                                  ILoginRegisterRepository loginRegisterRepository,
                                  ILogger<LoginRegisterService> logger) : ILoginRegisterService
{
 
    public async Task<CrewRegistrationDraftDto?> BuildCrewDraftAsync(CrewRegistrationStepOneDto dto)
    {
        try
        {
            string profilePath = await SaveProfileImageAsync(environment, dto.ProfilePicture);

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
    
    public async Task<IdentityResult> CompleteCrewRegistrationAsync(CrewRegistrationCompleteDto? registrationDto)
    {
        if (registrationDto is null || registrationDto.Draft is null)
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
        
        CrewRegistrationDraftDto draft = registrationDto.Draft;
        IReadOnlyCollection<int> skills = registrationDto.SkillNumbers;
        
        ApplicationUser user = new ApplicationUser
        {
            UserName = draft.UserName,
            Email = draft.Email,
            PhoneNumber = draft.PhoneNumber
        };
        
        await using IDbContextTransaction transaction = await loginRegisterRepository.BeginTransactionAsync();
        try
        {
            IdentityResult result = await userManager.CreateAsync(user, draft.Password);
            if (!result.Succeeded)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                
                foreach (IdentityError error in result.Errors) 
                    logger.LogError(string.Format(IdentityCreateFailed, error.Description));
                
                return result;
            }
            
            Crew newCrew = new Crew
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProfileImagePath = draft.ProfilePicturePath!,
                FirstName = draft.FirstName,
                LastName = draft.LastName,
                Nickname = draft.Nickname,
                Biography = draft.Biography,
                IsActive = true,
                IsDeleted = false
            };
            
            await loginRegisterRepository.CreateCrewAsync(newCrew);
            await loginRegisterRepository.AddCrewSkillsAsync(newCrew.Id, skills);
            
            int expectedRows = 1 + skills.Count;
            int effectedRows = await loginRegisterRepository.SaveAllChangesAsync();
            if (effectedRows < expectedRows)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                await userManager.DeleteAsync(user);
                
                logger.LogError(RegistrationTransactionFailure + string.Format(EffectedDbRowsFailure, expectedRows, effectedRows));
                return IdentityResult.Failed(new IdentityError
                {
                    Description = RegistrationTransactionFailure
                });
            }
            
            await loginRegisterRepository.CommitTransactionAsync(transaction);
            await signInManager.SignInAsync(user, isPersistent: false);
        
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            await loginRegisterRepository.RollbackTransactionAsync(transaction);
            try { await userManager.DeleteAsync(user); } catch { /* ignored */ }

            logger.LogError(e, RegistrationTransactionFailure + e.Message);
            return IdentityResult.Failed(new IdentityError
            {
                Description = RegistrationTransactionFailure
            });
        }
    }

    public async Task<IdentityResult> CompleteCastRegistrationAsync(CastRegistrationDto? registrationDto)
    {
        if (registrationDto is null)
        {
            logger.LogError(ErrorCreatingCast);
            return IdentityResult.Failed(new IdentityError
            {
                Description = ErrorCreatingCast
            });
        }
        
        ApplicationUser user = new ApplicationUser
        {
            UserName = registrationDto.UserName,
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber
        };
        
        await using IDbContextTransaction transaction = await loginRegisterRepository.BeginTransactionAsync();
        try
        {
            IdentityResult result = await userManager.CreateAsync(user, registrationDto.Password);
            if (!result.Succeeded)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                
                foreach (IdentityError error in result.Errors) 
                    logger.LogError(string.Format(IdentityCreateFailed, error.Description));
                
                return result;
            }
            
            string profilePath = await SaveProfileImageAsync(environment, registrationDto.ProfilePicture);
            
            Cast newCast = new Cast
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ProfileImagePath = profilePath,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Nickname = registrationDto.Nickname,
                BirthDate = registrationDto.BirthDate,
                Gender = registrationDto.Gender,
                Biography = registrationDto.Biography,
                IsActive = true,
                IsDeleted = false
            };
            
            await loginRegisterRepository.CreateCastAsync(newCast);

            byte expectedRows = 1;
            int effectedRows = await loginRegisterRepository.SaveAllChangesAsync();
            if (effectedRows < expectedRows)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                await userManager.DeleteAsync(user);
                
                logger.LogError(RegistrationTransactionFailure + string.Format(EffectedDbRowsFailure, expectedRows, effectedRows));
                return IdentityResult.Failed(new IdentityError
                {
                    Description = RegistrationTransactionFailure
                });
            }
            
            await loginRegisterRepository.CommitTransactionAsync(transaction);
            await signInManager.SignInAsync(user, false);
            
            return IdentityResult.Success;
        }
        catch (NotSupportedException nse)
        {
            await loginRegisterRepository.RollbackTransactionAsync(transaction);
            try { await userManager.DeleteAsync(user); } catch { /* ignored */ }
            
            logger.LogError(nse, RegistrationTransactionFailure + nse.Message);
            IdentityResult.Failed(new IdentityError
            {
                Description = RegistrationTransactionFailure
            });
            
            throw new NotSupportedException(nse.Message, nse);
        }
        catch (Exception e)
        {
            await loginRegisterRepository.RollbackTransactionAsync(transaction);
            try { await userManager.DeleteAsync(user); } catch { /* ignored */ }
            
            logger.LogError(e, RegistrationTransactionFailure + e.Message);
            return IdentityResult.Failed(new IdentityError
            {
                Description = RegistrationTransactionFailure
            });
        }
    }
    
    public async Task<LoginStatusDto> LoginStatusAsync(LoginRequestDto? loginDto)
    {
        LoginStatusDto statusDto = new LoginStatusDto();

        if (loginDto is null || loginDto.Role != CrewString && loginDto.Role != CastString)
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

        SignInResult result = await signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
        if (!result.Succeeded)
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
                if (crewExist)
                {
                    statusDto.IsSucceeded = true;
                    statusDto.Role = CrewString;
                
                    return statusDto;
                }

                await signInManager.SignOutAsync();
            
                statusDto.IsSucceeded = false;
                statusDto.Role = CrewString;
            
                return statusDto;
            }
            case CastString:
            {
                bool castExist = await loginRegisterRepository.CastExistsByUserIdAsync(user.Id);
                if (castExist)
                {
                    statusDto.IsSucceeded = true;
                    statusDto.Role = CastString;
                
                    return statusDto;
                }

                await signInManager.SignOutAsync();
            
                statusDto.IsSucceeded = false;
                statusDto.Role = CastString;
            
                return statusDto;
            }
            default:
                await signInManager.SignOutAsync();
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
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
using ViewModels.LoginAndRegistration;
using GCommon.UI;

using static Utilities.HelperSaveProfile;
using static GCommon.OutputMessages;
using static GCommon.OutputMessages.Register;

public class LoginRegisterService(UserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  IWebHostEnvironment environment,
                                  ILoginRegisterRepository repository,
                                  ILogger<LoginRegisterService> logger) : ILoginRegisterService
{
 
    public async Task<CrewRegistrationDraftDto?> BuildCrewDraftAsync(CrewRegistrationStepOneInputModel model)
    {
        try
        {
            string profilePath = await SaveProfileImageAsync(environment, model.ProfilePicture);

            CrewRegistrationDraftDto draft = new CrewRegistrationDraftDto
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Nickname = model.Nickname,
                Biography = model.Biography,
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

    public CrewRegistrationStepTwoInputModel GetNewModelWithSkills()
    { 
        CrewRegistrationStepTwoInputModel inputModel = new CrewRegistrationStepTwoInputModel();

        GetSkills(inputModel);
        return inputModel;
    }

    public void GetSkills(CrewRegistrationStepTwoInputModel inputModel)
    {
        inputModel.SkillsByDepartment = CrewRolesDepartmentCatalog.GetRolesByDepartment();
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
        
        await using IDbContextTransaction transaction = await repository.BeginTransactionAsync();
        try
        {
            IdentityResult result = await userManager.CreateAsync(user, draft.Password);
            if (!result.Succeeded)
            {
                await repository.RollbackTransactionAsync(transaction);
                
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
            
            await repository.CreateCrewAsync(newCrew);
            await repository.AddCrewSkillsAsync(newCrew.Id, skills);
            
            int expectedRows = 1 + skills.Count;
            int effectedRows = await repository.SaveAllChangesAsync();
            if (effectedRows < expectedRows)
            {
                await repository.RollbackTransactionAsync(transaction);
                await userManager.DeleteAsync(user);
                
                logger.LogError(RegistrationTransactionFailure + string.Format(EffectedDbRowsFailure, expectedRows, effectedRows));
                return IdentityResult.Failed(new IdentityError
                {
                    Description = RegistrationTransactionFailure
                });
            }
            
            await repository.CommitTransactionAsync(transaction);
            await signInManager.SignInAsync(user, isPersistent: false);
        
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            await repository.RollbackTransactionAsync(transaction);
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
        
        await using IDbContextTransaction transaction = await repository.BeginTransactionAsync();
        try
        {
            IdentityResult result = await userManager.CreateAsync(user, registrationDto.Password);
            if (!result.Succeeded)
            {
                await repository.RollbackTransactionAsync(transaction);
                
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
            
            await repository.CreateCastAsync(newCast);

            byte expectedRows = 1;
            int effectedRows = await repository.SaveAllChangesAsync();
            if (effectedRows < expectedRows)
            {
                await repository.RollbackTransactionAsync(transaction);
                await userManager.DeleteAsync(user);
                
                logger.LogError(RegistrationTransactionFailure + string.Format(EffectedDbRowsFailure, expectedRows, effectedRows));
                return IdentityResult.Failed(new IdentityError
                {
                    Description = RegistrationTransactionFailure
                });
            }
            
            await repository.CommitTransactionAsync(transaction);
            await signInManager.SignInAsync(user, false);
            
            return IdentityResult.Success;
        }
        catch (NotSupportedException nse)
        {
            await repository.RollbackTransactionAsync(transaction);
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
            await repository.RollbackTransactionAsync(transaction);
            try { await userManager.DeleteAsync(user); } catch { /* ignored */ }
            
            logger.LogError(e, RegistrationTransactionFailure + e.Message);
            return IdentityResult.Failed(new IdentityError
            {
                Description = RegistrationTransactionFailure
            });
        }
    }
    
    public async Task<(bool Succeeded, string Role)> LoginStatusAsync(LoginRequestDto? loginDto)
    {
        if (loginDto is null || loginDto.Role != CrewString && loginDto.Role != CastString)
            return (false, string.Empty);

        ApplicationUser? user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user is null)
        {
            logger.LogError(string.Format(UserNotFound, loginDto.UserName));
            return (false, string.Empty);
        }

        SignInResult result = await signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
        if (!result.Succeeded)
        {
            logger.LogError(string.Format(LoginFailedPass, loginDto.UserName));
            return (false, string.Empty);
        }

        if (loginDto.Role == CrewString)
        {
            bool crewExist = await repository.CrewExistsByUserIdAsync(user.Id);
            if (crewExist)
                return (true, CrewString);

            await signInManager.SignOutAsync();
            return (false, CrewString);
        }

        if (loginDto.Role == CastString)
        {
            bool castExist = await repository.CastExistsByUserIdAsync(user.Id);
            if (castExist)
                return (true, CastString);

            await signInManager.SignOutAsync();
            return (false, CrewString);
        }

        await signInManager.SignOutAsync();
        logger.LogError(LoginFailedRole);
        return (false, string.Empty);
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }
}
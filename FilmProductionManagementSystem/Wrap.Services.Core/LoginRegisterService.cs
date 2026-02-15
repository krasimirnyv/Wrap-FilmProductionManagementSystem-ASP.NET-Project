namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;

using Data;
using Data.Models;
using Data.Models.Infrastructure;

using ViewModels.LoginAndRegistration;
using ViewModels.LoginAndRegistration.Helpers;

using Interface;
using GCommon.Enums;

using static Utilities.HelperSaveProfile;

public class LoginRegisterService(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                FilmProductionDbContext context,
                                IWebHostEnvironment environment) : ILoginRegisterService
{
 
    public async Task<CrewRegistrationStepOneDraft> BuildCrewDraftAsync(CrewRegistrationStepOneInputModel model)
    {
        CrewRegistrationStepOneDraft draftModel = new CrewRegistrationStepOneDraft
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Nickname = model.Nickname,
            Biography = model.Biography,
            ProfilePicturePath = await SaveProfileImageAsync(environment, model.ProfilePicturePath)
        };

        return draftModel;
    }

    public CrewRegistrationStepTwoInputModel GetNewModelWithSkills()
    { 
        CrewRegistrationStepTwoInputModel inputModel = new CrewRegistrationStepTwoInputModel();

        GetSkills(inputModel);
        return inputModel;
    }

    public void GetSkills(CrewRegistrationStepTwoInputModel inputModel)
    {
        inputModel.SkillsByDepartment = CrewRolesDepartments.GetRolesByDepartment();
    }

    public async Task<IdentityResult> CompleteCrewRegistrationAsync(CrewRegistrationStepOneDraft draft, IReadOnlyCollection<int> skills)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = draft.UserName,
            Email = draft.Email,
            PhoneNumber = draft.PhoneNumber
        };

        IdentityResult result = await userManager.CreateAsync(user, draft.Password);
        if (!result.Succeeded)
            return result;
        
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

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

        await context.CrewMembers.AddAsync(newCrew);

        foreach (CrewRoleType skill in skills)
        {
            await context.CrewSkills.AddAsync(new CrewSkill
            {
                Id = Guid.NewGuid(),
                CrewMemberId = newCrew.Id,
                RoleType = skill
            });
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        await signInManager.SignInAsync(user, false);
        
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> CompleteCastRegistrationAsync(CastRegistrationInputModel model)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };
        
        IdentityResult result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return result;

        Cast newCast = new Cast
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ProfileImagePath = await SaveProfileImageAsync(environment, model.ProfilePicturePath),
            FirstName = model.FirstName,
            LastName = model.LastName,
            Nickname = model.Nickname,
            BirthDate = model.BirthDate,
            Gender = model.Gender,
            Biography = model.Biography,
            IsActive = true,
            IsDeleted = false
        };

        await context.CastMembers.AddAsync(newCast);
        await context.SaveChangesAsync();
        
        await signInManager.SignInAsync(user, false);
        
        return IdentityResult.Success;
    }

    public async Task<bool> IsUsernameAndPasswordCorrectAsync(AccountLogInInputModel model)
    {
        ApplicationUser? user = await GetValueAsync(model);
        return user is not null;
    }

    public async Task<(bool, string)> LoginStatusAsync(AccountLogInInputModel model)
    {
        ApplicationUser? user = await GetValueAsync(model);

        if (user is null)
            return (false, string.Empty);
        
        switch (model.Role)
        {
            case "Crew":
            {
                bool crewExist = await context.CrewMembers
                    .AnyAsync(c => c.UserId == user.Id);

                if (crewExist) 
                    return (true, model.Role);
                
                await signInManager.SignOutAsync();
                return (false, "Crew");

            }
            case "Cast":
            {
                bool castExist = await context.CastMembers
                    .AnyAsync(c => c.UserId == user.Id);

                if (castExist) 
                    return (true, model.Role);
                
                await signInManager.SignOutAsync();
                return (false, "Cast");

            }
            default:
                await signInManager.SignOutAsync();
                return (false, model.Role);
        }
    }

    public async Task LogoutAsync()
    {
        await signInManager.SignOutAsync();
    }

    /// <summary>
    /// Gets the user based on the provided username and password in the login form.
    /// </summary>
    /// <param name="model">AccountLogInInputModel</param>
    /// <returns>ApplicationUser -> if succeeded returns user, else - null</returns>
    private async Task<ApplicationUser?> GetValueAsync(AccountLogInInputModel model)
    {
        ApplicationUser? user = await userManager.FindByNameAsync(model.UserName);
        
        if (user is null)
            return null;
        
        SignInResult signInResult = await signInManager
            .PasswordSignInAsync(user, model.Password, model.RememberMe, false);

        return signInResult.Succeeded ? user : null;
    }
}
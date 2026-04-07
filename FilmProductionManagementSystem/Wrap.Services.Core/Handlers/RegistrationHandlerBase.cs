namespace Wrap.Services.Core.Handlers;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Interfaces;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;

using static GCommon.OutputMessages.Register;

public abstract class RegistrationHandlerBase<TRegistrationDto>(UserManager<ApplicationUser> userManager, 
                                                                SignInManager<ApplicationUser> signInManager,
                                                                ILoginRegisterRepository loginRegisterRepository,
                                                                ILogger<RegistrationHandlerBase<TRegistrationDto>> logger) : IRegistrationHandler<TRegistrationDto>
{
    public async Task<IdentityResult> CompleteRegistrationAsync(TRegistrationDto registrationDto)
    {
        IdentityResult? identityResult = ValidateDto(registrationDto);
        if (identityResult is { Succeeded: false })
            return identityResult;
        
        ApplicationUser user = BuildUser(registrationDto);

        await using IDbContextTransaction transaction = await loginRegisterRepository.BeginTransactionAsync();
        try
        {
            IdentityResult createResult = await CreateIdentityUserAsync(user, registrationDto);
            if (!createResult.Succeeded)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                LogIdentityErrors(createResult);
                return createResult;
            }

            IdentityResult roleResult = await AssignRolesAsync(user, registrationDto);
            if (!roleResult.Succeeded)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                try { await userManager.DeleteAsync(user); } catch { /* ignored */ }

                LogIdentityErrors(roleResult);
                return roleResult;
            }

            int expectedRows = await PersistDomainDataAsync(registrationDto, user);
            int affectedRows = await loginRegisterRepository.SaveAllChangesAsync();
            if (affectedRows < expectedRows)
            {
                await loginRegisterRepository.RollbackTransactionAsync(transaction);
                await userManager.DeleteAsync(user);

                logger.LogError(RegistrationTransactionFailure + string.Format(EffectedDbRowsFailure, expectedRows, affectedRows));
                
                return IdentityResult.Failed(new IdentityError
                {
                    Description = RegistrationTransactionFailure
                });
            }

            await loginRegisterRepository.CommitTransactionAsync(transaction);
            await signInManager.SignInAsync(user, false);

            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            await loginRegisterRepository.RollbackTransactionAsync(transaction);
            try { await userManager.DeleteAsync(user); }catch { /* ignored */ }

            LogUnhandledException(e);
            return IdentityResult.Failed(new IdentityError
            {
                Description = RegistrationTransactionFailure
            });
        }
    }

    protected virtual async Task<IdentityResult> CreateIdentityUserAsync(ApplicationUser user, TRegistrationDto registrationDto)
    {
        string password = GetPassword(registrationDto);
        IdentityResult result = await userManager.CreateAsync(user, password);
        
        return result;
    }
    
    protected virtual async Task<IdentityResult> AssignRolesAsync(ApplicationUser user, TRegistrationDto registrationDto)
        => await Task.FromResult(IdentityResult.Success);
    
    protected virtual void LogIdentityErrors(IdentityResult result)
    {
        foreach (IdentityError error in result.Errors)
            logger.LogError(string.Format(IdentityCreateFailed, error.Description));
    }
    
    protected virtual void LogUnhandledException(Exception exception)
    {
        logger.LogError(exception, RegistrationTransactionFailure + exception.Message);
    }
    
    protected abstract IdentityResult? ValidateDto(TRegistrationDto registrationDto);
    
    protected abstract ApplicationUser BuildUser(TRegistrationDto registrationDto);
    
    protected abstract string GetPassword(TRegistrationDto registrationDto);
    
    protected abstract Task<int> PersistDomainDataAsync(TRegistrationDto registrationDto, ApplicationUser user);
}
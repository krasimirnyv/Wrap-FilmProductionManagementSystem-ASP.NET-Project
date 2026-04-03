namespace Wrap.Services.Core.Handlers;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Utilities.ImageLogic.Interfaces;
using Models.LoginAndRegistration;
using Data.Models;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;

using static GCommon.OutputMessages.Register;
using static GCommon.DataFormat;

public class CastRegistrationHandler(UserManager<ApplicationUser> userManager,
                                     SignInManager<ApplicationUser> signInManager,
                                     IImageService imageService,
                                     IVariantImageStrategyResolver imageStrategyResolver,
                                     ILoginRegisterRepository loginRegisterRepository,
                                     ILogger<CastRegistrationHandler> logger) : RegistrationHandlerBase<CastRegistrationDto>(userManager,
                                                                                                                             signInManager,
                                                                                                                             loginRegisterRepository,
                                                                                                                             logger)
{
    private readonly ILoginRegisterRepository repository = loginRegisterRepository;
    
    protected override IdentityResult ValidateDto(CastRegistrationDto? registrationDto)
    {
        if (registrationDto is null)
        {
            logger.LogError(ErrorCreatingCrew);
            return IdentityResult.Failed(new IdentityError
            {
                Description = ErrorCreatingCrew
            });
        }
        
        return IdentityResult.Success;
    }

    protected override ApplicationUser BuildUser(CastRegistrationDto registrationDto)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = registrationDto.UserName,
            Email = registrationDto.Email,
            PhoneNumber = registrationDto.PhoneNumber
        };

        return user;
    }

    protected override string GetPassword(CastRegistrationDto registrationDto)
    {
        string password = registrationDto.Password;
        return password;
    }

    protected override async Task<int> PersistDomainDataAsync(CastRegistrationDto registrationDto, ApplicationUser user)
    {
        IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ProfileFolderName);
        string profilePath = await imageService.SaveImageAsync(registrationDto.ProfilePicture, strategy);

        Cast newCast = new Cast
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ProfileImagePath = profilePath,
            FirstName = registrationDto.FirstName,
            LastName = registrationDto.LastName,
            Nickname = registrationDto.Nickname,
            Biography = registrationDto.Biography,
            BirthDate = registrationDto.BirthDate,
            Gender = registrationDto.Gender,
            IsActive = false,
            IsDeleted = false
        };
        
        await repository.CreateCastAsync(newCast);

        int expectedRows = 1;
        return expectedRows;
    }
}
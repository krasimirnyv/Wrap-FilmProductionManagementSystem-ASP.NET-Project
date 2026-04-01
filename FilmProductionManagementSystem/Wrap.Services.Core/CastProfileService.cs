namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Interfaces;
using Utilities.ImageLogic.Interfaces;
using Data.Models;
using Data.Models.MappingEntities;
using Data.Repository.Interfaces;
using Models.Profile;
using Models.Profile.NestedDtos;

using static GCommon.OutputMessages.Profile;
using static GCommon.DataFormat;

public class CastProfileService(IProfileRepository profileRepository,
                                IImageService imageService,
                                IVariantImageStrategyResolver imageStrategyResolver,
                                ILogger<CastProfileService> logger) : ICastProfileService
{
    public async Task<CastProfileDto> GetCastProfileDataAsync(string username)
    {
        Cast? cast = await profileRepository.GetCastByUsernameAsync(username);
        if (cast is null)
            throw new ArgumentNullException(string.Format(CastNotFoundMessage, username));

        IReadOnlyCollection<ProductionCast> productions = await profileRepository.GetCastProductionsAsync(cast.Id);
        IReadOnlyCollection<SceneCast> scenes = await profileRepository.GetCastScenesAsync(cast.Id);
        
        CastProfileDto castProfile = new CastProfileDto
        {
            FirstName = cast.FirstName,
            LastName = cast.LastName,
            ProfileImagePath = cast.ProfileImagePath!,
            Nickname = cast.Nickname ?? EmptyNickname,
            UserName = cast.User.UserName!,
            Email = cast.User.Email!,
            PhoneNumber = cast.User.PhoneNumber!,
            Age = cast.Age.ToString(),
            Gender = cast.Gender.ToString(),
            IsActive = cast.IsActive,
            Biography = cast.Biography,
            
            Productions = productions
                .Select(pc => new CastMemberProductionDto
                {
                    ProductionId = pc.ProductionId.ToString(),
                    ProductionTitle = pc.Production.Title,
                    ProjectStatus = pc.Production.StatusType.ToString(),
                    CharacterName = pc.Role,
                })
                .ToArray()
                .AsReadOnly(),
            
            Scenes = scenes
                .Select(sc => new CastMemberSceneDto
                {
                    SceneId = sc.SceneId.ToString(),
                    SceneName = sc.Scene.SceneName,
                    ProductionTitle = sc.Scene.Production.Title,
                    CharacterName = sc.Role
                })
                .ToArray()
                .AsReadOnly()
        };
        
        return castProfile;
    }

    public async Task<EditCastProfileDto> GetEditCastProfileAsync(string username)
    {
        Cast? cast = await profileRepository.GetCastByUsernameAsync(username);
        if (cast is null)
            throw new ArgumentNullException(string.Format(CastNotFoundMessage, username));

        EditCastProfileDto editCastDto = new EditCastProfileDto
        {
            FirstName = cast.FirstName,
            LastName = cast.LastName,
            Nickname = cast.Nickname,
            PhoneNumber = cast.User.PhoneNumber!,
            Biography = cast.Biography,
            ProfileImage = null,
            // Read-only properties
            Email = cast.User.Email,
            CurrentProfileImagePath = cast.ProfileImagePath,
            Age = cast.Age.ToString(),
            Gender = cast.Gender.ToString(),
        };

        return editCastDto;
    }
    
    public async Task UpdateCastProfileAsync(string username, EditCastProfileDto castDto)
    {
        await using IDbContextTransaction transaction = await profileRepository.BeginTransactionAsync();
        try
        {
            Cast? cast = await profileRepository.GetCastForUpdateAsync(username);
            if (cast is null)
                throw new ArgumentNullException(string.Format(CastNotFoundMessage, username));
        
            cast.FirstName = castDto.FirstName;
            cast.LastName = castDto.LastName;
            cast.Nickname = castDto.Nickname;
            cast.Biography = castDto.Biography;
            cast.User.PhoneNumber = castDto.PhoneNumber;
        
            if (castDto.ProfileImage is not null && castDto.ProfileImage.Length > 0)
            {
                IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ProfileFolderName);
                string newImagePath = await imageService.ReplaceAsync(castDto.CurrentProfileImagePath, castDto.ProfileImage, strategy);
                cast.ProfileImagePath = newImagePath;
            }
        
            await profileRepository.SaveAllChangesAsync();
            await profileRepository.CommitTransactionAsync(transaction);
        }
        catch (NotSupportedException nse)
        {
            await profileRepository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + nse.Message);
            throw new NotSupportedException(nse.Message, nse);
        }
        catch (Exception e)
        {
            await profileRepository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + e.Message);
            throw new Exception(e.Message);
        }
    }
}
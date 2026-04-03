namespace Wrap.Services.Core;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Interfaces;
using Utilities.ImageLogic.Interfaces;
using Data.Models;
using Data.Models.MappingEntities;
using Data.Models.Infrastructure;
using Data.Repository.Interfaces;
using Data.Dtos.Crew;
using Models.Profile;
using Models.Profile.NestedDtos;
using GCommon.Enums;
using GCommon.UI;

using static GCommon.OutputMessages.Profile;
using static GCommon.DataFormat;

public class CrewProfileService(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IProfileRepository profileRepository,
                                IImageService imageService,
                                IVariantImageStrategyResolver imageStrategyResolver,
                                ILogger<CrewProfileService> logger) : ICrewProfileService
{
    public async Task<CrewProfileDto> GetCrewProfileDataAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));
        
        IReadOnlyCollection<CrewSkill> skills = await profileRepository.GetCrewSkillsAsync(crew.Id);
        IEnumerable<CrewRoleType> skillTypes = skills.Select(cs => cs.RoleType);
        
        IDictionary<string, ICollection<CrewRoleType>> departmentSkills = GroupSkillsByDepartment(skillTypes);
    
        IReadOnlyCollection<ProductionCrew> productions = await profileRepository.GetCrewProductionsAsync(crew.Id);
        IReadOnlyCollection<SceneCrew> scenes = await profileRepository.GetCrewScenesAsync(crew.Id);
        
        CrewProfileDto crewProfile = new CrewProfileDto
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            ProfileImagePath = crew.ProfileImagePath!,
            Nickname = crew.Nickname ?? EmptyNickname,
            UserName = crew.User.UserName!,
            Email = crew.User.Email!,
            PhoneNumber = crew.User.PhoneNumber!,
            IsActive = crew.IsActive,
            Biography = crew.Biography,
            DepartmentSkills = departmentSkills,

            Productions = productions
                .Select(pc => new CrewMemberProductionDto
                {
                    ProductionId = pc.ProductionId.ToString(),
                    ProductionTitle = pc.Production.Title,
                    ProjectStatus = pc.Production.StatusType.ToString(),
                    RoleType = pc.RoleType
                })
                .ToArray()
                .AsReadOnly(),

            Scenes = scenes
                .Select(sc => new CrewMemberSceneDto
                {
                    SceneId = sc.SceneId.ToString(),
                    SceneName = sc.Scene.SceneName,
                    ProductionTitle = sc.Scene.Production.Title,
                    RoleType = sc.RoleType
                })
                .ToArray()
                .AsReadOnly()
        };
        
        return crewProfile;
    }

    public async Task<EditCrewProfileDto> GetEditCrewProfileAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        EditCrewProfileDto editCrewDto = new EditCrewProfileDto
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            Nickname = crew.Nickname,
            PhoneNumber = crew.User.PhoneNumber!,
            Biography = crew.Biography,
            ProfileImage = null,
            // Read-only properties
            Email = crew.User.Email,
            CurrentProfileImagePath = crew.ProfileImagePath
        };
        
        return editCrewDto;
    }

    public async Task UpdateCrewProfileAsync(string username, EditCrewProfileDto crewDto)
    {
        await using IDbContextTransaction transaction = await profileRepository.BeginTransactionAsync();
        try
        {
            Crew? crew = await profileRepository.GetCrewByUsernameAsync(username);
            if (crew is null)
                throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));
            
            crew.FirstName = crewDto.FirstName;
            crew.LastName = crewDto.LastName;
            crew.Nickname = crewDto.Nickname;
            crew.Biography = crewDto.Biography;
            crew.User.PhoneNumber = crewDto.PhoneNumber;
        
            if (crewDto.ProfileImage is not null && crewDto.ProfileImage.Length > 0)
            { 
                IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ProfileFolderName);
                string newImagePath = await imageService.ReplaceAsync(crewDto.CurrentProfileImagePath, crewDto.ProfileImage, strategy);
                crew.ProfileImagePath = newImagePath;
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
    
    public async Task<EditSkillsDto> GetEditSkillsAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        IReadOnlyCollection<CrewSkill> crewSkills = await profileRepository.GetCrewSkillsAsync(crew.Id);
        
        IReadOnlyCollection<CrewRoleType> currentSkills = crewSkills
            .Select(cs => cs.RoleType)
            .ToArray()
            .AsReadOnly();
        
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> allSkillsCatalog = CrewRolesDepartmentCatalog.GetRolesByDepartment();

        EditSkillsDto skillsDto = new EditSkillsDto
        {
            CurrentSkills = currentSkills,
            AllDepartments = allSkillsCatalog
        };
        
        return skillsDto;
    }

    public async Task UpdateSkillsAsync(string username, UpdateSkillsDto skillsDto)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsNoTrackingAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        IReadOnlyCollection<CrewSkill> currentSkills = await profileRepository.GetCrewSkillsForUpdateAsync(crew.Id);
        
        ICollection<CrewRoleType> newSkills = ParseSelectedSkills(skillsDto.SelectedSkills);
        if (newSkills.Count == 0)
            throw new ArgumentException(NoSkillsSelected);
        
        HashSet<CrewRoleType> currentSkillSet = currentSkills.Select(cs => cs.RoleType).ToHashSet();
        HashSet<CrewRoleType> newSkillSet = newSkills.ToHashSet();
        
        ICollection<CrewSkill> skillsToRemove = currentSkills
            .Where(cs => !newSkillSet.Contains(cs.RoleType))
            .ToList();
        
        ICollection<CrewSkill> skillsToAdd = newSkillSet
            .Where(crt => !currentSkillSet.Contains(crt))
            .Select(roleType => new CrewSkill
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crew.Id,
                RoleType = roleType
            })
            .ToList();
        
        await using IDbContextTransaction transaction = await profileRepository.BeginTransactionAsync();
        try
        {
            if (skillsToRemove.Count > 0)
                await profileRepository.RemoveCrewSkillsAsync(skillsToRemove);
        
            if (skillsToAdd.Count > 0)
                await profileRepository.AddCrewSkillsAsync(skillsToAdd);

            await profileRepository.SaveAllChangesAsync();
            await profileRepository.CommitTransactionAsync(transaction);
        }
        catch (Exception e)
        {
            await profileRepository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingSkills,  username) + e.Message);
            throw new Exception(e.Message);
        }
    }

    public async Task<DeleteProfileDto> GetDeleteCrewProfileAsync(string username)
    {
        Crew? crew = await profileRepository.GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        DeleteProfileDto deleteCrewDto = new DeleteProfileDto
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            ProfileImagePath = crew.ProfileImagePath!,
            UserName = crew.User.UserName!,
            Email = crew.User.Email!,
            PhoneNumber = crew.User.PhoneNumber!,
            ProductionsCount = crew.CrewMemberProductions.Count,
            ScenesCount = crew.CrewMemberScenes.Count,
            SkillsCount = crew.Skills.Count
        };
        
        return deleteCrewDto;
    }

    public async Task<bool> DeleteCrewProfileAsync(string username, DeleteProfileDto dto)
    {
        Crew? crew = await profileRepository.GetCrewByUsernameAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));
        
        ApplicationUser user = crew.User;
        bool isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            logger.LogError(string.Format(FailedPassword, username));
            return false;
        }
        
        IVariantImageStrategy strategy = imageStrategyResolver.Resolve(ProfileFolderName);
        
        await imageService.DeleteAsync(crew.ProfileImagePath, strategy);
        
        await profileRepository.DeleteCrewProfileAsync(crew.Id);
        await profileRepository.SaveAllChangesAsync();
        
        await signInManager.SignOutAsync();
        return true;
    }

    public async Task<string> DownloadCrewProfileDataAsync(string username)
    {
        CrewPersonalDataDto[]? crewPersonalData = await profileRepository.DownloadCrewDataAsync(username);
        if (crewPersonalData is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        string json = JsonConvert
            .SerializeObject(crewPersonalData, Formatting.Indented);
        
        return json;
    }

    private static ICollection<CrewRoleType> ParseSelectedSkills(string selectedSkillsString)
    {
        if (string.IsNullOrWhiteSpace(selectedSkillsString))
            return new HashSet<CrewRoleType>();
    
        ICollection<CrewRoleType> listSelectedSkills = selectedSkillsString
            .Split(CommaSplitter, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => (CrewRoleType)int.Parse(s.Trim()))
            .ToList();

        return listSelectedSkills;
    }
    
    /// <summary>
    /// Groups skills by their departments
    /// </summary>
    /// <param name="userSkills">Already selected user's skills</param>
    /// <returns>IDictionary<string, ICollection<CrewRoleType>> map with collection
    /// where the Key is the name of the department and the Values are the roles/skills</returns>
    private static IDictionary<string, ICollection<CrewRoleType>> GroupSkillsByDepartment(IEnumerable<CrewRoleType> userSkills)
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> allDepartment = CrewRolesDepartmentCatalog.GetRolesByDepartment();

        HashSet<CrewRoleType> userSkillsSet = userSkills.ToHashSet();
        
        IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<CrewRoleType>>> userDepartments = allDepartment
            .Where(dept => dept.Value.Any(role => userSkillsSet.Contains(role)))
            .ToArray()
            .AsReadOnly();
        
        IDictionary<string, ICollection<CrewRoleType>> departmentSkills = new Dictionary<string, ICollection<CrewRoleType>>();
        
        foreach (KeyValuePair<string, IReadOnlyCollection<CrewRoleType>> department in userDepartments)
        {
            ICollection<CrewRoleType> userSkillsInDept = department.Value
                .Where(role => userSkillsSet.Contains(role))
                .ToArray();

            if (userSkillsInDept.Count > 0)
                departmentSkills[department.Key] = userSkillsInDept;
        }
        
        return departmentSkills;
    }
}
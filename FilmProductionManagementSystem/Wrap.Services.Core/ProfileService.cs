namespace Wrap.Services.Core;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using Interfaces;
using Data.Models;
using Models.Profile;
using Models.Profile.NestedDtos;
using Wrap.Data.Repository.Interfaces;
using GCommon.Enums;
using GCommon.UI;

using static Utilities.HelperSaveProfile;
using static GCommon.OutputMessages.Profile;
using static GCommon.DataFormat;

public class ProfileService(IProfileRepository repository,
                            IWebHostEnvironment environment,
                            ILogger<ProfileService> logger) : IProfileService
{
    public async Task<bool> IsUserCrewAsync(string username)
    {
        Crew? crewMembers = await repository.GetCrewByUsernameAsync(username);
        return crewMembers is not null;
    }
    
    public async Task<bool> IsUserCastAsync(string username)
    {
        Cast? castMember = await repository.GetCastByUsernameAsync(username);
        return castMember is not null;
    }

    public async Task<ProfileRoleDto> GetRoleInfoAsync(string username)
    {
        Crew? crew = await repository.GetCrewByUsernameAsync(username);
        if (crew is not null)
            return new ProfileRoleDto { IsCrew = true, IsCast = false };

        Cast? cast = await repository.GetCastByUsernameAsync(username);
        if (cast is not null)
            return new ProfileRoleDto { IsCrew = false, IsCast = true };
        
        return new ProfileRoleDto { IsCrew = false, IsCast = false };
    }

    public async Task<CrewProfileDto> GetCrewProfileDataAsync(string username)
    {
        Crew? crew = await repository.GetCrewByUsernameAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));
        
        // Get user's skills with departments
        IReadOnlyCollection<CrewSkill> skills = await repository.GetCrewSkillsAsync(crew.Id);
        IEnumerable<CrewRoleType> skillTypes = skills.Select(cs => cs.RoleType);
        
        IDictionary<string, ICollection<CrewRoleType>> departmentSkills = GroupSkillsByDepartment(skillTypes);
    
        // Get production
        IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, CrewRoleType RoleType)> productions
            = await repository.GetCrewProductionsAsync(crew.Id);
        
        // Get scenes
        IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, CrewRoleType RoleType)> scenes 
            = await repository.GetCrewScenesAsync(crew.Id);
        
        // Create CrewProfileDto
        CrewProfileDto crewProfile = new CrewProfileDto
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            ProfileImagePath = crew.ProfileImagePath,
            Nickname = crew.Nickname ?? EmptyNickname,
            UserName = crew.User.UserName!,
            Email = crew.User.Email!,
            PhoneNumber = crew.User.PhoneNumber!,
            IsActive = crew.IsActive,
            Biography = crew.Biography,
            DepartmentSkills = departmentSkills,
            Productions = productions
                .Select(p => new CrewMemberProductionDto
                {
                    ProductionId = p.ProductionId.ToString(),
                    ProductionTitle = p.Title,
                    RoleType = p.RoleType,
                    ProjectStatus = p.Status
                })
                .ToArray(),
            Scenes = scenes
                .Select(s => new CrewMemberSceneDto
                {
                    SceneId = s.SceneId.ToString(),
                    SceneName = s.SceneName,
                    ProductionTitle = s.ProductionTitle,
                    RoleType = s.RoleType
                })
                .ToArray()
        };
        
        return crewProfile;
    }

    public async Task<CastProfileDto> GetCastProfileDataAsync(string username)
    {
        Cast? cast = await repository.GetCastByUsernameAsync(username);
        if (cast is null)
            throw new ArgumentNullException(string.Format(CastNotFoundMessage, username));
        
        // Get production
       IReadOnlyCollection<(Guid ProductionId, string Title, string? Description, string Status, string? CharacterName)> productions 
            = await repository.GetCastProductionsAsync(cast.Id);
        
        // Get scenes
        IReadOnlyCollection<(Guid SceneId, string SceneName, string ProductionTitle, string? CharacterName)> scenes 
            = await repository.GetCastScenesAsync(cast.Id);
        
        // Create CastProfileViewModel
        CastProfileDto castProfile = new CastProfileDto
        {
            FirstName = cast.FirstName,
            LastName = cast.LastName,
            ProfileImagePath = cast.ProfileImagePath,
            Nickname = cast.Nickname ?? EmptyNickname,
            UserName = cast.User.UserName!,
            Email = cast.User.Email!,
            PhoneNumber = cast.User.PhoneNumber!,
            Age = cast.Age.ToString(),
            Gender = cast.Gender.ToString(),
            IsActive = cast.IsActive,
            Biography = cast.Biography,
            Productions = productions
                .Select(p => new CastMemberProductionDto
                {
                    ProductionId = p.ProductionId.ToString(),
                    ProductionTitle = p.Title,
                    CharacterName = p.CharacterName,
                    ProjectStatus = p.Status
                })
                .ToArray(),
            Scenes = scenes
                .Select(s => new CastMemberSceneDto
                {
                    SceneId = s.SceneId.ToString(),
                    SceneName = s.SceneName,
                    ProductionTitle = s.ProductionTitle,
                    CharacterName = s.CharacterName
                })
                .ToArray()
        };
        
        return castProfile;
    }

    public async Task<EditCrewProfileDto> GetEditCrewProfileAsync(string username)
    {
        Crew? crew = await repository.GetCrewByUsernameAsync(username);
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
            CurrentProfileImagePath = crew.ProfileImagePath,
        };
        
        return editCrewDto;
    }

    public async Task<EditCastProfileDto> GetEditCastProfileAsync(string username)
    {
        Cast? cast = await repository.GetCastByUsernameAsync(username);
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

    public async Task UpdateCrewProfileAsync(string username, EditCrewProfileDto crewDto)
    {
        await using IDbContextTransaction transaction = await repository.BeginTransactionAsync();
        try
        {
            Crew? crew = await repository.GetCrewForUpdateAsync(username);
            if (crew is null)
                throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));
            
            crew.FirstName = crewDto.FirstName;
            crew.LastName = crewDto.LastName;
            crew.Nickname = crewDto.Nickname;
            crew.Biography = crewDto.Biography;
            crew.User.PhoneNumber = crewDto.PhoneNumber;
        
            if (crewDto.ProfileImage is not null && crewDto.ProfileImage.Length > 0)
            { 
                string newImagePath = await SaveProfileImageAsync(environment, crewDto.ProfileImage); 
                crew.ProfileImagePath = newImagePath;
            }

            await repository.SaveAllChangesAsync();
            await repository.CommitTransactionAsync(transaction);
        }
        catch (NotSupportedException nse)
        {
            await repository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + nse.Message);
            throw new NotSupportedException(nse.Message, nse);
        }
        catch (Exception e)
        {
            await repository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + e.Message);
            throw new Exception(e.Message);
        }
    }
    
    public async Task UpdateCastProfileAsync(string username, EditCastProfileDto castDto)
    {
        await using IDbContextTransaction transaction = await repository.BeginTransactionAsync();
        try
        {
            Cast? cast = await repository.GetCastForUpdateAsync(username);
            if (cast is null)
                throw new ArgumentNullException(string.Format(CastNotFoundMessage, username));
        
            cast.FirstName = castDto.FirstName;
            cast.LastName = castDto.LastName;
            cast.Nickname = castDto.Nickname;
            cast.Biography = castDto.Biography;
            cast.User.PhoneNumber = castDto.PhoneNumber;
        
            if (castDto.ProfileImage is not null && castDto.ProfileImage.Length > 0)
            {
                string newImagePath = await SaveProfileImageAsync(environment, castDto.ProfileImage);
                cast.ProfileImagePath = newImagePath;
            }
        
            await repository.SaveAllChangesAsync();
            await repository.CommitTransactionAsync(transaction);
        }
        catch (NotSupportedException nse)
        {
            await repository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + nse.Message);
            throw new NotSupportedException(nse.Message, nse);
        }
        catch (Exception e)
        {
            await repository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingProfile,  username) + e.Message);
            throw new Exception(e.Message);
        }
    }

    public async Task<EditSkillsDto> GetEditSkillsAsync(string username)
    {
        Crew? crew = await repository.GetCrewByUsernameAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        IReadOnlyCollection<CrewSkill> crewSkills = await repository.GetCrewSkillsAsync(crew.Id);
        
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
        Crew? crew = await repository.GetCrewByUsernameAsync(username);
        if (crew is null)
            throw new ArgumentNullException(string.Format(CrewNotFoundMessage, username));

        IReadOnlyCollection<CrewSkill> currentSkills = await repository.GetCrewSkillsForUpdateAsync(crew.Id);
        
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
        
        await using IDbContextTransaction transaction = await repository.BeginTransactionAsync();
        try
        {
            if (skillsToRemove.Count > 0)
                await repository.RemoveCrewSkillsAsync(skillsToRemove);
        
            if (skillsToAdd.Count > 0)
                await repository.AddCrewSkillsAsync(skillsToAdd);

            await repository.SaveAllChangesAsync();
            await repository.CommitTransactionAsync(transaction);
        }
        catch (Exception e)
        {
            await repository.RollbackTransactionAsync(transaction);
            logger.LogError(string.Format(ErrorUpdatingSkills,  username) + e.Message);
            throw new Exception(e.Message);
        }
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
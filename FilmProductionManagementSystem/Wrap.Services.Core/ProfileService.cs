namespace Wrap.Services.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

using Interface;

using Data;
using Data.Models;

using ViewModels.Profile;
using ViewModels.Profile.HelperViewModels;
using ViewModels.LoginAndRegistration.Helpers;

using GCommon.Enums;

using static Utilities.HelperSaveProfile;

public class ProfileService(FilmProductionDbContext context,
                            IWebHostEnvironment environment) : IProfileService
{
    private const string CrewNotFoundMessage = "Crew member with username '{0}' not found.";
    private const string CastNotFoundMessage = "Cast member with username '{0}' not found.";

    
    public async Task<bool> IsUserCrewAsync(string username)
    {
        Crew? crewMembers = await GetCrewMemberAsync(username);
        return crewMembers is not null;
    }
    
    public async Task<bool> IsUserCastAsync(string username)
    {
        Cast? castMember = await GetCastMemberAsync(username);
        return castMember is not null;
    }

    public async Task<CrewProfileViewModel> GetCrewProfileDataAsync(string username)
    {
        Crew? crew = await GetCrewMemberAsync(username);
        if (crew is null)
            throw new ArgumentException(string.Format(CrewNotFoundMessage, username));
        
        // Get user's skills with departments
        IEnumerable<CrewRoleType> userSkills = await context
            .CrewSkills
            .AsNoTracking()
            .Where(cs => cs.CrewMemberId == crew.Id)
            .Select(cs => cs.RoleType)
            .ToArrayAsync();
        
        IDictionary<string, ICollection<CrewRoleType>> departmentSkills = GroupSkillsByDepartment(userSkills);
    
        // Get production
        ICollection<CrewMemberProduction>? productions = await context
            .ProductionsCrewMembers
            .Include(pc => pc.Production)
            .AsNoTracking()
            .Where(pc => pc.CrewMemberId == crew.Id)
            .Select(pc => new CrewMemberProduction
            {
                ProductionId = pc.ProductionId.ToString(),
                ProductionTitle = pc.Production.Title,
                RoleType = null, //TODO: It's hardcoded for now because of simplifying the app - pc.CrewMember.RoleType
                ProjectStatus = pc.Production.StatusType.ToString()
            })
            .ToListAsync();
        
        // Get scenes
        ICollection<CrewMemberScene> scenes = await context
            .ScenesCrewMembers
            .Include(scm => scm.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CrewMemberId == crew.Id)
            .Select(sc => new CrewMemberScene
            {
                SceneId = sc.SceneId.ToString(),
                SceneName = sc.Scene.SceneName,
                ProductionTitle = sc.Scene.Production.Title,
                RoleType = null //TODO: It's hardcoded for now because of simplifying the app - pc.CrewMember.RoleType
            })
            .ToListAsync();
        
        // Get CrewProfileViewModel
        CrewProfileViewModel viewModel = new CrewProfileViewModel
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            ProfileImagePath = crew.ProfileImagePath,
            Nickname = crew.Nickname ?? " - ",
            UserName = crew.User.UserName!,
            Email = crew.User.Email!,
            PhoneNumber = crew.User.PhoneNumber!,
            IsActive = crew.IsActive,
            Biography = crew.Biography,
            DepartmentSkills = departmentSkills,
            CrewMemberProductions = productions,
            CrewMemberScenes = scenes
        };
        
        return viewModel;
    }

    public async Task<CastProfileViewModel> GetCastProfileDataAsync(string username)
    {
        Cast? cast = await GetCastMemberAsync(username);
        if (cast is null)
            throw new ArgumentException(string.Format(CastNotFoundMessage, username));
        
        // Get production
        IEnumerable<CastMemberProduction> productions = await context
            .ProductionsCastMembers
            .Include(pc => pc.Production)
            .AsNoTracking()
            .Where(pc => pc.CastMemberId == cast.Id)
            .Select(pc => new CastMemberProduction
            {
                ProductionId = pc.ProductionId.ToString(),
                ProductionTitle = pc.Production.Title,
                CharacterName = null, //TODO: It's hardcoded for now because of simplifying the app - pc.CrewMember.RoleType
                ProjectStatus = pc.Production.StatusType.ToString()
            })
            .ToArrayAsync();
        
        // Get scenes
        IEnumerable<CastMemberScene> scenes = await context
            .ScenesCastMembers
            .Include(scm => scm.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CastMemberId == cast.Id)
            .Select(sc => new CastMemberScene
            {
                SceneId = sc.SceneId.ToString(),
                SceneName = sc.Scene.SceneName,
                ProductionTitle = sc.Scene.Production.Title
            })
            .ToArrayAsync();
        
        // Get CastProfileViewModel
        CastProfileViewModel viewModel = new CastProfileViewModel
        {
            FirstName = cast.FirstName,
            LastName = cast.LastName,
            ProfileImagePath = cast.ProfileImagePath,
            Nickname = cast.Nickname ?? " - ",
            UserName = cast.User.UserName!,
            Email = cast.User.Email!,
            PhoneNumber = cast.User.PhoneNumber!,
            Age = cast.Age.ToString(),
            Gender = cast.Gender.ToString(),
            Role = cast.Role,
            IsActive = cast.IsActive,
            Biography = cast.Biography,
            CastMemberProductions = productions,
            CastMemberScenes = scenes
        };

        return viewModel;
    }

    public async Task<EditCrewProfileViewModel> GetEditCrewProfileAsync(string username)
    {
        Crew? crew = await GetCrewMemberAsync(username);
        if (crew is null)
            throw new ArgumentException(string.Format(CrewNotFoundMessage, username));
        
        EditCrewProfileViewModel viewModel = new EditCrewProfileViewModel
        {
            FirstName = crew.FirstName,
            LastName = crew.LastName,
            Nickname = crew.Nickname,
            PhoneNumber = crew.User.PhoneNumber!,
            Biography = crew.Biography,
            Email = crew.User.Email!,
            CurrentProfileImagePath = crew.ProfileImagePath
        };

        return viewModel;
    }

    public async Task UpdateCrewProfileAsync(string username, EditCrewProfileViewModel model)
    {
        Crew? crew = await context
            .CrewMembers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.UserName!.ToLower() == username.ToLower());

        if (crew is null)
            throw new ArgumentException(string.Format(CrewNotFoundMessage, username));
        
        crew.FirstName = model.FirstName;
        crew.LastName = model.LastName;
        crew.Nickname = model.Nickname;
        crew.Biography = model.Biography;
        crew.User.PhoneNumber = model.PhoneNumber;
        
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            string newImagePath = await SaveProfileImageAsync(environment, model.ProfileImage);
            crew.ProfileImagePath = newImagePath;
        }
        
        await context.SaveChangesAsync();
    }

    public async Task<EditCastProfileViewModel> GetEditCastProfileAsync(string username)
    {
        Cast? cast = await GetCastMemberAsync(username);

        if (cast is null)
            throw new ArgumentException(string.Format(CastNotFoundMessage, username));

        EditCastProfileViewModel viewModel = new EditCastProfileViewModel
        {
            FirstName = cast.FirstName,
            LastName = cast.LastName,
            Nickname = cast.Nickname,
            PhoneNumber = cast.User.PhoneNumber!,
            CurrentRole = cast.Role,
            Biography = cast.Biography,
            Email = cast.User.Email!,
            CurrentProfileImagePath = cast.ProfileImagePath,
            Age = cast.Age.ToString(),
            Gender = cast.Gender.ToString()
        };

        return viewModel;
    }

    public async Task UpdateCastProfileAsync(string username, EditCastProfileViewModel model)
    {
        Cast? cast = await context
            .CastMembers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.User.UserName!.ToLower() == username.ToLower());

        if (cast is null)
            throw new ArgumentException(string.Format(CastNotFoundMessage, username));
        
        cast.FirstName = model.FirstName;
        cast.LastName = model.LastName;
        cast.Nickname = model.Nickname;
        cast.Role = model.CurrentRole;
        cast.Biography = model.Biography;
        cast.User.PhoneNumber = model.PhoneNumber;
        
        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            string newImagePath = await SaveProfileImageAsync(environment, model.ProfileImage);
            cast.ProfileImagePath = newImagePath;
        }
        
        await context.SaveChangesAsync();
    }

    public async Task<EditSkillsViewModel> GetEditSkillsAsync(string username)
    {
        Crew? crew = await GetCrewMemberAsync(username);
        if (crew is null)
            throw new ArgumentException(string.Format(CrewNotFoundMessage, username));

        ICollection<CrewRoleType> currentSkills = await context
            .CrewSkills
            .AsNoTracking()
            .Where(cs => cs.CrewMemberId == crew.Id)
            .Select(cs => cs.RoleType)
            .ToArrayAsync();

        EditSkillsViewModel viewModel = new EditSkillsViewModel
        {
            CurrentSkills = currentSkills,
            AllDepartments = CrewRolesDepartments.GetRolesByDepartment()
        };

        return viewModel;
    }

    public async Task UpdateSkillsAsync(string username, EditSkillsViewModel model)
    {
        Crew? crew = await GetCrewMemberAsync(username);
        if (crew is null)
            throw new ArgumentException(string.Format(CrewNotFoundMessage, username));

        IList<CrewSkill> currentSkills = await context
            .CrewSkills
            .Where(cs => cs.CrewMemberId == crew.Id)
            .ToListAsync();
        
        HashSet<CrewRoleType> currentSkillTypes = currentSkills
            .Select(s => s.RoleType)
            .ToHashSet();
        
        ICollection<CrewRoleType> newSkills = ParseSelectedSkills(model.SelectedSkills);
        if (newSkills.Count == 0)
            throw new ArgumentException("At least one skill must be selected.");
        
        ICollection<CrewSkill> skillsToRemove = currentSkills
            .Where(s => !newSkills.Contains(s.RoleType))
            .ToList();
        
        IEnumerable<CrewRoleType> skillsToAdd = newSkills
            .Where(s => !currentSkillTypes.Contains(s))
            .ToList();

        if (skillsToRemove.Count > 0)
            context.CrewSkills.RemoveRange(skillsToRemove);
        
        foreach (CrewRoleType skill in skillsToAdd)
        {
            context.CrewSkills.Add(new CrewSkill
            {
                Id = Guid.NewGuid(),
                RoleType = skill,
                CrewMemberId = crew.Id
            });
        }
        
        await context.SaveChangesAsync();
    }
    
    private async Task<Crew?> GetCrewMemberAsync(string username)
    {
        Crew? crewMembers = await context
            .CrewMembers
            .Include(cm => cm.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName!.ToLower() == username.ToLower());
        
        return crewMembers;
    }

    private async Task<Cast?> GetCastMemberAsync(string username)
    {
        Cast? castMember = await context
            .CastMembers
            .Include(cm => cm.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName!.ToLower() == username.ToLower());
        
        return castMember;
    }
    
    private static ICollection<CrewRoleType> ParseSelectedSkills(string selectedSkillsString)
    {
        if (string.IsNullOrWhiteSpace(selectedSkillsString))
            return new List<CrewRoleType>();
    
        ICollection<CrewRoleType> listSelectedSkills = selectedSkillsString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
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
    private static IDictionary<string, ICollection<CrewRoleType>> GroupSkillsByDepartment(
        IEnumerable<CrewRoleType> userSkills)
    {
        IReadOnlyDictionary<string, IReadOnlyCollection<CrewRoleType>> allDepartment = CrewRolesDepartments.GetRolesByDepartment();

        HashSet<CrewRoleType> userSkillsSet = userSkills.ToHashSet();
        
        IReadOnlyCollection<KeyValuePair<string, IReadOnlyCollection<CrewRoleType>>> userDepartments = allDepartment
            .Where(dept => dept.Value.Any(role => userSkillsSet.Contains(role)))
            .ToArray()
            .AsReadOnly();
        
        IDictionary<string, ICollection<CrewRoleType>> departmentSkills = new Dictionary<string, ICollection<CrewRoleType>>();
        
        foreach (var department in userDepartments)
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
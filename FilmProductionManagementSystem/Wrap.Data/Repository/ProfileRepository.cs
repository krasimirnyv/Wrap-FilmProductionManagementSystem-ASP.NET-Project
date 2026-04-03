namespace Wrap.Data.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Interfaces;
using Models;
using Models.MappingEntities;
using Models.Infrastructure;
using Dtos.Crew;
using Dtos.Cast;

public class ProfileRepository(FilmProductionDbContext dbContext)
    : BaseRepository(dbContext), IProfileRepository
{
    private const string PseudoDeletedUsername = "deleted_{0}";
    private const string PseudoDeletedEmail = "deleted_{0}@deleted.local";

    public async Task<Crew?> GetCrewByUsernameAsNoTrackingAsync(string username)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return crew;
    }

    public async Task<Cast?> GetCastByUsernameAsNoTrackingAsync(string username)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.User)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return cast;
    }

    public async Task<Crew?> GetCrewWithAllDataIncludedByUsernameAsNoTrackingAsync(string username)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.User)
            .Include(c => c.Skills)
            .Include(c => c.CrewMemberProductions)
            .Include(c => c.CrewMemberScenes)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return crew;    
    }

    public async Task<Cast?> GetCastWithAllDataIncludedByUsernameAsNoTrackingAsync(string username)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.User)
            .Include(c => c.CastMemberProductions)
            .Include(c => c.CastMemberScenes)
            .AsNoTracking()
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return cast;
    }

    public async Task<Crew?> GetCrewByUsernameAsync(string username)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return crew;
    }

    public async Task<Cast?> GetCastByUsernameAsync(string username)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower());
        
        return cast;
    }

    public async Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsAsync(Guid crewId)
    {
        IReadOnlyCollection<CrewSkill> skills = await Context!
            .CrewSkills
            .AsNoTracking()
            .Where(cs => cs.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return skills;
    }

    public async Task<IReadOnlyCollection<ProductionCrew>> GetCrewProductionsAsync(Guid crewId)
    {
        IReadOnlyCollection<ProductionCrew> crewProductions = await Context!
            .ProductionsCrewMembers
            .Include(c => c.Production)
            .AsNoTracking()
            .Where(pc => pc.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return crewProductions;
    }

    public async Task<IReadOnlyCollection<SceneCrew>> GetCrewScenesAsync(Guid crewId)
    {
        IReadOnlyCollection<SceneCrew> crewScenes = await Context!
            .ScenesCrewMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return crewScenes;
    }

    public async Task<IReadOnlyCollection<ProductionCast>> GetCastProductionsAsync(Guid castId)
    {
        IReadOnlyCollection<ProductionCast> castProductions = await Context!
                .ProductionsCastMembers
                .Include(pc => pc.Production)
                .AsNoTracking()
                .Where(pc => pc.CastMemberId == castId)
                .ToArrayAsync();

        return castProductions;
    }

    public async Task<IReadOnlyCollection<SceneCast>> GetCastScenesAsync(Guid castId)
    {
        IReadOnlyCollection<SceneCast> castScenes = await Context!
            .ScenesCastMembers
            .Include(sc => sc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(sc => sc.CastMemberId == castId)
            .ToArrayAsync();
        
        return castScenes;
    }

    public async Task<IReadOnlyCollection<CrewSkill>> GetCrewSkillsForUpdateAsync(Guid crewId)
    {
        IReadOnlyCollection<CrewSkill> skills = await Context!
            .CrewSkills
            .Where(c => c.CrewMemberId == crewId)
            .ToArrayAsync();
        
        return skills;
    }

    public async Task AddCrewSkillsAsync(IEnumerable<CrewSkill> skillsToAdd)
    {
        await Context!.AddRangeAsync(skillsToAdd);
    }

    public Task RemoveCrewSkillsAsync(IEnumerable<CrewSkill> skillsToRemove)
    {
        Context!.RemoveRange(skillsToRemove);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteCrewProfileAsync(Guid crewId)
    {
        Crew? crew = await Context!
            .CrewMembers
            .Include(c => c.Skills)             
            .Include(c => c.CrewMemberProductions)
            .Include(c => c.CrewMemberScenes)
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.Id == crewId);

        if (crew is null)
            return false;
        
        if (crew.Skills.Count > 0)
            Context.CrewSkills.RemoveRange(crew.Skills);

        if (crew.CrewMemberProductions.Count > 0)
            Context.ProductionsCrewMembers.RemoveRange(crew.CrewMemberProductions);

        if (crew.CrewMemberScenes.Count > 0)
            Context.ScenesCrewMembers.RemoveRange(crew.CrewMemberScenes);

        crew.ProfileImagePath = null;
        crew.Nickname = null;
        crew.Biography = null;
        crew.IsActive = false;
        crew.IsDeleted = true;
        
        ApplicationUser user = crew.User;
        ResettingApplicationUser(user);
        
        return true;
    }
    
    public async Task<bool> DeleteCastProfileAsync(Guid castId)
    {
        Cast? cast = await Context!
            .CastMembers
            .Include(c => c.CastMemberProductions)
            .Include(c => c.CastMemberScenes)
            .Include(c => c.User)
            .SingleOrDefaultAsync(c => c.Id == castId);

        if (cast is null)
            return false;

        if (cast.CastMemberProductions.Count > 0)
            Context.ProductionsCastMembers.RemoveRange(cast.CastMemberProductions);

        if (cast.CastMemberScenes.Count > 0)
            Context.ScenesCastMembers.RemoveRange(cast.CastMemberScenes);

        cast.ProfileImagePath = null;
        cast.Nickname = null;
        cast.Biography = null;
        cast.IsActive = false;
        cast.IsDeleted = true;
        
        ApplicationUser user = cast.User;
        ResettingApplicationUser(user);

        return true;
    }

    public async Task<CrewPersonalDataDto[]?> DownloadCrewDataAsync(string username)
    {
        CrewPersonalDataDto[]? crewPersonalDataDto = await Context!
            .CrewMembers
            .Include(c => c.User)
            .Include(c => c.Skills)
            .Include(c => c.CrewMemberProductions)
            .ThenInclude(cp => cp.Production)
            .Include(c => c.CrewMemberScenes)
            .ThenInclude(csc => csc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower())
            .Select(c => new CrewPersonalDataDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Nickname = c.Nickname,
                Biography = c.Biography,

                User = new CrewUserDataDto
                {
                    Id = c.User.Id,
                    UserName = c.User.UserName,
                    Email = c.User.Email,
                    EmailConfirmed = c.User.EmailConfirmed,
                    PhoneNumber = c.User.PhoneNumber,
                    PhoneNumberConfirmed = c.User.PhoneNumberConfirmed,
                    LockoutEnabled = c.User.LockoutEnabled,
                    LockoutEnd = c.User.LockoutEnd
                },

                SkillsCount = c.Skills.Count,
                Skills = c.Skills
                    .Select(s => new CrewSkillDataDto
                    {
                        Skill = s.RoleType.ToString()
                    }).ToArray(),

                ProductionsCount = c.CrewMemberProductions.Count,
                Productions = c.CrewMemberProductions
                    .Select(cp => new CrewProductionDataDto
                    {
                        Title = cp.Production.Title,
                        Description = cp.Production.Description,
                        RoleInProduction = cp.RoleType.ToString()
                    }).ToArray(),

                ScenesCount = c.CrewMemberScenes.Count,
                Scenes = c.CrewMemberScenes
                    .Select(cs => new CrewSceneDataDto
                    {
                        SceneNumber = cs.Scene.SceneNumber,
                        SceneType = cs.RoleType.ToString(),
                        SceneName = cs.Scene.SceneName,
                        Location = cs.Scene.Location,
                        ProductionTitle = cs.Scene.Production.Title
                    }).ToArray()
            })
            .ToArrayAsync();
        
        return crewPersonalDataDto;
    }

    public async Task<CastPersonalDataDto[]?> DownloadCastDataAsync(string username)
    {
        CastPersonalDataDto[]? castPersonalDataDto = await Context!
            .CastMembers
            .Include(c => c.User)
            .Include(c => c.CastMemberProductions)
            .ThenInclude(cp => cp.Production)
            .Include(c => c.CastMemberScenes)
            .ThenInclude(csc => csc.Scene)
            .ThenInclude(s => s.Production)
            .AsNoTracking()
            .Where(c => c.User.UserName != null && c.User.UserName.ToLower() == username.ToLower())
            .Select(c => new CastPersonalDataDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Nickname = c.Nickname,
                Biography = c.Biography,
                Birthday = c.BirthDate,
                Age = c.Age,
                Gender = c.Gender.ToString(),

                User = new CastUserDataDto
                {
                    Id = c.User.Id,
                    UserName = c.User.UserName,
                    Email = c.User.Email,
                    EmailConfirmed = c.User.EmailConfirmed,
                    PhoneNumber = c.User.PhoneNumber,
                    PhoneNumberConfirmed = c.User.PhoneNumberConfirmed,
                    LockoutEnabled = c.User.LockoutEnabled,
                    LockoutEnd = c.User.LockoutEnd
                },

                ProductionsCount = c.CastMemberProductions.Count,
                Productions = c.CastMemberProductions
                    .Select(cp => new CastProductionDataDto
                    {
                        Title = cp.Production.Title,
                        Description = cp.Production.Description,
                        RoleInProduction = cp.Role
                    })
                    .ToArray(),

                ScenesCount = c.CastMemberScenes.Count,
                Scenes = c.CastMemberScenes
                    .Select(cs => new CastSceneDataDto
                    {
                        SceneNumber = cs.Scene.SceneNumber,
                        SceneType = cs.Role,
                        SceneName = cs.Scene.SceneName,
                        Location = cs.Scene.Location,
                        ProductionTitle = cs.Scene.Production.Title
                    })
                    .ToArray()
            })
            .ToArrayAsync();
        
        return castPersonalDataDto;
    }

    public async Task<int> SaveAllChangesAsync()
    {
        int affectedRows = await SaveChangesAsync();
        return affectedRows;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        IDbContextTransaction transaction = await Context!.Database.BeginTransactionAsync();
        return transaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }
    
    private static void ResettingApplicationUser(ApplicationUser user)
    {
        string pseudoUserName = string.Format(PseudoDeletedUsername, $"{user.Id:N}");
        user.UserName = pseudoUserName;
        user.NormalizedUserName = pseudoUserName.ToUpperInvariant();
        
        string pseudoEmail = string.Format(PseudoDeletedEmail, $"{user.Id:N}");
        user.Email = pseudoEmail;
        user.NormalizedEmail = pseudoEmail.ToUpperInvariant();
        user.EmailConfirmed = false;
        
        user.PasswordHash = null;
        
        user.PhoneNumber = null;
        user.PhoneNumberConfirmed = false;
        
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;
    }
}
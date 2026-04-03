namespace Wrap.Data.Repository;

using Interfaces;
using Models;
using GCommon.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class LoginRegisterRepository(FilmProductionDbContext dbContext) 
    : BaseRepository(dbContext), ILoginRegisterRepository
{
    public async Task<bool> CrewExistsByUserIdAsync(Guid userId)
    { 
        return await Context!
            .CrewMembers
            .AnyAsync(c => c.UserId == userId);
    }

    public async Task<bool> CastExistsByUserIdAsync(Guid userId)
    {
        return await Context!
            .CastMembers
            .AnyAsync(c => c.UserId == userId);
    }

    public async Task CreateCrewAsync(Crew crew)
    {
        await Context!.CrewMembers.AddAsync(crew);
    }

    public async Task CreateCastAsync(Cast cast)
    {
        await Context!.CastMembers.AddAsync(cast);
    }

    public async Task AddCrewSkillsAsync(Guid crewId, IReadOnlyCollection<int> skills)
    {
        foreach (int skillNumber in skills)
        {
            CrewRoleType skill = (CrewRoleType)skillNumber;

            CrewSkill newSkill = new CrewSkill
            {
                Id = Guid.NewGuid(),
                CrewMemberId = crewId,
                RoleType = skill
            };
            
            await Context!.CrewSkills.AddAsync(newSkill);
        }
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await Context!.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    { 
        await transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }

    public async Task<int> SaveAllChangesAsync()
    { 
        int affectedRows = await SaveChangesAsync();
        return affectedRows;
    }
}
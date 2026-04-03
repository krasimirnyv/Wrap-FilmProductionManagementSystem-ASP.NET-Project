namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

using static GCommon.DataFormat;
using static Common.EntityIdentificationConstants;

public class CrewConfiguration : IEntityTypeConfiguration<Crew>
{
    public void Configure(EntityTypeBuilder<Crew> entity)
    {
        entity.HasQueryFilter(c => !c.IsDeleted);

        entity.HasData(CrewMembersSeed);
    }
    
    private static readonly Crew[] CrewMembersSeed =
    [
        new()
        {
            Id = CrewId1,
            UserId = CrewUserId1,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Alex",
            LastName = "Petrov",
            Nickname = "AP/DP",
            Biography = "Director of Photography focused on natural light and handheld storytelling.",
            IsActive = true,
            IsDeleted = false
        },
        new()
        {
            Id = CrewId2,
            UserId = CrewUserId2,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Maria",
            LastName = "Georgieva",
            Nickname = "Maya",
            Biography = "Production coordinator with experience in budgeting, call sheets and logistics.",
            IsActive = true,
            IsDeleted = false
        },
        new()
        {
            Id = CrewId3,
            UserId = CrewUserId3,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Ivan",
            LastName = "Dimitrov",
            Nickname = null,
            Biography = "Sound recordist and boom operator. Location sound, ADR planning, and on-set workflow.",
            IsActive = true,
            IsDeleted = false
        }
    ];
}
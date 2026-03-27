namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static GCommon.DataFormat;
using static Common.EntityIdentificationConstants;

public class CastConfiguration : IEntityTypeConfiguration<Cast>
{
    public void Configure(EntityTypeBuilder<Cast> entity)
    {
        entity
            .HasOne(c => c.User)
            .WithMany(au => au.CastMembers)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(CastMembersSeed);
    }
    
    private static readonly Cast[] CastMembersSeed =
    [
        new()
        {
            Id = CastId1,
            UserId = CastUserId1,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Elena",
            LastName = "Stoyanova",
            Nickname = "Eli",
            BirthDate = new DateTime(1998, 4, 12),
            Gender = GenderType.Female,
            Biography = "Stage and screen actor, passionate about drama and indie productions.",
            IsActive = true,
            IsDeleted = false
        },
        new()
        {
            Id = CastId2,
            UserId = CastUserId2,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Georgi",
            LastName = "Ivanov",
            Nickname = null,
            BirthDate = new DateTime(1995, 9, 3),
            Gender = GenderType.Male,
            Biography = "Film actor with a focus on action and thrillers.",
            IsActive = true,
            IsDeleted = false
        },
        new()
        {
            Id = CastId3,
            UserId = CastUserId3,
            ProfileImagePath = DefaultProfilePath,
            FirstName = "Siyana",
            LastName = "Petrova",
            Nickname = "Sisi",
            BirthDate = new DateTime(2001, 1, 25),
            Gender = GenderType.Female,
            Biography = "Aspiring actor, experienced in commercials and short films.",
            IsActive = true,
            IsDeleted = false
        }
    ];
}
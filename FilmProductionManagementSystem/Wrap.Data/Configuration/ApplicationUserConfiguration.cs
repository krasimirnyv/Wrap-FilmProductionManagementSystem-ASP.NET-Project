namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.Infrastructure;

using static Common.EntityIdentificationConstants;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.HasData(ApplicationUsersSeed);
    }
    
    private static readonly ApplicationUser[] ApplicationUsersSeed =
    [
        // CrewMembers
        new()
        {
            Id = CrewUserId1,
            UserName = "alex.petrov",
            NormalizedUserName = "ALEX.PETROV",
            Email = "alex.petrov@wrap.local",
            NormalizedEmail = "ALEX.PETROV@WRAP.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "seed-crew-sec-1",
            ConcurrencyStamp = "seed-crew-con-1",
            PhoneNumber = "+359888000001",
            PhoneNumberConfirmed = true,
            PasswordHash = null
        },
        new()
        {
            Id = CrewUserId2,
            UserName = "maria.georgieva",
            NormalizedUserName = "MARIA.GEORGIEVA",
            Email = "maria.georgieva@wrap.local",
            NormalizedEmail = "MARIA.GEORGIEVA@WRAP.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "seed-crew-sec-2",
            ConcurrencyStamp = "seed-crew-con-2",
            PhoneNumber = "+359888000002",
            PhoneNumberConfirmed = true,
            PasswordHash = null
        },
        new()
        {
            Id = CrewUserId3,
            UserName = "ivan.dimitrov",
            NormalizedUserName = "IVAN.DIMITROV",
            Email = "ivan.dimitrov@wrap.local",
            NormalizedEmail = "IVAN.DIMITROV@WRAP.LOCAL",
            EmailConfirmed = true,
            SecurityStamp = "seed-crew-sec-3",
            ConcurrencyStamp = "seed-crew-con-3",
            PhoneNumber = "+359888000003",
            PhoneNumberConfirmed = true,
            PasswordHash = null
        },
        
        // CastMembers
        new()
        {
            Id = CastUserId1,
            UserName = "elena.stoyanova",
            NormalizedUserName = "ELENA.STOYANOVA",
            Email = "elena.stoyanova@wrap.local",
            NormalizedEmail = "ELENA.STOYANOVA@WRAP.LOCAL",
            EmailConfirmed = true,
            PhoneNumber = "+359888100001",
            PhoneNumberConfirmed = true,
            SecurityStamp = "seed-cast-sec-1",
            ConcurrencyStamp = "seed-cast-con-1",
            PasswordHash = null
        },
        new()
        {
            Id = CastUserId2,
            UserName = "georgi.ivanov",
            NormalizedUserName = "GEORGI.IVANOV",
            Email = "georgi.ivanov@wrap.local",
            NormalizedEmail = "GEORGI.IVANOV@WRAP.LOCAL",
            EmailConfirmed = true,
            PhoneNumber = "+359888100002",
            PhoneNumberConfirmed = true,
            SecurityStamp = "seed-cast-sec-2",
            ConcurrencyStamp = "seed-cast-con-2",
            PasswordHash = null
        },
        new()
        {
            Id = CastUserId3,
            UserName = "siyana.petrova",
            NormalizedUserName = "SIYANA.PETROVA",
            Email = "siyana.petrova@wrap.local",
            NormalizedEmail = "SIYANA.PETROVA@WRAP.LOCAL",
            EmailConfirmed = true,
            PhoneNumber = "+359888100003",
            PhoneNumberConfirmed = true,
            SecurityStamp = "seed-cast-sec-3",
            ConcurrencyStamp = "seed-cast-con-3",
            PasswordHash = null
        }
    ];
}
namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class ProductionCrewConfiguration : IEntityTypeConfiguration<ProductionCrew>
{
    public void Configure(EntityTypeBuilder<ProductionCrew> entity)
    {
        entity.HasKey(pc => new { pc.ProductionId, pc.CrewMemberId });

        entity
            .HasOne(pc => pc.Production)
            .WithMany(p => p.ProductionCrewMembers)
            .HasForeignKey(pc => pc.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(pc => pc.CrewMember)
            .WithMany(c => c.CrewMemberProductions)
            .HasForeignKey(pc => pc.CrewMemberId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasQueryFilter(pc => !pc.CrewMember.IsDeleted);

        entity.HasData(ProductionCrewSeed);
    }

    private static readonly ProductionCrew[] ProductionCrewSeed =
    [
        new() { ProductionId = ProductionIdFestivalRun, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { ProductionId = ProductionIdFestivalRun, CrewMemberId = CrewId2, RoleType = CrewRoleType.DirectorOfPhotography },
        new() { ProductionId = ProductionIdFestivalRun, CrewMemberId = CrewId3, RoleType = CrewRoleType.ProductionSoundMixer },

        new() { ProductionId = ProductionIdOnHold, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },

        new() { ProductionId = ProductionIdWildTracks, CrewMemberId = CrewId2, RoleType = CrewRoleType.CameraOperator },

        new() { ProductionId = ProductionIdTheLastTake, CrewMemberId = CrewId3, RoleType = CrewRoleType.BoomOperator },
        new() { ProductionId = ProductionIdTheLastTake, CrewMemberId = CrewId2, RoleType = CrewRoleType.Colorist }
    ];
}
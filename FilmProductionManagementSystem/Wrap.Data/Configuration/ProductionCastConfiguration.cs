namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

using static Common.EntityIdentificationConstants;

public class ProductionCastConfiguration : IEntityTypeConfiguration<ProductionCast>
{
    public void Configure(EntityTypeBuilder<ProductionCast> entity)
    {
        entity.HasKey(pc => new { pc.ProductionId, pc.CastMemberId });

        entity
            .HasOne(pc => pc.Production)
            .WithMany(p => p.ProductionCastMembers)
            .HasForeignKey(pc => pc.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(pc => pc.CastMember)
            .WithMany(c => c.CastMemberProductions)
            .HasForeignKey(pc => pc.CastMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(ProductionCastSeed);
    }
    
    private static readonly ProductionCast[] ProductionCastSeed =
    [
        new() { ProductionId = ProductionIdTestFilm, CastMemberId = CastId1, Role = "Detective Ivanov" },
        new() { ProductionId = ProductionIdTestFilm, CastMemberId = CastId2, Role = "Witness" },

        new() { ProductionId = ProductionIdMidnightDreams, CastMemberId = CastId3, Role = "Lead Actress" },

        new() { ProductionId = ProductionIdPaperPlanes, CastMemberId = CastId1, Role = "Private Investigator" },
        new() { ProductionId = ProductionIdPaperPlanes, CastMemberId = CastId2, Role = "Antagonist" },

        new() { ProductionId = ProductionIdTheLastTake, CastMemberId = CastId2, Role = "Supporting Role" },
    ];
}
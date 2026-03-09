namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

public class ProductionCrewConfiguration : IEntityTypeConfiguration<ProductionCrew>
{
    public void Configure(EntityTypeBuilder<ProductionCrew> entity)
    {
        entity.HasKey(pc => new { pc.ProductionId, pc.CrewMemberId });

        entity
            .HasOne(pc => pc.Production)
            .WithMany(p => p.ProductionCrewMembers)
            .HasForeignKey(pc => pc.ProductionId);

        entity
            .HasOne(pc => pc.CrewMember)
            .WithMany(c => c.CrewMemberProductions)
            .HasForeignKey(pc => pc.CrewMemberId);
    }
}
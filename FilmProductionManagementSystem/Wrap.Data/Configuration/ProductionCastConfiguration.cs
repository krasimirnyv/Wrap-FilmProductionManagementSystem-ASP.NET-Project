namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

public class ProductionCastConfiguration : IEntityTypeConfiguration<ProductionCast>
{
    public void Configure(EntityTypeBuilder<ProductionCast> entity)
    {
        entity.HasKey(pc => new { pc.ProductionId, pc.CastMemberId });

        entity
            .HasOne(pc => pc.Production)
            .WithMany(p => p.ProductionCastMembers)
            .HasForeignKey(pc => pc.ProductionId);

        entity
            .HasOne(pc => pc.CastMember)
            .WithMany(c => c.CastMemberProductions)
            .HasForeignKey(pc => pc.CastMemberId);
    }
}
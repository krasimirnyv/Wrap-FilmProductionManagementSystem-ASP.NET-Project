namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

public class SceneCastConfiguration : IEntityTypeConfiguration<SceneCast>
{
    public void Configure(EntityTypeBuilder<SceneCast> entity)
    {
        entity.HasKey(sc => new { sc.SceneId, sc.CastMemberId });

        entity
            .HasOne(sc => sc.Scene)
            .WithMany(s => s.SceneCastMembers)
            .HasForeignKey(sc => sc.SceneId);

        entity
            .HasOne(sc => sc.CastMember)
            .WithMany(c => c.CastMemberScenes)
            .HasForeignKey(sc => sc.CastMemberId);
    }
}
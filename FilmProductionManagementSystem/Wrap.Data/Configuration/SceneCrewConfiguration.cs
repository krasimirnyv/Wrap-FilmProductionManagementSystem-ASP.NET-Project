namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

public class SceneCrewConfiguration : IEntityTypeConfiguration<SceneCrew>
{
    public void Configure(EntityTypeBuilder<SceneCrew> entity)
    {
        entity.HasKey(sc => new { sc.SceneId, sc.CrewMemberId });

        entity
            .HasOne(sc => sc.Scene)
            .WithMany(s => s.SceneCrewMembers)
            .HasForeignKey(sc => sc.SceneId);

        entity
            .HasOne(sc => sc.CrewMember)
            .WithMany(c => c.CrewMemberScenes)
            .HasForeignKey(sc => sc.CrewMemberId);
    }
}
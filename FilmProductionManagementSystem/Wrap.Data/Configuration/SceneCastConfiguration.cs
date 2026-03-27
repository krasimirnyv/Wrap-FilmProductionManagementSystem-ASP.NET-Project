namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;

using static Common.EntityIdentificationConstants;

public class SceneCastConfiguration : IEntityTypeConfiguration<SceneCast>
{
    public void Configure(EntityTypeBuilder<SceneCast> entity)
    {
        entity.HasKey(sc => new { sc.SceneId, sc.CastMemberId });

        entity
            .HasOne(sc => sc.Scene)
            .WithMany(s => s.SceneCastMembers)
            .HasForeignKey(sc => sc.SceneId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(sc => sc.CastMember)
            .WithMany(c => c.CastMemberScenes)
            .HasForeignKey(sc => sc.CastMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasData(SceneCastSeed);
    }
    
    private static readonly SceneCast[] SceneCastSeed =
    [
        new() { SceneId = SceneId_TestFilm_01, CastMemberId = CastId1, Role = "Detective Ivanov" },
        new() { SceneId = SceneId_TestFilm_01, CastMemberId = CastId2, Role = "Witness" },
        new() { SceneId = SceneId_TestFilm_02, CastMemberId = CastId1, Role = "Detective Ivanov" },

        new() { SceneId = SceneId_MidnightDreams_01, CastMemberId = CastId3, Role = "Lead Actress" },

        new() { SceneId = SceneId_PaperPlanes_01, CastMemberId = CastId1, Role = "Private Investigator" },
        new() { SceneId = SceneId_PaperPlanes_01, CastMemberId = CastId2, Role = "Antagonist" },

        new() { SceneId = SceneId_TheLastTake_01, CastMemberId = CastId2, Role = "Supporting Role" },
    ];
}
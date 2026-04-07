namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.MappingEntities;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class SceneCrewConfiguration : IEntityTypeConfiguration<SceneCrew>
{
    public void Configure(EntityTypeBuilder<SceneCrew> entity)
    {
        entity.HasKey(sc => new { sc.SceneId, sc.CrewMemberId });

        entity
            .HasOne(sc => sc.Scene)
            .WithMany(s => s.SceneCrewMembers)
            .HasForeignKey(sc => sc.SceneId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(sc => sc.CrewMember)
            .WithMany(c => c.CrewMemberScenes)
            .HasForeignKey(sc => sc.CrewMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasQueryFilter(sc => !sc.CrewMember.IsDeleted);
        
        entity.HasData(SceneCrewSeed);
    }
    
    private static readonly SceneCrew[] SceneCrewSeed =
    [
        // Test Film – Scene 01
        new() { SceneId = SceneId_TestFilm_01, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { SceneId = SceneId_TestFilm_01, CrewMemberId = CrewId2, RoleType = CrewRoleType.DirectorOfPhotography },
        new() { SceneId = SceneId_TestFilm_01, CrewMemberId = CrewId3, RoleType = CrewRoleType.ProductionSoundMixer },

        // Test Film – Scene 02
        new() { SceneId = SceneId_TestFilm_02, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { SceneId = SceneId_TestFilm_02, CrewMemberId = CrewId2, RoleType = CrewRoleType.CameraOperator },

        // Test Film – Scene 03
        new() { SceneId = SceneId_TestFilm_03, CrewMemberId = CrewId2, RoleType = CrewRoleType.FirstAssistantCamera },
        new() { SceneId = SceneId_TestFilm_03, CrewMemberId = CrewId3, RoleType = CrewRoleType.BoomOperator },

        // Midnight Dreams – Scene 01
        new() { SceneId = SceneId_MidnightDreams_01, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { SceneId = SceneId_MidnightDreams_01, CrewMemberId = CrewId2, RoleType = CrewRoleType.DirectorOfPhotography },

        // Midnight Dreams – Scene 02
        new() { SceneId = SceneId_MidnightDreams_02, CrewMemberId = CrewId2, RoleType = CrewRoleType.Gaffer },
        new() { SceneId = SceneId_MidnightDreams_02, CrewMemberId = CrewId3, RoleType = CrewRoleType.SoundRecordist },

        // Paper Planes – Scene 01
        new() { SceneId = SceneId_PaperPlanes_01, CrewMemberId = CrewId1, RoleType = CrewRoleType.Producer },
        new() { SceneId = SceneId_PaperPlanes_01, CrewMemberId = CrewId2, RoleType = CrewRoleType.DirectorOfPhotography },

        // Paper Planes – Scene 02
        new() { SceneId = SceneId_PaperPlanes_02, CrewMemberId = CrewId1, RoleType = CrewRoleType.FirstAssistantDirector },
        new() { SceneId = SceneId_PaperPlanes_02, CrewMemberId = CrewId3, RoleType = CrewRoleType.ProductionSoundMixer },

        // The Last Take – Scene 01
        new() { SceneId = SceneId_TheLastTake_01, CrewMemberId = CrewId2, RoleType = CrewRoleType.CameraOperator },
        new() { SceneId = SceneId_TheLastTake_01, CrewMemberId = CrewId3, RoleType = CrewRoleType.BoomOperator },

        // The Last Take – Scene 02
        new() { SceneId = SceneId_TheLastTake_02, CrewMemberId = CrewId1, RoleType = CrewRoleType.Director },
        new() { SceneId = SceneId_TheLastTake_02, CrewMemberId = CrewId2, RoleType = CrewRoleType.DirectorOfPhotography },

        // The Last Take – Scene 03
        new() { SceneId = SceneId_TheLastTake_03, CrewMemberId = CrewId1, RoleType = CrewRoleType.Producer },
        new() { SceneId = SceneId_TheLastTake_03, CrewMemberId = CrewId3, RoleType = CrewRoleType.SoundDesigner },
    ];
}
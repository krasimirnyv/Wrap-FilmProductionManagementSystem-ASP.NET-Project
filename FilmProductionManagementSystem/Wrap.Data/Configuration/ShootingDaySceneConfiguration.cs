namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

using static Common.EntityIdentificationConstants;

public class ShootingDaySceneConfiguration : IEntityTypeConfiguration<ShootingDayScene>
{
    public void Configure(EntityTypeBuilder<ShootingDayScene> entity)
    {
        entity.HasIndex(sds => new { sds.ShootingDayId, sds.SceneId }).IsUnique();

        entity
            .HasOne(sds => sds.Scene)
            .WithMany(s => s.ShootingDayScenes)
            .HasForeignKey(sds => sds.SceneId)
            .OnDelete(DeleteBehavior.NoAction);

        entity
            .HasOne(sds => sds.ShootingDay)
            .WithMany(sd => sd.ShootingDayScenes)
            .HasForeignKey(sds => sds.ShootingDayId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(ShootingDayScenesSeed);
    }
    
    private static readonly ShootingDayScene[] ShootingDayScenesSeed =
    [
        // Test Film: Day 1 -> Scene 1, Scene 2
        new()
        {
            Id = ShootingDaySceneId_TestFilm_01_01,
            ShootingDayId = ShootingDayId_TestFilm_01,
            SceneId = SceneId_TestFilm_01,
            Order = 1
        },
        new()
        {
            Id = ShootingDaySceneId_TestFilm_01_02,
            ShootingDayId = ShootingDayId_TestFilm_01,
            SceneId = SceneId_TestFilm_02,
            Order = 2
        },

        // Test Film: Day 2 -> Scene 3
        new()
        {
            Id = ShootingDaySceneId_TestFilm_02_01,
            ShootingDayId = ShootingDayId_TestFilm_02,
            SceneId = SceneId_TestFilm_03,
            Order = 1
        },

        // Midnight Dreams: Day 1 -> Scene 1
        new()
        {
            Id = ShootingDaySceneId_MidnightDreams_01_01,
            ShootingDayId = ShootingDayId_MidnightDreams_01,
            SceneId = SceneId_MidnightDreams_01,
            Order = 1
        },

        // Midnight Dreams: Day 2 -> Scene 2
        new()
        {
            Id = ShootingDaySceneId_MidnightDreams_02_01,
            ShootingDayId = ShootingDayId_MidnightDreams_02,
            SceneId = SceneId_MidnightDreams_02,
            Order = 1
        },

        // Paper Planes: Day 1 -> Scene 1, Scene 2
        new()
        {
            Id = ShootingDaySceneId_PaperPlanes_01_01,
            ShootingDayId = ShootingDayId_PaperPlanes_01,
            SceneId = SceneId_PaperPlanes_01,
            Order = 1
        },
        new()
        {
            Id = ShootingDaySceneId_PaperPlanes_01_02,
            ShootingDayId = ShootingDayId_PaperPlanes_01,
            SceneId = SceneId_PaperPlanes_02,
            Order = 2
        },

        // The Last Take: Day 1 -> Scene 1, Scene 2
        new()
        {
            Id = ShootingDaySceneId_TheLastTake_01_01,
            ShootingDayId = ShootingDayId_TheLastTake_01,
            SceneId = SceneId_TheLastTake_01,
            Order = 1
        },
        new()
        {
            Id = ShootingDaySceneId_TheLastTake_01_02,
            ShootingDayId = ShootingDayId_TheLastTake_01,
            SceneId = SceneId_TheLastTake_02,
            Order = 2
        },

        // The Last Take: Day 2 -> Scene 3
        new()
        {
            Id = ShootingDaySceneId_TheLastTake_02_01,
            ShootingDayId = ShootingDayId_TheLastTake_02,
            SceneId = SceneId_TheLastTake_03,
            Order = 1
        }
    ];
}
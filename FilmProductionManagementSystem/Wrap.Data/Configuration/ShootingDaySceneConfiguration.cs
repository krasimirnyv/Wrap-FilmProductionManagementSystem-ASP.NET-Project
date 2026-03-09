namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class ShootingDaySceneConfiguration : IEntityTypeConfiguration<ShootingDayScene>
{
    public void Configure(EntityTypeBuilder<ShootingDayScene> entity)
    {
        entity.HasIndex(sds => new { sds.ShootingDayId, sds.SceneId });

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
    }
}
namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class ShootingDayConfiguration : IEntityTypeConfiguration<ShootingDay>
{
    public void Configure(EntityTypeBuilder<ShootingDay> entity)
    {
        entity
            .HasOne(sd => sd.Production)
            .WithMany(p => p.ShootingDays)
            .HasForeignKey(sd => sd.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
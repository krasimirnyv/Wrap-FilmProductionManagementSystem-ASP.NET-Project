namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class SceneConfiguration : IEntityTypeConfiguration<Scene>
{
    public void Configure(EntityTypeBuilder<Scene> entity)
    {
        entity
            .HasOne(s => s.Production)
            .WithMany(p => p.Scenes)
            .HasForeignKey(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
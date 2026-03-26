namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static GCommon.DataFormat;

public class ProductionConfiguration : IEntityTypeConfiguration<Production>
{
    public void Configure(EntityTypeBuilder<Production> entity)
    {
        entity
            .HasOne(p => p.Script)
            .WithOne(s => s.Production)
            .HasForeignKey<Script>(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(productions);
    }
    
    private readonly Production[] productions =
    [
        new()
        {
            Id = Guid.Parse("809f7100-5b1d-4eee-8b82-8e4084ef0928"),
            Title = "Test Film",
            Description = "This is a test",
            Budget = 1000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Concept,
            StatusStartDate = DateTime.UtcNow,
            StatusEndDate = DateTime.UtcNow + TimeSpan.FromDays(5)
        }
    ];
}
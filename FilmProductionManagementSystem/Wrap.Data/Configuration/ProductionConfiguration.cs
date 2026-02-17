namespace Wrap.Data.Configuration;

using Models;
using GCommon.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductionConfiguration : IEntityTypeConfiguration<Production>
{
    private readonly Production[] productions =
    [
        new Production
        {
            Id = Guid.Parse("809f7100-5b1d-4eee-8b82-8e4084ef0928"),
            Title = "Test Film",
            Description = "This is a test",
            Budget = 1000.00M,
            StatusType = ProductionStatusType.Concept,
            StatusStartDate = DateTime.UtcNow,
            StatusEndDate = DateTime.UtcNow + TimeSpan.FromDays(5)
        }
    ];
    
    public void Configure(EntityTypeBuilder<Production> entity)
    {
        entity.HasData(productions);
    }
}
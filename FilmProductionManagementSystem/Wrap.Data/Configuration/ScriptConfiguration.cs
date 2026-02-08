namespace Wrap.Data.Configuration;

using Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ScriptConfiguration : IEntityTypeConfiguration<Script>
{
    private readonly Script[] scripts =
    [
        new()
        {
            Id = Guid.Parse("36ca9b58-d902-4195-bf80-2e6518ad3c6d"),
            Title = "Test Screenplay",
            LastEditedAt = DateTime.UtcNow,
            ProductionId = Guid.Parse("809f7100-5b1d-4eee-8b82-8e4084ef0928")
        }
    ];
    
    public void Configure(EntityTypeBuilder<Script> entity)
    {
        entity.HasData(scripts);
    }
}
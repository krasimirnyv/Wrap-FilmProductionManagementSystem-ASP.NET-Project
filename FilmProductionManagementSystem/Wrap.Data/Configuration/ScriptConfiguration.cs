namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

public class ScriptConfiguration : IEntityTypeConfiguration<Script>
{
    public void Configure(EntityTypeBuilder<Script> entity)
    {
        entity
            .HasMany(s => s.ScriptBlocks)
            .WithOne(sb => sb.Script)
            .HasForeignKey(sb => sb.ScriptId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(scripts);
    }
    
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
}
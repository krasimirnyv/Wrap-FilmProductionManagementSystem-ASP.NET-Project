namespace Wrap.Data.Configuration;

using Models;
using GCommon.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ScriptBlockConfiguration : IEntityTypeConfiguration<ScriptBlock>
{
    // In Program.cs or a seeder
    private readonly ScriptBlock[] scriptBlocks =
    [
        new()
        {
            Id = Guid.Parse("65a5a61f-204d-4274-be48-bf4b440ff6a1"),
            OrderIndex = 0,
            BlockType = ScriptBlockType.SceneHeading,
            Content = "INT. COFFEE SHOP - DAY",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            ScriptId = Guid.Parse("36ca9b58-d902-4195-bf80-2e6518ad3c6d")
        },
        new()
        {
            Id = Guid.Parse("c5cc8272-c35d-4b4b-bb31-137df7fe86d5"),
            OrderIndex = 1,
            BlockType = ScriptBlockType.Action,
            Content = "A bustling morning crowd. Steam rises from espresso machines.",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            ScriptId = Guid.Parse("36ca9b58-d902-4195-bf80-2e6518ad3c6d")
        },
        new()
        {
            Id = Guid.Parse("befd3f37-d237-4e46-8a21-e08704c6ef00"),
            OrderIndex = 2,
            BlockType = ScriptBlockType.Character,
            Content = "JOHN",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            ScriptId = Guid.Parse("36ca9b58-d902-4195-bf80-2e6518ad3c6d")
        },
        new()
        {
            Id = Guid.Parse("3bed994a-cbee-4d60-b22f-a922b82eb841"),
            OrderIndex = 3,
            BlockType = ScriptBlockType.Dialogue,
            Content = "I'll have a double espresso. Make it strong.",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow,
            ScriptId = Guid.Parse("36ca9b58-d902-4195-bf80-2e6518ad3c6d")
        }
    ];

    public void Configure(EntityTypeBuilder<ScriptBlock> entity)
    {
        entity.HasData(scriptBlocks);
    }
}
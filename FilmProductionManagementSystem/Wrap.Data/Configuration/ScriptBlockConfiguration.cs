namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class ScriptBlockConfiguration : IEntityTypeConfiguration<ScriptBlock>
{
    public void Configure(EntityTypeBuilder<ScriptBlock> entity)
    {
        entity
            .HasOne(sb => sb.Script)
            .WithMany(s => s.ScriptBlocks)
            .HasForeignKey(sb => sb.ScriptId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasIndex(sb => new { sb.ScriptId, sb.OrderIndex }).IsUnique();
        
        entity.HasData(ScriptBlocks);
    }
    
    private static readonly DateTime SeedCreatedAtUtc = new(2026, 03, 09, 13, 22, 05, DateTimeKind.Utc);
    private static readonly DateTime SeedModifiedAtUtc = new(2026, 03, 09, 13, 22, 05, DateTimeKind.Utc);

    private static readonly ScriptBlock[] ScriptBlocks =
    [
        new()
        {
            Id = ScriptBlock_01,
            OrderIndex = 0,
            BlockType = ScriptBlockType.SceneHeading,
            Content = "INT. COFFEE SHOP - DAY",
            CreatedAt = SeedCreatedAtUtc,
            LastModifiedAt = SeedModifiedAtUtc,
            ScriptId = ScriptId_01
        },
        new()
        {
            Id = ScriptBlock_02,
            OrderIndex = 1,
            BlockType = ScriptBlockType.Action,
            Content = "A bustling morning crowd. Steam rises from espresso machines.",
            CreatedAt = SeedCreatedAtUtc,
            LastModifiedAt = SeedModifiedAtUtc,
            ScriptId = ScriptId_01
        },
        new()
        {
            Id = ScriptBlock_03,
            OrderIndex = 2,
            BlockType = ScriptBlockType.Character,
            Content = "JOHN",
            CreatedAt = SeedCreatedAtUtc,
            LastModifiedAt = SeedModifiedAtUtc,
            ScriptId = ScriptId_01
        },
        new()
        {
            Id = ScriptBlock_04,
            OrderIndex = 3,
            BlockType = ScriptBlockType.Dialogue,
            Content = "I'll have a double espresso. Make it strong.",
            CreatedAt = SeedCreatedAtUtc,
            LastModifiedAt = SeedModifiedAtUtc,
            ScriptId = ScriptId_01
        }
    ];
}
namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static Common.EntityIdentificationConstants;

public class ScriptConfiguration : IEntityTypeConfiguration<Script>
{
    public void Configure(EntityTypeBuilder<Script> entity)
    {
        entity
            .HasMany(s => s.ScriptBlocks)
            .WithOne(sb => sb.Script)
            .HasForeignKey(sb => sb.ScriptId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(Scripts);
    }
    
    private static readonly DateTime SeedLastEditedAtUtc = new(2026, 03, 26, 13, 22, 05, DateTimeKind.Utc);
    
    private static readonly Script[] Scripts =
    [
        new()
        {
            Id = ScriptId_01,
            Title = "Test Screenplay",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.Draft,
            RevisionType = null,
            ProductionId = ProductionIdTestFilm
        },
        new()
        {
            Id = ScriptId_02,
            Title = "Midnight Dreams — Draft",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.Draft,
            RevisionType = null,
            ProductionId = ProductionIdMidnightDreams
        },
        new()
        {
            Id = ScriptId_03,
            Title = "Paper Planes — Shooting Script",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.ShootingScript,
            RevisionType = ScriptRevisionType.PinkRevision,
            ProductionId = ProductionIdPaperPlanes
        },
        new()
        {
            Id = ScriptId_04,
            Title = "The Last Take — Blue Revision",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.ProductionDraft,
            RevisionType = ScriptRevisionType.BlueRevision,
            ProductionId = ProductionIdTheLastTake
        },
        new()
        {
            Id = ScriptId_05,
            Title = "Neon Skyline — Outline",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.Outline,
            RevisionType = null,
            ProductionId = ProductionIdNeonSkyline
        },
        new()
        {
            Id = ScriptId_06,
            Title = "Wild Tracks — Polish",
            LastEditedAt = SeedLastEditedAtUtc,
            StageType = ScriptStageType.Polish,
            RevisionType = ScriptRevisionType.YellowRevision,
            ProductionId = ProductionIdWildTracks
        }
    ];
}
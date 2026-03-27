namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static GCommon.DataFormat;
using static Common.EntityIdentificationConstants;

public class ProductionConfiguration : IEntityTypeConfiguration<Production>
{
    public void Configure(EntityTypeBuilder<Production> entity)
    {
        entity
            .HasOne(p => p.Script)
            .WithOne(s => s.Production)
            .HasForeignKey<Script>(s => s.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(ProductionsSeed);
    }
    
    private static readonly Production[] ProductionsSeed =
    [
        new()
        {
            Id = ProductionIdTestFilm,
            Title = "Test Film",
            Description = "This is a test",
            Budget = 1000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Concept,
            StatusStartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 06, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdMidnightDreams,
            Title = "Midnight Dreams",
            Description = "A neo-noir mystery unfolding over one sleepless night.",
            Budget = 250000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Development,
            StatusStartDate = new DateTime(2026, 01, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 15, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdPaperPlanes,
            Title = "Paper Planes",
            Description = "A coming-of-age drama about ambition, fear, and first love.",
            Budget = 120000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Casting,
            StatusStartDate = new DateTime(2026, 02, 20, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 20, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdTheLastTake,
            Title = "The Last Take",
            Description = "A comedic behind-the-scenes story about a cursed film set.",
            Budget = 180000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Preproduction,
            StatusStartDate = new DateTime(2026, 03, 05, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 04, 05, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdNeonSkyline,
            Title = "Neon Skyline",
            Description = "A sci-fi thriller set in a near-future megacity.",
            Budget = 750000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Financing,
            StatusStartDate = new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 01, 25, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdWildTracks,
            Title = "Wild Tracks",
            Description = "A documentary journey following musicians on the road.",
            Budget = 90000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.LocationScouting,
            StatusStartDate = new DateTime(2026, 02, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 18, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdOnHold,
            Title = "On Hold",
            Description = "A character study about burnout and reinvention.",
            Budget = 60000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.OnHold,
            StatusStartDate = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdReshootSeason,
            Title = "Reshoot Season",
            Description = "A tense drama about fixing a film before the deadline.",
            Budget = 320000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Reshoots,
            StatusStartDate = new DateTime(2026, 04, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 04, 25, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdColorAndNoise,
            Title = "Color & Noise",
            Description = "An art-house film exploring memory through sound and color.",
            Budget = 140000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.ColorGrading,
            StatusStartDate = new DateTime(2026, 05, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 05, 20, 0, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ProductionIdFestivalRun,
            Title = "Festival Run",
            Description = "A finished film preparing for its festival circuit debut.",
            Budget = 210000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.FestivalCircuit,
            StatusStartDate = new DateTime(2026, 06, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 09, 01, 0, 0, 0, DateTimeKind.Utc)
        }
    ];
}
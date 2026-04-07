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

        entity
            .HasOne(p => p.CreatedByUser)
            .WithMany()
            .HasForeignKey(p => p.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
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
            StatusEndDate = new DateTime(2026, 03, 06, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
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
            StatusEndDate = new DateTime(2026, 02, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
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
            StatusEndDate = new DateTime(2026, 03, 20, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
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
            StatusEndDate = new DateTime(2026, 04, 05, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
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
            StatusEndDate = new DateTime(2026, 01, 25, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
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
            StatusEndDate = new DateTime(2026, 02, 18, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
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
            StatusEndDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
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
            StatusEndDate = new DateTime(2026, 04, 25, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
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
            StatusEndDate = new DateTime(2026, 05, 20, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
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
            StatusEndDate = new DateTime(2026, 09, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },

        new()
        {
            Id = ProductionIdSilentEcho,
            Title = "Silent Echo",
            Description = "A psychological drama about memory, guilt, and unresolved loss.",
            Budget = 175000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Financing,
            StatusStartDate = new DateTime(2026, 01, 12, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 05, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdGlassHorizon,
            Title = "Glass Horizon",
            Description = "A visually driven sci-fi drama set on the edge of a collapsing city.",
            Budget = 540000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Casting,
            StatusStartDate = new DateTime(2026, 01, 08, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 01, 28, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdAshesOfSummer,
            Title = "Ashes of Summer",
            Description = "A nostalgic indie drama about friendship fractured by a long-buried secret.",
            Budget = 130000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Cancelled,
            StatusStartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 28, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdVelvetSignal,
            Title = "Velvet Signal",
            Description = "A stylish thriller following a late-night radio host drawn into a conspiracy.",
            Budget = 280000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.LocationScouting,
            StatusStartDate = new DateTime(2026, 02, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 02, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdFadingLights,
            Title = "Fading Lights",
            Description = "A bittersweet romance between two artists during the final week of summer.",
            Budget = 95000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Preproduction,
            StatusStartDate = new DateTime(2026, 03, 15, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 04, 10, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdBrokenFrame,
            Title = "Broken Frame",
            Description = "A crime thriller in which a photographer uncovers evidence hidden in old negatives.",
            Budget = 410000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Reshoots,
            StatusStartDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 05, 12, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdNorthbound,
            Title = "Northbound",
            Description = "A road movie about estranged siblings forced to travel together across the country.",
            Budget = 160000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.LocationScouting,
            StatusStartDate = new DateTime(2026, 02, 14, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdAfterTheRain,
            Title = "After the Rain",
            Description = "A quiet character-driven story of renewal in a small coastal town.",
            Budget = 110000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.PostProduction,
            StatusStartDate = new DateTime(2026, 05, 18, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 06, 15, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdParallelVoices,
            Title = "Parallel Voices",
            Description = "An experimental ensemble film weaving together intersecting monologues.",
            Budget = 145000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.SoundDesign,
            StatusStartDate = new DateTime(2026, 06, 05, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 06, 28, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdFinalArrangement,
            Title = "Final Arrangement",
            Description = "A chamber drama centered on family tension during the reading of a final will.",
            Budget = 85000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Distribution,
            StatusStartDate = new DateTime(2026, 07, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 08, 10, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdCrimsonHarbor,
            Title = "Crimson Harbor",
            Description = "A noir thriller about a detective uncovering corruption in a decaying port city.",
            Budget = 430000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Development,
            StatusStartDate = new DateTime(2026, 01, 18, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 22, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdShatteredSilence,
            Title = "Shattered Silence",
            Description = "A courtroom drama about a musician fighting to reclaim her voice and reputation.",
            Budget = 275000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Financing,
            StatusStartDate = new DateTime(2026, 01, 05, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 10, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdGoldenHour,
            Title = "Golden Hour",
            Description = "A heartfelt drama following a family photographer confronting the passage of time.",
            Budget = 95000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Casting,
            StatusStartDate = new DateTime(2026, 02, 08, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdVelvetAshes,
            Title = "Velvet Ashes",
            Description = "An intimate psychological drama about fame, obsession, and self-destruction.",
            Budget = 210000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Preproduction,
            StatusStartDate = new DateTime(2026, 02, 25, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 25, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdFrozenLetters,
            Title = "Frozen Letters",
            Description = "A winter mystery centered around a box of unsent letters discovered in an abandoned house.",
            Budget = 150000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.LocationScouting,
            StatusStartDate = new DateTime(2026, 01, 22, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 02, 06, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdIronVeil,
            Title = "Iron Veil",
            Description = "A historical war drama about espionage, sacrifice, and impossible choices.",
            Budget = 980000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Rehearsals,
            StatusStartDate = new DateTime(2026, 03, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 18, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdSummerRewind,
            Title = "Summer Rewind",
            Description = "A light coming-of-age dramedy about three friends reliving their last summer together.",
            Budget = 120000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Production,
            StatusStartDate = new DateTime(2026, 04, 05, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 05, 02, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdHollowCity,
            Title = "Hollow City",
            Description = "A dystopian thriller about a journalist navigating a city emptied by fear and surveillance.",
            Budget = 650000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.OnHold,
            StatusStartDate = new DateTime(2026, 02, 14, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 04, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdBlueStatic,
            Title = "Blue Static",
            Description = "A mystery-thriller where eerie radio transmissions begin predicting violent events.",
            Budget = 360000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Reshoots,
            StatusStartDate = new DateTime(2026, 05, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 05, 24, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdPaperMoonlight,
            Title = "Paper Moonlight",
            Description = "A poetic indie romance unfolding over a single night in Sofia.",
            Budget = 80000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.PostProduction,
            StatusStartDate = new DateTime(2026, 05, 15, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 06, 12, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdDustAndNeon,
            Title = "Dust and Neon",
            Description = "A road thriller about a failed musician and a runaway chasing redemption.",
            Budget = 240000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.PictureLock,
            StatusStartDate = new DateTime(2026, 06, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 06, 14, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdSecondExposure,
            Title = "Second Exposure",
            Description = "A suspense drama in which a photographer discovers incriminating details in developed film.",
            Budget = 185000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.SoundDesign,
            StatusStartDate = new DateTime(2026, 06, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 06, 28, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdQuietSignal,
            Title = "Quiet Signal",
            Description = "A subtle sci-fi drama about loneliness, distance, and first contact.",
            Budget = 410000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.VisualEffects,
            StatusStartDate = new DateTime(2026, 06, 20, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 07, 18, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdTheOpenWindow,
            Title = "The Open Window",
            Description = "A chamber drama about neighbors whose lives intersect after a tragic accident.",
            Budget = 105000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Marketing,
            StatusStartDate = new DateTime(2026, 07, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 07, 30, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdLastDeparture,
            Title = "Last Departure",
            Description = "A tense airport-set drama about missed chances, final goodbyes, and unexpected reunions.",
            Budget = 195000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Distribution,
            StatusStartDate = new DateTime(2026, 07, 15, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 08, 20, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        },
        new()
        {
            Id = ProductionIdEchoLine,
            Title = "Echo Line",
            Description = "A minimalist drama about two strangers connected by a late-night emergency call.",
            Budget = 70000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Released,
            StatusStartDate = new DateTime(2026, 08, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 08, 01, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId3
        },
        new()
        {
            Id = ProductionIdBurningCanvas,
            Title = "Burning Canvas",
            Description = "An artist spirals into obsession while preparing the final exhibition of his career.",
            Budget = 155000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Completed,
            StatusStartDate = new DateTime(2026, 09, 10, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 09, 10, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId1
        },
        new()
        {
            Id = ProductionIdNoSignalTomorrow,
            Title = "No Signal Tomorrow",
            Description = "A post-apocalyptic survival story abandoned after loss of primary financing.",
            Budget = 500000.00M,
            Thumbnail = DefaultThumbnailPath,
            StatusType = ProductionStatusType.Cancelled,
            StatusStartDate = new DateTime(2026, 02, 01, 0, 0, 0, DateTimeKind.Utc),
            StatusEndDate = new DateTime(2026, 03, 12, 0, 0, 0, DateTimeKind.Utc),
            CreatedByUserId = CrewUserId2
        }
    ];
}
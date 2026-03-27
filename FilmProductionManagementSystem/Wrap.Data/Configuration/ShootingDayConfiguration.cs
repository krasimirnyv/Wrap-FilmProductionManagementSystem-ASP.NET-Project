namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;

using static Common.EntityIdentificationConstants;

public class ShootingDayConfiguration : IEntityTypeConfiguration<ShootingDay>
{
    public void Configure(EntityTypeBuilder<ShootingDay> entity)
    {
        entity
            .HasOne(sd => sd.Production)
            .WithMany(p => p.ShootingDays)
            .HasForeignKey(sd => sd.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        entity.HasData(ShootingDaysSeed);
    }
    
    private static readonly DateTime SeedDayUtcBase = new(2026, 03, 20, 08, 00, 00, DateTimeKind.Utc);

    private static readonly ShootingDay[] ShootingDaysSeed =
    [
        // Production 01 - "Test Film"
        new()
        {
            Id = ShootingDayId_TestFilm_01,
            ProductionId = ProductionIdTestFilm,
            Date = SeedDayUtcBase.AddDays(0),
            Notes = "Day 1: Coffee shop scene coverage."
        },
        new()
        {
            Id = ShootingDayId_TestFilm_02,
            ProductionId = ProductionIdTestFilm,
            Date = SeedDayUtcBase.AddDays(1),
            Notes = "Day 2: Pickups and insert shots."
        },

        // Production 02 - "Midnight Dreams"
        new()
        {
            Id = ShootingDayId_MidnightDreams_01,
            ProductionId = ProductionIdMidnightDreams,
            Date = SeedDayUtcBase.AddDays(3),
            Notes = "Night exterior sequence."
        },
        new()
        {
            Id = ShootingDayId_MidnightDreams_02,
            ProductionId = ProductionIdMidnightDreams,
            Date = SeedDayUtcBase.AddDays(4),
            Notes = "Studio interiors + dialogue scenes."
        },

        // Production 03 - "Paper Planes"
        new()
        {
            Id = ShootingDayId_PaperPlanes_01,
            ProductionId = ProductionIdPaperPlanes,
            Date = SeedDayUtcBase.AddDays(6),
            Notes = "School hallway + classroom scenes."
        },

        // Production 04 - "The Last Take"
        new()
        {
            Id = ShootingDayId_TheLastTake_01,
            ProductionId = ProductionIdTheLastTake,
            Date = SeedDayUtcBase.AddDays(8),
            Notes = "Stage scenes and rehearsal coverage."
        },
        new()
        {
            Id = ShootingDayId_TheLastTake_02,
            ProductionId = ProductionIdTheLastTake,
            Date = SeedDayUtcBase.AddDays(9),
            Notes = "Close-ups, alt takes, safety shots."
        },

        // Production 05 - "Neon Skyline"
        new()
        {
            Id = ShootingDayId_NeonSkyline_01,
            ProductionId = ProductionIdNeonSkyline,
            Date = SeedDayUtcBase.AddDays(11),
            Notes = "City night montage (neon locations)."
        },

        // Production 06 - "Wild Tracks"
        new()
        {
            Id = ShootingDayId_WildTracks_01,
            ProductionId = ProductionIdWildTracks,
            Date = SeedDayUtcBase.AddDays(13),
            Notes = "Outdoor travel sequence."
        },

        // Production 07 - "On Hold"
        new()
        {
            Id = ShootingDayId_OnHold_01,
            ProductionId = ProductionIdOnHold,
            Date = SeedDayUtcBase.AddDays(15),
            Notes = "Planned shoot (project currently on hold)."
        },

        // Production 08 - "Reshoot Season"
        new()
        {
            Id = ShootingDayId_ReshootSeason_01,
            ProductionId = ProductionIdReshootSeason,
            Date = SeedDayUtcBase.AddDays(17),
            Notes = "Reshoot day: continuity fixes + inserts."
        },
        
        // Production 09 - "Color & Noise"
        new()
        {
            Id = ShootingDayId_ColorAndNoise_01,
            ProductionId = ProductionIdColorAndNoise,
            Date = SeedDayUtcBase.AddDays(19),
            Notes = "B-roll day for post-production needs."
        },

        // Production 10 - "Festival Run"
        new()
        {
            Id = ShootingDayId_FestivalRun_01,
            ProductionId = ProductionIdFestivalRun,
            Date = SeedDayUtcBase.AddDays(21),
            Notes = "Press / promo shoot day."
        }
    ];
}
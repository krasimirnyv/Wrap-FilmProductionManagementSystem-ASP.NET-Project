namespace Wrap.Data.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models;
using GCommon.Enums;

using static GCommon.DataFormat;
using static Common.EntityIdentificationConstants;

public class ProductionAssetConfiguration : IEntityTypeConfiguration<ProductionAsset>
{
    public void Configure(EntityTypeBuilder<ProductionAsset> entity)
    {
        entity
            .HasOne(a => a.Production)
            .WithMany(p => p.ProductionAssets)
            .HasForeignKey(a => a.ProductionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(a => new { a.ProductionId, a.AssetType });

        entity.HasData(ProductionAssetsSeed);
    }
    
    private static readonly DateTime UploadedAt_01 = new(2026, 03, 01, 10, 00, 00, DateTimeKind.Utc);
    private static readonly DateTime UploadedAt_02 = new(2026, 03, 02, 11, 15, 00, DateTimeKind.Utc);
    private static readonly DateTime UploadedAt_03 = new(2026, 03, 03, 09, 40, 00, DateTimeKind.Utc);
    private static readonly DateTime UploadedAt_04 = new(2026, 03, 04, 14, 05, 00, DateTimeKind.Utc);
    
    private static readonly ProductionAsset[] ProductionAssetsSeed =
    [
        // Production 01 - "Test Film"
        new()
        {
            Id = AssetId_01,
            ProductionId = ProductionIdTestFilm,
            AssetType = ProductionAssetType.Storyboard,
            Title = "Storyboard v1",
            Description = "Basic scene coverage and camera beats.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_01
        },
        new()
        {
            Id = AssetId_02,
            ProductionId = ProductionIdTestFilm,
            AssetType = ProductionAssetType.Moodboard,
            Title = "Moodboard - Visual Tone",
            Description = "Lighting references and overall vibe.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_01
        },
        new()
        {
            Id = AssetId_03,
            ProductionId = ProductionIdTestFilm,
            AssetType = ProductionAssetType.ColorWheel,
            Title = "Color Palette - Day Scenes",
            Description = "Primary palette for day interiors.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_02
        },

        // Production 02 - "Midnight Dreams"
        new()
        {
            Id = AssetId_04,
            ProductionId = ProductionIdMidnightDreams,
            AssetType = ProductionAssetType.References,
            Title = "Reference Frames Pack",
            Description = "Reference stills for framing and composition.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_02
        },
        new()
        {
            Id = AssetId_05,
            ProductionId = ProductionIdMidnightDreams,
            AssetType = ProductionAssetType.Storyboard,
            Title = "Storyboard - Chase Sequence",
            Description = "Action sequence beats and movement.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_03
        },
        new()
        {
            Id = AssetId_06,
            ProductionId = ProductionIdMidnightDreams,
            AssetType = ProductionAssetType.Other,
            Title = "Shot List Draft",
            Description = "Draft PDF with lens & shot notes.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_03
        },

        // Production 03 - "Paper Planes"
        new()
        {
            Id = AssetId_07,
            ProductionId = ProductionIdPaperPlanes,
            AssetType = ProductionAssetType.Moodboard,
            Title = "Moodboard - Night Atmosphere",
            Description = "Neon, fog, wet streets.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_03
        },
        new()
        {
            Id = AssetId_08,
            ProductionId = ProductionIdPaperPlanes,
            AssetType = ProductionAssetType.ColorWheel,
            Title = "Color Palette - Night Scenes",
            Description = "Cold shadows with warm practicals.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_04
        },
        new()
        {
            Id = AssetId_09,
            ProductionId = ProductionIdPaperPlanes,
            AssetType = ProductionAssetType.References,
            Title = "Set Dressing References",
            Description = "Props and set dressing ideas.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_04
        },

        // Production 04 - "The Last Take"
        new()
        {
            Id = AssetId_10,
            ProductionId = ProductionIdTheLastTake,
            AssetType = ProductionAssetType.Storyboard,
            Title = "Storyboard - Finale",
            Description = "Final sequence beats and transitions.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_04
        },
        new()
        {
            Id = AssetId_11,
            ProductionId = ProductionIdTheLastTake,
            AssetType = ProductionAssetType.Other,
            Title = "Location Photos",
            Description = "Scouting photos for final location.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_04
        },
        new()
        {
            Id = AssetId_12,
            ProductionId = ProductionIdTheLastTake,
            AssetType = ProductionAssetType.References,
            Title = "Costume References",
            Description = "Wardrobe references for key characters.",
            FilePath = DefaultAssetPath,
            UploadedAt = UploadedAt_04
        }
    ];
}